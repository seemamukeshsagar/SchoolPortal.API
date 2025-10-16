using AutoMapper;
using SchoolPortal.API.DTOs.Class;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Mappings
{
    public class ClassProfile : Profile
    {
        public ClassProfile()
        {
            CreateMap<ClassMaster, ClassDto>();
            CreateMap<ClassMaster, ClassListDto>();
            CreateMap<CreateClassRequest, ClassMaster>();
            CreateMap<UpdateClassRequest, ClassMaster>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}