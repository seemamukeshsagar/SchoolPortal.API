using AutoMapper;
using SchoolPortal.API.DTOs;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Location Mappings
            CreateMap<CityMaster, CityDto>().ReverseMap();
            CreateMap<StateMaster, StateDto>().ReverseMap();
            CreateMap<CountryMaster, CountryDto>().ReverseMap();
        }
    }
}
