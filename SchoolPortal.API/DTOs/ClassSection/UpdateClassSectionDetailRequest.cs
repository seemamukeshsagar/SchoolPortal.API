// SchoolPortal.API/DTOs/ClassSection/UpdateClassSectionDetailRequest.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.DTOs.ClassSection
{
    public class UpdateClassSectionDetailRequest
    {
        [Required]
        public Guid Id { get; set; }
        
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
        
        public bool IsActive { get; set; }
        public string ModifiedBy { get; set; }
    }
}