// SchoolPortal.API\DTOs\Role\RoleDto.cs
using System;

namespace SchoolPortal.API.DTOs.Role
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public Guid SchoolId { get; set; }
        public string? SchoolName { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}