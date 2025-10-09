using AutoMapper;
using SchoolPortal.API.DTOs;
using SchoolPortal.API.DTOs.Company;
using SchoolPortal.API.Models;
using CompanyDto = SchoolPortal.API.DTOs.Company.CompanyDto;

namespace SchoolPortal.API.Mappings
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<CompanyMaster, CompanyDto>()
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.CountryName))
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.State.StateName))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.CityName))
                .ForMember(dest => dest.JurisdictionArea, opt => opt.MapFrom(src => src.JudistrictionArea))
                .ForMember(dest => dest.JurisdictionAreaName, opt => opt.MapFrom(src => src.JudistrictionAreaNavigation.CityName));

            CreateMap<CreateCompanyRequest, CompanyMaster>()
                .ForMember(dest => dest.JudistrictionArea, opt => opt.MapFrom(src => src.JurisdictionArea));
            
            CreateMap<UpdateCompanyRequest, CompanyMaster>()
                .ForMember(dest => dest.JudistrictionArea, opt => opt.MapFrom(src => src.JurisdictionArea));
            
            CreateMap<CountryMaster, CountryDto>();
            CreateMap<StateMaster, StateDto>();
            CreateMap<CityMaster, CityDto>();
        }
    }
}