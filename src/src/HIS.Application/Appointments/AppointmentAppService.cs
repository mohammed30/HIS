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
using Microsoft.AspNetCore.Hosting; // For Logo
using System.IO;
using HIS.Patients;
using HIS.Services;
using HIS.Appointments.Printing; // For TicketDocument
using QuestPDF.Fluent; // For GeneratePdf extension
using QuestPDF.Infrastructure; // For LicenseType
using Microsoft.AspNetCore.Mvc; // For routing attributes

namespace HIS.Appointments;

[Authorize(HISPermissions.Appointments.Default)]
public class AppointmentAppService : ApplicationService, IAppointmentAppService
{
    private readonly IRepository<Appointment, Guid> _appointmentRepository;
    private readonly IRepository<WaitingList, Guid> _waitingListRepository;
    private readonly IRepository<DoctorSchedule, Guid> _scheduleRepository;
    private readonly IRepository<Clinic, Guid> _clinicRepository;
    private readonly IRepository<Doctor, Guid> _doctorRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<ServiceItem, Guid> _serviceRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly AppointmentManager _appointmentManager;

    public AppointmentAppService(
        IRepository<Appointment, Guid> appointmentRepository,
        IRepository<WaitingList, Guid> waitingListRepository,
        IRepository<DoctorSchedule, Guid> scheduleRepository,
        IRepository<Clinic, Guid> clinicRepository,
        IRepository<Doctor, Guid> doctorRepository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<ServiceItem, Guid> serviceRepository,
        IWebHostEnvironment webHostEnvironment,
        AppointmentManager appointmentManager)
    {
        _appointmentRepository = appointmentRepository;
        _waitingListRepository = waitingListRepository;
        _scheduleRepository = scheduleRepository;
        _clinicRepository = clinicRepository;
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _serviceRepository = serviceRepository;
        _webHostEnvironment = webHostEnvironment;
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

    [Authorize(HISPermissions.Appointments.Create)]
    public async Task<AppointmentDto> BookClinicAppointmentAsync(BookClinicAppointmentDto input)
    {
        // 1. Create Appointment
        var appt = await _appointmentManager.CreateAsync(
            input.PatientId,
            input.DoctorId,
            input.ClinicId,
            input.AppointmentDate,
            input.Type,
            input.IsWalkIn, // Use from input
            "Clinic Booking",
            input.ServiceItemId // Pass ServiceItemId
        );
        
        await _appointmentRepository.InsertAsync(appt);

        // 2. Create Invoice if requested
        if (input.CreateInvoice)
        {
             // TODO: Inject IInvoiceAppService or use InvoiceManager directly
             // For now, we'll assume the client handles invoice creation via a separate call if needed, 
             // OR we inject InvoiceAppService here.
             // Given the scope, let's keep it simple: The frontend calls this for booking, 
             // and if it wants an invoice, it calls InvoiceService.Create separately OR we expand this later.
             // 
             // WAITING: To properly implement "Book/Bond" (Hajz/Sanad), we need to create the invoice here transactionally.
             // I'll add a TODO/Placeholder for Invoice integration.
        }

        return ObjectMapper.Map<Appointment, AppointmentDto>(appt);
    }

    [HttpGet]
    [Route("api/app/appointment/ticket-pdf")]
    public async Task<Volo.Abp.Content.IRemoteStreamContent> GetTicketPdfAsync([FromQuery] Guid appointmentId)
    {
        var appt = await _appointmentRepository.GetAsync(appointmentId);
        var patient = await _patientRepository.GetAsync(appt.PatientId);
        var doctor = await _doctorRepository.GetAsync(appt.DoctorId);
        var clinic = await _clinicRepository.GetAsync(appt.ClinicId);
        
        string serviceName = "زيارة عيادة / Consultation";
        decimal price = 0;

        if (appt.ServiceItemId.HasValue)
        {
             var service = await _serviceRepository.FindAsync(appt.ServiceItemId.Value);
             if (service != null)
             {
                 serviceName = service.Name;
                 price = service.Price;
             }
        }
        
        // Logo
        byte[] logoBytes = null;
        var logoPaths = new[]
        {
            Path.Combine(_webHostEnvironment.WebRootPath, "images", "logo", "Dark.png"),
            Path.Combine(_webHostEnvironment.WebRootPath, "images", "logo", "leptonxlite", "logo-dark.png"),
            Path.Combine(_webHostEnvironment.WebRootPath, "images", "logo", "logo-dark.png")
        };

        foreach (var path in logoPaths)
        {
            if (File.Exists(path))
            {
                logoBytes = await File.ReadAllBytesAsync(path);
                break;
            }
        }

        QuestPDF.Settings.License = LicenseType.Community;

        var document = new TicketDocument
        {
            TicketNumber = appt.Id.ToString().Substring(0, 8).ToUpper(),
            Date = appt.AppointmentDate,
            PatientName = patient.FullNameAr ?? (patient.FirstNameAr + " " + patient.LastNameAr),
            PatientFileNumber = patient.MRN,
            ClinicName = clinic.NameAr ?? clinic.NameEn,
            DoctorName = doctor.NameAr ?? doctor.NameEn,
            ServiceName = serviceName,
            Amount = price,
            UserName = CurrentUser.Name ?? "admin",
            LogoBytes = logoBytes
        };


        byte[] pdfBytes = document.GeneratePdf();
        var ms = new MemoryStream(pdfBytes);
        
        return new Volo.Abp.Content.RemoteStreamContent(
            ms, 
            $"Ticket_{appt.Id.ToString().Substring(0,8)}.pdf", 
            "application/pdf"
        );
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
