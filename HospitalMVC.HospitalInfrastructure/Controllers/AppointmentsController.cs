using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HospitalDomain.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using LibraryWebApplication.Controllers;

namespace HospitalMVC.HospitalInfrastructure.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly HospitalContext _hospitalContext;
        private readonly IdentityContext _identityContext;

        public AppointmentsController(HospitalContext context, IdentityContext identityContext)
        {
            _hospitalContext = context;
            _identityContext = identityContext;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            try
            {
                AccountController accountController = AccountController.GetInstance();

                User user = await accountController.GetAuthorizedUser();
                if (await accountController.IsUserInRole(user, "admin")) {
                    var hospitalContext = _hospitalContext.Appointments
                        .Include(a => a.PatientNavigation)
                        .Include(a => a.DoctorNavigation)
                        .Include(a => a.RoomNavigation);
                        return View(await hospitalContext.ToListAsync());
                } else
                {
                    string email = user.Email;

                    var patient = await _hospitalContext.Patients
                        .FirstOrDefaultAsync(p => p.Email == email);

                    var hospitalContext = _hospitalContext.Appointments
                        .Include(a => a.PatientNavigation)
                        .Include(a => a.DoctorNavigation)
                        .Include(a => a.RoomNavigation)
                        .Where(a => a.PatientNavigation.Email == email);  

                    return View(await hospitalContext.ToListAsync());

                }
            }
            catch
            {
                return default;
            }
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

        // GET: Appointments/Create
        public IActionResult Create()
        {
            // You don't need to create a new Appointment instance here
            // Instead, you just want to pass an empty Appointment model if needed
            var appointment = new Appointment();  // Still passing an empty Appointment model if needed

            // Filter available doctors (those who are not booked for the selected date and time, for the full 1-hour appointment)
            var availableDoctors = _hospitalContext.Doctors
                .Where(d => !_hospitalContext.Appointments
                    .Any(a => a.Doctor == d.Id && a.Date == appointment.Date &&
                              a.Time < appointment.Time.AddHours(1) && a.Time.AddHours(1) > appointment.Time))
                .ToList();

            ViewData["Doctor"] = new SelectList(availableDoctors, "Id", "Name");

            // Filter available rooms (those which are not booked for the selected date and time, for the full 1-hour appointment)
            var availableRooms = _hospitalContext.Rooms
                .Where(r => !_hospitalContext.Appointments
                    .Any(a => a.Room == r.Id && a.Date == appointment.Date &&
                              a.Time < appointment.Time.AddHours(1) && a.Time.AddHours(1) > appointment.Time))
                .ToList();

            ViewData["Room"] = new SelectList(availableRooms, "Id", "Type");

            // Filter patients (no changes to this part)
            ViewData["Patient"] = new SelectList(
                _hospitalContext.Patients.Select(p => new { p.Id, Name = p.Name + " (" + p.Contacts + ")" }),
                "Id", "Name");

            return View(appointment);  // Pass the empty appointment model to the view
        }




        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Time,Reason,Doctor,Patient,Room")] Appointment appointment)
        {
            // Check if the selected doctor and room are available at the selected date and time
            var doctorAvailable = !_hospitalContext.Appointments
                .Any(a => a.Doctor == appointment.Doctor && a.Date == appointment.Date &&
                          a.Time < appointment.Time.AddHours(1) && a.Time.AddHours(1) > appointment.Time);

            var roomAvailable = !_hospitalContext.Appointments
                .Any(a => a.Room == appointment.Room && a.Date == appointment.Date &&
                          a.Time < appointment.Time.AddHours(1) && a.Time.AddHours(1) > appointment.Time);

            if (!doctorAvailable || !roomAvailable)
            {
                ModelState.AddModelError("", "The selected doctor or room is not available at the specified time.");
                // Re-fetch available doctors and rooms if the selected ones are unavailable
                ViewData["Doctor"] = new SelectList(
                    _hospitalContext.Doctors.Where(d => !_hospitalContext.Appointments
                        .Any(a => a.Doctor == d.Id && a.Date == appointment.Date &&
                                  a.Time < appointment.Time.AddHours(1) && a.Time.AddHours(1) > appointment.Time))
                    .ToList(), "Id", "Name");

                ViewData["Room"] = new SelectList(
                    _hospitalContext.Rooms.Where(r => !_hospitalContext.Appointments
                        .Any(a => a.Room == r.Id && a.Date == appointment.Date &&
                                  a.Time < appointment.Time.AddHours(1) && a.Time.AddHours(1) > appointment.Time))
                    .ToList(), "Id", "Type");

                ViewData["Patient"] = new SelectList(
                    _hospitalContext.Patients.Select(p => new { p.Id, Name = p.Name + " (" + p.Contacts + ")" }),
                    "Id", "Name");

                return View(appointment); // Return the view with error and keep the selected data
            }

            // If doctor and room are available, proceed with appointment creation
            _hospitalContext.Add(appointment);
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
