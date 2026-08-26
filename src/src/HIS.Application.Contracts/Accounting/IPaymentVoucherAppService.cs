using System;
using HIS.Accounting.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Accounting
{
    public interface IPaymentVoucherAppService : 
        ICrudAppService<
            PaymentVoucherDto, 
            Guid, 
            VoucherFilterDto, 
            CreateUpdatePaymentVoucherDto>
    {
    }
}
