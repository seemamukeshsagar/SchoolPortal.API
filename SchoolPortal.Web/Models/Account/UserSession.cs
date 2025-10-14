using System.Collections.Generic;

namespace SchoolPortal.Web.Models.Account
{
    public class UserSession
    {
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public Guid? UserRoleId { get; set; }
        public bool? IsSuperUser { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<string> Privileges { get; set; } = new List<string>();
    }
}
