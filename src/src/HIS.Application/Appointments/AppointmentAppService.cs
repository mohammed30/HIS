using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Appointments.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

using HIS.Services; // For ServiceCategory if needed
using HIS.Appointments; // For Enums

namespace HIS.Appointments;

public class AppointmentAppService : ApplicationService, IAppointmentAppService
{
    private readonly IRepository<Appointment, Guid> _appointmentRepository;
    private readonly IRepository<WaitingList, Guid> _waitingListRepository;
    private readonly IRepository<DoctorSchedule, Guid> _scheduleRepository;
    private readonly AppointmentManager _appointmentManager;

    public AppointmentAppService(
        IRepository<Appointment, Guid> appointmentRepository,
        IRepository<WaitingList, Guid> waitingListRepository,
        IRepository<DoctorSchedule, Guid> scheduleRepository,
        AppointmentManager appointmentManager)
    {
        _appointmentRepository = appointmentRepository;
        _waitingListRepository = waitingListRepository;
        _scheduleRepository = scheduleRepository;
        _appointmentManager = appointmentManager;
    }

    // --- APPOINTMENTS ---

    public async Task<AppointmentDto> GetAsync(Guid id)
    {
        var appt = await _appointmentRepository.GetAsync(id);
        return ObjectMapper.Map<Appointment, AppointmentDto>(appt);
    }

    public async Task<List<AppointmentDto>> GetListAsync(Guid? doctorId, DateTime? startDate, DateTime? endDate)
    {
        var query = await _appointmentRepository.GetQueryableAsync();

        if (doctorId.HasValue)
        {
            query = query.Where(x => x.DoctorId == doctorId);
        }

        if (startDate.HasValue)
        {
            query = query.Where(x => x.AppointmentDate >= startDate);
        }

        if (endDate.HasValue)
        {
            query = query.Where(x => x.AppointmentDate <= endDate);
        }

        var items = await AsyncExecuter.ToListAsync(query);
        return ObjectMapper.Map<List<Appointment>, List<AppointmentDto>>(items);
    }

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto input)
    {
        var appt = await _appointmentManager.CreateAsync(
            input.PatientId,
            input.DoctorId,
            input.ClinicId,
            input.AppointmentDate,
            input.Type,
            input.Notes
        );

        await _appointmentRepository.InsertAsync(appt);
        return ObjectMapper.Map<Appointment, AppointmentDto>(appt);
    }

    public async Task<AppointmentDto> UpdateAsync(Guid id, CreateAppointmentDto input)
    {
        // Simple update: reschedule
        var appt = await _appointmentRepository.GetAsync(id);
        
        // If date changes, validate again?
        if (appt.AppointmentDate != input.AppointmentDate)
        {
             // ... Logic to re-validate schedule ...
             appt.AppointmentDate = input.AppointmentDate;
        }

        appt.Notes = input.Notes;
        appt.Type = input.Type;
        
        await _appointmentRepository.UpdateAsync(appt);
        return ObjectMapper.Map<Appointment, AppointmentDto>(appt);
    }

    public async Task CancelAsync(Guid id)
    {
        var appt = await _appointmentRepository.GetAsync(id);
        await _appointmentManager.CancelAsync(appt);
        await _appointmentRepository.UpdateAsync(appt);
    }

    public async Task<List<DateTime>> GetAvailableSlotsAsync(Guid doctorId, DateTime date)
    {
        // MVP: Just return 15 min slots for the working day that are not booked
        var schedule = await _scheduleRepository.FirstOrDefaultAsync(x => x.DoctorId == doctorId && x.DayOfWeek == date.DayOfWeek && x.IsActive);
        if (schedule == null) return new List<DateTime>();

        var slots = new List<DateTime>();
        var current = date.Date.Add(schedule.StartTime);
        var end = date.Date.Add(schedule.EndTime);

        // Get bookings
        var bookings = await _appointmentRepository.GetListAsync(x => x.DoctorId == doctorId && x.AppointmentDate >= current && x.AppointmentDate <= end && x.Status != AppointmentStatus.Cancelled);

        while (current < end)
        {
            if (!bookings.Any(b => b.AppointmentDate == current)) // Exact match for MVP
            {
                slots.Add(current);
            }
            current = current.AddMinutes(schedule.SlotDuration > 0 ? schedule.SlotDuration : 15);
        }

        return slots;
    }

    public async Task<List<LookupDto<Guid>>> GetDoctorLookupAsync(Guid? clinicId)
    {
        // Mock or simple query
        return new List<LookupDto<Guid>>(); 
    }

    public async Task<List<LookupDto<Guid>>> GetClinicLookupAsync()
    {
        return new List<LookupDto<Guid>>();
    }

    // --- WAITING LIST ---

    public async Task<PagedResultDto<WaitingListDto>> GetWaitingListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _waitingListRepository.GetQueryableAsync();
        query = query.Where(x => !x.IsResolved);

        var count = await AsyncExecuter.CountAsync(query);
        query = query.PageBy(input);

        var items = await AsyncExecuter.ToListAsync(query);
        return new PagedResultDto<WaitingListDto>(
            count,
            ObjectMapper.Map<List<WaitingList>, List<WaitingListDto>>(items)
        );
    }

    public async Task<WaitingListDto> AddToWaitingListAsync(CreateUpdateWaitingListDto input)
    {
        var item = new WaitingList(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            input.PatientId,
            input.DepartmentId,
            input.DoctorId,
            input.RequestDate,
            input.Priority
        )
        {
            Notes = input.Notes
        };

        await _waitingListRepository.InsertAsync(item);
        return ObjectMapper.Map<WaitingList, WaitingListDto>(item);
    }

    public async Task<WaitingListDto> UpdateWaitingListAsync(Guid id, CreateUpdateWaitingListDto input)
    {
        var item = await _waitingListRepository.GetAsync(id);
        item.Priority = input.Priority;
        item.Notes = input.Notes;
        item.IsResolved = input.IsResolved;
        
        await _waitingListRepository.UpdateAsync(item);
        return ObjectMapper.Map<WaitingList, WaitingListDto>(item);
    }

    public async Task DeleteFromWaitingListAsync(Guid id)
    {
        await _waitingListRepository.DeleteAsync(id);
    }
}
