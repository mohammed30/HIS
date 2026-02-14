using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HIS.Nursing;

public interface INursingAppService : IApplicationService
{
    // Medication Administration
    Task<MedicationAdministrationDto> CreateMedicationAdministrationAsync(CreateMedicationAdministrationDto input);
    Task<List<MedicationAdministrationDto>> GetMedicationAdministrationsAsync(Guid patientId);
    Task<List<DueMedicationDto>> GetDueMedicationsAsync(Guid patientId);
    
    // Care Plan

    Task<CarePlanDto> CreateCarePlanAsync(CreateCarePlanDto input);
    Task<List<CarePlanDto>> GetCarePlansAsync(Guid patientId);
    Task<CarePlanDto> UpdateCarePlanAsync(Guid id, CreateCarePlanDto input);
    Task DeleteCarePlanAsync(Guid id);
}
