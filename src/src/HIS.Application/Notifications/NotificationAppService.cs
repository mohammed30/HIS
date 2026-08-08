using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace HIS.Notifications;

[Authorize]
public class NotificationAppService : ApplicationService, INotificationAppService
{
    private readonly IRepository<Notification, Guid> _notificationRepo;
    private readonly IRepository<UserNotificationSettings, Guid> _settingsRepo;
    private readonly NotificationSender _notificationSender;
    private readonly IRepository<IdentityUser, Guid> _identityUserRepository;

    public NotificationAppService(
        IRepository<Notification, Guid> notificationRepo,
        IRepository<UserNotificationSettings, Guid> settingsRepo,
        NotificationSender notificationSender,
        IRepository<IdentityUser, Guid> identityUserRepository)
    {
        _notificationRepo = notificationRepo;
        _settingsRepo = settingsRepo;
        _notificationSender = notificationSender;
        _identityUserRepository = identityUserRepository;
    }

    // ── User endpoints ──────────────────────────────────────────────────────

    public async Task<PagedResultDto<NotificationDto>> GetMyNotificationsAsync(GetNotificationsInput input)
    {
        var userId = CurrentUser.GetId();

        var queryable = await _notificationRepo.GetQueryableAsync();
        queryable = queryable.Where(n => n.UserId == userId);

        if (input.IsRead.HasValue)
            queryable = queryable.Where(n => n.IsRead == input.IsRead.Value);

        if (!string.IsNullOrWhiteSpace(input.Type))
            queryable = queryable.Where(n => n.Type == input.Type);

        var total = await AsyncExecuter.CountAsync(queryable);

        queryable = queryable
            .OrderByDescending(n => n.CreatedAt)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(queryable);
        var dtos = ObjectMapper.Map<List<Notification>, List<NotificationDto>>(items);

        return new PagedResultDto<NotificationDto>(total, dtos);
    }

    public async Task MarkAsReadAsync(Guid id)
    {
        var userId = CurrentUser.GetId();
        var notification = await _notificationRepo.GetAsync(id);

        if (notification.UserId != userId)
            throw new UserFriendlyException("Access denied.");

        notification.IsRead = true;
        await _notificationRepo.UpdateAsync(notification);
        await _notificationSender.SendReadEventAsync(userId, id);
    }

    public async Task MarkAllAsReadAsync()
    {
        var userId = CurrentUser.GetId();
        var queryable = await _notificationRepo.GetQueryableAsync();
        var unread = await AsyncExecuter.ToListAsync(
            queryable.Where(n => n.UserId == userId && !n.IsRead));

        foreach (var n in unread)
            n.IsRead = true;

        await _notificationRepo.UpdateManyAsync(unread);
        await _notificationSender.SendAllReadEventAsync(userId);
    }

    public async Task DeleteAsync(Guid id)
    {
        var userId = CurrentUser.GetId();
        var notification = await _notificationRepo.GetAsync(id);

        if (notification.UserId != userId)
            throw new UserFriendlyException("Access denied.");

        await _notificationRepo.DeleteAsync(id);
    }

    public async Task<int> GetUnreadCountAsync()
    {
        var userId = CurrentUser.GetId();
        var queryable = await _notificationRepo.GetQueryableAsync();
        return await AsyncExecuter.CountAsync(
            queryable.Where(n => n.UserId == userId && !n.IsRead));
    }

    public async Task<UserNotificationSettingsDto> GetMySettingsAsync()
    {
        var userId = CurrentUser.GetId();
        var settings = await GetOrCreateSettingsAsync(userId);
        return MapSettingsToDto(settings);
    }

    public async Task UpdateMySettingsAsync(UpdateNotificationSettingsDto input)
    {
        var userId = CurrentUser.GetId();
        var settings = await GetOrCreateSettingsAsync(userId);
        settings.SetEnabledTypes(input.EnabledTypes);
        await _settingsRepo.UpdateAsync(settings);
    }

    // ── Admin endpoints ──────────────────────────────────────────────────────

    [Authorize(HISPermissions.Settings.Default)]
    public async Task SendToUserAsync(Guid userId, CreateNotificationDto input)
    {
        var notification = new Notification(
            GuidGenerator.Create(),
            userId,
            input.Title,
            input.Message,
            input.Type,
            input.Url,
            input.EntityId,
            sentBy: CurrentUser.UserName);

        await _notificationRepo.InsertAsync(notification);
        var dto = ObjectMapper.Map<Notification, NotificationDto>(notification);
        await _notificationSender.SendToUserAsync(userId, dto);
    }

    [Authorize(HISPermissions.Settings.Default)]
    public async Task SendToAllAsync(CreateNotificationDto input)
    {
        var users = await _identityUserRepository.GetListAsync();

        var notifications = users.Select(u => new Notification(
            GuidGenerator.Create(),
            u.Id,
            input.Title,
            input.Message,
            input.Type,
            input.Url,
            input.EntityId,
            sentBy: CurrentUser.UserName
        )).ToList();

        await _notificationRepo.InsertManyAsync(notifications);

        // Send one broadcast event to all connected clients
        var demoDto = ObjectMapper.Map<Notification, NotificationDto>(notifications.First());
        await _notificationSender.SendToAllAsync(demoDto);
    }

