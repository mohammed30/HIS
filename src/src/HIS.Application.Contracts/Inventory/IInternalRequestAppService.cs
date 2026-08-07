using System;
using System.Threading.Tasks;
using HIS.Inventory.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Inventory;

public interface IInternalRequestAppService : ICrudAppService<InternalRequestDto, Guid, InternalRequestGetListInput, CreateUpdateInternalRequestDto>
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

    /// <summary>
    /// إرجاع كميات من أصناف سبق صرفها لمريض منوم (مرتجع) - ينشئ طلب مرتجع قيد الانتظار
    /// </summary>
    Task<InternalRequestDto> ReturnItemsAsync(ReturnInternalRequestDto input);

    /// <summary>
    /// يتم استدعاؤه من قبل الصيدلية للموافقة على المرتجع وتنفيذ الحركات المخزنية والمالية
    /// </summary>
    Task<InternalRequestDto> ApproveReturnAsync(Guid requestId);

    /// <summary>
    /// جلب طلبات المرتجعات المعلقة للموافقة
    /// </summary>
    Task<PagedResultDto<InternalRequestDto>> GetPendingReturnsAsync(PagedAndSortedResultRequestDto input);
}
