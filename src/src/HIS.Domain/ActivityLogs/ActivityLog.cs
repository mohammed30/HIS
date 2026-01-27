using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.ActivityLogs;

/// <summary>
/// Represents a user activity log entry for tracking and auditing purposes.
/// </summary>
public class ActivityLog : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// The tenant ID for multi-tenancy support.
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// The ID of the user who performed the action.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// The username of the user who performed the action.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// The module where the action was performed (e.g., Patient, Pharmacy, Appointment).
    /// </summary>
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// The type of action performed.
    /// </summary>
    public ActivityAction Action { get; set; }

    /// <summary>
    /// The type of entity affected (e.g., Patient, Drug, Appointment).
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// The ID of the affected entity.
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// Human-readable description of the action.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// JSON representation of the old values (for update/delete operations).
    /// </summary>
    public string? OldValues { get; set; }

    /// <summary>
    /// JSON representation of the new values (for create/update operations).
    /// </summary>
    public string? NewValues { get; set; }

    /// <summary>
    /// The IP address of the user.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// The browser/client user agent.
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// The timestamp when the action occurred.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// The severity level of the log.
    /// </summary>
    public ActivityLogLevel Level { get; set; }

    /// <summary>
    /// Additional metadata as JSON.
    /// </summary>
    public string? AdditionalData { get; set; }

    /// <summary>
    /// Device type: Mobile, Desktop, Tablet
    /// </summary>
    public string? DeviceType { get; set; }

    /// <summary>
    /// Browser name (Chrome, Firefox, Safari, Edge)
    /// </summary>
    public string? BrowserName { get; set; }

    /// <summary>
    /// Browser version
    /// </summary>
    public string? BrowserVersion { get; set; }

    /// <summary>
    /// Operating system (Windows, iOS, Android, macOS, Linux)
    /// </summary>
    public string? OperatingSystem { get; set; }

    /// <summary>
    /// Country from IP geolocation
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// City from IP geolocation
    /// </summary>
    public string? City { get; set; }

    protected ActivityLog()
    {
    }

    public ActivityLog(
        Guid id,
        Guid? tenantId,
        Guid? userId,
        string? userName,
        string module,
        ActivityAction action,
        ActivityLogLevel level = ActivityLogLevel.Info,
        string? entityType = null,
        string? entityId = null,
        string? description = null,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null,
        string? userAgent = null)
        : base(id)
    {
        TenantId = tenantId;
        UserId = userId;
        UserName = userName;
        Module = module;
        Action = action;
        Level = level;
        EntityType = entityType;
        EntityId = entityId;
        Description = description;
        OldValues = oldValues;
        NewValues = newValues;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Timestamp = DateTime.UtcNow;
    }
}
