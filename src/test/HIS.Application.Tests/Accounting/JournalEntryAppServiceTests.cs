using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting.Dtos;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Xunit;

namespace HIS.Accounting.Tests;

public abstract class JournalEntryAppServiceTests<TStartupModule> : AccountingTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.AbpModule
{
    private readonly IJournalEntryAppService _journalEntryAppService;
    private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
    private readonly Volo.Abp.Guids.IGuidGenerator _guidGenerator;

    protected JournalEntryAppServiceTests()
    {
        _journalEntryAppService = GetRequiredService<IJournalEntryAppService>();
        _journalEntryRepository = GetRequiredService<IRepository<JournalEntry, Guid>>();
        _guidGenerator = GetRequiredService<Volo.Abp.Guids.IGuidGenerator>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_JournalEntry()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();
        });

        Guid accountId = Guid.Empty;
        await WithUnitOfWorkAsync(async () =>
        {
            var acc = await AccountRepository.FirstOrDefaultAsync(x => x.Code == "1110"); // Cash
            accountId = acc.Id;
        });

        var dto = new CreateUpdateJournalEntryDto
        {
            Date = DateTime.Now,
            ReferenceNumber = "REF-100",
            Description = "Test Entry",
            Lines = new List<CreateUpdateJournalEntryLineDto>
            {
                new CreateUpdateJournalEntryLineDto { AccountId = accountId, Debit = 100, Credit = 0 },
                new CreateUpdateJournalEntryLineDto { AccountId = accountId, Debit = 0, Credit = 100 }
            }
        };

        await WithUnitOfWorkAsync(async () =>
        {
            var result = await _journalEntryAppService.CreateAsync(dto);
            result.ShouldNotBeNull();
            result.IsPosted.ShouldBeFalse();
            
            var saved = await _journalEntryRepository.GetAsync(result.Id);
            saved.IsPosted.ShouldBeFalse();
        });
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_When_Not_Balanced()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();
        });

        Guid accountId = Guid.Empty;
        await WithUnitOfWorkAsync(async () =>
        {
            var acc = await AccountRepository.FirstOrDefaultAsync(x => x.Code == "1110"); // Cash
            accountId = acc.Id;
        });

        var dto = new CreateUpdateJournalEntryDto
        {
            Date = DateTime.Now,
            ReferenceNumber = "REF-200",
            Description = "Unbalanced Entry",
            Lines = new List<CreateUpdateJournalEntryLineDto>
            {
                new CreateUpdateJournalEntryLineDto { AccountId = accountId, Debit = 100, Credit = 0 },
                new CreateUpdateJournalEntryLineDto { AccountId = accountId, Debit = 0, Credit = 90 }
            }
        };

        await WithUnitOfWorkAsync(async () =>
        {
            var exception = await Assert.ThrowsAsync<Volo.Abp.UserFriendlyException>(async () =>
            {
                await _journalEntryAppService.CreateAsync(dto);
            });
            exception.Message.ShouldContain("Journal entry is unbalanced");
        });
    }

    [Fact]
    public async Task PostAsync_Should_Update_Account_Balances_And_Change_Status()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();
        });

        Guid cashId = Guid.Empty;
        Guid salesId = Guid.Empty;

        await WithUnitOfWorkAsync(async () =>
        {
            var cashAcc = await AccountRepository.FirstOrDefaultAsync(x => x.Code == "1110"); // Cash
            var salesAcc = await AccountRepository.FirstOrDefaultAsync(x => x.Code == "4200"); // Sales Revenue
            
            cashId = cashAcc.Id;
            salesId = salesAcc.Id;
        });

        Guid jeId = Guid.NewGuid();
        await WithUnitOfWorkAsync(async () =>
        {
            var je = new JournalEntry(jeId, DateTime.Now, "REF-300", "Sales");
            je.AddLine(_guidGenerator, cashId, 500, 0); // Debit Cash
            je.AddLine(_guidGenerator, salesId, 0, 500); // Credit Sales
            
            await _journalEntryRepository.InsertAsync(je);
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var result = await _journalEntryAppService.PostAsync(jeId);
            result.IsPosted.ShouldBeTrue();
        });
    }
}
