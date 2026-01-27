using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Settings;

/// <summary>
/// خدمة تطبيق الأقسام
/// </summary>
public class DepartmentAppService : CrudAppService<Department, DepartmentDto, Guid, GetDepartmentsInput, CreateUpdateDepartmentDto>, IDepartmentAppService
{
    public DepartmentAppService(IRepository<Department, Guid> repository) : base(repository)
    {
    }

    public override async Task<DepartmentDto> CreateAsync(CreateUpdateDepartmentDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
        {
            input.Code = $"DEP-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
        return await base.CreateAsync(input);
    }


    public async Task<List<LookupDto>> GetLookupAsync()
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return items.Select(x => new LookupDto { Id = x.Id, Name = x.NameAr }).ToList();
    }

    /// <summary>
    /// يُرجع قائمة الأقسام الطبية فقط (للاستخدام في تعريف الأطباء)
    /// </summary>
    public async Task<List<LookupDto>> GetMedicalDepartmentsLookupAsync()
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.IsActive && x.IsMedical).OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return items.Select(x => new LookupDto { Id = x.Id, Name = x.NameAr }).ToList();
    }

    protected override async Task<IQueryable<Department>> CreateFilteredQueryAsync(GetDepartmentsInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(input.SearchText))
        {
            queryable = queryable.Where(x =>
                x.Code.Contains(input.SearchText) ||
                x.NameAr.Contains(input.SearchText) ||
                (x.NameEn != null && x.NameEn.Contains(input.SearchText)));
        }

        if (input.IsActive.HasValue)
            queryable = queryable.Where(x => x.IsActive == input.IsActive);

        if (input.IsMedical.HasValue)
            queryable = queryable.Where(x => x.IsMedical == input.IsMedical);

        return queryable;
    }

    protected override IQueryable<Department> ApplyDefaultSorting(IQueryable<Department> query)
    {
        return query.OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr);
    }
}

/// <summary>
/// خدمة تطبيق التخصصات
/// </summary>
public class SpecialtyAppService : CrudAppService<Specialty, SpecialtyDto, Guid, GetSpecialtiesInput, CreateUpdateSpecialtyDto>, ISpecialtyAppService
{
    public SpecialtyAppService(IRepository<Specialty, Guid> repository) : base(repository)
    {
    }

    public override async Task<SpecialtyDto> CreateAsync(CreateUpdateSpecialtyDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
        {
            input.Code = $"SPC-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
        return await base.CreateAsync(input);
    }


    public async Task<List<LookupDto>> GetLookupAsync()
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return items.Select(x => new LookupDto { Id = x.Id, Name = x.NameAr }).ToList();
    }

    protected override async Task<IQueryable<Specialty>> CreateFilteredQueryAsync(GetSpecialtiesInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(input.SearchText))
        {
            queryable = queryable.Where(x =>
                x.Code.Contains(input.SearchText) ||
                x.NameAr.Contains(input.SearchText) ||
                (x.NameEn != null && x.NameEn.Contains(input.SearchText)));
        }

        if (input.IsActive.HasValue)
            queryable = queryable.Where(x => x.IsActive == input.IsActive);

        return queryable;
    }

    protected override IQueryable<Specialty> ApplyDefaultSorting(IQueryable<Specialty> query)
    {
        return query.OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr);
    }
}

/// <summary>
/// خدمة تطبيق العيادات
/// </summary>
public class ClinicAppService : CrudAppService<Clinic, ClinicDto, Guid, GetClinicsInput, CreateUpdateClinicDto>, IClinicAppService
{
    private readonly IRepository<Department, Guid> _departmentRepository;

    public ClinicAppService(
        IRepository<Clinic, Guid> repository,
        IRepository<Department, Guid> departmentRepository) : base(repository)
    {
        _departmentRepository = departmentRepository;
    }

    public override async Task<ClinicDto> CreateAsync(CreateUpdateClinicDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
        {
            input.Code = $"CLN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
        return await base.CreateAsync(input);
    }


    public async Task<List<LookupDto>> GetLookupAsync()
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return items.Select(x => new LookupDto { Id = x.Id, Name = x.NameAr }).ToList();
    }

