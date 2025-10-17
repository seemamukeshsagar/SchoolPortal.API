// SchoolPortal.API\DTOs\Role\UpdateRoleRequest.cs
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.DTOs.Role
{
    public class UpdateRoleRequest
    {
        [Required(ErrorMessage = "Role ID is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        [StringLength(100, ErrorMessage = "Role name cannot be longer than 100 characters")]
        public required string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Company ID is required")]
        public Guid CompanyId { get; set; }

        [Required(ErrorMessage = "School ID is required")]
        public Guid SchoolId { get; set; }

        public bool IsActive { get; set; }
    }
}