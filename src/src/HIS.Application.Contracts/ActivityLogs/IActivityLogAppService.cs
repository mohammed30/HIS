using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.ActivityLogs;

/// <summary>
/// Application service interface for activity log operations.
/// </summary>
public interface IActivityLogAppService : IApplicationService
{
    /// <summary>
    /// Gets a paginated list of activity logs.
    /// </summary>
    Task<PagedResultDto<ActivityLogDto>> GetListAsync(GetActivityLogsInput input);

    /// <summary>
    /// Gets a specific activity log by ID.
    /// </summary>
    Task<ActivityLogDto> GetAsync(Guid id);

    /// <summary>
    /// Gets activity logs for a specific user.
    /// </summary>
    Task<PagedResultDto<ActivityLogDto>> GetByUserAsync(Guid userId, int skipCount = 0, int maxResultCount = 20);

    /// <summary>
    /// Gets activity logs for a specific entity.
    /// </summary>
    Task<PagedResultDto<ActivityLogDto>> GetByEntityAsync(string entityType, string entityId, int skipCount = 0, int maxResultCount = 20);

    /// <summary>
    /// Gets available modules for filtering.
    /// </summary>
    Task<List<string>> GetModulesAsync();

    /// <summary>
    /// Exports activity logs to CSV.
    /// </summary>
    Task<byte[]> ExportToCsvAsync(GetActivityLogsInput input);
}
