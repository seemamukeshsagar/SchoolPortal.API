using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Security.Claims;
using System.Xml;

namespace SchoolPortal.Web.Controllers
{
    public class TimeTableController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TimeTablePeriods()
        {
            return View();
        }


        [HttpGet]
        public IActionResult TimeTableClassPeriods()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Generate()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TimeTableByClass()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TimeTableByTeacher()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TimeTableSubstitution()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TimeTableReport()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TimeTableByClassHistory()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TimeTableByTeacherHistory()
        {
            return View();
        }
    }
}
