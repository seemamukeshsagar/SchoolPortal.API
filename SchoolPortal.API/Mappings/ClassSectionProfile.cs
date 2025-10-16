// SchoolPortal.API/Mappings/ClassSectionProfile.cs
using AutoMapper;
using SchoolPortal.API.DTOs.ClassSection;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Mappings
{
    public class ClassSectionProfile : Profile
    {
        public ClassSectionProfile()
        {
            // Map from domain model to DTO
            CreateMap<ClassSectionDetail, ClassSectionDetailDto>()
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.ClassMaster.Name))
                .ForMember(dest => dest.SectionName, opt => opt.MapFrom(src => src.SectionMaster.Name))
                .ForMember(dest => dest.ClassTeacherName, opt => opt.MapFrom(src => 
                    src.ClassTeacher != null ? $"{src.ClassTeacher.FirstName} {src.ClassTeacher.LastName}" : null))
                .ForMember(dest => dest.MaxStudents, opt => opt.MapFrom(src => src.MaxStudents))
                .ForMember(dest => dest.AcademicYear, opt => opt.MapFrom(src => src.AcademicYear))
                .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks));

            // Map from create request to domain model
            CreateMap<CreateClassSectionDetailRequest, ClassSectionDetail>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            // Map from update request to domain model
            CreateMap<UpdateClassSectionDetailRequest, ClassSectionDetail>()
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}