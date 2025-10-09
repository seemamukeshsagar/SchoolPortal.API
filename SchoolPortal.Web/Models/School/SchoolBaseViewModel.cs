using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using SchoolPortal.API.DTOs.Company;
using CompanyDto = SchoolPortal.API.DTOs.Company.CompanyDto;
using SchoolPortal.API.DTOs;

namespace SchoolPortal.Web.Models.School
{
    public abstract class SchoolBaseViewModel
    {
        [Required(ErrorMessage = "School name is required")]
        [StringLength(200, ErrorMessage = "School name cannot exceed 200 characters")]
        public virtual required string Name { get; set; }
        
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public virtual required string Email { get; set; }
        
        [Required(ErrorMessage = "Address is required")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public required string Address1 { get; set; }
        
        [StringLength(500, ErrorMessage = "Address line 2 cannot exceed 500 characters")]
        public string? Address2 { get; set; }
        
        public Guid? CountryId { get; set; }
        public Guid? StateId { get; set; }
        public Guid? CityId { get; set; }
        
        [StringLength(20, ErrorMessage = "Zip code cannot exceed 20 characters")]
        public string? ZipCode { get; set; }
        
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        public string? Phone { get; set; }
        
        [StringLength(20, ErrorMessage = "Mobile cannot exceed 20 characters")]
        public string? Mobile { get; set; }
        
        [StringLength(4, ErrorMessage = "Establishment year must be 4 characters")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Establishment year must be a 4-digit number")]
        public string? EstablishmentYear { get; set; }
        
        public Guid? JudistrictionCountryId { get; set; }
        public Guid? JudistrictionStateId { get; set; }
        public Guid? JudistrictionCityId { get; set; }
        
        [StringLength(100, ErrorMessage = "Bank name cannot exceed 100 characters")]
        public string? BankName { get; set; }
        
        [StringLength(500, ErrorMessage = "Bank address cannot exceed 500 characters")]
        public string? BankAddress1 { get; set; }
        
        [StringLength(500, ErrorMessage = "Bank address line 2 cannot exceed 500 characters")]
        public string? BankAddress2 { get; set; }
        
        public Guid? BankCountryId { get; set; }
        public Guid? BankStateId { get; set; }
        public Guid? BankCityId { get; set; }
        
        [StringLength(20, ErrorMessage = "Bank zip code cannot exceed 20 characters")]
        public string? BankZipCode { get; set; }
        
        [StringLength(50, ErrorMessage = "Account number cannot exceed 50 characters")]
        public string? AccountNumber { get; set; }
        
        [Required(ErrorMessage = "Company is required")]
        public Guid CompanyId { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public string? Status { get; set; }
        public string? StatusMessage { get; set; }

        // Dropdown lists
        public List<CompanyDto> Companies { get; set; } = new List<CompanyDto>();
        public List<CountryDto> Countries { get; set; } = new List<CountryDto>();
        public List<StateDto> States { get; set; } = new List<StateDto>();
        public List<CityDto> Cities { get; set; } = new List<CityDto>();
        public List<CountryDto> JudistrictionCountries { get; set; } = new List<CountryDto>();
        public List<StateDto> JudistrictionStates { get; set; } = new List<StateDto>();
        public List<CityDto> JudistrictionCities { get; set; } = new List<CityDto>();
        public List<CountryDto> BankCountries { get; set; } = new List<CountryDto>();
        public List<StateDto> BankStates { get; set; } = new List<StateDto>();
        public List<CityDto> BankCities { get; set; } = new List<CityDto>();
    }
}
