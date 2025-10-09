using Microsoft.AspNetCore.Mvc;

namespace SchoolPortal.Web.Controllers
{
    public class PrivilegesController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
    }
}
