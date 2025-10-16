using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Web.Models.Section
{
    public class CreateSectionViewModel
    {
        [Required]
        [Display(Name = "Section Name")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Company")]
        public Guid CompanyId { get; set; }

        [Required]
        [Display(Name = "School")]
        public Guid SchoolId { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}