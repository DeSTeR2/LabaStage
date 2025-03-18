using HospitalDomain;
using HospitalDomain.Model;
using HospitalDomain.Utils;
using HospitalMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace HospitalMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HospitalContext _hospitalContext;
        private readonly UserManager<User> _userManager;

        public HomeController(HospitalContext hospitalContext, UserManager<User> userManager)
        {
            _hospitalContext = hospitalContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User != null && User.Identity.IsAuthenticated)
            {
                var viewModel = new HospitalMVC.ViewModels.HomeIndexViewModel();
                User user = await _userManager.GetUserAsync(User);
                if (User.IsInRole(Constants.Doctor))
                {
                    Doctor doctor = _hospitalContext.Doctors.First(d => d.Email == user.Email);
                    var appointments = _hospitalContext.Appointments
                         .Include(a => a.PatientNavigation)
                        .Include(a => a.DoctorNavigation)
                        .Include(a => a.RoomNavigation)
                        .Where(a => a.Doctor == doctor.Id).ToList();
                    viewModel.Appointments = appointments;
                    return View(viewModel);
                } 
                else if (User.IsInRole(Constants.User))
                {
                    Patient patient = _hospitalContext.Patients.First(d => d.Email == user.Email);
                    var appointments = _hospitalContext.Appointments
                        .Include(a => a.PatientNavigation)
                        .Include(a => a.DoctorNavigation)
                        .Include(a => a.RoomNavigation)
                        .Where(a => a.Patient == patient.Id).ToList();

                    viewModel.Appointments = appointments;
                    return View(viewModel);
                }
                else
                {
                    viewModel.Appointments = _hospitalContext.Appointments
                        .Include(a => a.PatientNavigation)
                        .Include(a => a.DoctorNavigation)
                        .Include(a => a.RoomNavigation)
                        .ToList();
                    View(viewModel);
                }
            }
            return View();
        }
        public IActionResult Statistics()
        {

            return View();
        }
    }
}
