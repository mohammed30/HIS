using System.ComponentModel.DataAnnotations;

namespace HIS.Notifications;

public class CreateNotificationDto
{
    [Required]
    [MaxLength(256)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1024)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(64)]
    public string Type { get; set; } = "system";

    [MaxLength(512)]
    public string? Url { get; set; }

    [MaxLength(128)]
    public string? EntityId { get; set; }
}
