using AutoMapper;
using HIS.Settings;

namespace HIS;

public class HISApplicationAutoMapperProfile : Profile
{
    public HISApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<Specialty, SpecialtyDto>();
        CreateMap<CreateUpdateSpecialtyDto, Specialty>();

        CreateMap<Department, DepartmentDto>();
        CreateMap<CreateUpdateDepartmentDto, Department>();

        CreateMap<Clinic, ClinicDto>();
        CreateMap<CreateUpdateClinicDto, Clinic>();

        CreateMap<Doctor, DoctorDto>();
        CreateMap<CreateUpdateDoctorDto, Doctor>();
        
        CreateMap<Laboratory, LaboratoryDto>();
        CreateMap<CreateUpdateLaboratoryDto, Laboratory>();
    }
}
