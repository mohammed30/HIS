using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Notifications;

public interface INotificationAppService : IApplicationService
{
    // ── User endpoints ──────────────────────────────────────────────────────

    /// <summary>Get current user's notifications with optional filtering.</summary>
    Task<PagedResultDto<NotificationDto>> GetMyNotificationsAsync(GetNotificationsInput input);

    /// <summary>Mark a single notification as read.</summary>
    Task MarkAsReadAsync(Guid id);

    /// <summary>Mark all of current user's notifications as read.</summary>
    Task MarkAllAsReadAsync();

    /// <summary>Delete a single notification.</summary>
    Task DeleteAsync(Guid id);

    /// <summary>Get the count of unread notifications for current user.</summary>
    Task<int> GetUnreadCountAsync();

    /// <summary>Get current user's notification settings/preferences.</summary>
    Task<UserNotificationSettingsDto> GetMySettingsAsync();

    /// <summary>Get the count of unread notifications for current user.</summary>
    Task<List<ModuleSubscriptionDto>> GetModuleSubscriptionsAsync();
    Task UpdateModuleSubscriptionsAsync(List<UpdateModuleSubscriptionDto> input);
    Task<List<HIS.Appointments.Dtos.LookupDto<Guid>>> GetUserLookupAsync();

    /// <summary>Save current user's notification type preferences.</summary>
    Task UpdateMySettingsAsync(UpdateNotificationSettingsDto input);

    // ── Admin endpoints ──────────────────────────────────────────────────────

    /// <summary>Admin: Send a notification to a specific user.</summary>
    Task SendToUserAsync(Guid userId, CreateNotificationDto input);

    /// <summary>Admin: Send a notification to ALL users.</summary>
    Task SendToAllAsync(CreateNotificationDto input);

    /// <summary>Admin: Enable or disable notifications for a specific user.</summary>
    Task SetUserSilenceAsync(Guid userId, SetUserSilenceDto input);

    /// <summary>Admin: Get notification status summary for all users.</summary>
    Task<List<UserNotificationSummaryDto>> GetUsersNotificationStatusAsync();
}
