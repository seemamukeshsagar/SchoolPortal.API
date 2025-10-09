using Microsoft.AspNetCore.Mvc;

namespace SchoolPortal.Web.Controllers
{
    public class SalaryController : Controller
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
        public IActionResult DesigGrades()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Structure()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EmpSalaryData()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Views()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Commit()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ViewHistory()
        {
            return View();
        }
    }
}
