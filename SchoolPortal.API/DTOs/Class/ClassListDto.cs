namespace SchoolPortal.API.DTOs.Class
{
    public class ClassListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ExamAssessment { get; set; }
        public bool IsActive { get; set; }
        public int? OrderBy { get; set; }
        public string CompanyName { get; set; }
        public string SchoolName { get; set; }
    }
}