    [Authorize(HISPermissions.Settings.Default)]
    public async Task SetUserSilenceAsync(Guid userId, SetUserSilenceDto input)
    {
        var settings = await GetOrCreateSettingsAsync(userId);
        settings.GlobalSilence = input.IsSilenced;
        settings.SilencedUntil = input.IsSilenced ? input.SilencedUntil : null;
        await _settingsRepo.UpdateAsync(settings);
    }

    [Authorize(HISPermissions.Settings.Default)]
    public async Task<List<UserNotificationSummaryDto>> GetUsersNotificationStatusAsync()
    {
        var users = await _identityUserRepository.GetListAsync();
        var notifQueryable = await _notificationRepo.GetQueryableAsync();
        var settingsQueryable = await _settingsRepo.GetQueryableAsync();
        var settingsList = await AsyncExecuter.ToListAsync(settingsQueryable);

        var result = new List<UserNotificationSummaryDto>();

        foreach (var user in users)
        {
            var userSettings = settingsList.FirstOrDefault(s => s.UserId == user.Id);
            var totalCount = await AsyncExecuter.CountAsync(
                notifQueryable.Where(n => n.UserId == user.Id));
            var unreadCount = await AsyncExecuter.CountAsync(
                notifQueryable.Where(n => n.UserId == user.Id && !n.IsRead));

            result.Add(new UserNotificationSummaryDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                IsEnabled = userSettings?.IsEnabled ?? true,
                GlobalSilence = userSettings?.GlobalSilence ?? false,
                SilencedUntil = userSettings?.SilencedUntil,
                UnreadCount = unreadCount,
                TotalCount = totalCount
            });
        }

        return result;
    }

    [Authorize(HISPermissions.Settings.Default)]
    public async Task<List<ModuleSubscriptionDto>> GetModuleSubscriptionsAsync()
    {
        var modules = new List<string> { "Appointments", "Radiology", "Pharmacy", "Emergency", "Operations", "Billing", "Inventory", "Laboratory", "Inpatient", "Accounting", "HR", "Reception", "Payments", "Nursing", "Insurance", "Patients" };
        var result = new List<ModuleSubscriptionDto>();
        
        foreach (var module in modules)
        {
            var settingValue = await SettingProvider.GetOrNullAsync($"Notifications.Subscribers.{module}");
            var userIds = string.IsNullOrWhiteSpace(settingValue) ? new List<Guid>() : settingValue.Split(',').Select(Guid.Parse).ToList();
            
            result.Add(new ModuleSubscriptionDto
            {
                ModuleName = module,
                DisplayName = GetModuleDisplayName(module),
                SubscribedUserIds = userIds
            });
        }
        return result;
    }

    [Authorize(HISPermissions.Settings.Default)]
    public async Task UpdateModuleSubscriptionsAsync(List<UpdateModuleSubscriptionDto> input)
    {
        var settingManager = LazyServiceProvider.LazyGetRequiredService<Volo.Abp.SettingManagement.ISettingManager>();
        
        foreach (var item in input)
        {
            var value = string.Join(",", item.SubscribedUserIds);
            if (CurrentTenant.Id.HasValue)
            {
                await settingManager.SetAsync($"Notifications.Subscribers.{item.ModuleName}", value, "T", CurrentTenant.Id.Value.ToString());
            }
            else
            {
                await settingManager.SetAsync($"Notifications.Subscribers.{item.ModuleName}", value, "G", null);
            }
        }
    }

    [Authorize(HISPermissions.Settings.Default)]
    public async Task<List<HIS.Appointments.Dtos.LookupDto<Guid>>> GetUserLookupAsync()
    {
        var users = await _identityUserRepository.GetListAsync();
        return users.Select(u => new HIS.Appointments.Dtos.LookupDto<Guid>
        {
            Id = u.Id,
            Name = string.IsNullOrWhiteSpace(u.Name) ? u.UserName : $"{u.Name} {u.Surname} ({u.UserName})"
        }).ToList();
    }

    private string GetModuleDisplayName(string module)
    {
        return module switch
        {
            "Appointments" => "المواعيد",
            "Radiology" => "الأشعة",
            "Pharmacy" => "الصيدلية",
            "Emergency" => "الطوارئ",
            "Operations" => "العمليات",
            "Billing" => "الفواتير",
            "Inventory" => "المخزون",
            "Laboratory" => "المختبر",
            "Inpatient" => "التنويم",
            "Accounting" => "الحسابات",
            "HR" => "الموارد البشرية",
            "Reception" => "الاستقبال",
            "Payments" => "المدفوعات",
            "Nursing" => "التمريض",
            "Insurance" => "التأمين",
            "Patients" => "المرضى",
            _ => module
        };
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task<UserNotificationSettings> GetOrCreateSettingsAsync(Guid userId)
    {
        var queryable = await _settingsRepo.GetQueryableAsync();
        var settings = await AsyncExecuter.FirstOrDefaultAsync(
            queryable.Where(s => s.UserId == userId));

        if (settings == null)
        {
            settings = new UserNotificationSettings(GuidGenerator.Create(), userId);
            await _settingsRepo.InsertAsync(settings);
        }

        return settings;
    }

    private static UserNotificationSettingsDto MapSettingsToDto(UserNotificationSettings s) =>
        new()
        {
            IsEnabled = s.IsEnabled,
            GlobalSilence = s.GlobalSilence,
            SilencedUntil = s.SilencedUntil,
            EnabledTypes = s.GetEnabledTypesList()
        };
}
