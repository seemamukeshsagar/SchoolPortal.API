using System;

namespace SchoolPortal.API.DTOs.School
{
	public class SchoolDto
	{
		public Guid Id { get; set; }
		public required string Name { get; set; }
		public required string Description { get; set; }
		public required string Email { get; set; }
		public required string Address1 { get; set; }
		public string? Address2 { get; set; }
		public Guid? CityId { get; set; }
		public string? CityName { get; set; }
		public Guid? StateId { get; set; }
		public string? StateName { get; set; }
		public Guid? CountryId { get; set; }
		public string? CountryName { get; set; }
		public string? ZipCode { get; set; }
		public string? Phone { get; set; }
		public string? Mobile { get; set; }
		public string? EstablishmentYear { get; set; }
		public Guid? JudistrictionCityId { get; set; }
		public string? JudistrictionCityName { get; set; }
		public Guid? JudistrictionStateId { get; set; }
		public string? JudistrictionStateName { get; set; }
		public Guid? JudistrictionCountryId { get; set; }
		public string? JudistrictionCountryName { get; set; }
		public string? BankName { get; set; }
		public string? BankAddress1 { get; set; }
		public string? BankAddress2 { get; set; }
		public Guid? BankCityId { get; set; }
		public string? BankCityName { get; set; }
		public Guid? BankStateId { get; set; }
		public string? BankStateName { get; set; }
		public Guid? BankCountryId { get; set; }
		public string? BankCountryName { get; set; }
		public string? BankZipCode { get; set; }
		public string? AccountNumber { get; set; }
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public Guid CompanyId { get; set; }
		public string? CompanyName { get; set; }
		public Guid? CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public Guid? ModifiedBy { get; set; }
		public DateTime? ModifiedDate { get; set; }
		public string? Status { get; set; }
		public string? StatusMessage { get; set; }
	}
}
