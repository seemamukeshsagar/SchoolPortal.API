using System;
using System.Collections.Generic;
using SchoolPortal.API.DTOs.Company;

namespace SchoolPortal.Web.Models.Company
{
    public abstract class CompanyBaseViewModel
    {
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public Guid CountryId { get; set; }
        public Guid StateId { get; set; }
        public Guid CityId { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;
        public string EstablishmentYear { get; set; }
        public Guid JudistrictionArea { get; set; }

        // Dropdown lists
        public List<CountryDto> Countries { get; set; } = new List<CountryDto>();
        public List<StateDto> States { get; set; } = new List<StateDto>();
        public List<CityDto> Cities { get; set; } = new List<CityDto>();
        public List<CityDto> JudistrictionAreas { get; set; } = new List<CityDto>();
    }
}