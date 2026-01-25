namespace HIS.ActivityLogs;

/// <summary>
/// Represents the type of action performed in an activity log.
/// </summary>
public enum ActivityAction
{
    /// <summary>
    /// User logged into the system.
    /// </summary>
    Login = 0,

    /// <summary>
    /// User logged out of the system.
    /// </summary>
    Logout = 1,

    /// <summary>
    /// A new entity was created.
    /// </summary>
    Create = 2,

    /// <summary>
    /// An existing entity was updated.
    /// </summary>
    Update = 3,

    /// <summary>
    /// An entity was deleted.
    /// </summary>
    Delete = 4,

    /// <summary>
    /// Data was viewed/read.
    /// </summary>
    View = 5,

    /// <summary>
    /// Data was exported.
    /// </summary>
    Export = 6,

    /// <summary>
    /// Data was imported.
    /// </summary>
    Import = 7,

    /// <summary>
    /// User attempted to access without permission.
    /// </summary>
    AccessDenied = 8,

    /// <summary>
    /// Failed login attempt.
    /// </summary>
    FailedLogin = 9
}
