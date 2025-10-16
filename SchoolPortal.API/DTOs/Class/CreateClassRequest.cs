using System.ComponentModel.DataAnnotations;

namespace SchoolPortal.API.DTOs.Class
{
    public class CreateClassRequest
    {
        [Required]
        public string Name { get; set; }
        
        public string ExamAssessment { get; set; }
        public bool IsGradePointApplicable { get; set; }
        public bool IsActive { get; set; } = true;
        public int? OrderBy { get; set; }
        public Guid CompanyId { get; set; }
        public Guid SchoolId { get; set; }
    }
}