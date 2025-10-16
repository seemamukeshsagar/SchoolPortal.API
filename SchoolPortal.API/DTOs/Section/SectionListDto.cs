namespace SchoolPortal.API.DTOs.Section
{
    public class SectionListDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string CompanyName { get; set; }
        public required string SchoolName { get; set; }
        public bool IsActive { get; set; }
    }
}