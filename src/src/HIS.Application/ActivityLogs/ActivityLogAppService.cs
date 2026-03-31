using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using HIS.Permissions;

namespace HIS.ActivityLogs;

/// <summary>
/// Application service implementation for activity log operations.
/// </summary>
[Authorize(HISPermissions.ActivityLogs.Default)]
public class ActivityLogAppService : ApplicationService, IActivityLogAppService
{
    private readonly IRepository<ActivityLog, Guid> _repository;

    public ActivityLogAppService(IRepository<ActivityLog, Guid> repository)
    {
        _repository = repository;
    }

    /// <inheritdoc/>
    public async Task<PagedResultDto<ActivityLogDto>> GetListAsync(GetActivityLogsInput input)
    {
        var queryable = await _repository.GetQueryableAsync();

        // Apply filters
        queryable = ApplyFilters(queryable, input);

        // Get total count
        var totalCount = await AsyncExecuter.CountAsync(queryable);

        // Apply sorting
        if (!string.IsNullOrEmpty(input.Sorting))
        {
            queryable = ApplySorting(queryable, input.Sorting);
        }
        else
        {
            queryable = queryable.OrderByDescending(x => x.Timestamp);
        }

        // Apply paging
        queryable = queryable.Skip(input.SkipCount).Take(input.MaxResultCount);

        var logs = await AsyncExecuter.ToListAsync(queryable);

        var dtos = ObjectMapper.Map<List<ActivityLog>, List<ActivityLogDto>>(logs);

        return new PagedResultDto<ActivityLogDto>(totalCount, dtos);
    }

    /// <inheritdoc/>
    public async Task<ActivityLogDto> GetAsync(Guid id)
    {
        var log = await _repository.GetAsync(id);
        return ObjectMapper.Map<ActivityLog, ActivityLogDto>(log);
    }

    /// <inheritdoc/>
    public async Task<PagedResultDto<ActivityLogDto>> GetByUserAsync(Guid userId, int skipCount = 0, int maxResultCount = 20)
    {
        var input = new GetActivityLogsInput
        {
            UserId = userId,
            SkipCount = skipCount,
            MaxResultCount = maxResultCount
        };
        return await GetListAsync(input);
    }

    /// <inheritdoc/>
    public async Task<PagedResultDto<ActivityLogDto>> GetByEntityAsync(string entityType, string entityId, int skipCount = 0, int maxResultCount = 20)
    {
        var input = new GetActivityLogsInput
        {
            EntityType = entityType,
            EntityId = entityId,
            SkipCount = skipCount,
            MaxResultCount = maxResultCount
        };
        return await GetListAsync(input);
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetModulesAsync()
    {
        var queryable = await _repository.GetQueryableAsync();
        var modules = await AsyncExecuter.ToListAsync(
            queryable.Select(x => x.Module).Distinct()
        );
        return modules;
    }

    /// <inheritdoc/>
    public async Task<byte[]> ExportToCsvAsync(GetActivityLogsInput input)
    {
        // Get all matching logs (no paging for export)
        input.SkipCount = 0;
        input.MaxResultCount = int.MaxValue;

        var queryable = await _repository.GetQueryableAsync();
        queryable = ApplyFilters(queryable, input);
        queryable = queryable.OrderByDescending(x => x.Timestamp);

        var logs = await AsyncExecuter.ToListAsync(queryable);

        // Build CSV
        var csv = new StringBuilder();
        csv.AppendLine("Id,Timestamp,UserName,Module,Action,Level,EntityType,EntityId,Description,IpAddress");

        foreach (var log in logs)
        {
            csv.AppendLine($"\"{log.Id}\",\"{log.Timestamp:yyyy-MM-dd HH:mm:ss}\",\"{log.UserName}\",\"{log.Module}\",\"{log.Action}\",\"{log.Level}\",\"{log.EntityType}\",\"{log.EntityId}\",\"{EscapeCsvField(log.Description)}\",\"{log.IpAddress}\"");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private IQueryable<ActivityLog> ApplyFilters(IQueryable<ActivityLog> queryable, GetActivityLogsInput input)
    {
        if (input.UserId.HasValue)
        {
            queryable = queryable.Where(x => x.UserId == input.UserId);
        }

        if (!string.IsNullOrEmpty(input.Module))
        {
            queryable = queryable.Where(x => x.Module == input.Module);
        }

        if (input.ActivityActionFilter.HasValue)
        {
            queryable = queryable.Where(x => x.Action == input.ActivityActionFilter);
        }

        if (input.Level.HasValue)
        {
            queryable = queryable.Where(x => x.Level == input.Level);
        }

        if (!string.IsNullOrEmpty(input.EntityType))
        {
            queryable = queryable.Where(x => x.EntityType == input.EntityType);
        }

        if (!string.IsNullOrEmpty(input.EntityId))
        {
            queryable = queryable.Where(x => x.EntityId == input.EntityId);
        }

        if (input.StartDate.HasValue)
        {
            queryable = queryable.Where(x => x.Timestamp >= input.StartDate.Value);
        }

        if (input.EndDate.HasValue)
        {
            queryable = queryable.Where(x => x.Timestamp <= input.EndDate.Value);
        }

        if (!string.IsNullOrEmpty(input.SearchText))
        {
            queryable = queryable.Where(x => 
                x.Description != null && x.Description.Contains(input.SearchText) ||
                x.UserName != null && x.UserName.Contains(input.SearchText));
        }

        return queryable;
    }

    private static IQueryable<ActivityLog> ApplySorting(IQueryable<ActivityLog> queryable, string sorting)
    {
        return sorting.ToLower() switch
        {
            "timestamp" => queryable.OrderBy(x => x.Timestamp),
            "timestamp desc" => queryable.OrderByDescending(x => x.Timestamp),
            "username" => queryable.OrderBy(x => x.UserName),
            "username desc" => queryable.OrderByDescending(x => x.UserName),
            "module" => queryable.OrderBy(x => x.Module),
            "module desc" => queryable.OrderByDescending(x => x.Module),
            "action" => queryable.OrderBy(x => x.Action),
            "action desc" => queryable.OrderByDescending(x => x.Action),
            _ => queryable.OrderByDescending(x => x.Timestamp)
        };
    }

    private static string EscapeCsvField(string? field)
    {
        if (string.IsNullOrEmpty(field)) return string.Empty;
        return field.Replace("\"", "\"\"").Replace("\n", " ").Replace("\r", "");
    }
}
