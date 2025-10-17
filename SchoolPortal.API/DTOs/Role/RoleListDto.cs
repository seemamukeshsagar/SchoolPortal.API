// SchoolPortal.API\DTOs\Role\RoleListDto.cs
namespace SchoolPortal.API.DTOs.Role
{
    public class RoleListDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? CompanyName { get; set; }
        public string? SchoolName { get; set; }
        public bool IsActive { get; set; }
    }
}