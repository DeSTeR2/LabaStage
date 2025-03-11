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

        public IActionResult Statistics()
        {
            // Your logic for statistics goes here
            return View();
        }

        // Optionally include your Error action here if needed.
    }
}
