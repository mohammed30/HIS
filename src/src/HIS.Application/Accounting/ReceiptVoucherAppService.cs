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
            PagedAndSortedResultRequestDto, 
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
        }

        protected override async Task<IQueryable<ReceiptVoucher>> CreateFilteredQueryAsync(PagedAndSortedResultRequestDto input)
        {
            return await Repository.WithDetailsAsync(x => x.Lines);
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

        public override async Task<ReceiptVoucherDto> CreateAsync(CreateUpdateReceiptVoucherDto input)
        {
            await CheckCreatePolicyAsync();

            string voucherNumber = "RV-" + DateTime.Now.Ticks.ToString().Substring(10); 

            var entity = MapToEntity(input);
            entity.VoucherNumber = voucherNumber;
            
            await Repository.InsertAsync(entity, autoSave: true);

            // Auto-Create Journal Entry
            await CreateJournalEntryAsync(entity, input);

            return await GetAsync(entity.Id);
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
                AmountInWords = $"{dto.Amount} جنيه فقط لا غير", // Need to implement Tafqeet for proper words if required later
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

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            var pdfBytes = QuestPDF.Fluent.GenerateExtensions.GeneratePdf(document);
            var stream = new System.IO.MemoryStream(pdfBytes);
            return new Volo.Abp.Content.RemoteStreamContent(stream, $"{dto.VoucherNumber}.pdf", "application/pdf");
        }
    }
}
