using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.DTOs.Company
{
	public class UpdateCompanyRequest : CreateCompanyRequest
	{
		[Required]
		public Guid Id { get; set; }
		
		[Required]
		[MaxLength(50)]
		public required string Status { get; set; }
		
		[MaxLength(500)]
		public required string StatusMessage { get; set; }
	}
}