using System;

namespace SchoolPortal.API.DTOs.Privilege
{
    public class PrivilegeDto
    {
        public Guid Id { get; set; }
        public required string PrivilegeName { get; set; }
        public int? PrivilegeParentId { get; set; }
        public string? ParentPrivilegeName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Status { get; set; }
        public string? StatusMessage { get; set; }
    }
}