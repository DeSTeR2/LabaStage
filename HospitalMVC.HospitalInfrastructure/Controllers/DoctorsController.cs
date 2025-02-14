using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HospitalDomain.Model;

namespace HospitalMVC.HospitalInfrastructure.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly HospitalContext _context;

        public DoctorsController(HospitalContext context)
        {
            _context = context;
        }

        // GET: Doctors
        public async Task<IActionResult> Index()
        {
            var doctors = _context.Doctors.Include(d => d.DepartmentNavigation);
            return View(await doctors.ToListAsync());
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            ViewData["Department"] = new SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        // POST: Doctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Speciality,Contact,Department")] Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Department"] = new SelectList(_context.Departments, "Id", "Name", doctor.Department);
                return View(doctor);
            }

            _context.Add(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound();

            ViewData["Department"] = new SelectList(_context.Departments, "Id", "Name", doctor.Department);
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Speciality,Contact,Department")] Doctor doctor)
        {
            Console.WriteLine($"Edit Called - ID: {id}, Name: {doctor.Name}, Speciality: {doctor.Speciality}");

            if (id != doctor.Id)
            {
                Console.WriteLine("Error: Doctor ID mismatch!");
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid!");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
                ViewData["Department"] = new SelectList(_context.Departments, "Id", "Name", doctor.Department);
                return View(doctor);
            }

            try
            {
                var existingDoctor = await _context.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
                if (existingDoctor == null)
                {
                    Console.WriteLine("Error: Doctor not found!");
                    return NotFound();
                }

                _context.Doctors.Update(doctor);
                await _context.SaveChangesAsync();
                Console.WriteLine("✅ Doctor updated successfully!");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(doctor.Id))
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


        // GET: Doctors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var doctor = await _context.Doctors.Include(d => d.DepartmentNavigation)
                                               .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }
    }
}
