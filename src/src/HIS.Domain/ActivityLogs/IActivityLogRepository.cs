using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HIS.ActivityLogs;

/// <summary>
/// Repository interface for ActivityLog entities.
/// </summary>
public interface IActivityLogRepository : IRepository<ActivityLog, Guid>
{
    /// <summary>
    /// Gets activity logs for a specific user.
    /// </summary>
    Task<List<ActivityLog>> GetByUserIdAsync(
        Guid userId, 
        int skipCount = 0, 
        int maxResultCount = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activity logs for a specific entity.
    /// </summary>
    Task<List<ActivityLog>> GetByEntityAsync(
        string entityType, 
        string entityId,
        int skipCount = 0,
        int maxResultCount = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activity logs within a date range.
    /// </summary>
    Task<List<ActivityLog>> GetByDateRangeAsync(
        DateTime startDate, 
        DateTime endDate,
        int skipCount = 0,
        int maxResultCount = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets activity logs by module.
    /// </summary>
    Task<List<ActivityLog>> GetByModuleAsync(
        string module,
        int skipCount = 0,
        int maxResultCount = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts total activity logs with optional filtering.
    /// </summary>
    Task<long> GetCountAsync(
        Guid? userId = null,
        string? module = null,
        ActivityAction? action = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        CancellationToken cancellationToken = default);
}
