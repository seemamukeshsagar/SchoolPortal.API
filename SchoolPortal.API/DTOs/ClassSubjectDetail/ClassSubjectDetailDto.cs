// SchoolPortal.API/DTOs/ClassSubjectDetail/ClassSubjectDetailDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.DTOs.ClassSubjectDetail
{
    public class ClassSubjectDetailDto
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public string? ClassName { get; set; }
        public Guid SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public bool IsActive { get; set; } = true;
        public string Status { get; set; } = "Active";
        public string? StatusMessage { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class CreateClassSubjectDetailRequest
    {
        [Required]
        public Guid ClassId { get; set; }
        
        [Required]
        public Guid SubjectId { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Active";
        
        [MaxLength(500)]
        public string? StatusMessage { get; set; }
        
        [Required]
        public Guid CreatedBy { get; set; }
    }

    public class UpdateClassSubjectDetailRequest
    {
        [Required]
        public Guid Id { get; set; }

        public bool? IsActive { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        [MaxLength(500)]
        public string? StatusMessage { get; set; }

        public Guid? ModifiedBy { get; set; }
    }
}