using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;

namespace SchoolPortal.Web.Controllers
{
    public class StudentsController : Controller
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
        public IActionResult ViewParents()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ViewAttendence()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DeleteAttendence()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CommitAttendence()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ViewAttendenceHistory()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ViewAbsenteeList()
        {
               return View();
        }

        [HttpGet]
        public IActionResult ViewStudentHistory()
        {
            return View();
        }
    }
}
