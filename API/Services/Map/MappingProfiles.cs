using API.DTO;
using AutoMapper;

namespace API.Services;

public class MappingProfiles:Profile
{
    public MappingProfiles()
    {
        CreateMap<URLDto, URL>();
        CreateMap<URL, URLDto>();
        CreateMap<URL, URLResponseDto>()
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedById));
    }
}