using System;

namespace SchoolPortal.API.DTOs.Company
{
    public class CompanyListDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? CityName { get; set; }
        public string? StateName { get; set; }
        public string? CountryName { get; set; }
        public string? JurisdictionArea { get; set; }
        public string? ZipCode { get; set; }
        public string? Email { get; set; }
        public string? EstablishedYear { get; set; }
    }
}
