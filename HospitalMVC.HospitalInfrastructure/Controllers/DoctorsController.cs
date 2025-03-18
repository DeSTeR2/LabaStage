using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HospitalDomain.Model;
using Utils;
using Microsoft.Build.Logging;
using Microsoft.AspNetCore.Identity;

namespace HospitalMVC.HospitalInfrastructure.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly HospitalContext _hospitalContext;
        private readonly IdentityContext _identityContext;
        private readonly UserManager<User> _userManager;

        public DoctorsController(HospitalContext context, IdentityContext identityContext, UserManager<User> userManager)
        {
            _hospitalContext = context;
            _identityContext = identityContext;
            _userManager = userManager;
        }

        // GET: Doctors
        public async Task<IActionResult> Index()
        {
            var doctors = _hospitalContext.Doctors.Include(d => d.DepartmentNavigation);

            FillDoctorPhotos(doctors);
            return View(await doctors.ToListAsync());
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            ViewData["Department"] = new SelectList(_hospitalContext.Departments, "Id", "Name");
            return View();
        }

        // POST: Doctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Speciality,Contact,Department")] Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Department"] = new SelectList(_hospitalContext.Departments, "Id", "Name", doctor.Department);
                return View(doctor);
            }

            _hospitalContext.Add(doctor);
            await _hospitalContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var doctor = await _hospitalContext.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound();

            ViewData["Department"] = new SelectList(_hospitalContext.Departments, "Id", "Name", doctor.Department);
            return View(doctor);
        }

/*        [HttpPost("Doctors/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Speciality,Contact,Department")] Doctor doctor)
        {
            if (id != doctor.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["Department"] = new SelectList(_context.Departments, "Id", "Name", doctor.Department);
                return View(doctor);
            }

            try
            {
                _context.Doctors.Update(doctor);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Doctors.Any(d => d.Id == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }*/

        // POST: Departments/UpdateDoctorsDepartment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDoctorsDepartment(int newDepartmentId)
        {
            var doctors = TempData["DoctorsToUpdate"] as List<Doctor>;
            var departmentIdToDelete = TempData["DepartmentIdToDelete"] as int?;

            if (doctors == null || departmentIdToDelete == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (newDepartmentId == 0)
            {
                // Handle error: No department selected
                ModelState.AddModelError("", "You must select a new department.");
                return View("SelectNewDepartment", new SelectNewDepartmentViewModel
                {
                    DoctorSelections = SelectNewDepartmentViewModel.Convert(doctors),
                    Departments = _hospitalContext.Departments.ToList()
                });
            }

            // Update the doctors' department to the new department
            foreach (var doctor in doctors)
            {
                doctor.Department = newDepartmentId;
                _hospitalContext.Doctors.Update(doctor);
            }

            // Delete the old department
            var departmentToDelete = await _hospitalContext.Departments.FindAsync(departmentIdToDelete);
            if (departmentToDelete != null)
            {
                _hospitalContext.Departments.Remove(departmentToDelete);
            }

            await _hospitalContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Speciality,Contact,Department")] Doctor doctor)
        {
            Console.WriteLine($"Edit Called - ID: {id}, Name: {doctor.Name}, Speciality: {doctor.Speciality}");

            ModelState.Remove("DepartmentNavigation");
            ModelState.Remove("UserNavigation");
            ModelState.Remove("Email");
            TryValidateModel(ModelState);

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
                ViewData["Department"] = new SelectList(_hospitalContext.Departments, "Id", "Name", doctor.Department);
                return View(doctor);
            }

            try
            {
                var existingDoctor = await _hospitalContext.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
                if (existingDoctor == null)
                {
                    Console.WriteLine("Error: Doctor not found!");
                    return NotFound();
                }

                // Validate User exists
                var user = _identityContext.Users.FirstOrDefault(u => u.PhoneNumber == doctor.Contact);
                if (user == null)
                {
                    Console.WriteLine("Error: No user found with this phone number!");
                    ModelState.AddModelError("Contact", "No user found with this phone number.");
                    ViewData["Department"] = new SelectList(_hospitalContext.Departments, "Id", "Name", doctor.Department);
                    return View(doctor);
                }

                doctor.Email = user.Email;

                // Verify UserId exists in the same context as Doctors
                var userExists = await _identityContext.Users.AnyAsync(u => u.PhoneNumber == doctor.Contact);
                if (!userExists)
                {
                    ModelState.AddModelError("UserId", "The selected user does not exist in the system.");
                    ViewData["Department"] = new SelectList(_hospitalContext.Departments, "Id", "Name", doctor.Department);
                    return View(doctor);
                }

                _hospitalContext.Doctors.Update(doctor);
                await _hospitalContext.SaveChangesAsync();
                Console.WriteLine("✅ Doctor updated successfully!");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.InnerException?.Message}");
                ModelState.AddModelError("", $"An error occurred while saving the doctor. Please check the data and try again. {ex.InnerException?.Message}");
                ViewData["Department"] = new SelectList(_hospitalContext.Departments, "Id", "Name", doctor.Department);
                return View(doctor);
            }
            catch (Exception)
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

            var doctor = await _hospitalContext.Doctors.Include(d => d.DepartmentNavigation)
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
            var doctor = await _hospitalContext.Doctors.FindAsync(id);
            if (doctor != null)
            {
                _hospitalContext.Doctors.Remove(doctor);
                await _hospitalContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var doctors = await _hospitalContext.Doctors.Include(d => d.DepartmentNavigation)
                                               .FirstOrDefaultAsync(m => m.Id == id);
            if (doctors == null)
                return NotFound();

            return View(doctors);
        }

        public IActionResult SelectDoctor(int? id)
        {
            return RedirectToAction("Create", "Appontemtns");
        }

        public IActionResult DoctorsByDepartment(int? departmentId)
        {
            var doctors = _hospitalContext.Doctors.Include(d => d.DepartmentNavigation);

            FillDoctorPhotos(doctors);

            ViewBag.Departments = _hospitalContext.Departments.ToList();
            ViewBag.SelectedDepartmentId = departmentId;

            return View(doctors.ToList());
        }

        private void FillDoctorPhotos(Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Doctor, Department> doctors)
        {
            if (doctors == null) return;
            foreach (var doctor in doctors)
            {
                var user = _identityContext.Users.FirstOrDefault(u => u.Email == doctor.Email);
                if (user == default)
                {
                    doctor.ProfilePictureUrl = Constants.DefaultProfileImage;
                }
                else
                {
                    doctor.ProfilePictureUrl = Constants.RootProfileImagesPath + user.ProfilePictureUrl ?? Constants.DefaultProfileImage;
                }
            }
        }

        private bool DoctorExists(int id)
        {
            return _hospitalContext.Doctors.Any(e => e.Id == id);
        }
    }
}
