using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace HIS.Settings;

public class DepartmentDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Department, Guid> _departmentRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;

    public DepartmentDataSeedContributor(
        IRepository<Department, Guid> departmentRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _departmentRepository = departmentRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        /*
         * Main Medical Departments
         */
        await CreateDepartmentAsync("DEP-ER", "قسم الطوارئ والاستقبال", "Emergency Room (ER)");
        await CreateDepartmentAsync("DEP-ICU", "قسم العناية المركزة", "Intensive Care Unit (ICU)");
        await CreateDepartmentAsync("DEP-OR", "قسم العمليات الجراحية", "Operating Room (OR)");
        await CreateDepartmentAsync("DEP-OPD", "العيادات الخارجية", "Outpatient Department (OPD)");
        await CreateDepartmentAsync("DEP-RAD", "قسم الأشعة والتصوير", "Radiology / Imaging Department");
        await CreateDepartmentAsync("DEP-LAB", "المختبر / المعمل", "Laboratory Department");
        await CreateDepartmentAsync("DEP-PHARM", "قسم الصيدلية", "Pharmacy");
        await CreateDepartmentAsync("DEP-INP", "قسم التنويم / الأقسام الداخلية", "Inpatient Wards");
        await CreateDepartmentAsync("DEP-CSSD", "قسم التعقيم المركزي", "Central Sterile Supply Department (CSSD)");

        /*
         * Medical Specialties
         */
        await CreateDepartmentAsync("SPEC-IM", "قسم الباطنة", "Internal Medicine Department");
        await CreateDepartmentAsync("SPEC-GS", "قسم الجراحة العامة", "General Surgery");
        await CreateDepartmentAsync("SPEC-OBGYN", "قسم النساء والتوليد", "Obstetrics and Gynecology (OB/GYN)");
        await CreateDepartmentAsync("SPEC-PED", "قسم الأطفال", "Pediatrics Department");
        await CreateDepartmentAsync("SPEC-ORTHO", "قسم العظام", "Orthopedic Department");
        await CreateDepartmentAsync("SPEC-CARDIO", "قسم القلب", "Cardiology Department");
        await CreateDepartmentAsync("SPEC-DERM", "قسم الجلدية", "Dermatology Department");
        await CreateDepartmentAsync("SPEC-ENT", "قسم الأنف والأذن والحنجرة", "ENT Department");
        await CreateDepartmentAsync("SPEC-URO", "قسم المسالك البولية", "Urology Department");
        await CreateDepartmentAsync("SPEC-OPH", "قسم العيون", "Ophthalmology Department"); // Needed for the user's issue
        await CreateDepartmentAsync("SPEC-FAM", "طب الأسرة", "Family Medicine");
        await CreateDepartmentAsync("SPEC-COMM", "طب المجتمع", "Community Medicine");
        await CreateDepartmentAsync("SPEC-FOREN", "الطب الشرعي والسموم", "Forensic Medicine and Toxicology");
        await CreateDepartmentAsync("SPEC-NICU", "وحدة المبتسرين (حديثي الولادة)", "Neonatal Intensive Care Unit (NICU)");

        /*
         * Administrative & Support Departments
         */
        await CreateDepartmentAsync("ADM-HOSP", "إدارة المستشفى", "Hospital Administration");
        await CreateDepartmentAsync("ADM-RECB", "قسم الاستقبال", "Reception Desk");
        await CreateDepartmentAsync("ADM-HR", "إدارة الموارد البشرية", "Human Resources (HR)");
        await CreateDepartmentAsync("ADM-FIN", "الإدارة المالية", "Financial Department");
        await CreateDepartmentAsync("ADM-PURCH", "إدارة المشتريات", "Purchasing Department");
        await CreateDepartmentAsync("ADM-IT", "قسم تقنية المعلومات", "IT Department");
        await CreateDepartmentAsync("ADM-MRD", "قسم التسجيل الطبي (الأرشيف)", "Medical Records Department (MRD)");

        /*
         * Wards/Units
         */
        await CreateDepartmentAsync("WARD-MALE", "قسم تنويم الرجال", "Male Medical/Surgical Ward");
        await CreateDepartmentAsync("WARD-FEMALE", "قسم تنويم النساء", "Female Medical/Surgical Ward");
        await CreateDepartmentAsync("WARD-PVT", "أجنحة خاصة", "Private Suites");
    }

    private async Task CreateDepartmentAsync(string code, string nameAr, string nameEn)
    {
        if (await _departmentRepository.FirstOrDefaultAsync(d => d.Code == code) == null)
        {
            var department = new Department(
                _guidGenerator.Create(),
                _currentTenant.Id,
                code,
                nameAr
            )
            {
                NameEn = nameEn
            };

            await _departmentRepository.InsertAsync(department);
        }
    }
}
