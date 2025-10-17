using AutoMapper;
using SchoolPortal.API.DTOs.Privilege;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Mappings
{
    public class PrivilegeProfile : Profile
    {
        public PrivilegeProfile()
        {
            CreateMap<Privilege, PrivilegeDto>();
            CreateMap<Privilege, PrivilegeListDto>();
            CreateMap<CreatePrivilegeRequest, Privilege>();
            CreateMap<UpdatePrivilegeRequest, Privilege>();
        }
    }
}