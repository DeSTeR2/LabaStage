using Microsoft.AspNetCore.Mvc;

namespace HospitalMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Return the Index view that will contain the links.
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Optionally include your Error action here if needed.
    }
}
