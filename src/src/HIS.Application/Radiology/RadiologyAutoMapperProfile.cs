using AutoMapper;

namespace HIS.Radiology;

public class RadiologyAutoMapperProfile : Profile
{
    public RadiologyAutoMapperProfile()
    {
        CreateMap<RadiologyRequest, RadiologyRequestDto>();
        CreateMap<CreateUpdateRadiologyRequestDto, RadiologyRequest>();
    }
}
