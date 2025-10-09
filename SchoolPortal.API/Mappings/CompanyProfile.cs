using AutoMapper;
using SchoolPortal.API.DTOs.Company;
using SchoolPortal.API.Models;

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
                .ForMember(dest => dest.JudistrictionAreaName, opt => opt.MapFrom(src => src.City.CityName));

            CreateMap<CreateCompanyRequest, CompanyMaster>();
            CreateMap<UpdateCompanyRequest, CompanyMaster>();
            
            CreateMap<CountryMaster, CountryDto>();
            CreateMap<StateMaster, StateDto>();
            CreateMap<CityMaster, CityDto>();
        }
    }
}