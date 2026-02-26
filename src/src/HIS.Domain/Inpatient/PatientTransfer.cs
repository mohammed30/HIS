using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Inpatient;

/// <summary>
/// سجل نقل المريض بين الغرف
/// </summary>
public class PatientTransfer : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; protected set; }

    public Guid AdmissionId { get; protected set; }
    
    public Guid FromRoomId { get; protected set; }
    public Guid? FromBedId { get; protected set; }

    public Guid ToRoomId { get; protected set; }
    public Guid? ToBedId { get; protected set; }

    public DateTime TransferDate { get; set; }
    public string? Reason { get; set; }
    
    public int DaysInPreviousRoom { get; set; }
    public decimal PreviousRoomDailyRate { get; set; }
    public decimal PreviousRoomTotalAmount { get; set; }

    protected PatientTransfer()
    {
    }

    public PatientTransfer(
        Guid id, 
        Guid? tenantId, 
        Guid admissionId, 
        Guid fromRoomId, 
        Guid? fromBedId, 
        Guid toRoomId, 
        Guid? toBedId, 
        DateTime transferDate,
        int daysInPreviousRoom,
        decimal previousRoomDailyRate,
        decimal previousRoomTotalAmount) : base(id)
    {
        TenantId = tenantId;
        AdmissionId = admissionId;
        FromRoomId = fromRoomId;
        FromBedId = fromBedId;
        ToRoomId = toRoomId;
        ToBedId = toBedId;
        TransferDate = transferDate;
        DaysInPreviousRoom = daysInPreviousRoom;
        PreviousRoomDailyRate = previousRoomDailyRate;
        PreviousRoomTotalAmount = previousRoomTotalAmount;
    }
}
