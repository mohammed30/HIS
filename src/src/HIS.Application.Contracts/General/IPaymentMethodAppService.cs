using System;
using HIS.General.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.General;

public interface IPaymentMethodAppService : ICrudAppService<
    PaymentMethodDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateUpdatePaymentMethodDto>
{

}
