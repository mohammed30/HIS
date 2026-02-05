using System;
using HIS.Clinical;

namespace HIS.Pharmacy;

public class PendingPrescriptionDto : MedicalOrderDto
{
    public string PatientName { get; set; }
    public string PatientMRN { get; set; } // Medical Record Number
    // Add other fields if needed, e.g. DoctorName
}
