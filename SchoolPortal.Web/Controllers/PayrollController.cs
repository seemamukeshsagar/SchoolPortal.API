using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace SchoolPortal.Web.Controllers
{
    public class PayrollController : Controller
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
        public IActionResult ViewDepartment()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ViewDesignation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ViewPaymentModes()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ViewGrades()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ViewEmployeeTypes()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ViewEmployeeCategory()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ViewEmployeeList()
        {
            return View();
        }
    }
}
