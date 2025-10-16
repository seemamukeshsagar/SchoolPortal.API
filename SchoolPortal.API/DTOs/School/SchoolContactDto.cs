using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.DTOs.School
{
    public class SchoolContactDto
    {
        public Guid Id { get; set; }
        public Guid SchoolId { get; set; }
        public string ContactType { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? MobilePhone { get; set; } = string.Empty;
        public string? AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; } = string.Empty;
        public Guid CityId { get; set; }
        public Guid StateId { get; set; }
        public Guid CountryId { get; set; }
        public bool IsActive { get; set; }
        public string? CityName { get; set; }
        public string? StateName { get; set; }
        public string? CountryName { get; set; }
        public string? SchoolName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class CreateSchoolContactDto
    {
        [Required]
        public Guid SchoolId { get; set; }
        
        [Required]
        public string ContactType { get; set; } = string.Empty;
        
        [Required]
        public string Value { get; set; } = string.Empty;
        
        public bool IsPrimary { get; set; }
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? MobilePhone { get; set; } = string.Empty;
        public string? AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; } = string.Empty;
        
        [Required]
        public Guid CityId { get; set; }
        
        [Required]
        public Guid StateId { get; set; }
        
        [Required]
        public Guid CountryId { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public class UpdateSchoolContactDto : CreateSchoolContactDto
    {
        public Guid Id { get; set; }
    }
}