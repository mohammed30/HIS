using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Appointments;

public class WaitingList : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; } // Null implies "First Available" or "Any Doctor" in that department/specialty? 
                                        // For now, let's assume specific doctor or null for clinic/department waiting list if we add DepartmentId later.
    public Guid DepartmentId { get; set; } // Useful to wait for "Orthopedics" generally

    public DateTime RequestDate { get; set; }
    public WaitingListPriority Priority { get; set; }
    public string Notes { get; set; }
    public bool IsResolved { get; set; }

    protected WaitingList() { }

    public WaitingList(Guid id, Guid? tenantId, Guid patientId, Guid departmentId, Guid? doctorId, DateTime requestDate, WaitingListPriority priority)
        : base(id)
    {
        TenantId = tenantId;
        PatientId = patientId;
        DepartmentId = departmentId;
        DoctorId = doctorId;
        RequestDate = requestDate;
        Priority = priority;
        IsResolved = false;
    }
}


