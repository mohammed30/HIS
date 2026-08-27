using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting;
using HIS.Billing;
using HIS.Reports;
using NSubstitute;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Xunit;
using System.Runtime.Serialization;

namespace HIS.Application.Tests.Reports
{
    public class UserFinancialReportAppServiceTests
    {
        private readonly IRepository<Payment, Guid> _paymentRepository;
        private readonly IRepository<InpatientDeposit, Guid> _inpatientDepositRepository;
        private readonly IRepository<ReceiptVoucher, Guid> _receiptVoucherRepository;
        private readonly IRepository<PaymentVoucher, Guid> _paymentVoucherRepository;
        private readonly IIdentityUserRepository _userRepository;
        private readonly UserFinancialReportAppService _service;

        public UserFinancialReportAppServiceTests()
        {
            _paymentRepository = Substitute.For<IRepository<Payment, Guid>>();
            _inpatientDepositRepository = Substitute.For<IRepository<InpatientDeposit, Guid>>();
            _receiptVoucherRepository = Substitute.For<IRepository<ReceiptVoucher, Guid>>();
            _paymentVoucherRepository = Substitute.For<IRepository<PaymentVoucher, Guid>>();
            _userRepository = Substitute.For<IIdentityUserRepository>();

            _service = new UserFinancialReportAppService(
                _paymentRepository,
                _inpatientDepositRepository,
                _receiptVoucherRepository,
                _paymentVoucherRepository,
                _userRepository
            );
        }
        
        private T CreateEntity<T>(Guid id)
        {
            var entity = (T)FormatterServices.GetUninitializedObject(typeof(T));
            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty != null && idProperty.CanWrite)
            {
                idProperty.SetValue(entity, id);
            }
            return entity;
        }

        [Fact]
        public async Task GetListAsync_Should_Aggregate_Transactions_Across_Modules()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var today = DateTime.UtcNow.Date;

            // Mock Data
            var paymentId = Guid.NewGuid();
            var payment = CreateEntity<Payment>(paymentId);
            payment.CreatorId = userId;
            payment.CreationTime = today;
            payment.PaymentDate = today;
            payment.Amount = 500;
            payment.PaymentMethod = PaymentMethod.Cash;
            payment.Invoice = CreateEntity<Invoice>(Guid.NewGuid());
            payment.Invoice.InvoiceNumber = "INV-1";

            var depositId = Guid.NewGuid();
            var deposit = CreateEntity<InpatientDeposit>(depositId);
            deposit.CreatorId = userId;
            deposit.CreationTime = today.AddHours(1);
            deposit.DepositDate = today;
            deposit.Amount = 1000;
            deposit.PaymentMethod = PaymentMethod.Cash;

            var receiptId = Guid.NewGuid();
            var receipt = CreateEntity<ReceiptVoucher>(receiptId);
            receipt.CreatorId = userId;
            receipt.CreationTime = today.AddHours(2);
            receipt.Date = today;
            receipt.Amount = 300;
            receipt.VoucherNumber = "RV-1";

            var paymentVouchId = Guid.NewGuid();
            var paymentVouch = CreateEntity<PaymentVoucher>(paymentVouchId);
            paymentVouch.CreatorId = userId;
            paymentVouch.CreationTime = today.AddHours(3);
            paymentVouch.Date = today;
            paymentVouch.Amount = 150;
            paymentVouch.VoucherNumber = "PV-1";

            var user = new IdentityUser(userId, "testuser", "test@test.com");

            _paymentRepository.GetQueryableAsync().Returns(Task.FromResult(new List<Payment> { payment }.AsQueryable()));
            _inpatientDepositRepository.GetQueryableAsync().Returns(Task.FromResult(new List<InpatientDeposit> { deposit }.AsQueryable()));
            _receiptVoucherRepository.GetQueryableAsync().Returns(Task.FromResult(new List<ReceiptVoucher> { receipt }.AsQueryable()));
            _paymentVoucherRepository.GetQueryableAsync().Returns(Task.FromResult(new List<PaymentVoucher> { paymentVouch }.AsQueryable()));
            
            _paymentRepository.WithDetailsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Payment, object>>[]>()).Returns(Task.FromResult(new List<Payment> { payment }.AsQueryable()));

            _userRepository.GetListByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
                .ReturnsForAnyArgs(Task.FromResult(new List<IdentityUser> { user }));
            
            _userRepository.FindAsync(userId, Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>()).Returns(Task.FromResult(user));

            var input = new GetUserFinancialTransactionsInput
            {
                UserId = userId,
                StartDate = today,
                EndDate = today,
                SkipCount = 0,
                MaxResultCount = 100
            };

            // Act
            var result = await _service.GetListAsync(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.TotalCount); // 4 transactions

            // Verify totals
            var cashPayments = result.Items.Where(x => x.Amount > 0).Sum(x => x.Amount); // Incoming
            var cashVouchersOut = result.Items.Where(x => x.Amount < 0).Sum(x => Math.Abs(x.Amount)); // Outgoing

            Assert.Equal(1800, cashPayments); // 500 (Payment) + 1000 (Deposit) + 300 (Receipt)
            Assert.Equal(150, cashVouchersOut); // 150 (PaymentVoucher)
        }
        
        [Fact]
        public async Task GetListAsync_Should_Filter_By_ModuleName()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var today = DateTime.UtcNow.Date;

            // Mock Data
            var paymentId2 = Guid.NewGuid();
            var payment = CreateEntity<Payment>(paymentId2);
            payment.CreatorId = userId;
            payment.CreationTime = today;
            payment.PaymentDate = today;
            payment.Amount = 500;
            payment.PaymentMethod = PaymentMethod.BankTransfer;
            payment.Invoice = CreateEntity<Invoice>(Guid.NewGuid());
            payment.Invoice.InvoiceNumber = "INV-1";

            var receiptId2 = Guid.NewGuid();
            var receipt = CreateEntity<ReceiptVoucher>(receiptId2);
            receipt.CreatorId = userId;
            receipt.CreationTime = today.AddHours(2);
            receipt.Date = today;
            receipt.Amount = 300;
            receipt.VoucherNumber = "RV-1";

            _paymentRepository.GetQueryableAsync().Returns(Task.FromResult(new List<Payment> { payment }.AsQueryable()));
            _inpatientDepositRepository.GetQueryableAsync().Returns(Task.FromResult(new List<InpatientDeposit>().AsQueryable()));
            _receiptVoucherRepository.GetQueryableAsync().Returns(Task.FromResult(new List<ReceiptVoucher> { receipt }.AsQueryable()));
            _paymentVoucherRepository.GetQueryableAsync().Returns(Task.FromResult(new List<PaymentVoucher>().AsQueryable()));
            _paymentRepository.WithDetailsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Payment, object>>[]>()).Returns(Task.FromResult(new List<Payment> { payment }.AsQueryable()));

            var filterUser = new IdentityUser(userId, "testuser", "test@test.com");
            _userRepository.GetListByIdsAsync(Arg.Any<IEnumerable<Guid>>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
                .ReturnsForAnyArgs(Task.FromResult(new List<IdentityUser> { filterUser }));

            var input = new GetUserFinancialTransactionsInput
            {
                ModuleName = "Payment", // Only payments should be returned
                StartDate = today,
                EndDate = today
            };

            // Act
            var result = await _service.GetListAsync(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal("Payment", result.Items.First().ModuleName);
            Assert.Equal(500, result.Items.First().Amount);
        }
    }
}
