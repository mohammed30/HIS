using System;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting.Dtos;
using HIS.Inventory;
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
    public class PaymentVoucherAppService : 
        CrudAppService<
            PaymentVoucher, 
            PaymentVoucherDto, 
            Guid, 
            PagedAndSortedResultRequestDto, 
            CreateUpdatePaymentVoucherDto>, 
        IPaymentVoucherAppService
    {
        private readonly IRepository<Supplier, Guid> _supplierRepository;
        private readonly IRepository<PaymentMethod, Guid> _paymentMethodRepository;
        private readonly IRepository<Account, Guid> _accountRepository;
        private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
        private readonly IWebHostEnvironment _env;

        public PaymentVoucherAppService(
            IRepository<PaymentVoucher, Guid> repository,
            IRepository<Supplier, Guid> supplierRepository,
            IRepository<PaymentMethod, Guid> paymentMethodRepository,
            IRepository<Account, Guid> accountRepository,
            IRepository<JournalEntry, Guid> journalEntryRepository,
            IWebHostEnvironment env) 
            : base(repository)
        {
            _supplierRepository = supplierRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _accountRepository = accountRepository;
            _journalEntryRepository = journalEntryRepository;
            _env = env;
        }

        protected override async Task<IQueryable<PaymentVoucher>> CreateFilteredQueryAsync(PagedAndSortedResultRequestDto input)
        {
            return await Repository.WithDetailsAsync(x => x.Lines);
        }

        public override async Task<PaymentVoucherDto> GetAsync(Guid id)
        {
            var query = await Repository.WithDetailsAsync(x => x.Lines);
            var entity = await AsyncExecuter.FirstOrDefaultAsync(query, x => x.Id == id);
            
            var dto = MapToGetOutputDto(entity);
            
            // Populate lookup names manually or via extra queries if needed
            if (entity.SupplierId.HasValue)
            {
                var supplier = await _supplierRepository.FindAsync(entity.SupplierId.Value);
                dto.SupplierName = supplier?.Name;
            }

            if (entity.PaymentMethodId.HasValue)
            {
                var pm = await _paymentMethodRepository.FindAsync(entity.PaymentMethodId.Value);
                dto.PaymentMethodName = pm?.NameEn ?? pm?.NameAr;
            }

            // Populate account names for lines
            foreach (var lineDto in dto.Lines)
            {
                var account = await _accountRepository.FindAsync(lineDto.AccountId);
                lineDto.AccountName = account?.Name;
            }

            return dto;
        }

        public override async Task<PaymentVoucherDto> CreateAsync(CreateUpdatePaymentVoucherDto input)
        {
            await CheckCreatePolicyAsync();

            string voucherNumber = "PV-" + DateTime.Now.Ticks.ToString().Substring(10); 

            var entity = MapToEntity(input);
            entity.VoucherNumber = voucherNumber;
            
            await Repository.InsertAsync(entity, autoSave: true);

            // Auto-Create Journal Entry
            await CreateJournalEntryAsync(entity, input);

            return await GetAsync(entity.Id);
        }

        private async Task CreateJournalEntryAsync(PaymentVoucher voucher, CreateUpdatePaymentVoucherDto input)
        {
            if (voucher.Amount <= 0) return;

            // Credit Cash or Bank based on PaymentMethod. For now, default to Cash (1110) if not specified or found
            var cashAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1110");
            var creditAccount = cashAccount; // Default

            if (input.PaymentMethodId.HasValue)
            {
                 var pm = await _paymentMethodRepository.FindAsync(input.PaymentMethodId.Value);
                 if (pm != null && (pm.NameEn.Contains("Bank", StringComparison.OrdinalIgnoreCase) || pm.NameAr.Contains("بنك")))
                 {
                     var bankAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Name.Contains("Bank") || x.NameAr.Contains("بنك"));
                     if (bankAccount != null) creditAccount = bankAccount;
                 }
            }

            if (creditAccount != null)
            {
                string payeeName = voucher.PayeeName;
                if (voucher.SupplierId.HasValue && string.IsNullOrEmpty(payeeName))
                {
                    var supplier = await _supplierRepository.FindAsync(voucher.SupplierId.Value);
                    payeeName = supplier?.Name ?? "";
                }

                var je = new JournalEntry(
                    GuidGenerator.Create(),
                    voucher.Date,
                    voucher.VoucherNumber,
                    $"سند صرف رقم {voucher.VoucherNumber} - {(string.IsNullOrEmpty(payeeName) ? "جهات أخرى" : payeeName)}" + (string.IsNullOrEmpty(voucher.Description) ? "" : $" - {voucher.Description}")
                );

                // Debit the accounts from lines
                foreach (var line in input.Lines)
                {
                    if (line.Amount > 0)
                    {
                        je.AddLine(GuidGenerator, line.AccountId, line.Amount, 0);
                    }
                }

                // Credit Cash/Bank
                je.AddLine(GuidGenerator, creditAccount.Id, 0, voucher.Amount);

                await _journalEntryRepository.InsertAsync(je);
            }
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("api/app/payment-voucher/pdf/{id}")]
        public async Task<Volo.Abp.Content.IRemoteStreamContent> GetPaymentPdfAsync(Guid id)
        {
            var dto = await GetAsync(id);
            
            var document = new HIS.Accounting.Printing.VoucherDocument
            {
                IsReceipt = false,
                VoucherNumber = dto.VoucherNumber,
                Date = dto.Date,
                PartyName = dto.SupplierId.HasValue ? dto.SupplierName : dto.PayeeName,
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

            var pdfBytes = QuestPDF.Fluent.GenerateExtensions.GeneratePdf(document);
            var stream = new System.IO.MemoryStream(pdfBytes);
            var printTime = Clock.Now;
            var fileName = $"سند_صرف_{dto.VoucherNumber}_{printTime:yyyy-MM-dd_HH-mm-ss}.pdf";
            return new Volo.Abp.Content.RemoteStreamContent(stream, fileName, "application/pdf");
        }
    }
}
