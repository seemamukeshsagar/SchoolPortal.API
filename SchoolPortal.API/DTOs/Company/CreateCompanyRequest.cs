using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.DTOs.Company
{
	public class CreateCompanyRequest
	{
		[Required]
		public string CompanyName { get; set; }
		public string Description { get; set; }
		public string Address { get; set; }
		
		[Required]
		public Guid CountryId { get; set; }
		
		[Required]
		public Guid StateId { get; set; }
		
		[Required]
		public Guid CityId { get; set; }
		
		public string ZipCode { get; set; }
		
		[EmailAddress]
		public string Email { get; set; }
		
		public bool IsActive { get; set; } = true;
		public string EstablishmentYear { get; set; }
		
		[Required]
		public Guid JudistrictionArea { get; set; }
	}
}