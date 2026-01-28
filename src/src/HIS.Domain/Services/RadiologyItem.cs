using System;

namespace HIS.Services;

public class RadiologyItem : ServiceItem
{
    public string Modality { get; set; } // CT, MRI, X-Ray
    public string BodyPart { get; set; } // Left Leg, Chest, Head
    public string Instructions { get; set; } // Fasting 4 hours, etc.

    protected RadiologyItem() { }

    public RadiologyItem(Guid id, string code, string name, Guid? departmentId, string modality, string bodyPart)
        : base(id, code, name, ServiceCategory.Radiology, departmentId)
    {
        Modality = modality;
        BodyPart = bodyPart;
    }
}
