using Microsoft.AspNetCore.Mvc;

namespace SchoolPortal.Web.Controllers
{
    public class LeavesController : Controller
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
        public IActionResult ViewCategory()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GenerateLeaveData()
        {
            return View();
        }        
    }
}
