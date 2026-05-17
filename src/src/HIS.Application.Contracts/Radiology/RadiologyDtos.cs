using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Radiology;

public class RadiologyRequestDto : FullAuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; }
    public Guid? DoctorId { get; set; }
    public string DoctorName { get; set; }
    public Guid RadiologyItemId { get; set; }
    public string RadiologyItemName { get; set; }
    public DateTime RequestDate { get; set; }
    public RadiologyRequestStatus Status { get; set; }
    public string ReportBody { get; set; }
    public string TechnicianNotes { get; set; }
    public DateTime? ReportDate { get; set; }
    public string RequestNumber { get; set; }
    public string RequestingDepartmentName { get; set; }
    public string AdmissionRoom { get; set; }
}

public class CreateUpdateRadiologyRequestDto
{
    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public Guid RadiologyItemId { get; set; }
    public string ReportBody { get; set; }
    public string TechnicianNotes { get; set; }
    public RadiologyRequestStatus Status { get; set; }
}

public class GetRadiologyRequestInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public RadiologyRequestStatus? Status { get; set; }
}

public interface IRadiologyAppService : ICrudAppService<RadiologyRequestDto, Guid, GetRadiologyRequestInput, CreateUpdateRadiologyRequestDto>
{
    Task<List<RadiologyRequestDto>> GetPatientResultsAsync(Guid patientId);
    Task<Volo.Abp.Content.IRemoteStreamContent> GetRadiologyResultPdfAsync(Guid id);
}
