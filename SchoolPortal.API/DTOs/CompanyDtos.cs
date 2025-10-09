using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
        public required string Address { get; set; }
        [MaxLength(20)]
        public required string PhoneNumber { get; set; }
        [EmailAddress]
        [MaxLength(100)]
        public required string Email { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int? SchoolId { get; set; }
    }

    public class CreateCompanyDto
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
        public required string Address { get; set; }
        [MaxLength(20)]
        public required string PhoneNumber { get; set; }
        [EmailAddress]
        [MaxLength(100)]
        public required string Email { get; set; }
        public int? SchoolId { get; set; }
    }

    public class UpdateCompanyDto
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
        public required string Address { get; set; }
        [MaxLength(20)]
        public required string PhoneNumber { get; set; }
        [EmailAddress]
        [MaxLength(100)]
        public required string Email { get; set; }
        public bool IsActive { get; set; } = true;
        public int? SchoolId { get; set; }
    }
}
