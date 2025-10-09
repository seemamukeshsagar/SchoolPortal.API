using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;

namespace SchoolPortal.Web.Controllers
{
    public class TransportController : Controller
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
        public IActionResult Locations()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Drivers()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Cleaners()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Routes()
        {
            return View();
        }
    }
}
