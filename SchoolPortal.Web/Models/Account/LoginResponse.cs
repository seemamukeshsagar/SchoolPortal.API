using System.Collections.Generic;

namespace SchoolPortal.Web.Models.Account
{
    public class LoginResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Privileges { get; set; }
    }
}
