using System;
using System.Threading.Tasks;
using HIS.Inventory.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Inventory;

public interface IInternalRequestAppService : ICrudAppService<InternalRequestDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateInternalRequestDto>
{
    /// <summary>
    /// يتم استدعاؤه من قبل الصيدلية أو التمريض لإرسال الطلب للاعتماد
    /// </summary>
    Task<InternalRequestDto> SubmitRequestAsync(Guid id);

    /// <summary>
    /// يتم استدعاؤه من قبل أمين المستودع الرئيسي للموافقة وصرف الكميات
    /// </summary>
    Task<InternalRequestDto> ApproveAndFulfillAsync(Guid id);

    /// <summary>
    /// يتم استدعاؤه من قبل القسم الطالب لتأكيد استلام الكميات
    /// </summary>
    Task<InternalRequestDto> ConfirmReceiptAsync(Guid id);

    /// <summary>
    /// إلغاء الطلب من قبل التمريض وعكس القيود المالية إن وجدت
    /// </summary>
    Task<InternalRequestDto> CancelRequestAsync(Guid id);
}
