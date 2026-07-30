using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting;
using HIS.Billing;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace HIS.Reports
{
    public class UserFinancialReportAppService : ApplicationService, IUserFinancialReportAppService
    {
        private readonly IRepository<Payment, Guid> _paymentRepository;
        private readonly IRepository<InpatientDeposit, Guid> _inpatientDepositRepository;
        private readonly IRepository<ReceiptVoucher, Guid> _receiptVoucherRepository;
        private readonly IRepository<PaymentVoucher, Guid> _paymentVoucherRepository;
        private readonly IIdentityUserRepository _userRepository;

        public UserFinancialReportAppService(
            IRepository<Payment, Guid> paymentRepository,
            IRepository<InpatientDeposit, Guid> inpatientDepositRepository,
            IRepository<ReceiptVoucher, Guid> receiptVoucherRepository,
            IRepository<PaymentVoucher, Guid> paymentVoucherRepository,
            IIdentityUserRepository userRepository)
        {
            _paymentRepository = paymentRepository;
            _inpatientDepositRepository = inpatientDepositRepository;
            _receiptVoucherRepository = receiptVoucherRepository;
            _paymentVoucherRepository = paymentVoucherRepository;
            _userRepository = userRepository;
        }

        public async Task<PagedResultDto<UserFinancialTransactionDto>> GetListAsync(GetUserFinancialTransactionsInput input)
        {
            if (input.StartDate.HasValue)
            {
                input.StartDate = input.StartDate.Value.Date;
            }
            if (input.EndDate.HasValue)
            {
                input.EndDate = input.EndDate.Value.Date.AddDays(1).AddTicks(-1);
            }

            var transactions = new List<UserFinancialTransactionDto>();

            // 1. Fetch Payments
            if (string.IsNullOrEmpty(input.ModuleName) || input.ModuleName == "Payment")
            {
                var paymentQuery = await _paymentRepository.GetQueryableAsync();
                var payments = paymentQuery
                    .WhereIf(input.UserId.HasValue, x => x.CreatorId == input.UserId)
                    .WhereIf(input.StartDate.HasValue, x => x.PaymentDate >= input.StartDate)
                    .WhereIf(input.EndDate.HasValue, x => x.PaymentDate <= input.EndDate)
                    .ToList();

                transactions.AddRange(payments.Select(x => new UserFinancialTransactionDto
                {
                    TransactionId = x.Id,
                    UserId = x.CreatorId,
                    ModuleName = "Payment",
                    TransactionType = "إيراد (فاتورة)",
                    Amount = x.Amount,
                    TransactionDate = x.PaymentDate,
                    Description = x.Notes ?? "سداد فاتورة",
                    ReferenceNumber = x.PaymentNumber
                }));
            }

            // 2. Fetch Inpatient Deposits
            if (string.IsNullOrEmpty(input.ModuleName) || input.ModuleName == "InpatientDeposit")
            {
                var depositQuery = await _inpatientDepositRepository.GetQueryableAsync();
                var deposits = depositQuery
                    .WhereIf(input.UserId.HasValue, x => x.CreatorId == input.UserId)
                    .WhereIf(input.StartDate.HasValue, x => x.DepositDate >= input.StartDate)
                    .WhereIf(input.EndDate.HasValue, x => x.DepositDate <= input.EndDate)
                    .ToList();

                transactions.AddRange(deposits.Select(x => new UserFinancialTransactionDto
                {
                    TransactionId = x.Id,
                    UserId = x.CreatorId,
                    ModuleName = "InpatientDeposit",
                    TransactionType = "تأمين تنويم",
                    Amount = x.Amount,
                    TransactionDate = x.DepositDate,
                    Description = x.Notes ?? "مبلغ تأمين",
                    ReferenceNumber = x.ReceiptNumber
                }));
            }

            // 3. Fetch Receipt Vouchers
            if (string.IsNullOrEmpty(input.ModuleName) || input.ModuleName == "ReceiptVoucher")
            {
                var receiptQuery = await _receiptVoucherRepository.GetQueryableAsync();
                var receipts = receiptQuery
                    .WhereIf(input.UserId.HasValue, x => x.CreatorId == input.UserId)
                    .WhereIf(input.StartDate.HasValue, x => x.Date >= input.StartDate)
                    .WhereIf(input.EndDate.HasValue, x => x.Date <= input.EndDate)
                    .ToList();

                transactions.AddRange(receipts.Select(x => new UserFinancialTransactionDto
                {
                    TransactionId = x.Id,
                    UserId = x.CreatorId,
                    ModuleName = "ReceiptVoucher",
                    TransactionType = "سند قبض",
                    Amount = x.Amount,
                    TransactionDate = x.Date,
                    Description = x.Description,
                    ReferenceNumber = x.VoucherNumber
                }));
            }

            // 4. Fetch Payment Vouchers (Outgoing)
            if (string.IsNullOrEmpty(input.ModuleName) || input.ModuleName == "PaymentVoucher")
            {
                var voucherQuery = await _paymentVoucherRepository.GetQueryableAsync();
                var vouchers = voucherQuery
                    .WhereIf(input.UserId.HasValue, x => x.CreatorId == input.UserId)
                    .WhereIf(input.StartDate.HasValue, x => x.Date >= input.StartDate)
                    .WhereIf(input.EndDate.HasValue, x => x.Date <= input.EndDate)
                    .ToList();

                transactions.AddRange(vouchers.Select(x => new UserFinancialTransactionDto
                {
                    TransactionId = x.Id,
                    UserId = x.CreatorId,
                    ModuleName = "PaymentVoucher",
                    TransactionType = "سند صرف",
                    Amount = -x.Amount, // Negative to indicate outgoing
                    TransactionDate = x.Date,
                    Description = x.Description,
                    ReferenceNumber = x.VoucherNumber
                }));
            }

            // Map User Names
            var userIds = transactions.Where(x => x.UserId.HasValue).Select(x => x.UserId.Value).Distinct().ToList();
            var users = await _userRepository.GetListByIdsAsync(userIds);
            var userDict = users.ToDictionary(x => x.Id, x => x.Name ?? x.UserName);

            foreach (var tx in transactions)
            {
                if (tx.UserId.HasValue && userDict.TryGetValue(tx.UserId.Value, out var userName))
                {
                    tx.UserName = userName;
                }
                else
                {
                    tx.UserName = "نظام / غير معروف";
                }
            }

            // Sort and Paginate
            var sortedTransactions = transactions.OrderByDescending(x => x.TransactionDate).ToList();
            
            var paginatedTransactions = sortedTransactions
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToList();

            return new PagedResultDto<UserFinancialTransactionDto>(sortedTransactions.Count, paginatedTransactions);
        }
    }
}
