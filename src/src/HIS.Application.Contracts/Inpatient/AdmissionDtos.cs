using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace HIS.Inpatient;

#region Admission DTOs
public class AdmissionDto : FullAuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string? PatientName { get; set; }
    public string? PatientFileNumber { get; set; }
    public Guid RoomId { get; set; }
    public string? RoomNumber { get; set; }
    public string? RoomTypeName { get; set; }
    public Guid? BedId { get; set; }
    public string? BedNumber { get; set; }
    public DateTime AdmissionDate { get; set; }
    public DateTime? DischargeDate { get; set; }
    public int NumberOfDays { get; set; }
    public decimal InsuranceCeiling { get; set; }
    public string? CompanionName { get; set; }
    public string? CompanionPhone { get; set; }
    public string? CompanionAddress { get; set; }
    public string? Purpose { get; set; }
    public decimal PharmacyPercentage { get; set; }
    public bool IsServicesStopped { get; set; }
    public AdmissionStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal InsuranceAmount { get; set; }
    public decimal DueAmount { get; set; }
    public string? Notes { get; set; }
    public Guid? InvoiceId { get; set; }
}

public class CreateUpdateAdmissionDto
{
    public Guid PatientId { get; set; }
    public Guid RoomId { get; set; }
    public Guid BedId { get; set; }
    public decimal InsuranceCeiling { get; set; }
    public string? CompanionName { get; set; }
    public string? CompanionPhone { get; set; }
    public string? CompanionAddress { get; set; }
    public string? Purpose { get; set; }
    public decimal PharmacyPercentage { get; set; }
    public bool IsServicesStopped { get; set; }
    public string? Notes { get; set; }
}

public class DischargeAdmissionDto
{
    public DateTime DischargeDate { get; set; }
    public string? Notes { get; set; }
}

public class GetAdmissionsInput : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public Guid? PatientId { get; set; }
    public AdmissionStatus? Status { get; set; }
    public Guid? RoomId { get; set; }
    public int? RoomTypeId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
#endregion

#region Interface
public interface IAdmissionAppService
{
}
#endregion
