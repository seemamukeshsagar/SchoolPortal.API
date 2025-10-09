using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using SchoolPortal.API.DTOs.Company;
using SchoolPortal.API.DTOs;

namespace SchoolPortal.Web.Models.Company
{
    public abstract class CompanyBaseViewModel
    {
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
        public virtual required string CompanyName { get; set; }
        
        [Required(ErrorMessage = "Description is required")]
        public required string Description { get; set; }
        
        [Required(ErrorMessage = "Address is required")]
        public required string Address { get; set; }
        
        [Required(ErrorMessage = "Country is required")]
        public Guid CountryId { get; set; }
        
        [Required(ErrorMessage = "State is required")]
        public Guid StateId { get; set; }
        
        [Required(ErrorMessage = "City is required")]
        public Guid CityId { get; set; }
        
        [Required(ErrorMessage = "Zip code is required")]
        [StringLength(20, ErrorMessage = "Zip code cannot exceed 20 characters")]
        public required string ZipCode { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public virtual required string Email { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [Required(ErrorMessage = "Establishment year is required")]
        public required string EstablishmentYear { get; set; }
        
        [Required(ErrorMessage = "Jurisdiction area is required")]
        public Guid JudistrictionArea { get; set; }

        // Dropdown lists
        public List<CountryDto> Countries { get; set; } = new List<CountryDto>();
        public List<StateDto> States { get; set; } = new List<StateDto>();
        public List<CityDto> Cities { get; set; } = new List<CityDto>();
        public List<CityDto> JudistrictionAreas { get; set; } = new List<CityDto>();
    }
}