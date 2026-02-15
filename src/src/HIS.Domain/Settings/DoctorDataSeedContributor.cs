using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace HIS.Settings;

public class DoctorDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Doctor, Guid> _doctorRepository;
    private readonly IRepository<Department, Guid> _departmentRepository;
    private readonly IRepository<Specialty, Guid> _specialtyRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;

    public DoctorDataSeedContributor(
        IRepository<Doctor, Guid> doctorRepository,
        IRepository<Department, Guid> departmentRepository,
        IRepository<Specialty, Guid> specialtyRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _doctorRepository = doctorRepository;
        _departmentRepository = departmentRepository;
        _specialtyRepository = specialtyRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // 1. Cardiology
        await CreateDoctorAsync("DOC-001", "د. أحمد علي", "Dr. Ahmed Ali", "SPEC-CARDIO", "SUB-CARDIO", 300);
        await CreateDoctorAsync("DOC-002", "د. مريم حسن", "Dr. Maryam Hassan", "SPEC-CARDIO", "SUB-CARDIO", 350);

        // 2. ENT
        await CreateDoctorAsync("DOC-003", "د. خالد محمود", "Dr. Khalid Mahmoud", "SPEC-ENT", "SP-ENT", 200);
        await CreateDoctorAsync("DOC-004", "د. سارة إبراهيم", "Dr. Sarah Ibrahim", "SPEC-ENT", "SP-ENT", 200);

        // 3. Internal Medicine / Endocrinology / Gastro
        await CreateDoctorAsync("DOC-005", "د. محمد يوسف", "Dr. Mohammed Yousef", "SPEC-IM", "SP-IM", 250);
        await CreateDoctorAsync("DOC-006", "د. ليلى عبد الله", "Dr. Laila Abdullah", "SPEC-IM", "SUB-ENDO", 300);
        await CreateDoctorAsync("DOC-007", "د. عمر فاروق", "Dr. Omar Farouk", "SPEC-IM", "SUB-GASTRO", 280);

        // 4. Dermatology
        await CreateDoctorAsync("DOC-008", "د. نورا جاسم", "Dr. Nora Jassim", "SPEC-DERM", "SUB-DERM", 250);
        await CreateDoctorAsync("DOC-009", "د. فهد العتيبي", "Dr. Fahad Al-Otaibi", "SPEC-DERM", "SUB-DERM", 250);

        // 5. General Surgery
        await CreateDoctorAsync("DOC-010", "د. سامي القحطاني", "Dr. Sami Al-Qahtani", "SPEC-GS", "SP-GS", 400);
        await CreateDoctorAsync("DOC-011", "د. ريم الماجد", "Dr. Reem Al-Majed", "SPEC-GS", "SP-GS", 400);

        // 6. Family Medicine
        await CreateDoctorAsync("DOC-012", "د. يحيى الشهري", "Dr. Yahya Al-Shehri", "SPEC-FAM", "SP-FAM", 150);
        await CreateDoctorAsync("DOC-013", "د. أمل الحربي", "Dr. Amal Al-Harbi", "SPEC-FAM", "SP-FAM", 150);

        // 7. Urology
        await CreateDoctorAsync("DOC-014", "د. فيصل بن علي", "Dr. Faisal Bin Ali", "SPEC-URO", "SUB-URO", 300);
        await CreateDoctorAsync("DOC-015", "د. منيرة الفاضل", "Dr. Munira Al-Fadel", "SPEC-URO", "SUB-URO", 300);

        // 8. Orthopedic
        await CreateDoctorAsync("DOC-016", "د. صالح العبد", "Dr. Saleh Al-Abd", "SPEC-ORTHO", "SP-ORTHO", 350);
        await CreateDoctorAsync("DOC-017", "د. باسمة السعيد", "Dr. Basma Al-Said", "SPEC-ORTHO", "SP-ORTHO", 350);

        // 9. Ophthalmology
        await CreateDoctorAsync("DOC-018", "د. عماد الدين", "Dr. Imad Al-Din", "SPEC-OPH", "SP-OPH", 250);
        await CreateDoctorAsync("DOC-019", "د. حصة العمري", "Dr. Hessa Al-Omari", "SPEC-OPH", "SP-OPH", 250);

        // 10. Psychiatry (OPD Dept for now)
        await CreateDoctorAsync("DOC-020", "د. طارق الحبيب", "Dr. Tariq Al-Habib", "DEP-OPD", "SUB-PSYCH", 500);
        await CreateDoctorAsync("DOC-021", "د. منى الصواف", "Dr. Mona Al-Sawaf", "DEP-OPD", "SUB-PSYCH", 500);

        // 11. New Surgeon for Operations Test
        await CreateDoctorAsync("DOC-MH-001", "د. محمد حسن", "Dr. Mohammed Hassan", "SPEC-GS", "SP-GS", 400);
    }

    private async Task CreateDoctorAsync(string code, string nameAr, string nameEn, string departmentCode, string specialtyCode, decimal fee)
    {
        if (await _doctorRepository.FirstOrDefaultAsync(d => d.Code == code) == null)
        {
            var department = await _departmentRepository.FirstOrDefaultAsync(d => d.Code == departmentCode);
            var specialty = await _specialtyRepository.FirstOrDefaultAsync(s => s.Code == specialtyCode);

            if (department != null && specialty != null)
            {
                var doctor = new Doctor(
                    _guidGenerator.Create(),
                    _currentTenant.Id,
                    code,
                    nameAr,
                    specialty.Id,
                    department.Id
                )
                {
                    NameEn = nameEn,
                    ConsultationFee = fee,
                    FollowUpFee = fee / 2,
                    AppointmentDuration = 15,
                    IsActive = true
                };

                await _doctorRepository.InsertAsync(doctor);
            }
        }
    }
}
