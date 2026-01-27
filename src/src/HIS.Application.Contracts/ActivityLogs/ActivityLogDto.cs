using System;
using Volo.Abp.Application.Dtos;

namespace HIS.ActivityLogs;

/// <summary>
/// DTO for representing an activity log entry.
/// </summary>
public class ActivityLogDto : FullAuditedEntityDto<Guid>
{
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string Module { get; set; } = string.Empty;
    public ActivityAction Action { get; set; }
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public string? Description { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
    public ActivityLogLevel Level { get; set; }
    public string? AdditionalData { get; set; }
    
    // Device & Location Info
    public string? DeviceType { get; set; }
    public string? BrowserName { get; set; }
    public string? BrowserVersion { get; set; }
    public string? OperatingSystem { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
}
