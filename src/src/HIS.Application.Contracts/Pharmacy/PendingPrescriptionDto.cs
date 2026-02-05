using System;
using HIS.Clinical;

namespace HIS.Pharmacy;

public class PendingPrescriptionDto : MedicalOrderDto
{
    public string PatientName { get; set; }
    public string PatientMRN { get; set; }
    
    // Prescription Details
    public string Dosage { get; set; }
    public string Frequency { get; set; }
    public string Route { get; set; }
    public string Duration { get; set; }
    public string Instructions { get; set; }
}
