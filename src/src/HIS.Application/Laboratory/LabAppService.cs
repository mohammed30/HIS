using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using HIS.Laboratory.Dtos;
using HIS.Patients;
using HIS.Settings;
using HIS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Microsoft.AspNetCore.Hosting;

namespace HIS.Laboratory;

// [Authorize(HISPermissions.Laboratory.Default)] // Assuming permissions exist or will be added later
public class LabAppService : ApplicationService, ILabAppService
{
    private readonly IRepository<LabTest, Guid> _testRepository;
    private readonly IRepository<LabRequest, Guid> _requestRepository;
    private readonly IRepository<LabAppointment, Guid> _appointmentRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Doctor, Guid> _doctorRepository;
    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;

    public LabAppService(
        IRepository<LabTest, Guid> testRepository,
        IRepository<LabRequest, Guid> requestRepository,
        IRepository<LabAppointment, Guid> appointmentRepository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Doctor, Guid> doctorRepository,
        IRepository<ServiceItem, Guid> serviceItemRepository,
        IWebHostEnvironment env)
    {
        _testRepository = testRepository;
        _requestRepository = requestRepository;
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _serviceItemRepository = serviceItemRepository;
        _env = env;
    }

    private readonly IWebHostEnvironment _env;

    // --- TESTS ---

    public async Task<PagedResultDto<LabTestDto>> GetTestsAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _testRepository.GetQueryableAsync();
        
        var totalCount = await AsyncExecuter.CountAsync(query);
        
        query = query.OrderBy(input.Sorting ?? nameof(LabTest.Name))
                     .PageBy(input);

