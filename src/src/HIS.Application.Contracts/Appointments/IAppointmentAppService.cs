using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HIS.Appointments.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Appointments;

public interface IAppointmentAppService : IApplicationService
{
    Task<AppointmentDto> GetAsync(Guid id);
    Task<List<AppointmentDto>> GetListAsync(Guid? doctorId, DateTime? startDate, DateTime? endDate);
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto input);
    Task<AppointmentDto> UpdateAsync(Guid id, CreateAppointmentDto input); // Reusing Create DTO for simplicity
    Task CancelAsync(Guid id);
    Task<List<DateTime>> GetAvailableSlotsAsync(Guid doctorId, DateTime date);
    Task<List<LookupDto<Guid>>> GetDoctorLookupAsync(Guid? clinicId);
    Task<List<LookupDto<Guid>>> GetClinicLookupAsync();
}
