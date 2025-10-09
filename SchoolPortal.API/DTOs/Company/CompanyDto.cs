using System;

namespace SchoolPortal.API.DTOs.Company
{
	public class CompanyDto
	{
		public Guid Id { get; set; }
		public required string CompanyName { get; set; }
		public required string Description { get; set; }
		public required string Address { get; set; }
		public Guid CountryId { get; set; }
		public required string CountryName { get; set; }
		public Guid StateId { get; set; }
		public required string StateName { get; set; }
		public Guid CityId { get; set; }
		public required string CityName { get; set; }
		public required string ZipCode { get; set; }
		public required string Email { get; set; }
		public bool IsActive { get; set; }
		public required string EstablishmentYear { get; set; }
		public Guid JurisdictionArea { get; set; }
		public required string JurisdictionAreaName { get; set; }
		public required string Status { get; set; }
		public required string StatusMessage { get; set; }
	}
}