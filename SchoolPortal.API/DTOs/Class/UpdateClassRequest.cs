using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.DTOs.Class
{
    public class UpdateClassRequest
    {
        [Required]
        public string Name { get; set; }
        
        public string ExamAssessment { get; set; }
        public bool IsGradePointApplicable { get; set; }
        public bool IsActive { get; set; }
        public int? OrderBy { get; set; }
    }
}