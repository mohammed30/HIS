using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.DependencyInjection;

namespace HIS.Notifications;

/// <summary>
/// Helper service to push notifications to connected clients via SignalR.
/// Inject this anywhere in the Application layer to send real-time notifications.
/// </summary>
public class NotificationSender : ITransientDependency
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationSender(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    /// <summary>Push a notification to a specific user (all their active connections).</summary>
    public async Task SendToUserAsync(Guid userId, NotificationDto notification)
    {
        await _hubContext.Clients
            .Group(userId.ToString())
            .SendAsync("ReceiveNotification", notification);
    }

    /// <summary>Push a notification to ALL connected clients.</summary>
    public async Task SendToAllAsync(NotificationDto notification)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
    }

    /// <summary>Push a "notification read" event to update badge count on all user tabs.</summary>
    public async Task SendReadEventAsync(Guid userId, Guid notificationId)
    {
        await _hubContext.Clients
            .Group(userId.ToString())
            .SendAsync("NotificationRead", notificationId);
    }

    /// <summary>Notify user that all their notifications have been marked as read.</summary>
    public async Task SendAllReadEventAsync(Guid userId)
    {
        await _hubContext.Clients
            .Group(userId.ToString())
            .SendAsync("AllNotificationsRead");
    }
}
