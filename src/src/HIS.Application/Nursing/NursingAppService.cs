using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Clinical;
using HIS.Patients;
using HIS.Nursing;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace HIS.Nursing;

[Authorize]
public class NursingAppService : HISAppService, INursingAppService
{
    private readonly IRepository<MedicationAdministration, Guid> _medAdminRepo;
    private readonly IRepository<CarePlan, Guid> _carePlanRepo;
    private readonly IRepository<Patient, Guid> _patientRepo;
    private readonly IRepository<MedicalOrder, Guid> _medicalOrderRepo;

    public NursingAppService(
        IRepository<MedicationAdministration, Guid> medAdminRepo,
        IRepository<CarePlan, Guid> carePlanRepo,
        IRepository<Patient, Guid> patientRepo,
        IRepository<MedicalOrder, Guid> medicalOrderRepo)
    {
        _medAdminRepo = medAdminRepo;
        _carePlanRepo = carePlanRepo;
        _patientRepo = patientRepo;
        _medicalOrderRepo = medicalOrderRepo;
    }

    // --- Medication Administration ---

    public async Task<MedicationAdministrationDto> CreateMedicationAdministrationAsync(CreateMedicationAdministrationDto input)
    {
        var medAdmin = ObjectMapper.Map<CreateMedicationAdministrationDto, MedicationAdministration>(input);
        
        // Get Order details to populate DrugName snapshot
        var order = await _medicalOrderRepo.GetAsync(input.MedicalOrderId);
        medAdmin.DrugName = order.ServiceName;

        await _medAdminRepo.InsertAsync(medAdmin);

        var dto = ObjectMapper.Map<MedicationAdministration, MedicationAdministrationDto>(medAdmin);
        var patient = await _patientRepo.GetAsync(input.PatientId);
        dto.PatientName = patient.FullNameAr;

        return dto;
    }

    public async Task<List<MedicationAdministrationDto>> GetMedicationAdministrationsAsync(Guid patientId)
    {
        var query = await _medAdminRepo.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            query.Where(x => x.PatientId == patientId)
                 .OrderByDescending(x => x.AdministrationTime)
        );

        var dtos = ObjectMapper.Map<List<MedicationAdministration>, List<MedicationAdministrationDto>>(items);
        
        if (dtos.Any())
        {
            var patient = await _patientRepo.GetAsync(patientId);
            foreach (var dto in dtos)
            {
                dto.PatientName = patient.FullNameAr;
            }
        }
        
        return dtos;
    }

    public async Task<List<DueMedicationDto>> GetDueMedicationsAsync(Guid patientId)
    {
        var query = await _medicalOrderRepo.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            query.Where(x => x.PatientId == patientId && 
                             x.Type == OrderType.Medication && 
                             (x.Status == OrderStatus.Pending || x.Status == OrderStatus.InProgress))
                 .OrderByDescending(x => x.CreationTime)
        );

        return ObjectMapper.Map<List<MedicalOrder>, List<DueMedicationDto>>(items);
    }


    // --- Care Plan ---

    public async Task<CarePlanDto> CreateCarePlanAsync(CreateCarePlanDto input)
    {
        var carePlan = ObjectMapper.Map<CreateCarePlanDto, CarePlan>(input);
        await _carePlanRepo.InsertAsync(carePlan);
        
        var dto = ObjectMapper.Map<CarePlan, CarePlanDto>(carePlan);
        var patient = await _patientRepo.GetAsync(input.PatientId);
        dto.PatientName = patient.FullNameAr;
        
        return dto;
    }

    public async Task<List<CarePlanDto>> GetCarePlansAsync(Guid patientId)
    {
        var query = await _carePlanRepo.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            query.Where(x => x.PatientId == patientId)
                 .OrderByDescending(x => x.DateCreate)
        );

        var dtos = ObjectMapper.Map<List<CarePlan>, List<CarePlanDto>>(items);
        
        if (dtos.Any())
        {
            var patient = await _patientRepo.GetAsync(patientId);
            foreach (var dto in dtos)
            {
                dto.PatientName = patient.FullNameAr;
            }
        }

        return dtos;
    }

    public async Task<CarePlanDto> UpdateCarePlanAsync(Guid id, CreateCarePlanDto input)
    {
        var carePlan = await _carePlanRepo.GetAsync(id);
        
        carePlan.Diagnosis = input.Diagnosis;
        carePlan.Goal = input.Goal;
        carePlan.Interventions = input.Interventions;
        carePlan.Status = input.Status;
        // Evaluation update logic can be added or handled separately
        
        await _carePlanRepo.UpdateAsync(carePlan);
        
        var dto = ObjectMapper.Map<CarePlan, CarePlanDto>(carePlan);
        var patient = await _patientRepo.GetAsync(carePlan.PatientId);
        dto.PatientName = patient.FullNameAr;
        
        return dto;
    }

    public async Task DeleteCarePlanAsync(Guid id)
    {
        await _carePlanRepo.DeleteAsync(id);
    }
}
