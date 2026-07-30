using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using HIS.Laboratory.Dtos;
using HIS.Patients;
using HIS.Settings;
using HIS.Services;
using HIS.Inventory;
using HIS.Inpatient;
using HIS.Rooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;


namespace HIS.Laboratory;

// [Authorize(HISPermissions.Laboratory.Default)] // Assuming permissions exist or will be added later
public class LabAppService : ApplicationService, ILabAppService
{
    private readonly IRepository<LabTest, Guid> _testRepository;
    private readonly IRepository<LabTestCategory, Guid> _categoryRepository;
    private readonly IRepository<LabRequest, Guid> _requestRepository;
    private readonly IRepository<LabAppointment, Guid> _appointmentRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Doctor, Guid> _doctorRepository;
    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;
    private readonly IRepository<InternalRequest, Guid> _internalRequestRepository;
    private readonly IRepository<Department, Guid> _departmentRepository;
    private readonly IRepository<Admission, Guid> _admissionRepository;
    private readonly IRepository<Room, Guid> _roomRepository;

    public LabAppService(
        IRepository<LabTest, Guid> testRepository,
        IRepository<LabTestCategory, Guid> categoryRepository,
        IRepository<LabRequest, Guid> requestRepository,
        IRepository<LabAppointment, Guid> appointmentRepository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Doctor, Guid> doctorRepository,
        IRepository<ServiceItem, Guid> serviceItemRepository,
        IRepository<InternalRequest, Guid> internalRequestRepository,
        IRepository<Department, Guid> departmentRepository,
        IRepository<Admission, Guid> admissionRepository,
        IRepository<Room, Guid> roomRepository)
    {
        _testRepository = testRepository;
        _categoryRepository = categoryRepository;
        _requestRepository = requestRepository;
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _serviceItemRepository = serviceItemRepository;
        _internalRequestRepository = internalRequestRepository;
        _departmentRepository = departmentRepository;
        _admissionRepository = admissionRepository;
        _roomRepository = roomRepository;
    }


    // --- CATEGORIES ---

    public async Task<List<LabTestCategoryDto>> GetCategoriesWithTestsAsync()
    {
        var categories = await _categoryRepository.GetListAsync();
        var tests = await _testRepository.GetListAsync();

        var categoryDtos = categories
            .OrderBy(c => c.SortOrder)
            .Select(c => new LabTestCategoryDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                ParentId = c.ParentId,
                SortOrder = c.SortOrder,
                IsActive = c.IsActive
            }).ToList();

        var testDtos = tests
            .OrderBy(t => t.Code)
            .Select(t =>
            {
                var dto = ObjectMapper.Map<LabTest, LabTestDto>(t);
                dto.CategoryName = categories.FirstOrDefault(c => c.Id == t.CategoryId)?.Name;
                return dto;
            }).ToList();

        // Build tree: assign tests & children to parent categories
        foreach (var cat in categoryDtos)
        {
            cat.Tests = testDtos.Where(t => t.CategoryId == cat.Id).ToList();
            cat.Children = categoryDtos.Where(c => c.ParentId == cat.Id).OrderBy(c => c.SortOrder).ToList();
        }

