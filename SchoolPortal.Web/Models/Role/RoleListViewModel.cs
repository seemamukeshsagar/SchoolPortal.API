using System;

namespace SchoolPortal.Web.Models.Role
{
    public class RoleListViewModel
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string CompanyName { get; set; }
        public required string SchoolName { get; set; }
        public bool IsActive { get; set; }
        public string Status => IsActive ? "Active" : "Inactive";
    }
}