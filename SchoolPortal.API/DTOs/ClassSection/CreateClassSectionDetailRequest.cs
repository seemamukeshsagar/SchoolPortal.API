// SchoolPortal.API/DTOs/ClassSection/CreateClassSectionDetailRequest.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.DTOs.ClassSection
{
    public class CreateClassSectionDetailRequest
    {
        [Required]
        public Guid ClassId { get; set; }
        
        [Required]
        public Guid SectionId { get; set; }
        
        public Guid? ClassTeacherId { get; set; }
        
        [Required]
        public int MaxStudents { get; set; }
        
        [Required]
        public string AcademicYear { get; set; }
        
        public string Remarks { get; set; }
        
        public bool IsActive { get; set; } = true;
        public string CreatedBy { get; set; }
    }
}