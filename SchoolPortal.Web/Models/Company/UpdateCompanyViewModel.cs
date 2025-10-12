using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Web.Models.Company
{
    public class UpdateCompanyViewModel : CompanyBaseViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
        public override required string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public override required string Email { get; set; } = string.Empty;

        public string? Status { get; set; }
        public string? StatusMessage { get; set; }

        // Display name properties for showing current selections
        public string? CountryName { get; set; }
        public string? StateName { get; set; }
        public string? CityName { get; set; }
        public string? JurisdictionAreaName { get; set; }
    }
}