        var items = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<LabTestDto>(
            totalCount,
            ObjectMapper.Map<List<LabTest>, List<LabTestDto>>(items)
        );
    }

    public async Task<LabTestDto> CreateTestAsync(CreateUpdateLabTestDto input)
    {
        // Auto-generate code
        var code = await GenerateTestCodeAsync();
        
        var test = new LabTest(
            GuidGenerator.Create(),
            code,
            input.Name,
            input.Price
        )
        {
            Instructions = input.Instructions,
            ReferenceRange = input.ReferenceRange,
            Unit = input.Unit,
            IsActive = input.IsActive
        };

        await _testRepository.InsertAsync(test);
        return ObjectMapper.Map<LabTest, LabTestDto>(test);
    }
    
    private async Task<string> GenerateTestCodeAsync()
    {
        var query = await _testRepository.GetQueryableAsync();
        var count = await AsyncExecuter.CountAsync(query);
        return $"LAB-{(count + 1).ToString("D4")}"; // LAB-0001, LAB-0002, etc.
    }

    [HttpPut]
    [Route("api/app/lab/test/{id}")]
    public async Task<LabTestDto> UpdateTestAsync(Guid id, CreateUpdateLabTestDto input)
    {
        var test = await _testRepository.GetAsync(id);
        
        test.UpdateInfo(input.Name, input.Price, input.Instructions, input.ReferenceRange, input.Unit);
        test.IsActive = input.IsActive;

        await _testRepository.UpdateAsync(test);
        return ObjectMapper.Map<LabTest, LabTestDto>(test);
    }

    public async Task DeleteTestAsync(Guid id)
    {
        await _testRepository.DeleteAsync(id);
    }

    // --- REQUESTS ---

    public async Task<PagedResultDto<LabRequestDto>> GetRequestsAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _requestRepository.GetQueryableAsync();
        
        var totalCount = await AsyncExecuter.CountAsync(query);
        
        query = query.OrderBy(input.Sorting ?? "RequestDate DESC")
                     .PageBy(input);
                     
        var requests = await AsyncExecuter.ToListAsync(query);
        
        // Fetch related data
        var patientIds = requests.Select(x => x.PatientId).Distinct().ToList();
        var doctorIds = requests.Select(x => x.DoctorId).Distinct().ToList();
        var testIds = requests.Select(x => x.ServiceItemId).Distinct().ToList();

        var patients = await _patientRepository.GetListAsync(x => patientIds.Contains(x.Id));
        var doctors = await _doctorRepository.GetListAsync(x => doctorIds.Contains(x.Id));
        
        // Fetch from LabTests table instead of ServiceItems
        var labTests = await _testRepository.GetListAsync(x => testIds.Contains(x.Id));

        var dtos = requests.Select(r =>
        {
            var dto = ObjectMapper.Map<LabRequest, LabRequestDto>(r);
            var p = patients.FirstOrDefault(x => x.Id == r.PatientId);
            var d = doctors.FirstOrDefault(x => x.Id == r.DoctorId);
            var t = labTests.FirstOrDefault(x => x.Id == r.ServiceItemId);

            dto.PatientName = p != null ? $"{p.FirstNameAr} {p.LastNameAr}" : "Unknown";
            dto.DoctorName = d?.NameAr ?? "Unknown";
            dto.TestName = t?.Name ?? "Unknown";
            dto.TestCode = t?.Code ?? "-";
            
            return dto;
        }).ToList();

        return new PagedResultDto<LabRequestDto>(totalCount, dtos);
    }

    public async Task<LabRequestDto> CreateRequestAsync(CreateLabRequestDto input)
    {
        var request = new LabRequest(
            GuidGenerator.Create(),
            input.PatientId,
            input.DoctorId,
            input.ServiceItemId  // Using ServiceItemId from unified services
        )
        {
            Notes = input.Notes
        };

        await _requestRepository.InsertAsync(request);
        return ObjectMapper.Map<LabRequest, LabRequestDto>(request);
    }

    [HttpPost]
    [Route("api/app/lab/collect-sample/{id}")]
    public async Task<LabRequestDto> CollectSampleAsync(Guid id)
    {
        var request = await _requestRepository.GetAsync(id);
        if (request.Status != LabRequestStatus.Requested)
        {
            throw new Volo.Abp.UserFriendlyException("Sample can only be collected for Requested status.");
        }

        request.Status = LabRequestStatus.SampleCollected;
        await _requestRepository.UpdateAsync(request);
        return ObjectMapper.Map<LabRequest, LabRequestDto>(request);
    }

    [HttpPost]
    [Route("api/app/lab/complete-request/{id}")]
    public async Task<LabRequestDto> CompleteRequestAsync(Guid id, UpdateLabResultDto input)
    {
        var request = await _requestRepository.GetAsync(id);
        // Allow completion from any state for flexibility, or enforce flow
        
        request.Status = LabRequestStatus.Completed;
        request.Result = input.Result;
        request.Notes = input.Notes; // Update notes if needed

        await _requestRepository.UpdateAsync(request);
        return ObjectMapper.Map<LabRequest, LabRequestDto>(request);
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("api/app/lab/result-pdf/{id}")]
    public async Task<Volo.Abp.Content.IRemoteStreamContent> GetResultPdfAsync(Guid id)
    {
        var request = await _requestRepository.GetAsync(id);
        
        var patient = await _patientRepository.GetAsync(request.PatientId);
        var doctor = await _doctorRepository.GetAsync(request.DoctorId);
        // Fetch from LabTests as we switched to it
        var test = await _testRepository.GetAsync(request.ServiceItemId);

        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        
        // Try to load logo from wwwroot
        byte[] logoBytes = null;
        var logoPath = System.IO.Path.Combine(_env.WebRootPath ?? "", "images", "logo", "Dark.png");
        
        // Fallback for development
        if (!System.IO.File.Exists(logoPath))
        {
            var devPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "images", "logo", "Dark.png");
            if (System.IO.File.Exists(devPath)) logoPath = devPath;
        }

        if (System.IO.File.Exists(logoPath))
        {
            logoBytes = await System.IO.File.ReadAllBytesAsync(logoPath);
        }
        
        var document = new HIS.Laboratory.Printing.LabResultDocument
        {
            PatientName = $"{patient.FirstNameAr} {patient.LastNameAr}",
            PatientId = patient.Id.ToString().Substring(0, 8).ToUpper(),
            DoctorName = doctor.NameAr,
            TestName = test.Name,
            TestCode = test.Code,
            RequestDate = request.RequestDate,
            Result = request.Result,
            ReferenceRange = test.ReferenceRange,
            TestUnit = test.Unit,
            Notes = request.Notes,
            TechnicianName = "", // Can be populated from staff context if available
            LogoBytes = logoBytes
        };

        var pdfBytes = QuestPDF.Fluent.GenerateExtensions.GeneratePdf(document);
        var stream = new System.IO.MemoryStream(pdfBytes);
        var printTime = Clock.Now;
        return new Volo.Abp.Content.RemoteStreamContent(stream, $"نتيجة_تحليل_{printTime:yyyy-MM-dd_HH-mm-ss}.pdf", "application/pdf");
    }



    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("api/app/lab/request-order-pdf/{id}")]
    public async Task<Volo.Abp.Content.IRemoteStreamContent> GetRequestOrderPdfAsync(Guid id)
    {
        var request = await _requestRepository.GetAsync(id);
        var patient = await _patientRepository.GetAsync(request.PatientId);
        var doctor = await _doctorRepository.GetAsync(request.DoctorId);
        var test = await _testRepository.GetAsync(request.ServiceItemId);

        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        
        byte[] logoBytes = null;
        var logoPath = System.IO.Path.Combine(_env.WebRootPath ?? "", "images", "logo", "Dark.png");
        
        if (!System.IO.File.Exists(logoPath))
        {
            var devPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "images", "logo", "Dark.png");
            if (System.IO.File.Exists(devPath)) logoPath = devPath;
        }

        if (System.IO.File.Exists(logoPath)) logoBytes = await System.IO.File.ReadAllBytesAsync(logoPath);

        var document = new HIS.Laboratory.Printing.LabRequestDocument
        {
            PatientName = $"{patient.FirstNameAr} {patient.LastNameAr}",
            PatientId = patient.Id.ToString().Substring(0, 8).ToUpper(),
            DoctorName = doctor.NameAr,
            TestName = test.Name,
            TestCode = test.Code,
            RequestDate = request.RequestDate,
            Status = request.Status.ToString(),
            LogoBytes = logoBytes
        };

        var pdfBytes = QuestPDF.Fluent.GenerateExtensions.GeneratePdf(document);
        var stream = new System.IO.MemoryStream(pdfBytes);
        var printTime = Clock.Now;
        return new Volo.Abp.Content.RemoteStreamContent(stream, $"طلب_تحليل_{printTime:yyyy-MM-dd_HH-mm-ss}.pdf", "application/pdf");
    }

    // --- APPOINTMENTS (حجوزات المعمل) ---

    public async Task<PagedResultDto<LabAppointmentDto>> GetAppointmentsAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _appointmentRepository.GetQueryableAsync();
        
        var totalCount = await AsyncExecuter.CountAsync(query);
        
        query = query.OrderBy(input.Sorting ?? "AppointmentDate DESC")
                     .PageBy(input);
                     
        var appointments = await AsyncExecuter.ToListAsync(query);
        
        // Fetch related data
        var patientIds = appointments.Select(x => x.PatientId).Distinct().ToList();
        var serviceItemIds = appointments.Where(x => x.ServiceItemId.HasValue)
                                         .Select(x => x.ServiceItemId!.Value).Distinct().ToList();

        var patients = await _patientRepository.GetListAsync(x => patientIds.Contains(x.Id));
        var serviceItems = serviceItemIds.Any() 
            ? await _serviceItemRepository.GetListAsync(x => serviceItemIds.Contains(x.Id))
            : new List<ServiceItem>();

        var dtos = appointments.Select(a =>
        {
            var dto = ObjectMapper.Map<LabAppointment, LabAppointmentDto>(a);
            var p = patients.FirstOrDefault(x => x.Id == a.PatientId);
            var s = a.ServiceItemId.HasValue 
                ? serviceItems.FirstOrDefault(x => x.Id == a.ServiceItemId.Value) 
                : null;

            dto.PatientName = p != null ? $"{p.FirstNameAr} {p.LastNameAr}" : "Unknown";
            dto.TestName = s?.Name;
            dto.TestCode = s?.Code;
            
            return dto;
        }).ToList();

        return new PagedResultDto<LabAppointmentDto>(totalCount, dtos);
    }

    public async Task<LabAppointmentDto> GetAppointmentAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetAsync(id);
        var dto = ObjectMapper.Map<LabAppointment, LabAppointmentDto>(appointment);
        
        // Fetch patient name
        var patient = await _patientRepository.FindAsync(appointment.PatientId);
        dto.PatientName = patient != null ? $"{patient.FirstNameAr} {patient.LastNameAr}" : "Unknown";
        
        // Fetch test info if available
        if (appointment.ServiceItemId.HasValue)
        {
            var service = await _serviceItemRepository.FindAsync(appointment.ServiceItemId.Value);
            dto.TestName = service?.Name;
            dto.TestCode = service?.Code;
        }
        
        return dto;
    }

    public async Task<LabAppointmentDto> CreateAppointmentAsync(CreateLabAppointmentDto input)
    {
        var appointment = new LabAppointment(
            GuidGenerator.Create(),
            input.PatientId,
            input.AppointmentDate,
            input.ServiceItemId
        )
        {
            PreferredTime = input.PreferredTime,
            Notes = input.Notes,
            IsFasting = input.IsFasting
        };

        // Copy preparation instructions from ServiceItem if available
        if (input.ServiceItemId.HasValue)
        {
            var service = await _serviceItemRepository.FindAsync(input.ServiceItemId.Value);
            appointment.PreparationInstructions = service?.Instructions;
        }

        await _appointmentRepository.InsertAsync(appointment);
        return await GetAppointmentAsync(appointment.Id);
    }

    public async Task<LabAppointmentDto> UpdateAppointmentAsync(Guid id, UpdateLabAppointmentDto input)
    {
        var appointment = await _appointmentRepository.GetAsync(id);
        
        appointment.ServiceItemId = input.ServiceItemId;
        appointment.AppointmentDate = input.AppointmentDate;
        appointment.PreferredTime = input.PreferredTime;
        appointment.Notes = input.Notes;
        appointment.IsFasting = input.IsFasting;

        // Update preparation instructions if service changed
        if (input.ServiceItemId.HasValue)
        {
            var service = await _serviceItemRepository.FindAsync(input.ServiceItemId.Value);
            appointment.PreparationInstructions = service?.Instructions;
        }
        else
        {
            appointment.PreparationInstructions = null;
        }

        await _appointmentRepository.UpdateAsync(appointment);
        return await GetAppointmentAsync(id);
    }

    public async Task CancelAppointmentAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetAsync(id);
        appointment.Cancel();
        await _appointmentRepository.UpdateAsync(appointment);
    }

    public async Task<LabAppointmentDto> ConfirmAppointmentAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetAsync(id);
        appointment.Confirm();
        await _appointmentRepository.UpdateAsync(appointment);
        return await GetAppointmentAsync(id);
    }

    public async Task<LabAppointmentDto> CheckInAppointmentAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetAsync(id);
        appointment.CheckIn();
        await _appointmentRepository.UpdateAsync(appointment);
        return await GetAppointmentAsync(id);
    }

    public async Task<LabAppointmentDto> CompleteAppointmentAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetAsync(id);
        appointment.Complete();
        await _appointmentRepository.UpdateAsync(appointment);
        return await GetAppointmentAsync(id);
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("api/app/lab/appointment-pdf/{id}")]
    public async Task<Volo.Abp.Content.IRemoteStreamContent> GetAppointmentPdfAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetAsync(id);
        var patient = await _patientRepository.GetAsync(appointment.PatientId);
        
        string testName = "-", testCode = "-", instructions = "-";
        if (appointment.ServiceItemId.HasValue)
        {
            var service = await _serviceItemRepository.FindAsync(appointment.ServiceItemId.Value);
            testName = service?.Name;
            testCode = service?.Code;
            instructions = service?.Instructions;
        }
        
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        
        byte[] logoBytes = null;
        var logoPath = System.IO.Path.Combine(_env.WebRootPath ?? "", "images", "logo", "Dark.png");
        
        if (!System.IO.File.Exists(logoPath))
        {
            var devPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "images", "logo", "Dark.png");
            if (System.IO.File.Exists(devPath)) logoPath = devPath;
        }

        if (System.IO.File.Exists(logoPath)) logoBytes = await System.IO.File.ReadAllBytesAsync(logoPath);

        var document = new HIS.Laboratory.Printing.LabAppointmentDocument
        {
            PatientName = $"{patient.FirstNameAr} {patient.LastNameAr}",
            PatientId = patient.Id.ToString().Substring(0, 8).ToUpper(),
            AppointmentDate = appointment.AppointmentDate,
            PreferredTime = appointment.PreferredTime?.ToString(@"hh\:mm"),
            TestName = testName,
            TestCode = testCode,
            PreparationInstructions = instructions,
            IsFasting = appointment.IsFasting,
            LogoBytes = logoBytes
        };

        var pdfBytes = QuestPDF.Fluent.GenerateExtensions.GeneratePdf(document);
        var stream = new System.IO.MemoryStream(pdfBytes);
        var printTime = Clock.Now;
        return new Volo.Abp.Content.RemoteStreamContent(stream, $"حجز_معمل_{printTime:yyyy-MM-dd_HH-mm-ss}.pdf", "application/pdf");
    }
}


