using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Diagnostics;

namespace SchoolPortal.Web.Controllers
{
    public class ReportsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult FeesDefaulters()
        {
            return View();
        }

        public IActionResult StudentICards()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ICards()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GendorReport()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Attendence()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AttendenceSummary()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AgeGradeReport()
        {
            return View();
        }

        [HttpGet]
        public IActionResult StudentSummary()
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
        public IActionResult TimeTableByAllTeachers()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TimeTableBySchool()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TimeTableBySchoolSummary()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ViewTeachersReport()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ViewTeacherDetails()
        {
            return View();
        }
    }
}
