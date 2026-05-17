using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Patients;
using HIS.Settings;
using HIS.Services;
using HIS.Rooms;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Content;
using HIS.Inventory;
using HIS.Inpatient;

namespace HIS.Radiology;

public class RadiologyAppService : CrudAppService<RadiologyRequest, RadiologyRequestDto, Guid, GetRadiologyRequestInput, CreateUpdateRadiologyRequestDto>, IRadiologyAppService
{
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Doctor, Guid> _doctorRepository;
    private readonly IRepository<RadiologyItem, Guid> _radiologyItemRepository;
    private readonly IRepository<InternalRequest, Guid> _internalRequestRepository;
    private readonly IRepository<Department, Guid> _departmentRepository;
    private readonly IRepository<Admission, Guid> _admissionRepository;
    private readonly IRepository<Room, Guid> _roomRepository;

    private readonly IWebHostEnvironment _env;

    public RadiologyAppService(
        IRepository<RadiologyRequest, Guid> repository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Doctor, Guid> doctorRepository,
        IRepository<RadiologyItem, Guid> radiologyItemRepository,
        IRepository<InternalRequest, Guid> internalRequestRepository,
        IRepository<Department, Guid> departmentRepository,
        IRepository<Admission, Guid> admissionRepository,
        IRepository<Room, Guid> roomRepository,
        IWebHostEnvironment env) 
        : base(repository)
    {
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _radiologyItemRepository = radiologyItemRepository;
        _internalRequestRepository = internalRequestRepository;
        _departmentRepository = departmentRepository;
        _admissionRepository = admissionRepository;
        _roomRepository = roomRepository;
        _env = env;
    }

    public override async Task<RadiologyRequestDto> GetAsync(Guid id)
    {
        var entity = await Repository.GetAsync(id);
        return await MapToDtoAsync(entity);
    }

    public override async Task<PagedResultDto<RadiologyRequestDto>> GetListAsync(GetRadiologyRequestInput input)
    {
        var query = await Repository.GetQueryableAsync();

        // Basic filtering example (can be extended)
        query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
            x => x.RequestNumber.Contains(input.Filter) || x.ReportBody.Contains(input.Filter));
            
        query = query.WhereIf(input.Status.HasValue, x => x.Status == input.Status.Value);

        var totalCount = await AsyncExecuter.CountAsync(query);

        var items = await AsyncExecuter.ToListAsync(
            query.OrderByDescending(x => x.RequestDate)
                 .PageBy(input.SkipCount, input.MaxResultCount)
        );

        var dtos = new List<RadiologyRequestDto>();
        foreach (var item in items)
        {
            dtos.Add(await MapToDtoAsync(item));
        }

        return new PagedResultDto<RadiologyRequestDto>(totalCount, dtos);
    }

    private async Task<RadiologyRequestDto> MapToDtoAsync(RadiologyRequest entity)
    {
        var dto = ObjectMapper.Map<RadiologyRequest, RadiologyRequestDto>(entity);
        
        var patient = await _patientRepository.FindAsync(entity.PatientId);
        dto.PatientName = patient?.FullNameAr ?? patient?.FullNameEn ?? "N/A";

        if (entity.DoctorId.HasValue)
        {
            var doctor = await _doctorRepository.FindAsync(entity.DoctorId.Value);
            dto.DoctorName = doctor?.NameAr ?? doctor?.NameEn ?? "N/A";
        }

        var radItem = await _radiologyItemRepository.FindAsync(entity.RadiologyItemId);
        dto.RadiologyItemName = radItem?.Name ?? "N/A";

        // Enrichment from Nursing Request
        if (!string.IsNullOrEmpty(entity.RequestNumber))
        {
            var internalRequest = (await _internalRequestRepository.GetQueryableAsync())
                .FirstOrDefault(x => x.RequestNumber == entity.RequestNumber);
                
            if (internalRequest != null)
            {
                var dept = await _departmentRepository.FindAsync(internalRequest.RequestingDepartmentId);
                dto.RequestingDepartmentName = dept?.NameAr ?? dept?.NameEn ?? "Unknown Ward";
                
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

        return dto;
    }

    public override async Task<RadiologyRequestDto> UpdateAsync(Guid id, CreateUpdateRadiologyRequestDto input)
    {
        var entity = await Repository.GetAsync(id);
        
        entity.ReportBody = input.ReportBody;
        entity.TechnicianNotes = input.TechnicianNotes;
        entity.Status = input.Status;
        
        if (input.Status == RadiologyRequestStatus.Reported && !entity.ReportDate.HasValue)
        {
            entity.ReportDate = DateTime.Now;
            entity.RadiologistId = CurrentUser.Id;
        }

        await Repository.UpdateAsync(entity);
        return await MapToDtoAsync(entity);
    }
    [Microsoft.AspNetCore.Mvc.HttpGet("patient-results")]
    public async Task<List<RadiologyRequestDto>> GetPatientResultsAsync(Guid patientId)
    {
        var items = await Repository.GetListAsync(x => x.PatientId == patientId && x.Status == RadiologyRequestStatus.Reported);
        var dtos = new List<RadiologyRequestDto>();

        foreach (var item in items)
        {
            dtos.Add(await MapToDtoAsync(item));
        }

        return dtos;
    }

    [HttpGet("result-pdf/{id}")]
    public async Task<IRemoteStreamContent> GetRadiologyResultPdfAsync(Guid id)
    {
        var entity = await Repository.GetAsync(id);
        var patient = await _patientRepository.GetAsync(entity.PatientId);
        var radItem = await _radiologyItemRepository.GetAsync(entity.RadiologyItemId);
        
        string doctorName = "-";
        if (entity.DoctorId.HasValue)
        {
            var doctor = await _doctorRepository.FindAsync(entity.DoctorId.Value);
            doctorName = doctor?.NameAr ?? doctor?.NameEn ?? "-";
        }

        // Logo logic
        byte[] logoBytes = null;
        var logoPath = System.IO.Path.Combine(_env.WebRootPath ?? "", "images", "logo", "Dark.png");
        if (!System.IO.File.Exists(logoPath))
        {
            var devPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "images", "logo", "Dark.png");
            if (System.IO.File.Exists(devPath)) logoPath = devPath;
        }
        if (System.IO.File.Exists(logoPath)) logoBytes = await System.IO.File.ReadAllBytesAsync(logoPath);

        // Required for QuestPDF Community version
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        var document = new Printing.RadiologyResultDocument
        {
            PatientName = $"{patient.FirstNameAr} {patient.LastNameAr}",
            PatientId = patient.Id.ToString().Substring(0, 8).ToUpper(),
            RequestDate = entity.RequestDate,
            ReportDate = entity.ReportDate,
            CustomRequestNumber = entity.RequestNumber,
            RadiologyItemName = radItem.Name,
            DoctorName = doctorName,
            RadiologistName = "قسم الأشعة", // Fallback
            ReportBody = entity.ReportBody,
            TechnicianNotes = entity.TechnicianNotes,
            LogoBytes = logoBytes
        };

        var pdfBytes = QuestPDF.Fluent.GenerateExtensions.GeneratePdf(document);
        var stream = new System.IO.MemoryStream(pdfBytes);
        var printTime = DateTime.Now;
        return new RemoteStreamContent(stream, $"تقرير_أشعة_{entity.RequestNumber}_{printTime:yyyyMMdd}.pdf", "application/pdf");
    }
}
