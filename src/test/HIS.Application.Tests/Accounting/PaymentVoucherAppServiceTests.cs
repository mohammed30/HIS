using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting.Dtos;
using HIS.General;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace HIS.Accounting.Tests;

public abstract class PaymentVoucherAppServiceTests<TStartupModule> : AccountingTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.AbpModule
{
    private readonly IPaymentVoucherAppService _paymentVoucherAppService;
    private readonly IRepository<PaymentVoucher, Guid> _paymentVoucherRepository;
    private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
    private readonly IRepository<PaymentMethod, Guid> _paymentMethodRepository;

    protected PaymentVoucherAppServiceTests()
    {
        _paymentVoucherAppService = GetRequiredService<IPaymentVoucherAppService>();
        _paymentVoucherRepository = GetRequiredService<IRepository<PaymentVoucher, Guid>>();
        _journalEntryRepository = GetRequiredService<IRepository<JournalEntry, Guid>>();
        _paymentMethodRepository = GetRequiredService<IRepository<PaymentMethod, Guid>>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Voucher_And_Post_Journal_Entry()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();
        });

        Guid paymentMethodId = Guid.NewGuid();
        Guid expenseAccountId = Guid.Empty;

        await WithUnitOfWorkAsync(async () =>
        {
            var pm = new PaymentMethod(paymentMethodId, "Bank Transfer", "Bank Transfer");
            await _paymentMethodRepository.InsertAsync(pm);
            
            var expenseAcc = await AccountRepository.FirstOrDefaultAsync(x => x.Code == "5200"); // COGS Expense
            expenseAccountId = expenseAcc.Id;
        });

        var dto = new CreateUpdatePaymentVoucherDto
        {
            PayeeName = "Medical Supplier Inc",
            PaymentMethodId = paymentMethodId,
            Amount = 1000.0m,
            Date = DateTime.Now,
            Description = "Payment for supplies",
            Lines = new List<CreateUpdatePaymentVoucherLineDto>
            {
                new CreateUpdatePaymentVoucherLineDto
                {
                    AccountId = expenseAccountId,
                    Amount = 1000.0m,
                    Description = "Supplies expense"
                }
            }
        };

        Guid voucherId = Guid.Empty;

        await WithUnitOfWorkAsync(async () =>
        {
            var result = await _paymentVoucherAppService.CreateAsync(dto);
            result.ShouldNotBeNull();
            result.Amount.ShouldBe(1000.0m);
            result.VoucherNumber.ShouldNotBeNullOrEmpty();
            
            voucherId = result.Id;
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var saved = await _paymentVoucherRepository.GetAsync(voucherId);
            saved.Amount.ShouldBe(1000.0m);

            // Check if Journal Entry was created
            var entries = await _journalEntryRepository.GetListAsync(x => x.ReferenceNumber == saved.VoucherNumber);
            entries.Count.ShouldBe(1);
        });
    }
}
