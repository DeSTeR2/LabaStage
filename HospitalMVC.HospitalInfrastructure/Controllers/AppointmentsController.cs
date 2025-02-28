using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HospitalDomain.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HospitalMVC.HospitalInfrastructure.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly HospitalContext _context;

        public AppointmentsController(HospitalContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            try
            {
                var hospitalContext = _context.Appointments.Include(a => a.PatientNavigation).Include(a => a.DoctorNavigation);
                return View(await hospitalContext.ToListAsync());
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

            var appointment = await _context.Appointments
                .Include(a => a.DoctorNavigation)
                .Include(a => a.PatientNavigation)
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
            ViewData["Doctor"] = new SelectList(
                _context.Doctors.Select(d => new { d.Id, Name = d.Name + " (" + d.Contact + ")" }),
                "Id", "Name"
            );

            ViewData["Patient"] = new SelectList(
                _context.Patients.Select(p => new { p.Id, Name = p.Name + " (" + p.Contacts + ")" }),
                "Id", "Name"
            );
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Time,Reason,Doctor,Patient")] Appointment appointment)
        {
            int id = Utils.Util.GetId(
                _context.Appointments
                .Select(a => a.Id)
                .OrderBy(id => id)
                .ToList()
                );

            appointment.Id = id;
            appointment.PatientNavigation = await _context.Patients.FindAsync(appointment.Patient);
            appointment.DoctorNavigation = await _context.Doctors.FindAsync(appointment.Doctor);

            ModelState.Remove("Id");
            ModelState.Remove("DoctorNavigation");
            ModelState.Remove("PatientNavigation");

            TryValidateModel(ModelState);

            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Doctor"] = new SelectList(
                _context.Doctors.Select(d => new { d.Id, Name = d.Name + " (" + d.Contact + ")" }),
                "Id", "Name", appointment.Doctor
            );

            ViewData["Patient"] = new SelectList(
                _context.Patients.Select(p => new { p.Id, Name = p.Name + " (" + p.Contacts + ")" }),
                "Id", "Name", appointment.Patient
            );

            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = _context.Appointments
                .Include(a => a.DoctorNavigation)
                .Include(a => a.PatientNavigation)
                .ToList()
                .Find(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            ViewData["Doctor"] = new SelectList(
                _context.Doctors.Select(d => new { d.Id, Name = d.Name + " (" + d.Contact + ")" }),
                "Id", "Name", appointment.Doctor
            );

            ViewData["Patient"] = new SelectList(
                _context.Patients.Select(p => new { p.Id, Name = p.Name + " (" + p.Contacts + ")" }),
                "Id", "Name", appointment.Patient
            );

            return View(appointment);
        }


        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Time,Reason,Doctor,Patient")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            appointment.PatientNavigation = await _context.Patients.FindAsync(appointment.Patient);
            appointment.DoctorNavigation = await _context.Doctors.FindAsync(appointment.Doctor);

            ModelState.Remove("DoctorNavigation");
            ModelState.Remove("PatientNavigation");

            TryValidateModel(ModelState);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
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
            ViewData["Doctor"] = new SelectList(_context.Doctors, "Id", "Contact", appointment.DoctorNavigation.Name);
            ViewData["Patient"] = new SelectList(_context.Patients, "Id", "Contacts", appointment.PatientNavigation.Name);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
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
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
