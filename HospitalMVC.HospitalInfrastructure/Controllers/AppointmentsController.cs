using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HospitalDomain.Model;
using Microsoft.AspNetCore.Identity;
using Utils;
using Newtonsoft.Json;
using System.Text.Json;

namespace HospitalMVC.HospitalInfrastructure.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly HospitalContext _hospitalContext;
        private readonly IdentityContext _identityContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AppointmentsController(
            HospitalContext context,
            IdentityContext identityContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _hospitalContext = context;
            _identityContext = identityContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            IQueryable<Appointment> hospitalContext = null!;

            if (CheckRole.IsInRoles(User, new string[] { Constants.Admin, Constants.Manager }))
            {
                hospitalContext = _hospitalContext.Appointments
                    .Include(a => a.PatientNavigation)
                    .Include(a => a.DoctorNavigation)
                    .Include(a => a.RoomNavigation);
            }
            else if (await _userManager.IsInRoleAsync(user, Constants.Doctor))
            {
                string email = user.Email;

                var patient = await _hospitalContext.Doctors
                    .FirstOrDefaultAsync(p => p.Email == email);

                hospitalContext = _hospitalContext.Appointments
                    .Include(a => a.PatientNavigation)
                    .Include(a => a.DoctorNavigation)
                    .Include(a => a.RoomNavigation)
                    .Where(a => a.PatientNavigation.Email == email);
            }
            else if (await _userManager.IsInRoleAsync(user, Constants.User))
            {
                string email = user.Email;

                var patient = await _hospitalContext.Patients
                    .FirstOrDefaultAsync(p => p.Email == email);

                hospitalContext = _hospitalContext.Appointments
                    .Include(a => a.PatientNavigation)
                    .Include(a => a.DoctorNavigation)
                    .Include(a => a.RoomNavigation)
                    .Where(a => a.PatientNavigation.Email == email);

            }

            if (hospitalContext == null)
            {
                RedirectToAction("Login", "Account");
            }

            return View(await hospitalContext.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _hospitalContext.Appointments
                .Include(a => a.DoctorNavigation)
                .Include(a => a.PatientNavigation)
                .Include(a => a.RoomNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        public async Task<IActionResult> SelectDoctor()
        {
            return RedirectToAction("DoctorsByDepartment", "Doctors");
        }
        public async Task<IActionResult> Create(int? doctroId) // Typo: "doctroId" should be "doctorId"
        {
            var appointment = new Appointment();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fix typo in parameter name (optional, for consistency)
            if (!doctroId.HasValue)
            {
                return BadRequest("Doctor ID is required.");
            }

            var doctor = _hospitalContext.Doctors.FirstOrDefault(d => d.Id == doctroId);
            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }

            ViewBag.DoctorId = doctor.Id;
            appointment.Doctor = doctor.Id;
            var closedDates = GetClossedDatesForDoctor(doctor.Id);
            string json = JsonConvert.SerializeObject(closedDates.Select(d => d.ToString("yyyy-MM-dd")).ToArray());

            ViewBag.PossibleDatesJson = json;

            DateTime dateTime = DateTime.Today;
            DateOnly currentDay = new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
            ViewBag.SelectedDate = currentDay;
            if (Utils.CheckRole.IsInRoles(User, new string[] { Constants.Admin, Constants.Manager }))
            {
                ViewBag.Patient = new SelectList(
                    _hospitalContext.Patients.Select(p => new { p.Id, Name = p.Name + " (" + p.Contacts + ")" }),
                    "Id", "Name");

                ViewBag.Room = new SelectList(_hospitalContext.Rooms, "Id", "Type");

                return View(appointment);
            }
            else if (Utils.CheckRole.IsInRoles(User, new string[] { Constants.User }))
            {
                var patient = _hospitalContext.Patients
                    .First(p => p.Email == user.Email);
                ViewBag.Patient = patient.Id;

                return View(appointment);
            }

            return RedirectToAction("Index", "Home");
        }

        public List<DateOnly> GetClossedDatesForDoctor(int doctorId)
        {
            var doctor = _hospitalContext.Doctors.First(d => d.Id == doctorId);
            var appointments = _hospitalContext.Appointments
                .Where(a => a.Doctor == doctorId)
                .GroupBy(a => a.Date)
                .ToList();

            int workingHours = Constants.EndWork - Constants.StartWork - 1;
            List<DateOnly> closedDates = new();

            foreach (var group in appointments)
            {
                int dateNumber = group.Count();
                if (dateNumber >= workingHours)
                {
                    closedDates.Add(group.Key);
                }
            }
            
            return closedDates;
        }

        [HttpGet]
        public JsonResult GetPossibleTimesForDoctor(int doctorId, DateOnly date)
        {
            var possibleTimes = GetAllWorkingHours();

            var dateTimes = _hospitalContext.Appointments
                .Where(a => a.Doctor == doctorId && a.Date == date)
                .Select(a => a.Time)
                .ToList();
            for (int i=0; i<dateTimes.Count; i++)
            {
                possibleTimes.Remove(dateTimes[i]);
            }

            List<SelectListItem> selectTimes = new List<SelectListItem>();
            for (int i=0; i<possibleTimes.Count;i++)
            {
                selectTimes.Add(new SelectListItem()
                {
                    Value = possibleTimes[i].ToString("hh:mm"),
                    Text = possibleTimes[i].ToString()
                });
            }   

            JsonResult result = Json(selectTimes);
            return result;
        }

        public List<TimeOnly> GetAllWorkingHours()
        {
            var possibleTimes = new List<TimeOnly>();
            for (int i=Constants.StartWork; i<Constants.EndWork; i++)
            {
                int hours = i;
                int minutes = 0;
                TimeOnly date = new TimeOnly(hours, minutes);
                possibleTimes.Add(date);
            }

            return possibleTimes;
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Time,Reason,Doctor,Patient,Room")] Appointment appointment)
        {
            var indexes = _hospitalContext.Appointments.Select(a => a.Id).OrderBy(id => id).ToList();
            appointment.Id = Utils.Util.GetId(indexes);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string email = user.Email;

            if (!User.IsInRole("admin"))
            {
                var patient = await _hospitalContext.Patients
                    .FirstOrDefaultAsync(p => p.Email == email);

                if (patient == null)
                {
                    ModelState.AddModelError("", "Patient not found.");
                    return View(appointment); // Handle the case where the patient is not found
                }

                appointment.Patient = patient.Id;
            }

            appointment.DoctorNavigation = _hospitalContext.Doctors.First(d => d.Id == appointment.Doctor);
            appointment.PatientNavigation = _hospitalContext.Patients.First(d => d.Id == appointment.Patient);

            if (appointment.Room != 0 && appointment.Room != null)
            {
                appointment.RoomNavigation = _hospitalContext.Rooms.First(d => d.Id == appointment.Room);
            } else
            {
                appointment.RoomNavigation = null;
                appointment.Room = null;
            }

            _hospitalContext.Appointments.Add(appointment);
            await _hospitalContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _hospitalContext.Appointments
                .Include(a => a.DoctorNavigation)
                .Include(a => a.PatientNavigation)
                .Include(a => a.RoomNavigation)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            // Filter available doctors (those who are not booked for the selected date and time, for the full 1-hour appointment, excluding the current doctor's appointment)
            ViewData["Doctor"] = new SelectList(
                _hospitalContext.Doctors.Where(d => !_hospitalContext.Appointments
                    .Any(a => a.Doctor == d.Id && a.Date == appointment.Date &&
                              a.Time < appointment.Time.AddHours(1) && a.Time.AddHours(1) > appointment.Time) || d.Id == appointment.Doctor),
                "Id", "Name");

            // Filter available rooms (those which are not booked for the selected date and time, for the full 1-hour appointment, excluding the current room)
            ViewData["Room"] = new SelectList(
                _hospitalContext.Rooms.Where(r => !_hospitalContext.Appointments
                    .Any(a => a.Room == r.Id && a.Date == appointment.Date &&
                              a.Time < appointment.Time.AddHours(1) && a.Time.AddHours(1) > appointment.Time) || r.Id == appointment.Room),
                "Id", "Type");

            // Filter patients
            ViewData["Patient"] = new SelectList(
                _hospitalContext.Patients.Select(p => new { p.Id, Name = p.Name + " (" + p.Contacts + ")" }),
                "Id", "Name");

            return View(appointment);
        }



        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Time,Reason,Doctor,Patient,Room")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            appointment.PatientNavigation = await _hospitalContext.Patients.FindAsync(appointment.Patient);
            appointment.DoctorNavigation = await _hospitalContext.Doctors.FindAsync(appointment.Doctor);
            appointment.RoomNavigation = await _hospitalContext.Rooms.FindAsync(appointment.Room);

            ModelState.Remove("DoctorNavigation");
            ModelState.Remove("PatientNavigation");
            ModelState.Remove("RoomNavigation");

            TryValidateModel(ModelState);

            if (ModelState.IsValid)
            {
                try
                {
                    _hospitalContext.Update(appointment);
                    await _hospitalContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(modelError.ErrorMessage);
                }

            }
            ViewData["Doctor"] = new SelectList(_hospitalContext.Doctors, "Id", "Contact", appointment.DoctorNavigation.Name);
            ViewData["Patient"] = new SelectList(_hospitalContext.Patients, "Id", "Contacts", appointment.PatientNavigation.Name);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _hospitalContext.Appointments
                .Include(a => a.DoctorNavigation)
                .Include(a => a.PatientNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _hospitalContext.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _hospitalContext.Appointments.Remove(appointment);
            }

            await _hospitalContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _hospitalContext.Appointments.Any(e => e.Id == id);
        }
    }
}
