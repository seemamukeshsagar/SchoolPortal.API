namespace SchoolPortal.Web.ViewModels
{
    public class UserSession
    {
        public string FullName { get; set; } = string.Empty;
        public Guid? UserRoleId { get; set; }
        public bool? IsSuperUser { get; set; }

    }
}
