using System;
using System.Threading.Tasks;
using HIS.General.Dtos;
using HIS.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.General;

public class PaymentMethodAppService :
    CrudAppService<
        PaymentMethod, 
        PaymentMethodDto, 
        Guid, 
        PagedAndSortedResultRequestDto, 
        CreateUpdatePaymentMethodDto>, 
    IPaymentMethodAppService
{
    public PaymentMethodAppService(IRepository<PaymentMethod, Guid> repository) 
        : base(repository)
    {
        GetPolicyName = HISPermissions.Definitions.PaymentMethods;
        GetListPolicyName = HISPermissions.Definitions.PaymentMethods;
        CreatePolicyName = HISPermissions.Definitions.PaymentMethods;
        UpdatePolicyName = HISPermissions.Definitions.PaymentMethods;
        DeletePolicyName = HISPermissions.Definitions.PaymentMethods;
    }
}
