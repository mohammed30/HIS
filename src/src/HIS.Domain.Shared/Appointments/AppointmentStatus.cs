namespace HIS.Appointments;

public enum AppointmentStatus
{
    Scheduled = 0,
    Confirmed = 1,
    Cancelled = 2,
    Completed = 3,
    NoShow = 4,
    /// <summary>
    /// وصل المريض
    /// </summary>
    CheckedIn = 5,
    /// <summary>
    /// داخل العيادة
    /// </summary>
    InConsultation = 6
}
