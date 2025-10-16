using System;

namespace SchoolPortal.API.DTOs.School
{
    public class SchoolContactDto
    {
        public Guid Id { get; set; }
        public Guid SchoolId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? MobilePhone { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public Guid CityId { get; set; }
        public Guid StateId { get; set; }
        public Guid CountryId { get; set; }
        public bool IsActive { get; set; }
        public string? CityName { get; set; }
        public string? StateName { get; set; }
        public string? CountryName { get; set; }
        public string? SchoolName { get; set; }
    }

    public class CreateSchoolContactDto
    {
        public Guid SchoolId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? MobilePhone { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public Guid CityId { get; set; }
        public Guid StateId { get; set; }
        public Guid CountryId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateSchoolContactDto : CreateSchoolContactDto
    {
        public Guid Id { get; set; }
    }
}
