namespace HIS.Rooms;

/// <summary>
/// نوع السرير
/// </summary>
public enum BedType
{
    /// <summary>عادي</summary>
    Standard = 0,
    /// <summary>كهربائي</summary>
    Electric = 1,
    /// <summary>عناية مركزة</summary>
    ICU = 2,
    /// <summary>حاضنة أطفال</summary>
    Incubator = 3,
    /// <summary>سرير ولادة</summary>
    Labor = 4,
    /// <summary>سرير طوارئ</summary>
    Emergency = 5,
    /// <summary>كرسي غسيل كلى</summary>
    DialysisChair = 6
}
