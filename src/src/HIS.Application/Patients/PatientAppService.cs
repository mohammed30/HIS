using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace HIS.Patients;

/// <summary>
/// خدمة تطبيق المرضى
/// </summary>
public class PatientAppService : ApplicationService, IPatientAppService
{
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;
    private static int _mrnCounter = 1000;

    public PatientAppService(
        IRepository<Patient, Guid> patientRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _patientRepository = patientRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }

    public async Task<PagedResultDto<PatientDto>> GetListAsync(GetPatientsInput input)
    {
        var queryable = await _patientRepository.GetQueryableAsync();

        // Apply filters
        queryable = ApplyFilters(queryable, input);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        // Apply sorting
        queryable = !string.IsNullOrEmpty(input.Sorting)
            ? ApplySorting(queryable, input.Sorting)
            : queryable.OrderByDescending(x => x.CreationTime);

        // Apply paging
        queryable = queryable.Skip(input.SkipCount).Take(input.MaxResultCount);

        var patients = await AsyncExecuter.ToListAsync(queryable);

        var dtos = patients.Select(MapToDto).ToList();

        return new PagedResultDto<PatientDto>(totalCount, dtos);
    }

    public async Task<PatientDto> GetAsync(Guid id)
    {
        var patient = await _patientRepository.GetAsync(id);
        return MapToDto(patient);
    }

    public async Task<PatientDto> CreateAsync(CreateUpdatePatientDto input)
    {
        var mrn = GenerateMRN();

        var patient = new Patient(
            id: _guidGenerator.Create(),
            tenantId: _currentTenant.Id,
            mrn: mrn,
            firstNameAr: input.FirstNameAr,
            lastNameAr: input.LastNameAr,
            dateOfBirth: input.DateOfBirth,
            gender: input.Gender,
            identityType: input.IdentityType,
            identityNumber: input.IdentityNumber,
            mobileNumber: input.MobileNumber
        )
        {
            MiddleNameAr = input.MiddleNameAr,
            FirstNameEn = input.FirstNameEn,
            MiddleNameEn = input.MiddleNameEn,
            LastNameEn = input.LastNameEn,
            MaritalStatus = input.MaritalStatus,
            Nationality = input.Nationality,
            IdentityExpiryDate = input.IdentityExpiryDate,
            PhoneNumber = input.PhoneNumber,
            Email = input.Email,
            Address = input.Address,
            City = input.City,
            EmergencyContactName = input.EmergencyContactName,
            EmergencyContactRelation = input.EmergencyContactRelation,
            EmergencyContactPhone = input.EmergencyContactPhone,
            Category = input.Category,
            BloodType = input.BloodType,
            Allergies = input.Allergies,
            Notes = input.Notes
        };

        await _patientRepository.InsertAsync(patient);

        return MapToDto(patient);
    }

    public async Task<PatientDto> UpdateAsync(Guid id, CreateUpdatePatientDto input)
    {
        var patient = await _patientRepository.GetAsync(id);

        patient.FirstNameAr = input.FirstNameAr;
        patient.MiddleNameAr = input.MiddleNameAr;
        patient.LastNameAr = input.LastNameAr;
        patient.FirstNameEn = input.FirstNameEn;
        patient.MiddleNameEn = input.MiddleNameEn;
        patient.LastNameEn = input.LastNameEn;
        patient.DateOfBirth = input.DateOfBirth;
        patient.Gender = input.Gender;
        patient.MaritalStatus = input.MaritalStatus;
        patient.Nationality = input.Nationality;
        patient.IdentityType = input.IdentityType;
        patient.IdentityNumber = input.IdentityNumber;
        patient.IdentityExpiryDate = input.IdentityExpiryDate;
        patient.MobileNumber = input.MobileNumber;
        patient.PhoneNumber = input.PhoneNumber;
        patient.Email = input.Email;
        patient.Address = input.Address;
        patient.City = input.City;
        patient.EmergencyContactName = input.EmergencyContactName;
        patient.EmergencyContactRelation = input.EmergencyContactRelation;
        patient.EmergencyContactPhone = input.EmergencyContactPhone;
        patient.Category = input.Category;
        patient.BloodType = input.BloodType;
        patient.Allergies = input.Allergies;
        patient.Notes = input.Notes;

        await _patientRepository.UpdateAsync(patient);

        return MapToDto(patient);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _patientRepository.DeleteAsync(id);
    }

    public async Task<List<PatientLookupDto>> SearchAsync(string searchText)
    {
        var queryable = await _patientRepository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(searchText))
        {
            queryable = queryable.Where(x =>
                x.MRN.Contains(searchText) ||
                x.FirstNameAr.Contains(searchText) ||
                x.LastNameAr.Contains(searchText) ||
                x.IdentityNumber.Contains(searchText) ||
                x.MobileNumber.Contains(searchText));
        }

