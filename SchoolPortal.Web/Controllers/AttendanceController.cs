using Microsoft.AspNetCore.Mvc;
using System;

namespace SchoolPortal.Web.Controllers
{
    public class AttendanceController : Controller
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
        public IActionResult Delete()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Commit()
        {
            return View();
        }

        [HttpGet]
        public IActionResult History()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Report()
        {
            return View();
        }
    }
}
