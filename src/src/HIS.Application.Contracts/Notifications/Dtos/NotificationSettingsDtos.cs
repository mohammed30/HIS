using System;
using System.Collections.Generic;

namespace HIS.Notifications;

public class UserNotificationSettingsDto
{
    public bool IsEnabled { get; set; }
    public bool GlobalSilence { get; set; }
    public DateTime? SilencedUntil { get; set; }
    public List<string> EnabledTypes { get; set; } = [];
}

public class UpdateNotificationSettingsDto
{
    /// <summary>Types the user wants to receive. Empty = all types.</summary>
    public List<string> EnabledTypes { get; set; } = [];
}

public class SetUserSilenceDto
{
    public bool IsSilenced { get; set; }
    public DateTime? SilencedUntil { get; set; }
}

public class UserNotificationSummaryDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsEnabled { get; set; }
    public bool GlobalSilence { get; set; }
    public DateTime? SilencedUntil { get; set; }
    public int UnreadCount { get; set; }
    public int TotalCount { get; set; }
}

public class GetNotificationsInput : Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto
{
    public bool? IsRead { get; set; }
    public string? Type { get; set; }
}

public class ModuleSubscriptionDto
{
    public string ModuleName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<Guid> SubscribedUserIds { get; set; } = [];
}

public class UpdateModuleSubscriptionDto
{
    public string ModuleName { get; set; } = string.Empty;
    public List<Guid> SubscribedUserIds { get; set; } = [];
}
