using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace SchoolPortal.Web.Controllers
{
    public class ExaminationController : Controller
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
        public IActionResult Edit()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GenerateReportCard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CommitToHistory()
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
