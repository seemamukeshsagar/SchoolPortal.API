using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.DTOs.Company
{
	public class CreateCompanyRequest
	{
		[Required]
		[MaxLength(200)]
		public required string CompanyName { get; set; }
		
		[MaxLength(1000)]
		public required string Description { get; set; }
		
		[Required]
		[MaxLength(500)]
		public required string Address { get; set; }
		
		[Required]
		public Guid CountryId { get; set; }
		
		[Required]
		public Guid StateId { get; set; }
		
		[Required]
		public Guid CityId { get; set; }
		
		[Required]
		[MaxLength(20)]
		public required string ZipCode { get; set; }
		
		[Required]
		[EmailAddress]
		[MaxLength(100)]
		public required string Email { get; set; }
		
		public bool IsActive { get; set; } = true;
		
		[Required]
		[MaxLength(4)]
		[RegularExpression(@"^\d{4}$", ErrorMessage = "Establishment year must be a 4-digit number")]
		public required string EstablishmentYear { get; set; }
		
		[Required]
		public Guid JurisdictionArea { get; set; }
	}
}