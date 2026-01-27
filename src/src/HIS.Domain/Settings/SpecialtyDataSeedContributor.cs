using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace HIS.Settings;

public class SpecialtyDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Specialty, Guid> _specialtyRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;

    public SpecialtyDataSeedContributor(
        IRepository<Specialty, Guid> specialtyRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _specialtyRepository = specialtyRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        /*
         * Main Clinical Specialties
         */
        await CreateSpecialtyAsync("SP-PED", "طب الأطفال", "Pediatrics");
        await CreateSpecialtyAsync("SP-OBGYN", "طب النساء والتوليد", "Obstetrics and Gynecology");
        await CreateSpecialtyAsync("SP-IM", "الطب الباطني", "Internal Medicine");
        await CreateSpecialtyAsync("SP-GS", "الجراحة العامة", "General Surgery");
        await CreateSpecialtyAsync("SP-FAM", "طب الأسرة", "Family Medicine");
        await CreateSpecialtyAsync("SP-ER", "طب الطوارئ", "Emergency Medicine");
        await CreateSpecialtyAsync("SP-ANES", "التخدير والعناية المركزة", "Anesthesiology and Intensive Care");
        await CreateSpecialtyAsync("SP-OPH", "طب العيون", "Ophthalmology");
        await CreateSpecialtyAsync("SP-ENT", "الأنف والأذن والحنجرة", "ENT (Ear, Nose, and Throat)");
        await CreateSpecialtyAsync("SP-ORTHO", "جراحة العظام", "Orthopedic Surgery");

        /*
         * Subspecialties
         */
        await CreateSpecialtyAsync("SUB-CARDIO", "طب القلب", "Cardiology");
        await CreateSpecialtyAsync("SUB-NEURO", "الأمراض العصبية", "Neurology");
        await CreateSpecialtyAsync("SUB-ONCO", "طب الأورام", "Oncology");
        await CreateSpecialtyAsync("SUB-DERM", "الأمراض الجلدية", "Dermatology");
        await CreateSpecialtyAsync("SUB-URO", "جراحة المسالك البولية", "Urology");
        await CreateSpecialtyAsync("SUB-NEPHRO", "أمراض الكلى", "Nephrology");
        await CreateSpecialtyAsync("SUB-GASTRO", "الجهاز الهضمي", "Gastroenterology");
        await CreateSpecialtyAsync("SUB-HEMA", "أمراض الدم", "Hematology");
        await CreateSpecialtyAsync("SUB-ENDO", "الغدد الصماء", "Endocrinology");
        await CreateSpecialtyAsync("SUB-PSYCH", "الطب النفسي", "Psychiatry");
        await CreateSpecialtyAsync("SUB-RAD", "الأشعة التشخيصية", "Diagnostic Radiology");
        await CreateSpecialtyAsync("SUB-PMR", "العلاج الطبيعي والتأهيل", "Physical Medicine and Rehabilitation");

        /*
         * Academic & Supportive Departments (Mapped as Specialties if doctors belong to them)
         */
        await CreateSpecialtyAsync("SUP-PATH", "علم الأمراض (الباثولوجيا)", "Pathology");
        await CreateSpecialtyAsync("SUP-PHARM-TOX", "علم الأدوية والسموم", "Pharmacology and Toxicology");
        await CreateSpecialtyAsync("SUP-MICRO", "الميكروبيولوجي (الأحياء الدقيقة)", "Microbiology");
        await CreateSpecialtyAsync("SUP-BIOCHEM", "الكيمياء الحيوية الطبية", "Medical Biochemistry");
        await CreateSpecialtyAsync("SUP-FOREN", "الطب الشرعي", "Forensic Medicine");
    }

    private async Task CreateSpecialtyAsync(string code, string nameAr, string nameEn)
    {
        if (await _specialtyRepository.FirstOrDefaultAsync(s => s.Code == code) == null)
        {
            var specialty = new Specialty(
                _guidGenerator.Create(),
                _currentTenant.Id,
                code,
                nameAr
            )
            {
                NameEn = nameEn
            };

            await _specialtyRepository.InsertAsync(specialty);
        }
    }
}
