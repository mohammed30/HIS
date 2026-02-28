using System;
using System.Collections.Generic;

namespace HIS.Patients;

public class PatientServiceItemDto
{
    public DateTime Date { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string ServiceDescription { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
}

public class PatientServicesReportDto
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string MRN { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public List<PatientServiceItemDto> Services { get; set; } = new();
    
    public decimal TotalAmountInvoiced { get; set; }
    public decimal TotalAmountPaid { get; set; }
    public decimal TotalAmountDue { get; set; }
}
