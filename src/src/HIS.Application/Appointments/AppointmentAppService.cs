using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Appointments.Dtos;
using HIS.Settings;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

using HIS.Services; // For ServiceCategory if needed
using HIS.Appointments; // For Enums

using Microsoft.AspNetCore.Authorization;
using HIS.Permissions;

namespace HIS.Appointments;

[Authorize(HISPermissions.Appointments.Default)]
public class AppointmentAppService : ApplicationService, IAppointmentAppService
{
    private readonly IRepository<Appointment, Guid> _appointmentRepository;
    private readonly IRepository<WaitingList, Guid> _waitingListRepository;
    private readonly IRepository<DoctorSchedule, Guid> _scheduleRepository;
    private readonly IRepository<Clinic, Guid> _clinicRepository;
    private readonly IRepository<Doctor, Guid> _doctorRepository;
    private readonly AppointmentManager _appointmentManager;

    public AppointmentAppService(
        IRepository<Appointment, Guid> appointmentRepository,
        IRepository<WaitingList, Guid> waitingListRepository,
        IRepository<DoctorSchedule, Guid> scheduleRepository,
        IRepository<Clinic, Guid> clinicRepository,
        IRepository<Doctor, Guid> doctorRepository,
        AppointmentManager appointmentManager)
    {
        _appointmentRepository = appointmentRepository;
        _waitingListRepository = waitingListRepository;
        _scheduleRepository = scheduleRepository;
        _clinicRepository = clinicRepository;
        _doctorRepository = doctorRepository;
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

    [Authorize(HISPermissions.Appointments.Create)]
    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto input)
    {
        var appt = await _appointmentManager.CreateAsync(
            input.PatientId,
            input.DoctorId,
            input.ClinicId,
            input.AppointmentDate,
            input.Type,
            input.IsWalkIn,
            input.Notes
        );

        await _appointmentRepository.InsertAsync(appt);
        return ObjectMapper.Map<Appointment, AppointmentDto>(appt);
    }

    [Authorize(HISPermissions.Appointments.Edit)]
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

    [Authorize(HISPermissions.Appointments.Edit)]
    public async Task CancelAsync(Guid id)
    {
        var appt = await _appointmentRepository.GetAsync(id);
        await _appointmentManager.CancelAsync(appt);
        await _appointmentRepository.UpdateAsync(appt);
    }

    [Authorize(HISPermissions.Appointments.Edit)]
    public async Task CheckInAsync(Guid id)
    {
        var appt = await _appointmentRepository.GetAsync(id);
        if (appt.Status != AppointmentStatus.Scheduled && appt.Status != AppointmentStatus.Confirmed)
        {
             // Allow check-in if scheduled/confirmed
             throw new Volo.Abp.UserFriendlyException("Cannot check-in. Appointment is not in Scheduled or Confirmed state.");
        }
        appt.Status = AppointmentStatus.CheckedIn;
        await _appointmentRepository.UpdateAsync(appt);
    }

    [Authorize(HISPermissions.Appointments.Edit)]
    public async Task StartConsultationAsync(Guid id)
    {
        var appt = await _appointmentRepository.GetAsync(id);
        if (appt.Status != AppointmentStatus.CheckedIn)
        {
             throw new Volo.Abp.UserFriendlyException("Patient must be Checked-In first.");
        }
        appt.Status = AppointmentStatus.InConsultation;
        await _appointmentRepository.UpdateAsync(appt);
    }

    [Authorize(HISPermissions.Appointments.Edit)]
    public async Task CompleteConsultationAsync(Guid id)
    {
        var appt = await _appointmentRepository.GetAsync(id);
        if (appt.Status != AppointmentStatus.InConsultation)
        {
             throw new Volo.Abp.UserFriendlyException("Appointments must be In-Consultation to complete.");
        }
        appt.Status = AppointmentStatus.Completed;
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
        var query = await _doctorRepository.GetQueryableAsync();
        
        if (clinicId.HasValue)
        {
            // Get the clinic to find its DepartmentId
            var clinic = await _clinicRepository.FindAsync(clinicId.Value);
            if (clinic != null)
            {
                query = query.Where(d => d.DepartmentId == clinic.DepartmentId);
            }
        }
        
        var doctors = await AsyncExecuter.ToListAsync(query.Where(d => d.IsActive));
        
        return doctors.Select(d => new LookupDto<Guid>
        {
            Id = d.Id,
            Name = d.NameAr ?? d.NameEn
        }).ToList();
    }

    public async Task<List<LookupDto<Guid>>> GetClinicLookupAsync()
    {
        var clinics = await _clinicRepository.GetListAsync(c => c.IsActive);
        
        return clinics.Select(c => new LookupDto<Guid>
        {
            Id = c.Id,
            Name = c.NameAr ?? c.NameEn
        }).ToList();
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

    [Authorize(HISPermissions.Appointments.Create)]
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

    [Authorize(HISPermissions.Appointments.Edit)]
    public async Task<WaitingListDto> UpdateWaitingListAsync(Guid id, CreateUpdateWaitingListDto input)
    {
        var item = await _waitingListRepository.GetAsync(id);
        item.Priority = input.Priority;
        item.Notes = input.Notes;
        item.IsResolved = input.IsResolved;
        
        await _waitingListRepository.UpdateAsync(item);
        return ObjectMapper.Map<WaitingList, WaitingListDto>(item);
    }

    [Authorize(HISPermissions.Appointments.Delete)]
    public async Task DeleteFromWaitingListAsync(Guid id)
    {
        await _waitingListRepository.DeleteAsync(id);
    }
}
