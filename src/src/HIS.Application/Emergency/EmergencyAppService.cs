using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using HIS.Emergency.Dtos;
using HIS.Patients;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Emergency;

public class EmergencyAppService : ApplicationService, IEmergencyAppService
{
    private readonly IRepository<EmergencyVisit, Guid> _visitRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;

    public EmergencyAppService(
        IRepository<EmergencyVisit, Guid> visitRepository,
        IRepository<Patient, Guid> patientRepository)
    {
        _visitRepository = visitRepository;
        _patientRepository = patientRepository;
    }

    public async Task<PagedResultDto<EmergencyVisitDto>> GetActiveVisitsAsync(PagedAndSortedResultRequestDto input)
    {
        // Active = Triaged (0) or TreatmentInProgress (1)
        var query = await _visitRepository.GetQueryableAsync();
        query = query.Where(x => x.Status == EmergencyVisitStatus.Triaged || x.Status == EmergencyVisitStatus.TreatmentInProgress);
        
        var totalCount = await AsyncExecuter.CountAsync(query);
        
        // Default sort by Severity (Ascending = 1 is highest priority), then ArrivalTime
        if (string.IsNullOrEmpty(input.Sorting))
        {
            query = query.OrderBy(x => x.Severity).ThenBy(x => x.ArrivalTime);
        }
        else
        {
            query = query.OrderBy(input.Sorting).PageBy(input);
        }
        
        var items = await AsyncExecuter.ToListAsync(query);
        
        // Map Info
        var patientIds = items.Select(x => x.PatientId).Distinct().ToList();
        var patients = await _patientRepository.GetListAsync(x => patientIds.Contains(x.Id));

        var dtos = items.Select(v =>
        {
            var dto = ObjectMapper.Map<EmergencyVisit, EmergencyVisitDto>(v);
            var p = patients.FirstOrDefault(x => x.Id == v.PatientId);
            dto.PatientName = p != null ? $"{p.FirstNameAr} {p.LastNameAr}" : "Unknown";
            return dto;
        }).ToList();

        return new PagedResultDto<EmergencyVisitDto>(totalCount, dtos);
    }

    public async Task<EmergencyVisitDto> RegisterAsync(CreateEmergencyVisitDto input)
    {
        var visit = new EmergencyVisit(
            GuidGenerator.Create(),
            input.PatientId,
            EmergencySeverity.NonUrgent, // Default until triaged
            input.ChiefComplaint
        );

        await _visitRepository.InsertAsync(visit);

        try
        {
            var notificationRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Notifications.Notification, Guid>>();
            var notificationSender = LazyServiceProvider.LazyGetRequiredService<HIS.Notifications.NotificationSender>();
            var settingProvider = LazyServiceProvider.LazyGetRequiredService<Volo.Abp.Settings.ISettingProvider>();

            var settingValue = await settingProvider.GetOrNullAsync("Notifications.Subscribers.Emergency");
            var userIds = string.IsNullOrWhiteSpace(settingValue) ? new List<Guid>() : settingValue.Split(',').Select(Guid.Parse).ToList();

            if (userIds.Any())
            {
                var notifications = userIds.Select(id => new HIS.Notifications.Notification(
                    GuidGenerator.Create(), 
                    id, 
                    "حالة طوارئ جديدة", 
                    $"تم تسجيل حالة طوارئ جديدة", 
                    "Emergency", 
                    "/emergency/visits", 
                    visit.Id.ToString(), 
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
            Microsoft.Extensions.Logging.LoggerExtensions.LogError(LazyServiceProvider.LazyGetRequiredService<Microsoft.Extensions.Logging.ILogger<EmergencyAppService>>(), ex, "Failed to send notification");
        }

        return ObjectMapper.Map<EmergencyVisit, EmergencyVisitDto>(visit);
    }

    public async Task<EmergencyVisitDto> PerformTriageAsync(Guid id, TriageDto input)
    {
        var visit = await _visitRepository.GetAsync(id);
        
        visit.Severity = input.Severity;
        visit.BloodPressure = input.BloodPressure;
        visit.HeartRate = input.HeartRate;
        visit.Temperature = input.Temperature;
        visit.RespiratoryRate = input.RespiratoryRate;
        visit.OxygenSaturation = input.OxygenSaturation;
        
        if (!string.IsNullOrEmpty(input.Notes))
        {
            visit.Notes = input.Notes;
        }

        // Auto move to TreatmentInProgress if it was Triaged? Or stay Triaged until doctor picks up?
        // Let's keep it Triaged (Active in queue)
        
        await _visitRepository.UpdateAsync(visit);
        return ObjectMapper.Map<EmergencyVisit, EmergencyVisitDto>(visit);
    }

    public async Task<EmergencyVisitDto> UpdateStatusAsync(Guid id, UpdateStatusDto input)
    {
        var visit = await _visitRepository.GetAsync(id);
        
        visit.Status = input.Status;
        if (!string.IsNullOrEmpty(input.Notes))
        {
            visit.Notes = input.Notes;
        }

        await _visitRepository.UpdateAsync(visit);
        return ObjectMapper.Map<EmergencyVisit, EmergencyVisitDto>(visit);
    }
}
