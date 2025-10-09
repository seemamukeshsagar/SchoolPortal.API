using AutoMapper;
using SchoolPortal.API.DTOs;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Company Mappings
            CreateMap<CompanyMaster, CompanyDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => (int)src.Id.GetHashCode()))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CompanyName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore()) // Not in CompanyMaster
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore()) // Not in CompanyMaster
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore()) // Not in CompanyMaster
                .ForMember(dest => dest.SchoolId, opt => opt.Ignore()); // Not in CompanyMaster

            CreateMap<CreateCompanyDto, CompanyMaster>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            CreateMap<UpdateCompanyDto, CompanyMaster>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Name));

            // Reverse mappings for updates
            CreateMap<CompanyDto, CompanyMaster>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Don't map ID back to prevent accidental changes
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            // Location Mappings
            CreateMap<CityMaster, CityDto>().ReverseMap();
            CreateMap<StateMaster, StateDto>().ReverseMap();
            CreateMap<CountryMaster, CountryDto>().ReverseMap();
        }
    }
}
