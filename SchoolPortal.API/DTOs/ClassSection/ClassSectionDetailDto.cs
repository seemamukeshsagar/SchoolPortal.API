// SchoolPortal.API/DTOs/ClassSection/ClassSectionDetailDto.cs
using SchoolPortal.API.DTOs.Class;
using SchoolPortal.API.DTOs.Section;
using SchoolPortal.API.DTOs.Teacher;
using System;

namespace SchoolPortal.API.DTOs.ClassSection
{
    public class ClassSectionDetailDto
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public Guid SectionId { get; set; }
        public Guid? ClassTeacherId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public int MaxStudents { get; set; }
        public string AcademicYear { get; set; }
        public string Remarks { get; set; }
        
        // Computed properties for display
        public string ClassName => Class?.Name;
        public string SectionName => Section?.Name;
        public string ClassTeacherName { get; set; }

        // Navigation properties
        public ClassDto Class { get; set; }
        public SectionDto Section { get; set; }
        public TeacherDto ClassTeacher { get; set; }
    }
}