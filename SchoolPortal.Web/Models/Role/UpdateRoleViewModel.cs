using System;

namespace SchoolPortal.Web.Models.Role
{
    public class UpdateRoleViewModel : RoleBaseViewModel
    {
        public Guid Id { get; set; }
        public string? CompanyName { get; set; }
        public string? SchoolName { get; set; }
    }
}