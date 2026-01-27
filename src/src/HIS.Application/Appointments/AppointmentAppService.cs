using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.ActivityLogs;
using HIS.Appointments.Dtos;
using HIS.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Appointments;

[Authorize]
public class AppointmentAppService : ApplicationService, IAppointmentAppService
{
    private readonly IRepository<Appointment, Guid> _appointmentRepository;
    private readonly IRepository<DoctorSchedule, Guid> _doctorScheduleRepository;
    private readonly IRepository<Doctor, Guid> _doctorRepository;
    private readonly IRepository<Clinic, Guid> _clinicRepository;
    private readonly ActivityLogManager _activityLogManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppointmentAppService(
        IRepository<Appointment, Guid> appointmentRepository,
        IRepository<DoctorSchedule, Guid> doctorScheduleRepository,
        IRepository<Doctor, Guid> doctorRepository,
        IRepository<Clinic, Guid> clinicRepository,
        ActivityLogManager activityLogManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _appointmentRepository = appointmentRepository;
        _doctorScheduleRepository = doctorScheduleRepository;
        _doctorRepository = doctorRepository;
        _clinicRepository = clinicRepository;
        _activityLogManager = activityLogManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AppointmentDto> GetAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetAsync(id);
        return await MapToDtoAsync(appointment);
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
            query = query.Where(x => x.AppointmentDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(x => x.AppointmentDate < endDate.Value);
        }

        var appointments = await AsyncExecuter.ToListAsync(query);
        var dtos = new List<AppointmentDto>();
        
        foreach (var app in appointments)
        {
            dtos.Add(await MapToDtoAsync(app));
        }

        return dtos;
    }

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto input)
    {
        // 1. Validate Doctor Schedule
        var dayOfWeek = input.AppointmentDate.DayOfWeek;
        var schedule = await _doctorScheduleRepository.FirstOrDefaultAsync(x => x.DoctorId == input.DoctorId && x.DayOfWeek == dayOfWeek && x.IsActive);

        if (schedule == null)
        {
            throw new UserFriendlyException("Doctor is not available on this day.");
        }

        // Check if time is within working hours
        var time = input.AppointmentDate.TimeOfDay;
        if (time < schedule.StartTime || time >= schedule.EndTime)
        {
             throw new UserFriendlyException("Selected time is outside working hours.");
        }

        // 2. Check for Overlap
        var existingAppointment = await _appointmentRepository.FirstOrDefaultAsync(x => 
            x.DoctorId == input.DoctorId && 
            x.AppointmentDate == input.AppointmentDate &&
            x.Status != AppointmentStatus.Cancelled);

        if (existingAppointment != null)
        {
            throw new UserFriendlyException("This slot is already booked.");
        }

        // 3. Create Appointment
        var appointment = new Appointment(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            CurrentUser.Id ?? Guid.Empty, // Assuming Patient is the current user for self-booking, or passed in input later
            input.DoctorId,
            input.ClinicId,
            input.AppointmentDate,
            AppointmentStatus.Pending,
            input.Type
        )
        {
            Notes = input.Notes
        };

        await _appointmentRepository.InsertAsync(appointment);

        // Log Activity
        await _activityLogManager.LogActivityAsync(
            module: "Appointments",
            action: ActivityAction.Create,
            description: $"تم حجز موعد جديد في {input.AppointmentDate:yyyy-MM-dd HH:mm}",
            entityType: "Appointment",
            entityId: appointment.Id.ToString(),
            newValues: new { input.DoctorId, input.ClinicId, input.AppointmentDate, input.Type },
            ipAddress: GetClientIp(),
            userAgent: GetUserAgent()
        );

        return await MapToDtoAsync(appointment);
    }

    public async Task<AppointmentDto> UpdateAsync(Guid id, CreateAppointmentDto input)
    {
        var appointment = await _appointmentRepository.GetAsync(id);

        if (appointment.AppointmentDate != input.AppointmentDate)
        {
             var dayOfWeek = input.AppointmentDate.DayOfWeek;
             var schedule = await _doctorScheduleRepository.FirstOrDefaultAsync(x => x.DoctorId == input.DoctorId && x.DayOfWeek == dayOfWeek && x.IsActive);
             if (schedule == null) throw new UserFriendlyException("Doctor is not available on this new day.");

             var existing = await _appointmentRepository.FirstOrDefaultAsync(x => x.DoctorId == input.DoctorId && x.AppointmentDate == input.AppointmentDate && x.Status != AppointmentStatus.Cancelled && x.Id != id);
             if (existing != null) throw new UserFriendlyException("This slot is already booked.");

             appointment.AppointmentDate = input.AppointmentDate;
        }

        appointment.Type = input.Type;
        appointment.Notes = input.Notes;
        
        await _appointmentRepository.UpdateAsync(appointment);
        return await MapToDtoAsync(appointment);
    }

    public async Task CancelAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetAsync(id);
        appointment.Status = AppointmentStatus.Cancelled;
        await _appointmentRepository.UpdateAsync(appointment);

        // Log Activity
        await _activityLogManager.LogActivityAsync(
            module: "Appointments",
            action: ActivityAction.Delete,
            description: $"تم إلغاء الموعد في {appointment.AppointmentDate:yyyy-MM-dd HH:mm}",
            entityType: "Appointment",
            entityId: id.ToString(),
            oldValues: new { appointment.DoctorId, appointment.AppointmentDate, appointment.Status },
            ipAddress: GetClientIp(),
            userAgent: GetUserAgent()
        );
    }

    public async Task<List<DateTime>> GetAvailableSlotsAsync(Guid doctorId, DateTime date)
    {
        var dayOfWeek = date.DayOfWeek;
        var schedule = await _doctorScheduleRepository.FirstOrDefaultAsync(x => x.DoctorId == doctorId && x.DayOfWeek == dayOfWeek && x.IsActive);

        if (schedule == null)
        {
            return new List<DateTime>();
        }

        var slots = new List<DateTime>();
        var currentTime = schedule.StartTime;
        var endTime = schedule.EndTime;
        var duration = TimeSpan.FromMinutes(schedule.SlotDuration);

        // Get booked appointments
        var bookedDates = await _appointmentRepository.GetListAsync(x => 
            x.DoctorId == doctorId && 
            x.AppointmentDate >= date.Date && 
            x.AppointmentDate < date.Date.AddDays(1) && 
            x.Status != AppointmentStatus.Cancelled);
        
        var bookedTimes = bookedDates.Select(x => x.AppointmentDate.TimeOfDay).ToHashSet();

        while (currentTime + duration <= endTime)
        {
            if (!bookedTimes.Contains(currentTime))
            {
                slots.Add(date.Date.Add(currentTime));
            }
            currentTime = currentTime.Add(duration);
        }

        return slots;
    }

    private async Task<AppointmentDto> MapToDtoAsync(Appointment appointment)
    {
        var doctor = await _doctorRepository.GetAsync(appointment.DoctorId);
        var clinic = await _clinicRepository.GetAsync(appointment.ClinicId);
        
        // TODO: Get Patient Name (Assuming User for now or Patient Repository)
        var patientName = "Patient"; 

        return new AppointmentDto
        {
            Id = appointment.Id,
            CreationTime = appointment.CreationTime,
            DoctorId = appointment.DoctorId,
            DoctorName = doctor.NameAr,
            ClinicId = appointment.ClinicId,
            ClinicName = clinic.NameAr,
            PatientId = appointment.PatientId,
            PatientName = patientName,
            AppointmentDate = appointment.AppointmentDate,
            Status = appointment.Status,
            Type = appointment.Type,
            Notes = appointment.Notes
        };
    }

    public async Task<List<LookupDto<Guid>>> GetDoctorLookupAsync(Guid? clinicId)
    {
        var query = await _doctorRepository.GetQueryableAsync();
        
        if (clinicId.HasValue)
        {
            var clinic = await _clinicRepository.GetAsync(clinicId.Value);
            query = query.Where(x => x.DepartmentId == clinic.DepartmentId);
        }
        
        var doctors = await AsyncExecuter.ToListAsync(query);
        return doctors.Select(x => new LookupDto<Guid> { Id = x.Id, Name = x.NameAr }).ToList();
    }

    public async Task<List<LookupDto<Guid>>> GetClinicLookupAsync()
    {
        var clinics = await _clinicRepository.GetListAsync();
        return clinics.Select(x => new LookupDto<Guid> { Id = x.Id, Name = x.NameAr }).ToList();
    }

    private string? GetClientIp() => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    private string? GetUserAgent() => _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString();
}
