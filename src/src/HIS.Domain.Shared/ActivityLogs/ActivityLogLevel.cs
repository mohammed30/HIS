namespace HIS.ActivityLogs;

/// <summary>
/// Represents the severity level of an activity log entry.
/// </summary>
public enum ActivityLogLevel
{
    /// <summary>
    /// Informational messages that record normal operations.
    /// </summary>
    Info = 0,

    /// <summary>
    /// Warning messages that indicate potential issues.
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Critical messages that indicate security or compliance concerns.
    /// </summary>
    Critical = 2
}
