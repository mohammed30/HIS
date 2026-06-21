using System;
using System.Collections.Generic;
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

        protected override async Task<BankTransactionDto> MapToGetOutputDtoAsync(BankTransaction entity)
        {
            var dto = await base.MapToGetOutputDtoAsync(entity);
            
            if (dto.BankAccountId.HasValue && dto.BankAccountId.Value != Guid.Empty)
            {
                var bankAccountId = dto.BankAccountId.Value;
                var bankAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Id == bankAccountId);
                dto.BankAccountName = bankAccount?.Name;
            }
            if (dto.OppositeAccountId.HasValue && dto.OppositeAccountId.Value != Guid.Empty)
            {
                var oppositeAccountId = dto.OppositeAccountId.Value;
                var oppAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Id == oppositeAccountId);
                dto.OppositeAccountName = oppAccount?.Name;
            }

            return dto;
        }

        protected override async Task<BankTransactionDto> MapToGetListOutputDtoAsync(BankTransaction entity)
        {
            var dto = await base.MapToGetListOutputDtoAsync(entity);
            
            if (dto.BankAccountId.HasValue && dto.BankAccountId.Value != Guid.Empty)
            {
                var bankAccountId = dto.BankAccountId.Value;
                var bankAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Id == bankAccountId);
                dto.BankAccountName = bankAccount?.Name;
            }
            if (dto.OppositeAccountId.HasValue && dto.OppositeAccountId.Value != Guid.Empty)
            {
                var oppositeAccountId = dto.OppositeAccountId.Value;
                var oppAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Id == oppositeAccountId);
                dto.OppositeAccountName = oppAccount?.Name;
            }

            return dto;
        }
    }
}
