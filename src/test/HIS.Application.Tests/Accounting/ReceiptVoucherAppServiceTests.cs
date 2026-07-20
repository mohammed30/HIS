using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting.Dtos;
using HIS.General;
using HIS.Patients;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace HIS.Accounting.Tests;

public abstract class ReceiptVoucherAppServiceTests<TStartupModule> : AccountingTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.AbpModule
{
    private readonly IReceiptVoucherAppService _receiptVoucherAppService;
    private readonly IRepository<ReceiptVoucher, Guid> _receiptVoucherRepository;
    private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<PaymentMethod, Guid> _paymentMethodRepository;

    protected ReceiptVoucherAppServiceTests()
    {
        _receiptVoucherAppService = GetRequiredService<IReceiptVoucherAppService>();
        _receiptVoucherRepository = GetRequiredService<IRepository<ReceiptVoucher, Guid>>();
        _journalEntryRepository = GetRequiredService<IRepository<JournalEntry, Guid>>();
        _patientRepository = GetRequiredService<IRepository<Patient, Guid>>();
        _paymentMethodRepository = GetRequiredService<IRepository<PaymentMethod, Guid>>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Voucher_And_Post_Journal_Entry()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();
        });

        Guid patientId = Guid.NewGuid();
        Guid paymentMethodId = Guid.NewGuid();
        Guid revenueAccountId = Guid.Empty;

        await WithUnitOfWorkAsync(async () =>
        {
            var pt = new Patient(patientId, null, "PT-02", "Omar", "B", new DateTime(1985, 1, 1), Gender.Male, IdentityType.NationalId, "12345", "050");
            await _patientRepository.InsertAsync(pt);

            var pm = new PaymentMethod(paymentMethodId, "Cash", "Cash");
            await _paymentMethodRepository.InsertAsync(pm);

            var revenueAcc = await AccountRepository.FirstOrDefaultAsync(x => x.Code == "4200");
            revenueAccountId = revenueAcc.Id;
        });

        var dto = new CreateUpdateReceiptVoucherDto
        {
            PatientId = patientId,
            PayerName = "Omar B",
            PaymentMethodId = paymentMethodId,
            Amount = 150.0m,
            Date = DateTime.Now,
            Description = "Deposit for admission",
            Lines = new List<CreateUpdateReceiptVoucherLineDto>
            {
                new CreateUpdateReceiptVoucherLineDto
                {
                    AccountId = revenueAccountId,
                    Amount = 150.0m,
                    Description = "Consultation Revenue"
                }
            }
        };

        Guid voucherId = Guid.Empty;

        await WithUnitOfWorkAsync(async () =>
        {
            var result = await _receiptVoucherAppService.CreateAsync(dto);
            result.ShouldNotBeNull();
            result.Amount.ShouldBe(150.0m);
            result.VoucherNumber.ShouldNotBeNullOrEmpty();
            
            voucherId = result.Id;
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var saved = await _receiptVoucherRepository.GetAsync(voucherId);
            saved.Amount.ShouldBe(150.0m);

            // Check if Journal Entry was created
            var entries = await _journalEntryRepository.GetListAsync(x => x.ReferenceNumber == saved.VoucherNumber);
            entries.Count.ShouldBe(1);
        });
    }
}
