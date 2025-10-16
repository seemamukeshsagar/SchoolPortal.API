using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.DTOs.Section
{
    public class UpdateSectionRequest
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        public Guid CompanyId { get; set; }
        [Required]
        public Guid SchoolId { get; set; }
        public bool IsActive { get; set; }
    }
}