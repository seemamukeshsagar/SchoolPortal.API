using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Web.Models.School
{
    public class SchoolContactViewModel
    {
        public Guid Id { get; set; }
        public Guid SchoolId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot be longer than 100 characters")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot be longer than 100 characters")]
        public string? LastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(255)]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Phone(ErrorMessage = "Invalid mobile number")]
        [StringLength(20)]
        [Display(Name = "Mobile")]
        public string? MobilePhone { get; set; }

        [StringLength(200)]
        [Display(Name = "Address Line 1")]
        public string? AddressLine1 { get; set; }

        [StringLength(200)]
        [Display(Name = "Address Line 2")]
        public string? AddressLine2 { get; set; }

        [Required(ErrorMessage = "City is required")]
        [Display(Name = "City")]
        public Guid CityId { get; set; }

        [Required(ErrorMessage = "State is required")]
        [Display(Name = "State")]
        public Guid StateId { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [Display(Name = "Country")]
        public Guid CountryId { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        // Display properties
        public string? CityName { get; set; }
        public string? StateName { get; set; }
        public string? CountryName { get; set; }
        public string? SchoolName { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
