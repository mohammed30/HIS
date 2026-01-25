using System;
using Volo.Abp.Application.Dtos;

namespace HIS.ActivityLogs;

/// <summary>
/// DTO for querying activity logs with filters.
/// </summary>
public class GetActivityLogsInput : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// Filter by user ID.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Filter by module name.
    /// </summary>
    public string? Module { get; set; }

    /// <summary>
    /// Filter by action type.
    /// </summary>
    public ActivityAction? Action { get; set; }

    /// <summary>
    /// Filter by log level.
    /// </summary>
    public ActivityLogLevel? Level { get; set; }

    /// <summary>
    /// Filter by entity type.
    /// </summary>
    public string? EntityType { get; set; }

    /// <summary>
    /// Filter by entity ID.
    /// </summary>
    public string? EntityId { get; set; }

    /// <summary>
    /// Filter logs from this date.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Filter logs until this date.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Search in description.
    /// </summary>
    public string? SearchText { get; set; }
}
