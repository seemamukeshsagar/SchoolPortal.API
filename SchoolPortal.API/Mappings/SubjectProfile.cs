// Mappings/SubjectProfile.cs
using AutoMapper;
using SchoolPortal.API.DTOs.Subject;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Mappings
{
    public class SubjectProfile : Profile
    {
        public SubjectProfile()
        {
            CreateMap<SubjectMaster, SubjectDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company != null ? src.Company.CompanyName : null))
                .ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.School != null ? src.School.Name : null))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByNavigation != null ? 
                    $"{src.CreatedByNavigation.FirstName} {src.CreatedByNavigation.LastName}" : null))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedByNavigation != null ? 
                    $"{src.ModifiedByNavigation.FirstName} {src.ModifiedByNavigation.LastName}" : null));

            CreateMap<CreateSubjectRequest, SubjectMaster>();
            CreateMap<UpdateSubjectRequest, SubjectMaster>();
        }
    }
}