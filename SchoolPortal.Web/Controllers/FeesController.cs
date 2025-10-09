using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using static System.Collections.Specialized.NameObjectCollectionBase;

namespace SchoolPortal.Web.Controllers
{
    public class FeesController : Controller
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
        public IActionResult GenerateData()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult ViewPayments()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult CommitFees()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult FeesHistory()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult FeesCollection()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Receipts()
        {
            return View();
        }
    }
}
