using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Web.Models.Privilege
{
    public class PrivilegeListViewModel
    {
        public required Guid Id { get; set; }

        [Display(Name = "Privilege Name")]
        public required string Name { get; set; }

        [Display(Name = "Parent Privilege")]
        public string? ParentPrivilegeName { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}