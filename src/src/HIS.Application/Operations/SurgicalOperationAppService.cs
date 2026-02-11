using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using HIS.Permissions;
using HIS.Patients;
using HIS.Settings;

namespace HIS.Operations;

/// <summary>
/// خدمة العمليات الجراحية
/// </summary>
[Authorize(HISPermissions.Reception.Default)]
public class SurgicalOperationAppService : CrudAppService<
    SurgicalOperation,
    SurgicalOperationDto,
    Guid,
    GetSurgicalOperationsInput,
    CreateUpdateSurgicalOperationDto>, ISurgicalOperationAppService
{
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Doctor, Guid> _doctorRepository;

    public SurgicalOperationAppService(
        IRepository<SurgicalOperation, Guid> repository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Doctor, Guid> doctorRepository) : base(repository)
    {
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
    }

    public override async Task<SurgicalOperationDto> CreateAsync(CreateUpdateSurgicalOperationDto input)
    {
        var operation = new SurgicalOperation(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            input.PatientId,
            input.OperationName,
            input.OperationDate
        )
        {
            DoctorId = input.DoctorId,
            OperationTypeId = input.OperationTypeId,
            Details = input.Details,
            TotalAmount = input.TotalAmount,
            CompanyShare = input.CompanyShare,
            PatientShare = input.PatientShare,
            InsuranceTotal = input.TotalAmount - input.PatientShare,
            Status = input.Status,
            AdmissionId = input.AdmissionId,
            Notes = input.Notes
        };

        await Repository.InsertAsync(operation);

        var dto = ObjectMapper.Map<SurgicalOperation, SurgicalOperationDto>(operation);
        await EnrichOperationDtoAsync(dto);
        return dto;
    }

    /// <summary>
    /// تحديث حالة العملية
    /// </summary>
    public async Task<SurgicalOperationDto> UpdateStatusAsync(Guid id, OperationStatus status)
    {
        var operation = await Repository.GetAsync(id);
        operation.Status = status;
        await Repository.UpdateAsync(operation);

        var dto = ObjectMapper.Map<SurgicalOperation, SurgicalOperationDto>(operation);
        await EnrichOperationDtoAsync(dto);
        return dto;
    }

    protected override async Task<IQueryable<SurgicalOperation>> CreateFilteredQueryAsync(GetSurgicalOperationsInput input)
    {
        var queryable = await base.CreateFilteredQueryAsync(input);

        return queryable
            .WhereIf(!string.IsNullOrWhiteSpace(input.SearchText),
                x => x.OperationName.Contains(input.SearchText!))
            .WhereIf(input.PatientId.HasValue, x => x.PatientId == input.PatientId!.Value)
            .WhereIf(input.DoctorId.HasValue, x => x.DoctorId == input.DoctorId!.Value)
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status!.Value)
            .WhereIf(input.FromDate.HasValue, x => x.OperationDate >= input.FromDate!.Value)
            .WhereIf(input.ToDate.HasValue, x => x.OperationDate <= input.ToDate!.Value);
    }

    protected override IQueryable<SurgicalOperation> ApplyDefaultSorting(IQueryable<SurgicalOperation> query)
    {
        return query.OrderByDescending(x => x.OperationDate);
    }

    public override async Task<SurgicalOperationDto> GetAsync(Guid id)
    {
        var dto = await base.GetAsync(id);
        await EnrichOperationDtoAsync(dto);
        return dto;
    }

    private async Task EnrichOperationDtoAsync(SurgicalOperationDto dto)
    {
        var patient = await _patientRepository.FindAsync(dto.PatientId);
        if (patient != null)
        {
            dto.PatientName = patient.FullNameAr;
        }

        if (dto.DoctorId.HasValue)
        {
            var doctor = await _doctorRepository.FindAsync(dto.DoctorId.Value);
            if (doctor != null)
            {
                dto.DoctorName = doctor.NameAr ?? doctor.NameEn;
            }
        }
    }
}
