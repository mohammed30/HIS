using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Settings;

/// <summary>
/// خدمة تطبيق الأقسام
/// </summary>
public interface IDepartmentAppService : ICrudAppService<DepartmentDto, Guid, GetDepartmentsInput, CreateUpdateDepartmentDto>
{
    Task<List<LookupDto>> GetLookupAsync();
    Task<List<LookupDto>> GetMedicalDepartmentsLookupAsync();
}

/// <summary>
/// خدمة تطبيق التخصصات
/// </summary>
public interface ISpecialtyAppService : ICrudAppService<SpecialtyDto, Guid, GetSpecialtiesInput, CreateUpdateSpecialtyDto>
{
    Task<List<LookupDto>> GetLookupAsync();
}

/// <summary>
/// خدمة تطبيق العيادات
/// </summary>
public interface IClinicAppService : ICrudAppService<ClinicDto, Guid, GetClinicsInput, CreateUpdateClinicDto>
{
    Task<List<LookupDto>> GetLookupAsync();
    Task<List<ClinicDto>> GetByDepartmentAsync(Guid departmentId);
}

/// <summary>
/// خدمة تطبيق الأطباء
/// </summary>
public interface IDoctorAppService : ICrudAppService<DoctorDto, Guid, GetDoctorsInput, CreateUpdateDoctorDto>
{
    Task<List<LookupDto>> GetLookupAsync();
    Task<List<DoctorDto>> GetBySpecialtyAsync(Guid specialtyId);
    Task<List<DoctorDto>> GetByDepartmentAsync(Guid departmentId);
    Task SyncOldDoctorsAccountsAsync();
}

/// <summary>
/// خدمة تطبيق المعامل
/// </summary>
public interface ILaboratoryAppService : ICrudAppService<LaboratoryDto, Guid, GetLaboratoriesInput, CreateUpdateLaboratoryDto>
{
    Task<List<LookupDto>> GetLookupAsync();
}