        queryable = queryable.Where(x => x.IsActive).Take(20);

        var patients = await AsyncExecuter.ToListAsync(queryable);

        return patients.Select(p => new PatientLookupDto
        {
            Id = p.Id,
            MRN = p.MRN,
            FullNameAr = p.FullNameAr,
            MobileNumber = p.MobileNumber
        }).ToList();
    }

    public async Task<PatientDto?> GetByMRNAsync(string mrn)
    {
        var queryable = await _patientRepository.GetQueryableAsync();
        var patient = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.MRN == mrn));
        return patient == null ? null : MapToDto(patient);
    }

    public async Task<PatientDto?> GetByIdentityNumberAsync(string identityNumber)
    {
        var queryable = await _patientRepository.GetQueryableAsync();
        var patient = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.IdentityNumber == identityNumber));
        return patient == null ? null : MapToDto(patient);
    }

    private static string GenerateMRN()
    {
        var mrn = $"MRN{DateTime.Now:yyyyMMdd}{++_mrnCounter:D4}";
        return mrn;
    }

    private static PatientDto MapToDto(Patient patient)
    {
        return new PatientDto
        {
            Id = patient.Id,
            MRN = patient.MRN,
            FirstNameAr = patient.FirstNameAr,
            MiddleNameAr = patient.MiddleNameAr,
            LastNameAr = patient.LastNameAr,
            FirstNameEn = patient.FirstNameEn,
            MiddleNameEn = patient.MiddleNameEn,
            LastNameEn = patient.LastNameEn,
            FullNameAr = patient.FullNameAr,
            FullNameEn = patient.FullNameEn,
            DateOfBirth = patient.DateOfBirth,
            Age = patient.Age,
            Gender = patient.Gender,
            MaritalStatus = patient.MaritalStatus,
            Nationality = patient.Nationality,
            IdentityType = patient.IdentityType,
            IdentityNumber = patient.IdentityNumber,
            IdentityExpiryDate = patient.IdentityExpiryDate,
            MobileNumber = patient.MobileNumber,
            PhoneNumber = patient.PhoneNumber,
            Email = patient.Email,
            Address = patient.Address,
            City = patient.City,
            EmergencyContactName = patient.EmergencyContactName,
            EmergencyContactRelation = patient.EmergencyContactRelation,
            EmergencyContactPhone = patient.EmergencyContactPhone,
            Category = patient.Category,
            BloodType = patient.BloodType,
            Allergies = patient.Allergies,
            Notes = patient.Notes,
            PhotoUrl = patient.PhotoUrl,
            IsActive = patient.IsActive,
            CreationTime = patient.CreationTime,
            LastModificationTime = patient.LastModificationTime
        };
    }

    private static IQueryable<Patient> ApplyFilters(IQueryable<Patient> queryable, GetPatientsInput input)
    {
        if (!string.IsNullOrEmpty(input.SearchText))
        {
            queryable = queryable.Where(x =>
                x.MRN.Contains(input.SearchText) ||
                x.FirstNameAr.Contains(input.SearchText) ||
                x.LastNameAr.Contains(input.SearchText) ||
                x.IdentityNumber.Contains(input.SearchText) ||
                x.MobileNumber.Contains(input.SearchText));
        }

        if (!string.IsNullOrEmpty(input.MRN))
            queryable = queryable.Where(x => x.MRN == input.MRN);

        if (!string.IsNullOrEmpty(input.IdentityNumber))
            queryable = queryable.Where(x => x.IdentityNumber == input.IdentityNumber);

        if (!string.IsNullOrEmpty(input.MobileNumber))
            queryable = queryable.Where(x => x.MobileNumber == input.MobileNumber);

        if (input.Gender.HasValue)
            queryable = queryable.Where(x => x.Gender == input.Gender);

        if (input.Category.HasValue)
            queryable = queryable.Where(x => x.Category == input.Category);

        if (input.IsActive.HasValue)
            queryable = queryable.Where(x => x.IsActive == input.IsActive);

        return queryable;
    }

    private static IQueryable<Patient> ApplySorting(IQueryable<Patient> queryable, string sorting)
    {
        return sorting.ToLower() switch
        {
            "mrn" => queryable.OrderBy(x => x.MRN),
            "mrn desc" => queryable.OrderByDescending(x => x.MRN),
            "firstnamear" => queryable.OrderBy(x => x.FirstNameAr),
            "firstnamear desc" => queryable.OrderByDescending(x => x.FirstNameAr),
            "creationtime" => queryable.OrderBy(x => x.CreationTime),
            "creationtime desc" => queryable.OrderByDescending(x => x.CreationTime),
            _ => queryable.OrderByDescending(x => x.CreationTime)
        };
    }
}
