using Microsoft.AspNetCore.Mvc;

namespace SchoolPortal.Web.Controllers
{
    public class InventoryController : Controller
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
        public IActionResult ItemTypes()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ItemLocation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ViewInventory()
        {
            return View();
        }
    }
}