    public async Task<List<ClinicDto>> GetByDepartmentAsync(Guid departmentId)
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.DepartmentId == departmentId && x.IsActive)
                     .OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return ObjectMapper.Map<List<Clinic>, List<ClinicDto>>(items);
    }

    protected override async Task<IQueryable<Clinic>> CreateFilteredQueryAsync(GetClinicsInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(input.SearchText))
        {
            queryable = queryable.Where(x =>
                x.Code.Contains(input.SearchText) ||
                x.NameAr.Contains(input.SearchText) ||
                (x.NameEn != null && x.NameEn.Contains(input.SearchText)));
        }

        if (input.DepartmentId.HasValue)
            queryable = queryable.Where(x => x.DepartmentId == input.DepartmentId);

        if (input.IsActive.HasValue)
            queryable = queryable.Where(x => x.IsActive == input.IsActive);

        return queryable;
    }

    protected override IQueryable<Clinic> ApplyDefaultSorting(IQueryable<Clinic> query)
    {
        return query.OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr);
    }
}

/// <summary>
/// خدمة تطبيق الأطباء
/// </summary>
public class DoctorAppService : CrudAppService<Doctor, DoctorDto, Guid, GetDoctorsInput, CreateUpdateDoctorDto>, IDoctorAppService
{
    public DoctorAppService(IRepository<Doctor, Guid> repository) : base(repository)
    {
    }

    public override async Task<DoctorDto> CreateAsync(CreateUpdateDoctorDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
        {
            input.Code = $"DOC-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
        return await base.CreateAsync(input);
    }


    public async Task<List<LookupDto>> GetLookupAsync()
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return items.Select(x => new LookupDto { Id = x.Id, Name = x.NameAr }).ToList();
    }

    public async Task<List<DoctorDto>> GetBySpecialtyAsync(Guid specialtyId)
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.SpecialtyId == specialtyId && x.IsActive)
                     .OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return ObjectMapper.Map<List<Doctor>, List<DoctorDto>>(items);
    }

    public async Task<List<DoctorDto>> GetByDepartmentAsync(Guid departmentId)
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.DepartmentId == departmentId && x.IsActive)
                     .OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return ObjectMapper.Map<List<Doctor>, List<DoctorDto>>(items);
    }

    protected override async Task<IQueryable<Doctor>> CreateFilteredQueryAsync(GetDoctorsInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(input.SearchText))
        {
            queryable = queryable.Where(x =>
                x.Code.Contains(input.SearchText) ||
                x.NameAr.Contains(input.SearchText) ||
                (x.NameEn != null && x.NameEn.Contains(input.SearchText)));
        }

        if (input.SpecialtyId.HasValue)
            queryable = queryable.Where(x => x.SpecialtyId == input.SpecialtyId);

        if (input.DepartmentId.HasValue)
            queryable = queryable.Where(x => x.DepartmentId == input.DepartmentId);

        if (input.IsActive.HasValue)
            queryable = queryable.Where(x => x.IsActive == input.IsActive);

        return queryable;
    }

    protected override IQueryable<Doctor> ApplyDefaultSorting(IQueryable<Doctor> query)
    {
        return query.OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr);
    }
}

/// <summary>
/// خدمة تطبيق المعامل
/// </summary>
public class LaboratoryAppService : CrudAppService<Laboratory, LaboratoryDto, Guid, GetLaboratoriesInput, CreateUpdateLaboratoryDto>, ILaboratoryAppService
{
    public LaboratoryAppService(IRepository<Laboratory, Guid> repository) : base(repository)
    {
    }

    public override async Task<LaboratoryDto> CreateAsync(CreateUpdateLaboratoryDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Code))
        {
            input.Code = $"LAB-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
        return await base.CreateAsync(input);
    }


    public async Task<List<LookupDto>> GetLookupAsync()
    {
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.IsActive).OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr));
        
        return items.Select(x => new LookupDto { Id = x.Id, Name = x.NameAr }).ToList();
    }

    protected override async Task<IQueryable<Laboratory>> CreateFilteredQueryAsync(GetLaboratoriesInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(input.SearchText))
        {
            queryable = queryable.Where(x =>
                x.Code.Contains(input.SearchText) ||
                x.NameAr.Contains(input.SearchText) ||
                (x.NameEn != null && x.NameEn.Contains(input.SearchText)));
        }

        if (input.IsActive.HasValue)
            queryable = queryable.Where(x => x.IsActive == input.IsActive);

        return queryable;
    }

    protected override IQueryable<Laboratory> ApplyDefaultSorting(IQueryable<Laboratory> query)
    {
        return query.OrderBy(x => x.SortOrder).ThenBy(x => x.NameAr);
    }
}
