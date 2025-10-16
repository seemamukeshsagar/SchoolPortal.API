using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.DTOs.School
{
    /// <summary>
    /// Request model for creating or updating a school contact
    /// </summary>
    public class SchoolContactRequest
    {
        [Required]
        public Guid SchoolId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }

        [Phone]
        [StringLength(20)]
        public string MobilePhone { get; set; }

        [StringLength(200)]
        public string AddressLine1 { get; set; }

        [StringLength(200)]
        public string AddressLine2 { get; set; }

        [Required]
        public Guid CityId { get; set; }

        [Required]
        public Guid StateId { get; set; }

        [Required]
        public Guid CountryId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
