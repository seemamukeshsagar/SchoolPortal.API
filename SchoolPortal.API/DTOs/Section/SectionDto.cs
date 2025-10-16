using System;

namespace SchoolPortal.API.DTOs.Section
{
    public class SectionDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public Guid CompanyId { get; set; }
        public required string CompanyName { get; set; }
        public Guid SchoolId { get; set; }
        public required string SchoolName { get; set; }
        public bool IsActive { get; set; }
        public string? Status { get; set; }
        public string? StatusMessage { get; set; }
    }
}