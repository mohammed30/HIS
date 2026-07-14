using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace HIS.Notifications;

/// <summary>
/// Stores per-user notification preferences and admin silence settings.
/// One record per user (upsert pattern).
/// </summary>
public class UserNotificationSettings : Entity<Guid>
{
    /// <summary>The user's ID this settings row belongs to.</summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Admin-controlled flag. When false, no notifications are delivered to this user.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Admin-controlled global silence. When true, overrides user preferences.
    /// </summary>
    public bool GlobalSilence { get; set; } = false;

    /// <summary>
    /// If set, notifications are silenced until this UTC datetime.
    /// </summary>
    public DateTime? SilencedUntil { get; set; }

    /// <summary>
    /// Comma-separated list of enabled notification types chosen by the user.
    /// Example: "appointment,lab,pharmacy"
    /// Empty string means ALL types are enabled.
    /// </summary>
    public string EnabledTypes { get; set; } = string.Empty;

    protected UserNotificationSettings() { }

    public UserNotificationSettings(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
        IsEnabled = true;
        GlobalSilence = false;
        EnabledTypes = string.Empty; // all enabled by default
    }

    public bool IsTypeEnabled(string type)
    {
        if (!IsEnabled) return false;
        if (GlobalSilence) return false;
        if (SilencedUntil.HasValue && DateTime.UtcNow < SilencedUntil.Value) return false;
        if (string.IsNullOrWhiteSpace(EnabledTypes)) return true; // all types enabled
        return EnabledTypes.Contains(type, StringComparison.OrdinalIgnoreCase);
    }

    public List<string> GetEnabledTypesList()
    {
        if (string.IsNullOrWhiteSpace(EnabledTypes))
            return [.. NotificationTypes.All];
        return [.. EnabledTypes.Split(',', StringSplitOptions.RemoveEmptyEntries)];
    }

    public void SetEnabledTypes(IEnumerable<string> types)
    {
        EnabledTypes = string.Join(",", types);
    }
}
