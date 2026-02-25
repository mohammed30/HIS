using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Operations;

#region SurgicalOperation DTOs
public class SurgicalOperationDto : FullAuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string? PatientName { get; set; }
    public Guid? DoctorId { get; set; }
    public string? DoctorName { get; set; }
    public Guid? OperationTypeId { get; set; }
    public string OperationName { get; set; } = string.Empty;
    public string? SpecialtyName { get; set; }
    public DateTime OperationDate { get; set; }
    public string? Details { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CompanyShare { get; set; }
    public decimal PatientShare { get; set; }
    public decimal InsuranceTotal { get; set; }
    public OperationStatus Status { get; set; }
    public Guid? InvoiceId { get; set; }
    public Guid? AdmissionId { get; set; }
    public string? Notes { get; set; }
}

public class CreateUpdateSurgicalOperationDto
{
    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public Guid? OperationTypeId { get; set; }
    public string OperationName { get; set; } = string.Empty;
    public DateTime OperationDate { get; set; }
    public string? Details { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CompanyShare { get; set; }
    public decimal PatientShare { get; set; }
    public OperationStatus Status { get; set; } = OperationStatus.Scheduled;
    public Guid? AdmissionId { get; set; }
    public string? Notes { get; set; }
}

public class GetSurgicalOperationsInput : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public Guid? PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public OperationStatus? Status { get; set; }
    public Guid? SpecialtyId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
#endregion

#region Interface
public interface ISurgicalOperationAppService : Volo.Abp.Application.Services.ICrudAppService<
    SurgicalOperationDto,
    Guid,
    GetSurgicalOperationsInput,
    CreateUpdateSurgicalOperationDto>
{
    System.Threading.Tasks.Task<Volo.Abp.Content.IRemoteStreamContent> GetOperationTicketPdfAsync(Guid id);
    System.Threading.Tasks.Task<SurgicalOperationDto> UpdateStatusAsync(Guid id, OperationStatus status);
}
#endregion
