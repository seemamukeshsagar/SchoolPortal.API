using System;
using System.Collections.Generic;

namespace SchoolPortal.API.DTOs.Company
{
	public class CountryDto
	{
		public Guid Id { get; set; }
		public string CountryName { get; set; }
		public List<StateDto> States { get; set; } = new List<StateDto>();
	}

	public class StateDto
	{
		public Guid Id { get; set; }
		public string StateName { get; set; }
		public List<CityDto> Cities { get; set; } = new List<CityDto>();
	}

	public class CityDto
	{
		public Guid Id { get; set; }
		public string CityName { get; set; }
	}
}