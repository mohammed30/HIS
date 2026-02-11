namespace HIS.Rooms;

/// <summary>
/// نوع الغرفة
/// </summary>
public enum RoomType
{
    /// <summary>عادي</summary>
    Standard = 0,
    /// <summary>خاص</summary>
    Private = 1,
    /// <summary>عناية مركزة</summary>
    ICU = 2,
    /// <summary>جناح</summary>
    Suite = 3,
    /// <summary>عزل</summary>
    Isolation = 4
}
