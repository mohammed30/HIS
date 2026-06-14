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
    private readonly IRepository<HIS.Accounting.CostCenter, Guid> _costCenterRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;

    public DepartmentDataSeedContributor(
        IRepository<Department, Guid> departmentRepository,
        IRepository<HIS.Accounting.CostCenter, Guid> costCenterRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _departmentRepository = departmentRepository;
        _costCenterRepository = costCenterRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        /*
         * Main Medical Departments
         */
        await CreateDepartmentAsync("DEP-ER", "قسم الطوارئ والاستقبال", "Emergency Room (ER)", true);
        await CreateDepartmentAsync("DEP-ICU", "قسم العناية المركزة", "Intensive Care Unit (ICU)", true);
        await CreateDepartmentAsync("DEP-OR", "قسم العمليات الجراحية", "Operating Room (OR)", true);
        await CreateDepartmentAsync("DEP-OPD", "العيادات الخارجية", "Outpatient Department (OPD)", true);
        await CreateDepartmentAsync("DEP-RAD", "قسم الأشعة والتصوير", "Radiology / Imaging Department", true);
        await CreateDepartmentAsync("DEP-LAB", "المختبر / المعمل", "Laboratory Department", true);
        await CreateDepartmentAsync("DEP-PHARM", "قسم الصيدلية", "Pharmacy", true);
        await CreateDepartmentAsync("DEP-INP", "قسم التنويم / الأقسام الداخلية", "Inpatient Wards", true);
        await CreateDepartmentAsync("DEP-CSSD", "قسم التعقيم المركزي", "Central Sterile Supply Department (CSSD)", true);
        await CreateDepartmentAsync("DEP-PT", "قسم العلاج الطبيعي", "Physiotherapy Department", true);

        /*
         * Medical Specialties
         */
        await CreateDepartmentAsync("SPEC-IM", "قسم الباطنة", "Internal Medicine Department", true);
        await CreateDepartmentAsync("SPEC-GS", "قسم الجراحة العامة", "General Surgery", true);
        await CreateDepartmentAsync("SPEC-OBGYN", "قسم النساء والتوليد", "Obstetrics and Gynecology (OB/GYN)", true);
        await CreateDepartmentAsync("SPEC-PED", "قسم الأطفال", "Pediatrics Department", true);
        await CreateDepartmentAsync("SPEC-ORTHO", "قسم العظام", "Orthopedic Department", true);
        await CreateDepartmentAsync("SPEC-CARDIO", "قسم القلب", "Cardiology Department", true);
        await CreateDepartmentAsync("SPEC-DERM", "قسم الجلدية", "Dermatology Department", true);
        await CreateDepartmentAsync("SPEC-ENT", "قسم الأنف والأذن والحنجرة", "ENT Department", true);
        await CreateDepartmentAsync("SPEC-URO", "قسم المسالك البولية", "Urology Department", true);
        await CreateDepartmentAsync("SPEC-OPH", "قسم العيون", "Ophthalmology Department", true);
        await CreateDepartmentAsync("SPEC-FAM", "طب الأسرة", "Family Medicine", true);
        await CreateDepartmentAsync("SPEC-COMM", "طب المجتمع", "Community Medicine", true);
        await CreateDepartmentAsync("SPEC-FOREN", "الطب الشرعي والسموم", "Forensic Medicine and Toxicology", true);
        await CreateDepartmentAsync("SPEC-NICU", "وحدة المبتسرين (حديثي الولادة)", "Neonatal Intensive Care Unit (NICU)", true);

        /*
         * Administrative & Support Departments
         */
        await CreateDepartmentAsync("ADM-HOSP", "إدارة المستشفى", "Hospital Administration", false);
        await CreateDepartmentAsync("ADM-RECB", "قسم الاستقبال", "Reception Desk", false);
        await CreateDepartmentAsync("ADM-HR", "إدارة الموارد البشرية", "Human Resources (HR)", false);
        await CreateDepartmentAsync("ADM-FIN", "الإدارة المالية", "Financial Department", false);
        await CreateDepartmentAsync("ADM-PURCH", "إدارة المشتريات", "Purchasing Department", false);
        await CreateDepartmentAsync("ADM-IT", "قسم تقنية المعلومات", "IT Department", false);
        await CreateDepartmentAsync("ADM-MRD", "قسم التسجيل الطبي (الأرشيف)", "Medical Records Department (MRD)", false);

        /*
         * Wards/Units
         */
        await CreateDepartmentAsync("WARD-MALE", "قسم تنويم الرجال", "Male Medical/Surgical Ward", true);
        await CreateDepartmentAsync("WARD-FEMALE", "قسم تنويم النساء", "Female Medical/Surgical Ward", true);
        await CreateDepartmentAsync("WARD-PVT", "أجنحة خاصة", "Private Suites", true);
    }

    private async Task CreateDepartmentAsync(string code, string nameAr, string nameEn, bool isMedical)
    {
        if (await _departmentRepository.FirstOrDefaultAsync(d => d.Code == code) == null)
        {
            // Auto-create Cost Center
            var costCenter = new HIS.Accounting.CostCenter(
                _guidGenerator.Create(),
                code,
                nameAr,
                nameEn
            );
            await _costCenterRepository.InsertAsync(costCenter);

            var department = new Department(
                _guidGenerator.Create(),
                _currentTenant.Id,
                code,
                nameAr
            )
            {
                NameEn = nameEn,
                IsMedical = isMedical,
                CostCenterId = costCenter.Id
            };

            await _departmentRepository.InsertAsync(department);
        }
    }
}
