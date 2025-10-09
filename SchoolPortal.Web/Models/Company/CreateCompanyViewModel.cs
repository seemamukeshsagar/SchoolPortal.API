using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Web.Models.Company
{
    public class CreateCompanyViewModel : CompanyBaseViewModel
    {
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
        public new string CompanyName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public new string Email { get; set; }
    }
}