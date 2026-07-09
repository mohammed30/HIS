using System;
using System.Collections.Generic;
using HIS.Billing;

namespace HIS.Pharmacy.Dtos;

public class PosSaleDto
{
    public Guid? PatientId { get; set; }
    public List<PosSaleItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
}

public class PosSaleItemDto
{
    public Guid DrugId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
}

public class PosProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Barcode { get; set; }
    public decimal Price { get; set; }
    public int CurrentStock { get; set; }
}

/// <summary>
/// DTO لعرض فاتورة نقطة البيع في القوائم
/// </summary>
public class PosInvoiceListDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string PatientName { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public InvoiceType InvoiceType { get; set; }
    public string? RejectionReason { get; set; }
    public string? OriginalInvoiceNumber { get; set; }
    public List<PosInvoiceItemDto> Items { get; set; } = new();
}

public class PosInvoiceItemDto
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string ServiceCode { get; set; }
}

/// <summary>
/// DTO لاعتماد الفاتورة وتسجيل الدفع (من المحاسب)
/// </summary>
public class PosApproveDto
{
    public decimal PaidAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// DTO لرفض الفاتورة (من المحاسب)
/// </summary>
public class PosRejectDto
{
    public string RejectionReason { get; set; }
}

/// <summary>
/// DTO للارتجاع الجزئي - يحتوي على الأصناف المراد إرجاعها
/// </summary>
public class PosPartialRefundDto
{
    public List<PosRefundItemDto> Items { get; set; } = new();
}

public class PosRefundItemDto
{
    /// <summary>معرف بند الفاتورة الأصلي</summary>
    public Guid InvoiceItemId { get; set; }
    /// <summary>الكمية المرتجعة</summary>
    public decimal ReturnQuantity { get; set; }
}

/// <summary>
/// نتيجة عملية الارتجاع
/// </summary>
public class PosRefundResultDto
{
    public Guid RefundInvoiceId { get; set; }
    public string RefundInvoiceNumber { get; set; }
    public decimal RefundAmount { get; set; }
}
