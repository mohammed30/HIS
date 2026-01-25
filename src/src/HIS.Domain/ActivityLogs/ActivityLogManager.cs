using System;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;

namespace HIS.ActivityLogs;

/// <summary>
/// Domain service for managing user activity logs.
/// </summary>
public class ActivityLogManager : ITransientDependency
{
    private readonly IRepository<ActivityLog, Guid> _repository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentUser _currentUser;
    private readonly ICurrentTenant _currentTenant;

    public ActivityLogManager(
        IRepository<ActivityLog, Guid> repository,
        IGuidGenerator guidGenerator,
        ICurrentUser currentUser,
        ICurrentTenant currentTenant)
    {
        _repository = repository;
        _guidGenerator = guidGenerator;
        _currentUser = currentUser;
        _currentTenant = currentTenant;
    }

    /// <summary>
    /// Logs a user activity.
    /// </summary>
    public async Task<ActivityLog> LogActivityAsync(
        string module,
        ActivityAction action,
        string? description = null,
        string? entityType = null,
        string? entityId = null,
        object? oldValues = null,
        object? newValues = null,
        ActivityLogLevel level = ActivityLogLevel.Info,
        string? ipAddress = null,
        string? userAgent = null)
    {
        var activityLog = new ActivityLog(
            id: _guidGenerator.Create(),
            tenantId: _currentTenant.Id,
            userId: _currentUser.Id,
            userName: _currentUser.UserName,
            module: module,
            action: action,
            level: level,
            entityType: entityType,
            entityId: entityId,
            description: description,
            oldValues: oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            newValues: newValues != null ? JsonSerializer.Serialize(newValues) : null,
            ipAddress: ipAddress,
            userAgent: userAgent
        );

        await _repository.InsertAsync(activityLog);
        return activityLog;
    }

    /// <summary>
    /// Logs a login activity.
    /// </summary>
    public async Task LogLoginAsync(
        Guid userId, 
        string userName, 
        bool success = true, 
        string? ipAddress = null, 
        string? userAgent = null)
    {
        var activityLog = new ActivityLog(
            id: _guidGenerator.Create(),
            tenantId: _currentTenant.Id,
            userId: userId,
            userName: userName,
            module: "Authentication",
            action: success ? ActivityAction.Login : ActivityAction.FailedLogin,
            level: success ? ActivityLogLevel.Info : ActivityLogLevel.Warning,
            description: success ? "User logged in successfully" : "Failed login attempt",
            ipAddress: ipAddress,
            userAgent: userAgent
        );

        await _repository.InsertAsync(activityLog);
    }

    /// <summary>
    /// Logs a logout activity.
    /// </summary>
    public async Task LogLogoutAsync(string? ipAddress = null, string? userAgent = null)
    {
        await LogActivityAsync(
            module: "Authentication",
            action: ActivityAction.Logout,
            description: "User logged out",
            ipAddress: ipAddress,
            userAgent: userAgent
        );
    }

    /// <summary>
    /// Logs an access denied activity.
    /// </summary>
    public async Task LogAccessDeniedAsync(
        string module, 
        string resource, 
        string? ipAddress = null, 
        string? userAgent = null)
    {
        await LogActivityAsync(
            module: module,
            action: ActivityAction.AccessDenied,
            description: $"Access denied to {resource}",
            level: ActivityLogLevel.Warning,
            ipAddress: ipAddress,
            userAgent: userAgent
        );
    }
}
