using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.DTOs.School
{
	public class UpdateSchoolRequest
	{
		[Required]
		public Guid Id { get; set; }
		
		[Required]
		[MaxLength(200)]
		public required string Name { get; set; }
		
		[MaxLength(1000)]
		public string? Description { get; set; }
		
		[Required]
		[EmailAddress]
		[MaxLength(100)]
		public required string Email { get; set; }
		
		[Required]
		[MaxLength(500)]
		public required string Address1 { get; set; }
		
		[MaxLength(500)]
		public string? Address2 { get; set; }
		
		public Guid? CityId { get; set; }
		
		public Guid? StateId { get; set; }
		
		public Guid? CountryId { get; set; }
		
		[MaxLength(20)]
		public string? ZipCode { get; set; }
		
		[MaxLength(20)]
		public string? Phone { get; set; }
		
		[MaxLength(20)]
		public string? Mobile { get; set; }
		
		[MaxLength(4)]
		[RegularExpression(@"^\d{4}$", ErrorMessage = "Establishment year must be a 4-digit number")]
		public string? EstablishmentYear { get; set; }
		
		public Guid? JudistrictionCityId { get; set; }
		
		public Guid? JudistrictionStateId { get; set; }
		
		public Guid? JudistrictionCountryId { get; set; }
		
		[MaxLength(100)]
		public string? BankName { get; set; }
		
		[MaxLength(500)]
		public string? BankAddress1 { get; set; }
		
		[MaxLength(500)]
		public string? BankAddress2 { get; set; }
		
		public Guid? BankCityId { get; set; }
		
		public Guid? BankStateId { get; set; }
		
		public Guid? BankCountryId { get; set; }
		
		[MaxLength(20)]
		public string? BankZipCode { get; set; }
		
		[MaxLength(50)]
		public string? AccountNumber { get; set; }
		
		[Required]
		public Guid CompanyId { get; set; }
		
		public bool IsActive { get; set; }
		
		public string? Status { get; set; }
		
		public string? StatusMessage { get; set; }
	}
}
