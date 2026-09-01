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
using HIS.Notifications;
using Volo.Abp.Identity;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace HIS.Accounting
{
    [Authorize(HISPermissions.Billing.Payments)]
    public class PaymentVoucherAppService : 
        CrudAppService<
            PaymentVoucher, 
            PaymentVoucherDto, 
            Guid, 
            VoucherFilterDto, 
            CreateUpdatePaymentVoucherDto>, 
        IPaymentVoucherAppService
    {
        private readonly IRepository<Supplier, Guid> _supplierRepository;
        private readonly IRepository<PaymentMethod, Guid> _paymentMethodRepository;
        private readonly IRepository<Account, Guid> _accountRepository;
        private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
        private readonly IWebHostEnvironment _env;
        
        // Notifications
        private readonly IRepository<Notification, Guid> _notificationRepo;
        private readonly NotificationSender _notificationSender;
        private readonly IIdentityUserRepository _userRepository;

        public PaymentVoucherAppService(
            IRepository<PaymentVoucher, Guid> repository,
            IRepository<Supplier, Guid> supplierRepository,
            IRepository<PaymentMethod, Guid> paymentMethodRepository,
            IRepository<Account, Guid> accountRepository,
            IRepository<JournalEntry, Guid> journalEntryRepository,
            IWebHostEnvironment env,
            IRepository<Notification, Guid> notificationRepo,
            NotificationSender notificationSender,
            IIdentityUserRepository userRepository) 
            : base(repository)
        {
            _supplierRepository = supplierRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _accountRepository = accountRepository;
            _journalEntryRepository = journalEntryRepository;
            _env = env;
            _notificationRepo = notificationRepo;
            _notificationSender = notificationSender;
            _userRepository = userRepository;
            UpdatePolicyName = HISPermissions.Billing.EditPaymentVouchers;
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

        protected override async Task<IQueryable<PaymentVoucher>> CreateFilteredQueryAsync(VoucherFilterDto input)
        {
            var query = await Repository.WithDetailsAsync(x => x.Lines);

            if (input.DateFrom.HasValue)
            {
                var dateFrom = input.DateFrom.Value.Date;
                query = query.Where(x => x.Date >= dateFrom);
            }
            if (input.DateTo.HasValue)
            {
                var dateTo = input.DateTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(x => x.Date <= dateTo);
            }

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
                    (x.PayeeName != null && x.PayeeName.Contains(input.Filter)) ||
                    (x.Description != null && x.Description.Contains(input.Filter)) ||
                    (x.Lines.Any(l => matchingAccountIds.Contains(l.AccountId)))
                );
            }

            return query;
        }

        public override async Task<PaymentVoucherDto> CreateAsync(CreateUpdatePaymentVoucherDto input)
        {
            await CheckCreatePolicyAsync();

            var maxSerial = await Repository.MaxAsync(x => (long?)x.SerialNumber) ?? 0;
            long nextSerial = maxSerial + 1;
            string voucherNumber = "PV-" + nextSerial.ToString("D6"); 

            var entity = MapToEntity(input);
            entity.SerialNumber = nextSerial;
            entity.VoucherNumber = voucherNumber;
            
            await Repository.InsertAsync(entity, autoSave: true);

            // Auto-Create Journal Entry
            await CreateJournalEntryAsync(entity, input);

            // Trigger Notification to all users
            try
            {
                var settingProvider = LazyServiceProvider.LazyGetRequiredService<Volo.Abp.Settings.ISettingProvider>();

                var settingValue = await settingProvider.GetOrNullAsync("Notifications.Subscribers.Accounting");
                var userIds = string.IsNullOrWhiteSpace(settingValue) ? new List<Guid>() : settingValue.Split(',').Select(Guid.Parse).ToList();

                if (userIds.Any())
                {
                    var notifications = userIds.Select(id => new Notification(
                        GuidGenerator.Create(),
                        id,
                        "سند صرف جديد",
                        $"تم إنشاء سند صرف جديد برقم {entity.VoucherNumber} بقيمة {entity.Amount}",
                        "Accounting",
                        "/accounting/payment-vouchers",
                        entity.Id.ToString(),
                        CurrentUser.UserName ?? "النظام"
                    )).ToList();

                    await _notificationRepo.InsertManyAsync(notifications);

                    foreach (var notif in notifications)
                    {
                        var dto = ObjectMapper.Map<Notification, NotificationDto>(notif);
                        await _notificationSender.SendToUserAsync(notif.UserId, dto);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log and ignore notification errors so it doesn't break the transaction
                Logger.LogError(ex, "Failed to send notification");
            }

            return await GetAsync(entity.Id);
        }

        [Microsoft.AspNetCore.Authorization.Authorize(HISPermissions.Billing.CancelPaymentVouchers)]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("api/app/payment-voucher/{id}/cancel")]
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
                    $"Reversal of Payment Voucher {voucher.VoucherNumber} - Reason: {reason}",
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
            var fileName = $"سند_صرف_{dto.VoucherNumber}_{printTime:yyyy-MM-dd_HH-mm-ss}.pdf";
            return new Volo.Abp.Content.RemoteStreamContent(stream, fileName, "application/pdf");
        }
    }
}
