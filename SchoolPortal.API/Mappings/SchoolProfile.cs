using AutoMapper;
using SchoolPortal.API.DTOs.School;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Mappings
{
    public class SchoolProfile : Profile
    {
        public SchoolProfile()
        {
            CreateMap<SchoolMaster, SchoolDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company != null ? src.Company.CompanyName : null))
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country != null ? src.Country.CountryName : null))
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.State != null ? src.State.StateName : null))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City != null ? src.City.CityName : null))
                .ForMember(dest => dest.JudistrictionCountryName, opt => opt.MapFrom(src => src.JudistrictionCountry != null ? src.JudistrictionCountry.CountryName : null))
                .ForMember(dest => dest.JudistrictionStateName, opt => opt.MapFrom(src => src.JudistrictionState != null ? src.JudistrictionState.StateName : null))
                .ForMember(dest => dest.JudistrictionCityName, opt => opt.MapFrom(src => src.JudistrictionCity != null ? src.JudistrictionCity.CityName : null))
                .ForMember(dest => dest.BankCountryName, opt => opt.MapFrom(src => src.BankCountry != null ? src.BankCountry.CountryName : null))
                .ForMember(dest => dest.BankStateName, opt => opt.MapFrom(src => src.BankState != null ? src.BankState.StateName : null))
                .ForMember(dest => dest.BankCityName, opt => opt.MapFrom(src => src.BankCity != null ? src.BankCity.CityName : null));

            CreateMap<CreateSchoolRequest, SchoolMaster>();
            CreateMap<UpdateSchoolRequest, SchoolMaster>();
        }
    }
}
