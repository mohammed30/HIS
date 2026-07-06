using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting.Dtos;
using HIS.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Accounting
{
    [Authorize(HISPermissions.Billing.JournalEntries)]
    public class BankTransactionAppService : 
        CrudAppService<
            BankTransaction, 
            BankTransactionDto, 
            Guid, 
            PagedAndSortedResultRequestDto, 
            CreateUpdateBankTransactionDto>, 
        IBankTransactionAppService
    {
        private readonly IJournalEntryAppService _journalEntryAppService;
        private readonly IRepository<Account, Guid> _accountRepository;

        public BankTransactionAppService(
            IRepository<BankTransaction, Guid> repository,
            IJournalEntryAppService journalEntryAppService,
            IRepository<Account, Guid> accountRepository) 
            : base(repository)
        {
            _journalEntryAppService = journalEntryAppService;
            _accountRepository = accountRepository;
        }

        public override async Task<BankTransactionDto> CreateAsync(CreateUpdateBankTransactionDto input)
        {
            var entity = await base.CreateAsync(input);

            // Create Journal Entry
            var jeDto = new CreateUpdateJournalEntryDto
            {
                Date = input.Date,
                Description = input.Description ?? "Bank Transaction " + input.ReferenceNumber,
                Lines = new List<CreateUpdateJournalEntryLineDto>
                {
                    new CreateUpdateJournalEntryLineDto
                    {
                        AccountId = input.BankAccountId,
                        Debit = input.TransactionType == BankTransactionType.Deposit ? input.Amount : 0,
                        Credit = input.TransactionType == BankTransactionType.Withdrawal ? input.Amount : 0
                    },
                    new CreateUpdateJournalEntryLineDto
                    {
                        AccountId = input.OppositeAccountId,
                        Debit = input.TransactionType == BankTransactionType.Withdrawal ? input.Amount : 0,
                        Credit = input.TransactionType == BankTransactionType.Deposit ? input.Amount : 0
                    }
                }
            };
            
            var je = await _journalEntryAppService.CreateAsync(jeDto);
            
            // Link JE
            var domainEntity = await Repository.GetAsync(entity.Id);
            domainEntity.RelatedJournalEntryId = je.Id;
            await Repository.UpdateAsync(domainEntity);
            entity.RelatedJournalEntryId = je.Id;

            return entity;
        }

        public override async Task<BankTransactionDto> GetAsync(Guid id)
        {
            var entity = await Repository.GetAsync(id);
            var dto = ObjectMapper.Map<BankTransaction, BankTransactionDto>(entity);
            
            if (dto.BankAccountId.HasValue && dto.BankAccountId.Value != Guid.Empty)
            {
                var bankAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Id == dto.BankAccountId.Value);
                dto.BankAccountName = bankAccount?.Name;
            }
            if (dto.OppositeAccountId.HasValue && dto.OppositeAccountId.Value != Guid.Empty)
            {
                var oppAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Id == dto.OppositeAccountId.Value);
                dto.OppositeAccountName = oppAccount?.Name;
            }

            return dto;
        }

        public override async Task<PagedResultDto<BankTransactionDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            try
            {
                var query = await CreateFilteredQueryAsync(input);
                var totalCount = await AsyncExecuter.CountAsync(query);
                
                query = ApplySorting(query, input);
                query = ApplyPaging(query, input);
                
                var entities = await AsyncExecuter.ToListAsync(query);

                var dtos = ObjectMapper.Map<List<BankTransaction>, List<BankTransactionDto>>(entities);

                var accountIds = new HashSet<Guid>();
                foreach (var e in entities)
                {
                    if (e.BankAccountId != Guid.Empty) accountIds.Add(e.BankAccountId);
                    if (e.OppositeAccountId != Guid.Empty) accountIds.Add(e.OppositeAccountId);
                }

                if (accountIds.Count > 0)
                {
                    var accounts = await _accountRepository.GetListAsync(x => accountIds.Contains(x.Id));
                    var accountDict = accounts.ToDictionary(x => x.Id, x => x.Name);

                    foreach (var dto in dtos)
                    {
                        if (dto.BankAccountId.HasValue && accountDict.TryGetValue(dto.BankAccountId.Value, out var bankName))
                            dto.BankAccountName = bankName;

                        if (dto.OppositeAccountId.HasValue && accountDict.TryGetValue(dto.OppositeAccountId.Value, out var oppName))
                            dto.OppositeAccountName = oppName;
                    }
                }

                return new PagedResultDto<BankTransactionDto>(totalCount, dtos);
            }
            catch (Exception ex)
            {
                throw new Volo.Abp.UserFriendlyException($"Debug Error: {ex.Message} | Inner: {ex.InnerException?.Message} | Stack: {ex.StackTrace}");
            }
        }
    }
}
