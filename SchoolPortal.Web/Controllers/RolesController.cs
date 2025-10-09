using Microsoft.AspNetCore.Mvc;

namespace SchoolPortal.Web.Controllers
{
    public class RolesController : Controller
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
