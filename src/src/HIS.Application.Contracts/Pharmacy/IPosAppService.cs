using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HIS.Billing;
using HIS.Pharmacy.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace HIS.Pharmacy;

public interface IPosAppService : IApplicationService
{
    // --- Product Lookup ---
    Task<PosProductDto> GetProductByBarcodeAsync(string barcode);
    Task<PosProductDto> GetProductByIdAsync(Guid id);
    Task<List<PosProductDto>> SearchProductsAsync(string query);

    // --- Sales Workflow ---

    /// <summary>إنشاء مسودة فاتورة (الصيدلي - الخطوة 1)</summary>
    Task<Guid> CreateDraftAsync(PosSaleDto input);

    /// <summary>إرسال الفاتورة للمحاسب (الصيدلي - الخطوة 3)</summary>
    Task SubmitForApprovalAsync(Guid invoiceId);

    /// <summary>اعتماد الفاتورة وتسجيل الدفع (المحاسب - الخطوة 5)</summary>
    Task ApproveAndPayAsync(Guid invoiceId, PosApproveDto input);

    /// <summary>رفض الفاتورة مع سبب الرفض (المحاسب - الخطوة 4 رفض)</summary>
    Task RejectAsync(Guid invoiceId, PosRejectDto input);

    /// <summary>تأكيد صرف الأصناف للمريض (الصيدلي - الخطوة 7)</summary>
    Task DispenseAsync(Guid invoiceId);

    // --- Return / Refund ---

    /// <summary>ارتجاع جزئي أو كلي لأصناف الفاتورة</summary>
    Task<PosRefundResultDto> PartialRefundAsync(Guid invoiceId, PosPartialRefundDto input);

    // --- Queries ---

    /// <summary>قائمة فواتير نقطة البيع مع إمكانية التصفية حسب الحالة</summary>
    Task<List<PosInvoiceListDto>> GetPosInvoicesAsync(InvoiceStatus? status = null);

    /// <summary>تفاصيل فاتورة واحدة</summary>
    Task<PosInvoiceListDto> GetInvoiceDetailsAsync(Guid invoiceId);

    // --- Printing ---
    Task<IRemoteStreamContent> GetInvoicePdfAsync(string idOrNumber);

    /// <summary>طباعة فاتورة الارتجاع بنسختين (أصل وصورة)</summary>
    Task<IRemoteStreamContent> GetReturnInvoicePdfAsync(Guid refundInvoiceId);

    // --- Legacy (kept for compatibility) ---
    Task<Guid> ProcessSaleAsync(PosSaleDto input);
    Task RefundSaleAsync(string invoiceNumber);
}
