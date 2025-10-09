using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.DTOs.Company
{
	public class UpdateCompanyRequest : CreateCompanyRequest
	{
		[Required]
		public Guid Id { get; set; }
		public string Status { get; set; }
		public string StatusMessage { get; set; }
	}
}