using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace SchoolPortal.Web.Controllers
{
    public class LibraryController : Controller
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
        public IActionResult BookCategory()
        {
            return View();
        }

        [HttpGet]                    
        public IActionResult Authors()
        {
            return View();
        }

        [HttpGet]                    
        public IActionResult Publishers()
        {
            return View();
        }

        [HttpGet]                    
        public IActionResult Suppliers()
        {
            return View();
        }

        [HttpGet]                    
        public IActionResult ViewBooks()
        {
            return View();
        }

        [HttpGet]                    
        public IActionResult ViewTransactions()
        {
            return View();
        }
    }
}
