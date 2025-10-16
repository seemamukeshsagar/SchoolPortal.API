using AutoMapper;
using SchoolPortal.API.DTOs.Section;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Profiles
{
    public class SectionProfile : Profile
    {
        public SectionProfile()
        {
            CreateMap<SectionMaster, SectionDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.School.Name));

            CreateMap<SectionMaster, SectionListDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.School.Name));

            CreateMap<CreateSectionRequest, SectionMaster>();
            CreateMap<UpdateSectionRequest, SectionMaster>();
        }
    }
}