// SchoolPortal.API\Mappings\RoleProfile.cs
using AutoMapper;
using SchoolPortal.API.DTOs.Role;
using SchoolPortal.API.Models;

namespace SchoolPortal.API.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            // Map from RoleMaster to RoleDto
            CreateMap<RoleMaster, RoleDto>()
                .ForMember(dest => dest.Status, opt => 
                    opt.MapFrom(src => src.IsActive ? "Active" : "Inactive"));

            // Map from RoleMaster to RoleListDto
            CreateMap<RoleMaster, RoleListDto>()
                .ForMember(dest => dest.IsActive, opt => 
                    opt.MapFrom(src => src.IsActive ? "Active" : "Inactive"));

            // Map from CreateRoleRequest to RoleMaster
            CreateMap<CreateRoleRequest, RoleMaster>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            // Map from UpdateRoleRequest to RoleMaster
            CreateMap<UpdateRoleRequest, RoleMaster>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());
        }
    }
}