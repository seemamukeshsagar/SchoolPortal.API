namespace SchoolPortal.Web.DTOs
{
    public class UserProfileDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
    }
}
