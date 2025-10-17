// SchoolPortal.API/Profiles/ClassSubjectDetailProfile.cs
using AutoMapper;
using SchoolPortal.API.DTOs.ClassSubjectDetail;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Mappings
{
    public class ClassSubjectDetailProfile : Profile
    {
        public ClassSubjectDetailProfile()
        {
            CreateMap<ClassSubjectDetail, ClassSubjectDetailDto>()
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.ClassMaster.Name))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName));

            CreateMap<CreateClassSubjectDetailRequest, ClassSubjectDetail>();
            CreateMap<UpdateClassSubjectDetailRequest, ClassSubjectDetail>();
        }
    }
}