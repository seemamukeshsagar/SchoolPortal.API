using System;

namespace SchoolPortal.API.DTOs.Company
{
	public class CompanyDto
	{
		public Guid Id { get; set; }
		public string CompanyName { get; set; }
		public string Description { get; set; }
		public string Address { get; set; }
		public Guid CountryId { get; set; }
		public string CountryName { get; set; }
		public Guid StateId { get; set; }
		public string StateName { get; set; }
		public Guid CityId { get; set; }
		public string CityName { get; set; }
		public string ZipCode { get; set; }
		public string Email { get; set; }
		public bool IsActive { get; set; }
		public string EstablishmentYear { get; set; }
		public Guid JudistrictionArea { get; set; }
		public string JudistrictionAreaName { get; set; }
		public string Status { get; set; }
		public string StatusMessage { get; set; }
	}
}