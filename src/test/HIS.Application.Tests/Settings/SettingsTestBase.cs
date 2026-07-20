using System;
using System.Threading.Tasks;
using HIS.Settings;
using Volo.Abp.Domain.Repositories;

namespace HIS.Settings.Tests;

public abstract class SettingsTestBase<TStartupModule> : HISApplicationTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.IAbpModule
{
    protected readonly IRepository<Department, Guid> DepartmentRepository;
    protected readonly IRepository<Specialty, Guid> SpecialtyRepository;
    protected readonly IRepository<Clinic, Guid> ClinicRepository;
    protected readonly IRepository<Doctor, Guid> DoctorRepository;
    protected readonly IRepository<Laboratory, Guid> LaboratoryRepository;

    protected SettingsTestBase()
    {
        DepartmentRepository = GetRequiredService<IRepository<Department, Guid>>();
        SpecialtyRepository = GetRequiredService<IRepository<Specialty, Guid>>();
        ClinicRepository = GetRequiredService<IRepository<Clinic, Guid>>();
        DoctorRepository = GetRequiredService<IRepository<Doctor, Guid>>();
        LaboratoryRepository = GetRequiredService<IRepository<Laboratory, Guid>>();
    }

    protected virtual async Task<Department> CreateDepartmentAsync(string name = "General Surgery")
    {
        var id = Guid.NewGuid();
        var dept = new Department(id, null, "DEP-" + id.ToString().Substring(0, 4), name)
        {
            IsMedical = true
        };
        await DepartmentRepository.InsertAsync(dept);
        return dept;
    }

    protected virtual async Task<Specialty> CreateSpecialtyAsync(string name = "Cardiology")
    {
        var id = Guid.NewGuid();
        var spec = new Specialty(id, null, "SPEC-" + id.ToString().Substring(0, 4), name);
        await SpecialtyRepository.InsertAsync(spec);
        return spec;
    }
}
