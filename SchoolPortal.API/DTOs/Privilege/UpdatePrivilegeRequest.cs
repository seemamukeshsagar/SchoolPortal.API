namespace SchoolPortal.API.DTOs.Privilege
{
    public class UpdatePrivilegeRequest
    {
        public required string PrivilegeName { get; set; }
        public int? PrivilegeParentId { get; set; }
        public bool IsActive { get; set; }
    }
}