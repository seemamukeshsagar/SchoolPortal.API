namespace SchoolPortal.API.DTOs.Privilege
{
    public class PrivilegeListDto
    {
        public Guid Id { get; set; }
        public required string PrivilegeName { get; set; }
        public string? ParentPrivilegeName { get; set; }
        public bool IsActive { get; set; }
    }
}