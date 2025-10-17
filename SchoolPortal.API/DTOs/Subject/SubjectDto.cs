// DTOs/Subject/SubjectDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.DTOs.Subject
{
    public class SubjectDto
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string SubjectName { get; set; } = null!;
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public Guid SchoolId { get; set; }
        public string? SchoolName { get; set; }
        public bool? IsScholastic { get; set; }
        public bool IsActive { get; set; }
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Active";
        [MaxLength(500)]
        public string? StatusMessage { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class CreateSubjectRequest
    {
        [Required]
        [MaxLength(200)]
        public string SubjectName { get; set; } = null!;
        
        [Required]
        public Guid CompanyId { get; set; }
        
        [Required]
        public Guid SchoolId { get; set; }
        
        public bool? IsScholastic { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Active";
        
        [MaxLength(500)]
        public string? StatusMessage { get; set; }
        
        [Required]
        public Guid CreatedBy { get; set; }
    }

    public class UpdateSubjectRequest
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string SubjectName { get; set; } = null!;
        
        [Required]
        public Guid CompanyId { get; set; }
        
        [Required]
        public Guid SchoolId { get; set; }
        
        public bool? IsScholastic { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Active";
        
        [MaxLength(500)]
        public string? StatusMessage { get; set; }
        
        [Required]
        public Guid ModifiedBy { get; set; }
    }
}