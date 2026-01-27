using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Settings;

#region Department DTOs
public class DepartmentDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? ExtensionNumber { get; set; }
    public Guid? ManagerId { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    public bool IsMedical { get; set; }
}

public class CreateUpdateDepartmentDto
{
    public string? Code { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? ExtensionNumber { get; set; }
    public Guid? ManagerId { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
    public bool IsMedical { get; set; } = false;
}

public class GetDepartmentsInput : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsMedical { get; set; }
}
#endregion

#region Specialty DTOs
public class SpecialtyDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class CreateUpdateSpecialtyDto
{
    public string? Code { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}

public class GetSpecialtiesInput : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public bool? IsActive { get; set; }
}
#endregion

#region Clinic DTOs
public class ClinicDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public Guid DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? Location { get; set; }
    public string? RoomNumber { get; set; }
    public string? ExtensionNumber { get; set; }
    public int Capacity { get; set; }
    public int AppointmentDuration { get; set; }
    public decimal ConsultationFee { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class CreateUpdateClinicDto
{
    public string? Code { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public Guid DepartmentId { get; set; }
    public string? Location { get; set; }
    public string? RoomNumber { get; set; }
    public string? ExtensionNumber { get; set; }
    public int Capacity { get; set; } = 4;
    public int AppointmentDuration { get; set; } = 15;
    public decimal ConsultationFee { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}

public class GetClinicsInput : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public Guid? DepartmentId { get; set; }
    public bool? IsActive { get; set; }
}
#endregion

#region Doctor DTOs
public class DoctorDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public Guid SpecialtyId { get; set; }
    public string? SpecialtyName { get; set; }
    public Guid DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string? LicenseNumber { get; set; }
    public DateTime? LicenseExpiryDate { get; set; }
    public string? MobileNumber { get; set; }
    public string? Email { get; set; }
    public string? Degree { get; set; }
    public decimal ConsultationFee { get; set; }
    public decimal FollowUpFee { get; set; }
    public int AppointmentDuration { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Bio { get; set; }
    public Guid? UserId { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class CreateUpdateDoctorDto
{
    public string? Code { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public Guid SpecialtyId { get; set; }
    public Guid DepartmentId { get; set; }
    public string? LicenseNumber { get; set; }
    public DateTime? LicenseExpiryDate { get; set; }
    public string? MobileNumber { get; set; }
    public string? Email { get; set; }
    public string? Degree { get; set; }
    public decimal ConsultationFee { get; set; }
    public decimal FollowUpFee { get; set; }
    public int AppointmentDuration { get; set; } = 15;
    public string? Bio { get; set; }
    public Guid? UserId { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}

public class GetDoctorsInput : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public Guid? SpecialtyId { get; set; }
    public Guid? DepartmentId { get; set; }
    public bool? IsActive { get; set; }
}
#endregion

#region Laboratory DTOs
public class LaboratoryDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? ExtensionNumber { get; set; }
    public Guid? ManagerId { get; set; }
    public string? WorkingHours { get; set; }
    public bool Is24Hours { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class CreateUpdateLaboratoryDto
{
    public string? Code { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? ExtensionNumber { get; set; }
    public Guid? ManagerId { get; set; }
    public string? WorkingHours { get; set; }
    public bool Is24Hours { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}

public class GetLaboratoriesInput : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public bool? IsActive { get; set; }
}
#endregion

#region Lookup DTOs
public class LookupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
#endregion
