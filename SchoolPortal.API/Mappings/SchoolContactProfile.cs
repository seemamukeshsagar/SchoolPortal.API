using AutoMapper;
using SchoolPortal.API.DTOs.School;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Mappings
{
    public class SchoolContactProfile : Profile
    {
        public SchoolContactProfile()
        {
            // Map from entity to DTO
            CreateMap<SchoolContactMaster, SchoolContactDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SchoolId, opt => opt.MapFrom(src => src.SchoolId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.MobilePhone, opt => opt.MapFrom(src => src.MobilePhone))
                .ForMember(dest => dest.AddressLine1, opt => opt.MapFrom(src => src.AddressLine1))
                .ForMember(dest => dest.AddressLine2, opt => opt.MapFrom(src => src.AddressLine2))
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityId))
                .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.StateId))
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City != null ? src.City.CityName : null))
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.State != null ? src.State.StateName : null))
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country != null ? src.Country.CountryName : null));

            // Map from DTO to entity for create/update
            CreateMap<SchoolContactRequest, SchoolContactMaster>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Will be set by the service
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore()) // Will be set by the service
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore()) // Will be set by the service
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore()) // Will be set by the service
                .ForMember(dest => dest.City, opt => opt.Ignore()) // Navigation property, handled by EF
                .ForMember(dest => dest.State, opt => opt.Ignore()) // Navigation property, handled by EF
                .ForMember(dest => dest.Country, opt => opt.Ignore()) // Navigation property, handled by EF
                .ForMember(dest => dest.School, opt => opt.Ignore()); // Navigation property, handled by EF
        }
    }
}
