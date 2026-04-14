using System;

namespace HIS.Radiology;

public enum RadiologyRequestStatus
{
    Requested = 0,
    UnderProcedure = 1,
    Reported = 2,
    Cancelled = 3
}
