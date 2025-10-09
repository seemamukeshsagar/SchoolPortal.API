using Microsoft.AspNetCore.Mvc;

namespace SchoolPortal.Web.Controllers
{
    public class SubjectController : Controller
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

        [HttpGet]
        public IActionResult SubCategory()
        {
            return View();
        }
    }
}
