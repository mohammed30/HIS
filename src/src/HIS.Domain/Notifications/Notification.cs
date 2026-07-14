using System;
using Volo.Abp.Domain.Entities;

namespace HIS.Notifications;

/// <summary>
/// Represents a notification sent to a specific user.
/// </summary>
public class Notification : Entity<Guid>
{
    /// <summary>The target user's ID.</summary>
    public Guid UserId { get; set; }

    /// <summary>Short title of the notification.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Full notification message.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Notification category type.
    /// Values: appointment, lab, pharmacy, radiology, inventory, billing, emergency, system
    /// </summary>
    public string Type { get; set; } = NotificationTypes.System;

    /// <summary>Optional URL to navigate to when clicking the notification.</summary>
    public string? Url { get; set; }

    /// <summary>Optional reference to a related entity ID.</summary>
    public string? EntityId { get; set; }

    /// <summary>Whether the user has read this notification.</summary>
    public bool IsRead { get; set; } = false;

    /// <summary>When the notification was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Who sent this notification.
    /// Null = automatic system notification.
    /// A username = manually sent by an admin.
    /// </summary>
    public string? SentBy { get; set; }

    protected Notification() { }

    public Notification(
        Guid id,
        Guid userId,
        string title,
        string message,
        string type = NotificationTypes.System,
        string? url = null,
        string? entityId = null,
        string? sentBy = null)
    {
        Id = id;
        UserId = userId;
        Title = title;
        Message = message;
        Type = type;
        Url = url;
        EntityId = entityId;
        SentBy = sentBy;
        CreatedAt = DateTime.UtcNow;
        IsRead = false;
    }
}
