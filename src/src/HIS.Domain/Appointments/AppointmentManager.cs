using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using HIS.Settings;


namespace HIS.Appointments;

public class AppointmentManager : DomainService
{
    private readonly IRepository<Appointment, Guid> _appointmentRepository;
    private readonly IRepository<DoctorSchedule, Guid> _scheduleRepository;
    private readonly IRepository<Doctor, Guid> _doctorRepository;

    public AppointmentManager(
        IRepository<Appointment, Guid> appointmentRepository,
        IRepository<DoctorSchedule, Guid> scheduleRepository,
        IRepository<Doctor, Guid> doctorRepository)
    {
        _appointmentRepository = appointmentRepository;
        _scheduleRepository = scheduleRepository;
        _doctorRepository = doctorRepository;
    }

    public async Task<Appointment> CreateAsync(
        Guid patientId,
        Guid doctorId,
        Guid clinicId,
        DateTime appointmentDate,
        AppointmentType type,
        bool isWalkIn = false,
        string notes = null,
        Guid? serviceItemId = null)
    {
        // 1. Validation: Doctor Schedule
        // Logic: Check if doctor works on this day (DayOfWeek) and within hours
        // For MVP, we can skip strict time slot validation or implemented basically
        
        var dayOfWeek = appointmentDate.DayOfWeek;
        var schedule = await _scheduleRepository.FirstOrDefaultAsync(x => x.DoctorId == doctorId && x.DayOfWeek == dayOfWeek && x.IsActive);
        
        if (schedule == null)
        {
            // If no schedule found, maybe warn or error? 
            // For now, let's allow but maybe log (or require schedule strictly)
            // throw new UserFriendlyException("Doctor is not scheduled for this day.");
        }
        else
        {
             // Check time range - Allow bypass for Emergency or Walk-In
             var time = appointmentDate.TimeOfDay;
             if (!isWalkIn && type != AppointmentType.Emergency)
             {
                 if (time < schedule.StartTime || time >= schedule.EndTime)
                 {
                     throw new Volo.Abp.UserFriendlyException("Selected time is outside doctor's working hours.");
                 }
             }
        }

        // 2. Validation: Overlap / Double Booking
        // Allow Emergency or Walk-In to override
        if (!isWalkIn && type != AppointmentType.Emergency)
        {
            // Simple check: is there an appointment at this exact time?
            // Better: Check range (StartTime < ExistingEnd && EndTime > ExistingStart)
            // Assuming 15 min slots for now
            var endTime = appointmentDate.AddMinutes(15); 
            
            var overlap = await _appointmentRepository.AnyAsync(x => 
                x.DoctorId == doctorId && 
                x.Status != AppointmentStatus.Cancelled &&
                x.AppointmentDate < endTime && 
                x.AppointmentDate.AddMinutes(15) > appointmentDate // basic logic
            );

            if (overlap)
            {
                throw new Volo.Abp.UserFriendlyException("This time slot is already booked.");
            }
        }

        // 3. Calculate Fee
        var doctor = await _doctorRepository.GetAsync(doctorId);
        decimal fee = doctor.ConsultationFee; // Fallback
        
        var hour = appointmentDate.Hour;
        if (hour < 14) // Before 2 PM
        {
            fee = doctor.MorningConsultationFee > 0 ? doctor.MorningConsultationFee : doctor.ConsultationFee;
        }
        else // 2 PM or later
        {
            fee = doctor.EveningConsultationFee > 0 ? doctor.EveningConsultationFee : doctor.ConsultationFee;
        }

        return new Appointment(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            patientId,
            doctorId,
            clinicId,
            appointmentDate,
            AppointmentStatus.Scheduled,
            type
        )
        {
            Notes = notes,
            IsWalkIn = isWalkIn,
            ServiceItemId = serviceItemId,
            ConsultationFee = fee
        };
    }

    public async Task CancelAsync(Appointment appointment)
    {
        appointment.Status = AppointmentStatus.Cancelled;
        // logic to notify waiting list could go here
    }
}
