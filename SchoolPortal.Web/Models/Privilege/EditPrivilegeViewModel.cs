using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Web.Models.Privilege
{
    public class EditPrivilegeViewModel
    {
        public required Guid Id { get; set; }

        [Required]
        [Display(Name = "Privilege Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public required string Name { get; set; }

        [Display(Name = "Parent Privilege")]
        public int? ParentPrivilegeId { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}