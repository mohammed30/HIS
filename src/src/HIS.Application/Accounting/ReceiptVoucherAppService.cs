using System;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting.Dtos;
using HIS.Patients;
using HIS.Permissions;
using HIS.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Accounting
{
    [Authorize(HISPermissions.Billing.Payments)]
    public class ReceiptVoucherAppService : 
        CrudAppService<
            ReceiptVoucher, 
            ReceiptVoucherDto, 
            Guid, 
            VoucherFilterDto, 
            CreateUpdateReceiptVoucherDto>, 
        IReceiptVoucherAppService
    {
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<PaymentMethod, Guid> _paymentMethodRepository;
        private readonly IRepository<Account, Guid> _accountRepository;
        private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
        private readonly IWebHostEnvironment _env;

        public ReceiptVoucherAppService(
            IRepository<ReceiptVoucher, Guid> repository,
            IRepository<Patient, Guid> patientRepository,
            IRepository<PaymentMethod, Guid> paymentMethodRepository,
            IRepository<Account, Guid> accountRepository,
            IRepository<JournalEntry, Guid> journalEntryRepository,
            IWebHostEnvironment env) 
            : base(repository)
        {
            _patientRepository = patientRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _accountRepository = accountRepository;
            _journalEntryRepository = journalEntryRepository;
            _env = env;
            UpdatePolicyName = HISPermissions.Billing.EditReceiptVouchers;
        }



        public override async Task<ReceiptVoucherDto> GetAsync(Guid id)
        {
            var query = await Repository.WithDetailsAsync(x => x.Lines);
            var entity = await AsyncExecuter.FirstOrDefaultAsync(query, x => x.Id == id);
            
            var dto = MapToGetOutputDto(entity);
            
            if (entity.PatientId.HasValue)
            {
                var patient = await _patientRepository.FindAsync(entity.PatientId.Value);
                // Assuming Patient has a Name or First/Last name. Patient Class has Names.
                dto.PatientName = patient != null ? $"{patient.FirstNameAr} {patient.LastNameAr}" : "";
            }

            if (entity.PaymentMethodId.HasValue)
            {
                var pm = await _paymentMethodRepository.FindAsync(entity.PaymentMethodId.Value);
                dto.PaymentMethodName = pm?.NameEn ?? pm?.NameAr;
            }

            foreach (var lineDto in dto.Lines)
            {
                var account = await _accountRepository.FindAsync(lineDto.AccountId);
                lineDto.AccountName = account?.Name;
            }

            return dto;
        }

        protected override async Task<IQueryable<ReceiptVoucher>> CreateFilteredQueryAsync(VoucherFilterDto input)
        {
            var query = await Repository.WithDetailsAsync(x => x.Lines);

            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                long? parsedFilter = null;
                if (long.TryParse(input.Filter, out long parsed)) parsedFilter = parsed;

                var accountQuery = await _accountRepository.GetQueryableAsync();
                var matchingAccountIds = accountQuery.Where(a => 
                    (a.Code != null && a.Code.Contains(input.Filter)) ||
                    (a.NameAr != null && a.NameAr.Contains(input.Filter)) ||
                    (a.Name != null && a.Name.Contains(input.Filter))
                ).Select(a => a.Id).ToList();

                query = query.Where(x => 
                    (parsedFilter.HasValue && x.SerialNumber == parsedFilter.Value) ||
                    (x.VoucherNumber != null && x.VoucherNumber.Contains(input.Filter)) ||
                    (x.PayerName != null && x.PayerName.Contains(input.Filter)) ||
                    (x.Description != null && x.Description.Contains(input.Filter)) ||
                    (x.Lines.Any(l => matchingAccountIds.Contains(l.AccountId)))
                );
            }

            return query;
        }

        public override async Task<ReceiptVoucherDto> CreateAsync(CreateUpdateReceiptVoucherDto input)
        {
            await CheckCreatePolicyAsync();

            var maxSerial = await Repository.MaxAsync(x => (long?)x.SerialNumber) ?? 0;
            long nextSerial = maxSerial + 1;
            string voucherNumber = "RV-" + nextSerial.ToString("D6"); 

            var entity = MapToEntity(input);
            entity.SerialNumber = nextSerial;
            entity.VoucherNumber = voucherNumber;
            
            await Repository.InsertAsync(entity, autoSave: true);

            // Auto-Create Journal Entry
            await CreateJournalEntryAsync(entity, input);

            return await GetAsync(entity.Id);
        }

        [Microsoft.AspNetCore.Authorization.Authorize(HISPermissions.Billing.CancelReceiptVouchers)]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("api/app/receipt-voucher/{id}/cancel")]
        public async Task CancelAsync(Guid id, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new Volo.Abp.UserFriendlyException("Cancellation reason is required.");

            var voucher = await Repository.GetAsync(id);
            if (voucher.IsCancelled)
                throw new Volo.Abp.UserFriendlyException("Voucher is already cancelled.");

            voucher.IsCancelled = true;
            voucher.CancellationReason = reason;
            voucher.CancellationTime = Clock.Now;
            voucher.CancelledByUserId = CurrentUser.Id;
            voucher.CancelledByUserName = CurrentUser.UserName;

            await Repository.UpdateAsync(voucher, autoSave: true);

            // Create Reversing Journal Entry
            var originalJe = await _journalEntryRepository.FirstOrDefaultAsync(x => x.ReferenceNumber == voucher.VoucherNumber && !x.Description.Contains("Reversal"));
            if (originalJe != null)
            {
                var reversingJe = new JournalEntry(
                    GuidGenerator.Create(),
                    Clock.Now,
                    voucher.VoucherNumber,
                    $"Reversal of Receipt Voucher {voucher.VoucherNumber} - Reason: {reason}",
                    isAutomatic: true
                );

                var linesQuery = await _journalEntryRepository.WithDetailsAsync(x => x.Lines);
                var originalJeWithLines = linesQuery.FirstOrDefault(x => x.Id == originalJe.Id);

                if (originalJeWithLines != null)
                {
                    foreach (var line in originalJeWithLines.Lines)
                    {
                        // Swap debit and credit
                        reversingJe.AddLine(GuidGenerator, line.AccountId, line.Credit, line.Debit);
                    }
                    await _journalEntryRepository.InsertAsync(reversingJe, autoSave: true);
                }
            }
        }

        private async Task CreateJournalEntryAsync(ReceiptVoucher voucher, CreateUpdateReceiptVoucherDto input)
        {
            if (voucher.Amount <= 0) return;

            // Debit Cash or Bank based on PaymentMethod. For now, default to Cash (1110) if not specified or found
            var cashAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1110");
            var debitAccount = cashAccount; // Default

            if (input.PaymentMethodId.HasValue)
            {
                 var pm = await _paymentMethodRepository.FindAsync(input.PaymentMethodId.Value);
                 if (pm != null && (pm.NameEn.Contains("Bank", StringComparison.OrdinalIgnoreCase) || pm.NameAr.Contains("بنك")))
                 {
                     var bankAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Name.Contains("Bank") || x.NameAr.Contains("بنك"));
                     if (bankAccount != null) debitAccount = bankAccount;
                 }
            }

            if (debitAccount != null)
            {
                string patientName = voucher.PayerName;
                if (voucher.PatientId.HasValue && string.IsNullOrEmpty(patientName))
                {
                    var patient = await _patientRepository.FindAsync(voucher.PatientId.Value);
                    patientName = patient != null ? $"{patient.FirstNameAr} {patient.LastNameAr}" : "";
                }

                var je = new JournalEntry(
                    GuidGenerator.Create(),
                    voucher.Date,
                    voucher.VoucherNumber,
                    $"سند قبض رقم {voucher.VoucherNumber} - {(string.IsNullOrEmpty(patientName) ? "جهات أخرى" : patientName)}"
                );

                // Debit Cash/Bank
                je.AddLine(GuidGenerator, debitAccount.Id, voucher.Amount, 0);

                // Credit the accounts from lines
                foreach (var line in input.Lines)
                {
                    if (line.Amount > 0)
                    {
                        je.AddLine(GuidGenerator, line.AccountId, 0, line.Amount);
                    }
                }

                await _journalEntryRepository.InsertAsync(je);
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("api/app/receipt-voucher/pdf/{id}")]
        public async Task<Volo.Abp.Content.IRemoteStreamContent> GetReceiptPdfAsync(Guid id)
        {
            var dto = await GetAsync(id);
            
            var document = new HIS.Accounting.Printing.VoucherDocument
            {
                IsReceipt = true,
                VoucherNumber = dto.VoucherNumber,
                Date = dto.Date,
                PartyName = dto.PatientId.HasValue ? dto.PatientName : dto.PayerName,
                PaymentMethodName = dto.PaymentMethodName,
                TotalAmount = dto.Amount,
                AmountInWords = $"{dto.Amount} جنيه فقط لا غير",
                IsCancelled = dto.IsCancelled,
                CancellationReason = dto.CancellationReason,
                Description = dto.Description,
                Lines = dto.Lines.Select(l => new HIS.Accounting.Printing.VoucherDocument.VoucherLineModel
                {
                    AccountName = l.AccountName,
                    Amount = l.Amount,
                    Description = l.Description
                }).ToList()
            };

            // Load Logo securely
            byte[] logoBytes = null;
            var logoPath = System.IO.Path.Combine(_env.WebRootPath ?? "", "images", "logo", "Dark.png");
            if (!System.IO.File.Exists(logoPath))
            {
                var devPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "images", "logo", "Dark.png");
                if (System.IO.File.Exists(devPath)) logoPath = devPath;
            }
            if (System.IO.File.Exists(logoPath)) logoBytes = await System.IO.File.ReadAllBytesAsync(logoPath);
            
            document.LogoBytes = logoBytes;

            var pdfBytes = QuestPDF.Fluent.GenerateExtensions.GeneratePdf(document);
            var stream = new System.IO.MemoryStream(pdfBytes);
            var printTime = Clock.Now;
            var fileName = $"سند_قبض_{dto.VoucherNumber}_{printTime:yyyy-MM-dd_HH-mm-ss}.pdf";
            return new Volo.Abp.Content.RemoteStreamContent(stream, fileName, "application/pdf");
        }
    }
}
