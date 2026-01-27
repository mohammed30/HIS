using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace HIS.Settings;

public class ClinicDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Clinic, Guid> _clinicRepository;
    private readonly IRepository<Department, Guid> _departmentRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;

    public ClinicDataSeedContributor(
        IRepository<Clinic, Guid> clinicRepository,
        IRepository<Department, Guid> departmentRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _clinicRepository = clinicRepository;
        _departmentRepository = departmentRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // 1. Cardiology Clinic -> Cardiology Department (SPEC-CARDIO)
        await CreateClinicAsync("CLINIC-CARDIO", "عيادة القلب", "Cardiology Clinic", "SPEC-CARDIO");

        // 2. Chronic Pain Clinic -> Outpatient (DEP-OPD) roughly or Anesthesia if exists. Mapping to OPD for safety.
        await CreateClinicAsync("CLINIC-PAIN", "عيادة الألم المزمن", "Chronic Pain Clinic", "DEP-OPD");

        // 3. Dental Clinic -> Outpatient (DEP-OPD) (Dental Dept not explicitly seeded in first batch, defaulting to OPD)
        await CreateClinicAsync("CLINIC-DENT", "عيادة الأسنان", "Dental Clinic", "DEP-OPD");

        // 4. ENT Clinic -> ENT Department (SPEC-ENT)
        await CreateClinicAsync("CLINIC-ENT", "عيادة الأنف والأذن والحنجرة", "ENT Clinic (Ear, Nose & Throat)", "SPEC-ENT");

        // 5. Endocrinology & Diabetes -> Internal Medicine (SPEC-IM) usually
        await CreateClinicAsync("CLINIC-ENDO", "عيادة أمراض الغدد الصماء والسكري", "Endocrinology & Diabetes Clinic", "SPEC-IM");

        // 6. Dermatology Clinic -> Dermatology Department (SPEC-DERM)
        await CreateClinicAsync("CLINIC-DERM", "عيادة الجلدية", "Dermatology Clinic", "SPEC-DERM");

        // 7. General Surgery Clinic -> General Surgery (SPEC-GS)
        await CreateClinicAsync("CLINIC-GS", "عيادة الجراحة العامة", "General Surgery Clinic", "SPEC-GS");

        // 8. Gastroenterology Clinic -> Internal Medicine (SPEC-IM)
        await CreateClinicAsync("CLINIC-GASTRO", "عيادة الجهاز الهضمي", "Gastroenterology Clinic", "SPEC-IM");

        // 9. Family Medicine Clinic -> Family Medicine (SPEC-FAM)
        await CreateClinicAsync("CLINIC-FAM", "عيادة طب الأسرة", "Family Medicine Clinic", "SPEC-FAM");

        // 10. Nutrition Clinic -> Outpatient (DEP-OPD)
        await CreateClinicAsync("CLINIC-NUTR", "عيادة التغذية", "Nutrition Clinic", "DEP-OPD");

        // 11. Nephrology & Urology Clinic -> Urology (SPEC-URO)
        await CreateClinicAsync("CLINIC-NEPH-URO", "عيادة الكلى والمسالك البولية", "Nephrology & Urology Clinic", "SPEC-URO");

        // 12. Internal Medicine Clinic -> Internal Medicine (SPEC-IM)
        await CreateClinicAsync("CLINIC-IM", "عيادة الباطنية", "Internal Medicine Clinic", "SPEC-IM");

        // 13. Psychiatry Clinic -> Outpatient (DEP-OPD) (Psych dept not in first seed)
        await CreateClinicAsync("CLINIC-PSYCH", "عيادة الطب النفسي", "Psychiatry Clinic", "DEP-OPD");

        // 14. Orthopedic Surgery Clinic -> Orthopedic (SPEC-ORTHO)
        await CreateClinicAsync("CLINIC-ORTHO", "عيادة جراحة العظام", "Orthopedic Surgery Clinic", "SPEC-ORTHO");

        // 15. Ophthalmology Clinic -> Ophthalmology (SPEC-OPH)
        await CreateClinicAsync("CLINIC-OPH", "عيادة العيون", "Ophthalmology Clinic", "SPEC-OPH");

        // 16. Stable Cases Clinic -> Outpatient (DEP-OPD)
        await CreateClinicAsync("CLINIC-STABLE", "عيادة الحالات المستقرة", "Stable Cases Clinic", "DEP-OPD");

        // 17. Rheumatology & Physical Medicine -> Internal Medicine (SPEC-IM) or Ortho? Usually IM or Rehab. Mapping to IM.
        await CreateClinicAsync("CLINIC-RHEUM", "عيادة المفاصل والطب الطبيعي", "Rheumatology & Physical Medicine Clinic", "SPEC-IM");
    }

    private async Task CreateClinicAsync(string code, string nameAr, string nameEn, string departmentCode)
    {
        if (await _clinicRepository.FirstOrDefaultAsync(c => c.Code == code) == null)
        {
            // Find Department
            var department = await _departmentRepository.FirstOrDefaultAsync(d => d.Code == departmentCode);
            
            // Fallback to OPD if department not found
            if (department == null)
            {
                 department = await _departmentRepository.FirstOrDefaultAsync(d => d.Code == "DEP-OPD");

                 // Safety Net: Create OPD if it doesn't exist to ensure Clinic is created
                 if (department == null)
                 {
                     department = new Department(
                         _guidGenerator.Create(),
                         _currentTenant.Id,
                         "DEP-OPD",
                         "العيادات الخارجية"
                     )
                     {
                         NameEn = "Outpatient Department (OPD)"
                     };
                     await _departmentRepository.InsertAsync(department, autoSave: true);
                 }
            }

            if (department != null)
            {
                var clinic = new Clinic(
                    _guidGenerator.Create(),
                    _currentTenant.Id,
                    code,
                    nameAr,
                    department.Id
                )
                {
                    NameEn = nameEn,
                    IsActive = true
                };

                await _clinicRepository.InsertAsync(clinic);
            }
        }
    }
}
