using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace HIS.Reports;

public class PaidTicketDto
{
    public Guid AppointmentId { get; set; }
    public string TicketNumber { get; set; }
    public string PatientName { get; set; }
    public string ClinicName { get; set; }
    public string DoctorName { get; set; }
    public string ServiceName { get; set; }
    public decimal Amount { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string CreatedByUser { get; set; }
    public DateTime CreationTime { get; set; }
}

public class GetPaidTicketsInput : PagedAndSortedResultRequestDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? CreatorUser { get; set; }
}

public class PharmacySalesDto
{
    public Guid DispensingId { get; set; }
    public string PatientName { get; set; }
    public string ProductName { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public string WithdrawalType { get; set; } // e.g., "From Warehouse", "POS"
    public string CreatedByUser { get; set; }
    public DateTime DispensingTime { get; set; }
}

public class GetPharmacySalesInput : PagedAndSortedResultRequestDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
