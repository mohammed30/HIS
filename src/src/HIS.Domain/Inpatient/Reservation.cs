using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Inpatient;

/// <summary>
/// الحجز - Reservation Entity
/// </summary>
public class Reservation : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public Guid PatientId { get; set; }

    public Guid RoomId { get; set; }

    public Guid? BedId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    public string? Notes { get; set; }

    protected Reservation() { }

    public Reservation(
        Guid id, 
        Guid? tenantId, 
        Guid patientId, 
        Guid roomId, 
        DateTime startDate, 
        DateTime endDate,
        Guid? bedId = null)
        : base(id)
    {
        TenantId = tenantId;
        PatientId = patientId;
        RoomId = roomId;
        BedId = bedId;
        StartDate = startDate;
        EndDate = endDate;
        Status = ReservationStatus.Pending;
    }
}