        // Return only root categories (no parent)
        return categoryDtos.Where(c => c.ParentId == null).ToList();
    }

    public async Task<List<LabTestCategoryDto>> GetCategoriesAsync()
    {
        var categories = await _categoryRepository.GetListAsync();
        return categories
            .OrderBy(c => c.SortOrder)
            .Select(c => new LabTestCategoryDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                ParentId = c.ParentId,
                SortOrder = c.SortOrder,
                IsActive = c.IsActive
            }).ToList();
    }

    // --- TESTS ---

    public async Task<PagedResultDto<LabTestDto>> GetTestsAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _testRepository.GetQueryableAsync();
        
        var totalCount = await AsyncExecuter.CountAsync(query);
        
        query = query.OrderBy(input.Sorting ?? nameof(LabTest.Name))
                     .PageBy(input);

        var items = await AsyncExecuter.ToListAsync(query);

        // Fetch category names
        var categoryIds = items.Where(x => x.CategoryId.HasValue).Select(x => x.CategoryId!.Value).Distinct().ToList();
        var categories = categoryIds.Any()
            ? await _categoryRepository.GetListAsync(x => categoryIds.Contains(x.Id))
            : new List<LabTestCategory>();

        var dtos = items.Select(t =>
        {
            var dto = ObjectMapper.Map<LabTest, LabTestDto>(t);
            dto.CategoryName = t.CategoryId.HasValue
                ? categories.FirstOrDefault(c => c.Id == t.CategoryId.Value)?.Name
                : null;
            return dto;
        }).ToList();

        return new PagedResultDto<LabTestDto>(totalCount, dtos);
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
            CategoryId = input.CategoryId,
            IsActive = input.IsActive
        };

        await _testRepository.InsertAsync(test);
        return ObjectMapper.Map<LabTest, LabTestDto>(test);
    }
    
    private async Task<string> GenerateTestCodeAsync()
    {
        // TODO: Replace with Database Sequence or IEntityCodeGenerator to prevent race conditions
        var uniqueSuffix = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
        return $"LAB-{uniqueSuffix}";
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

    public async Task<PagedResultDto<LabRequestDto>> GetRequestsAsync(GetLabRequestsInput input)
    {
        var requestQuery = await _requestRepository.GetQueryableAsync();
        var patientQuery = await _patientRepository.GetQueryableAsync();
        var doctorQuery = await _doctorRepository.GetQueryableAsync();
        var testQuery = await _testRepository.GetQueryableAsync();

        // Apply filters to requestQuery
        if (input.FromDate.HasValue)
        {
            var fromDate = input.FromDate.Value.Date;
            requestQuery = requestQuery.Where(x => x.RequestDate >= fromDate);
        }

        if (input.ToDate.HasValue)
        {
            var toDate = input.ToDate.Value.Date.AddDays(1);
            requestQuery = requestQuery.Where(x => x.RequestDate < toDate);
        }

        if (input.Status.HasValue)
        {
            requestQuery = requestQuery.Where(x => x.Status == input.Status.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(requestQuery);

        // Perform joins (Left join for doctor to handle non-doctor submitters)
        var combinedQuery = from request in requestQuery
                            join patient in patientQuery on request.PatientId equals patient.Id
                            join doctor in doctorQuery on request.DoctorId equals doctor.Id into doctorGroup
                            from doctor in doctorGroup.DefaultIfEmpty()
                            join test in testQuery on request.ServiceItemId equals test.Id
                            select new { request, patient, doctor, test };

        combinedQuery = combinedQuery.OrderBy(input.Sorting != null ? "request." + input.Sorting : "request.RequestDate DESC")
                                     .PageBy(input);

        var results = await AsyncExecuter.ToListAsync(combinedQuery);

        var dtos = new List<LabRequestDto>();
        foreach (var x in results)
        {
            var dto = ObjectMapper.Map<LabRequest, LabRequestDto>(x.request);
            dto.PatientName = $"{x.patient.FirstNameAr} {x.patient.LastNameAr}";
            dto.DoctorName = x.doctor?.NameAr ?? "N/A";
            dto.TestName = x.test.Name;
            dto.TestCode = x.test.Code;
            dto.ReferenceRange = x.test.ReferenceRange;
            dto.Unit = x.test.Unit;

            // Enrich with Inpatient context if it's a nursing request
            if (x.request.Notes != null && x.request.Notes.Contains("Nursing Req:"))
            {
                var reqPart = x.request.Notes.Split('.').First(); // Nursing Req: REQ-xxxx
                var reqNumber = reqPart.Replace("Nursing Req: ", "").Trim();
                
                var internalRequest = await _internalRequestRepository.FirstOrDefaultAsync(ir => ir.RequestNumber == reqNumber);
                if (internalRequest != null)
                {
                    var dept = await _departmentRepository.FindAsync(internalRequest.RequestingDepartmentId);
                    dto.RequestingDepartmentName = dept?.NameAr ?? dept?.NameEn ?? "N/A";

                    if (internalRequest.AdmissionId.HasValue)
                    {
                        var admission = await _admissionRepository.FindAsync(internalRequest.AdmissionId.Value);
                        if (admission != null)
                        {
                            var room = await _roomRepository.FindAsync(admission.RoomId);
                            dto.AdmissionRoom = room?.RoomNumber ?? "N/A";
                        }
                    }
                }
            }

            dtos.Add(dto);
        }

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
        if (string.IsNullOrEmpty(request.SampleNumber))
        {
            request.SampleNumber = await GenerateSampleNumberAsync();
        }

        request.Status = LabRequestStatus.SampleCollected;
        await _requestRepository.UpdateAsync(request);
        return ObjectMapper.Map<LabRequest, LabRequestDto>(request);
    }

    private async Task<string> GenerateSampleNumberAsync()
    {
        var datePrefix = DateTime.Now.ToString("yyMMdd");
        var uniqueSuffix = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
        // TODO: Replace with Database Sequence to prevent race conditions while keeping readable sequential format
        return $"{datePrefix}-{uniqueSuffix}";
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

        // --- Trigger Notification ---
        try
        {
            var notificationRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Notifications.Notification, Guid>>();
            var notificationSender = LazyServiceProvider.LazyGetRequiredService<HIS.Notifications.NotificationSender>();
            var settingProvider = LazyServiceProvider.LazyGetRequiredService<Volo.Abp.Settings.ISettingProvider>();

            var settingValue = await settingProvider.GetOrNullAsync("Notifications.Subscribers.Laboratory");
            var userIds = string.IsNullOrWhiteSpace(settingValue) ? new List<Guid>() : settingValue.Split(',').Select(Guid.Parse).ToList();

            if (userIds.Any())
            {
                var notifications = userIds.Select(id => new HIS.Notifications.Notification(
                    GuidGenerator.Create(), 
                    id, 
                    "نتيجة تحليل جاهزة", 
                    $"تم اعتماد نتيجة التحليل الخاصة بالمريض", 
                    "Laboratory", 
                    "/laboratory/requests", 
                    request.Id.ToString(), 
                    CurrentUser.UserName ?? "النظام")).ToList();
                
                await notificationRepo.InsertManyAsync(notifications);
                foreach (var notif in notifications)
                {
                    var dto = ObjectMapper.Map<HIS.Notifications.Notification, HIS.Notifications.NotificationDto>(notif);
                    await notificationSender.SendToUserAsync(notif.UserId, dto);
                }
            }
        }
        catch (Exception ex)
        {
            Microsoft.Extensions.Logging.LoggerExtensions.LogError(LazyServiceProvider.LazyGetRequiredService<Microsoft.Extensions.Logging.ILogger<LabAppService>>(), ex, "Failed to send notification");
        }

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
        
        // Load logo from a centralized setting or domain service, not IWebHostEnvironment
        byte[] logoBytes = null;

        
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
    [Microsoft.AspNetCore.Mvc.Route("api/app/lab/sample-barcode-pdf/{id}")]
    public async Task<Volo.Abp.Content.IRemoteStreamContent> GetSampleBarcodePdfAsync(Guid id)
    {
        var request = await _requestRepository.GetAsync(id);
        var patient = await _patientRepository.GetAsync(request.PatientId);
        var test = await _testRepository.GetAsync(request.ServiceItemId);

        if (string.IsNullOrEmpty(request.SampleNumber))
        {
            request.SampleNumber = await GenerateSampleNumberAsync();
            await _requestRepository.UpdateAsync(request);
        }

        var document = new HIS.Laboratory.Printing.LabSampleBarcodeDocument
        {
            PatientName = $"{patient.FirstNameAr} {patient.LastNameAr}",
            SampleNumber = request.SampleNumber,
            TestName = test.Name,
            RequestDate = request.RequestDate
        };

        var pdfBytes = QuestPDF.Fluent.GenerateExtensions.GeneratePdf(document);
        var stream = new System.IO.MemoryStream(pdfBytes);
        return new Volo.Abp.Content.RemoteStreamContent(stream, $"بارcode_عينة_{request.SampleNumber}.pdf", "application/pdf");
    }



    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("api/app/lab/request-order-pdf/{id}")]
    public async Task<Volo.Abp.Content.IRemoteStreamContent> GetRequestOrderPdfAsync(Guid id)
    {
        var request = await _requestRepository.GetAsync(id);
        var patient = await _patientRepository.GetAsync(request.PatientId);
        var doctor = await _doctorRepository.GetAsync(request.DoctorId);
        var test = await _testRepository.GetAsync(request.ServiceItemId);
        
        // Load logo from a centralized setting or domain service, not IWebHostEnvironment
        byte[] logoBytes = null;


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
        
        // Load logo from a centralized setting or domain service, not IWebHostEnvironment
        byte[] logoBytes = null;


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


