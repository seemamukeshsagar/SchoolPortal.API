using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Web.Models.Role
{
    public class RoleBaseViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        [Display(Name = "Role Name")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        public Guid CompanyId { get; set; }

        [Required(ErrorMessage = "School is required")]
        [Display(Name = "School")]
        public Guid SchoolId { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}