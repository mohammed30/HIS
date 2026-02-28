using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Patients;

/// <summary>
/// خدمة تطبيق المرضى
/// </summary>
public interface IPatientAppService : IApplicationService
{
    /// <summary>
    /// الحصول على قائمة المرضى مع البحث
    /// </summary>
    Task<PagedResultDto<PatientDto>> GetListAsync(GetPatientsInput input);

    /// <summary>
    /// الحصول على مريض بالمعرف
    /// </summary>
    Task<PatientDto> GetAsync(Guid id);

    /// <summary>
    /// إنشاء مريض جديد
    /// </summary>
    Task<PatientDto> CreateAsync(CreateUpdatePatientDto input);

    /// <summary>
    /// تحديث مريض
    /// </summary>
    Task<PatientDto> UpdateAsync(Guid id, CreateUpdatePatientDto input);

    /// <summary>
    /// حذف مريض
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// البحث السريع عن المرضى
    /// </summary>
    Task<List<PatientLookupDto>> SearchAsync(string searchText);

    /// <summary>
    /// البحث برقم الملف
    /// </summary>
    Task<PatientDto?> GetByMRNAsync(string mrn);

    /// <summary>
    /// البحث برقم الهوية
    /// </summary>
    Task<PatientDto?> GetByIdentityNumberAsync(string identityNumber);

    /// <summary>
    /// الحصول على تقرير خدمات المريض
    /// </summary>
    Task<PatientServicesReportDto> GetPatientServicesReportAsync(Guid patientId, bool showUnpaidOnly = false);

    /// <summary>
    /// الحصول على تقرير خدمات المريض كملف PDF
    /// </summary>
    Task<Volo.Abp.Content.IRemoteStreamContent> GetPatientServicesReportPdfAsync(Guid patientId, bool showUnpaidOnly = false);
}
