using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace HIS.Laboratory.Dtos;

public class LabTestCategoryDto : AuditedEntityDto<Guid>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public List<LabTestCategoryDto> Children { get; set; } = new();
    public List<LabTestDto> Tests { get; set; } = new();
}

public class LabTestDto : AuditedEntityDto<Guid>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string? Instructions { get; set; }
    public string? ReferenceRange { get; set; }
    public string? Unit { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUpdateLabTestDto
{
    public string? Code { get; set; }  // Auto-generated on create
    
    [Required]
    public string Name { get; set; }
    
    public decimal Price { get; set; }
    public string? Instructions { get; set; }
    public string? ReferenceRange { get; set; }
    public string? Unit { get; set; }
    public Guid? CategoryId { get; set; }
    public bool IsActive { get; set; }
}

public class LabRequestDto : AuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; }
    public Guid? DoctorId { get; set; }
    public string? DoctorName { get; set; }
    public bool IsExternalDoctor { get; set; }
    public string? ExternalDoctorName { get; set; }
    public Guid ServiceItemId { get; set; }
    public string TestName { get; set; }
    public string TestCode { get; set; }
    
    public DateTime RequestDate { get; set; }
    public LabRequestStatus Status { get; set; }
    public string? SampleNumber { get; set; }
    public string Result { get; set; }
    public string Notes { get; set; }
    public string? RequestingDepartmentName { get; set; }
    public string? AdmissionRoom { get; set; }
    public string? ReferenceRange { get; set; }
    public string? Unit { get; set; }
}

public class CreateLabRequestDto
{
    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public bool IsExternalDoctor { get; set; }
    public string? ExternalDoctorName { get; set; }
    public Guid ServiceItemId { get; set; }
    public string Notes { get; set; }
}

public class UpdateLabResultDto
{
    public string Result { get; set; }
    public string Notes { get; set; }
}

public class GetLabRequestsInput : PagedAndSortedResultRequestDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Filter { get; set; }
    public LabRequestStatus? Status { get; set; }
}

// --- Lab Appointments DTOs ---

public class LabAppointmentDto : AuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; }
    public Guid? ServiceItemId { get; set; }
    public string? TestName { get; set; }
    public string? TestCode { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan? PreferredTime { get; set; }
    public LabAppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? PreparationInstructions { get; set; }
    public bool IsFasting { get; set; }
}

public class CreateLabAppointmentDto
{
    public Guid PatientId { get; set; }
    public Guid? ServiceItemId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan? PreferredTime { get; set; }
    public string? Notes { get; set; }
    public bool IsFasting { get; set; }
}

public class UpdateLabAppointmentDto
{
    public Guid? ServiceItemId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan? PreferredTime { get; set; }
    public string? Notes { get; set; }
    public bool IsFasting { get; set; }
}
