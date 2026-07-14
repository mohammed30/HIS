using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Volo.Abp.Users;

namespace HIS.Notifications;

/// <summary>
/// SignalR Hub for real-time notification delivery.
/// Each connected user joins a group identified by their UserId,
/// so notifications can be pushed to a specific user from anywhere in the app.
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    private readonly ICurrentUser _currentUser;

    public NotificationHub(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public override async Task OnConnectedAsync()
    {
        // Add this connection to a group named after the current user's ID
        // This allows sending notifications to a specific user even if they
        // have multiple tabs/connections open.
        if (_currentUser.Id.HasValue)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                _currentUser.Id.Value.ToString());
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_currentUser.Id.HasValue)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                _currentUser.Id.Value.ToString());
        }

        await base.OnDisconnectedAsync(exception);
    }
}

