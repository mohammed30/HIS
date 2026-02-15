using System;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting.Dtos;
using HIS.General;
using HIS.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Accounting
{
    [Authorize(HISPermissions.Billing.ManageInvoices)]
    public class ContractClaimAppService : 
        CrudAppService<
            ContractClaim, 
            ContractClaimDto, 
            Guid, 
            PagedAndSortedResultRequestDto, 
            CreateUpdateContractClaimDto>, 
        IContractClaimAppService
    {
        private readonly IRepository<Contract, Guid> _contractRepository;

        public ContractClaimAppService(
            IRepository<ContractClaim, Guid> repository,
            IRepository<Contract, Guid> contractRepository) 
            : base(repository)
        {
            _contractRepository = contractRepository;
        }

        public override async Task<ContractClaimDto> GetAsync(Guid id)
        {
            var dto = await base.GetAsync(id);
            var contract = await _contractRepository.FindAsync(dto.ContractId);
            dto.ContractName = contract?.NameAr; // Prefer Arabic or English based on context, here taking Ar
            return dto;
        }

        public override async Task<ContractClaimDto> CreateAsync(CreateUpdateContractClaimDto input)
        {
            var entity = MapToEntity(input);
            entity.ClaimNumber = "CLM-" + DateTime.Now.Ticks.ToString().Substring(10);
            
            await Repository.InsertAsync(entity, autoSave: true);
            
            return await GetAsync(entity.Id);
        }
    }
}
