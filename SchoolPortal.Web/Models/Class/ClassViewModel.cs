using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.Web.Models.Class
{
    public class ClassViewModel
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Class name is required")]
        [Display(Name = "Class Name")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Exam Assessment Type")]
        public string ExamAssessment { get; set; } = string.Empty;
        
        [Display(Name = "Grade Point Applicable")]
        public bool IsGradePointApplicable { get; set; }
        
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
        
        [Required(ErrorMessage = "Company is required")]
        [Display(Name = "Company")]
        public Guid CompanyId { get; set; }
        
        [Required(ErrorMessage = "School is required")]
        [Display(Name = "School")]
        public Guid SchoolId { get; set; }
        
        [Display(Name = "Display Order")]
        public int? OrderBy { get; set; }
        
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;
        
        [Display(Name = "Status Message")]
        public string StatusMessage { get; set; } = string.Empty;
    }
}