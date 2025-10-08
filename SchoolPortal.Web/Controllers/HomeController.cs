using Microsoft.AspNetCore.Mvc;

namespace SchoolPortal.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
