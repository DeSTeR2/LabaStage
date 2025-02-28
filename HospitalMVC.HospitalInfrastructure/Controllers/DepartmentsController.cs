using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalDomain.Model;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.CodeAnalysis.Operations;

namespace HospitalMVC.HospitalInfrastructure.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly HospitalContext _context;

        public DepartmentsController(HospitalContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Departments.ToListAsync());
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Location")] Department department)
        {
            if (!ModelState.IsValid)
            {
                return View(department);
            }

            var ids = await _context.Departments
                .Select(m => m.Id)
                .OrderBy(id => id)
                .ToListAsync();

            department.Id = Utils.Util.GetId(ids);
            _context.Add(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(department);
            }

            try
            {
                _context.Attach(department);
                _context.Entry(department).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(department.Id))
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

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department != null)
            {
                // Check if any doctors are dependent on this department
                var doctors = await _context.Doctors.Where(d => d.Department == id).ToListAsync();

                if (doctors.Any())
                {
                    // Store only the doctor IDs in TempData
                    TempData["DoctorIdsToUpdate"] = doctors.Select(d => d.Id).ToList();
                    TempData["DepartmentIdToDelete"] = department.Id;

                    return RedirectToAction("SelectNewDepartment", "Departments");
                }
                else
                {
                    // If no doctors are dependent, just delete the department
                    _context.Departments.Remove(department);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }



        // GET: Departments/SelectNewDepartment
        public async Task<IActionResult> SelectNewDepartment()
        {
            var doctorIds = TempData["DoctorIdsToUpdate"] as IList<int>;
            var departmentIdToDelete = TempData["DepartmentIdToDelete"] as int?;

            if (doctorIds == null || departmentIdToDelete == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Retrieve the doctors from the database by their IDs
            var doctors = _context.Doctors.Where(d => doctorIds.Contains(d.Id)).ToList();
            var departments = _context.Departments.ToList(); // Get all departments for selection

            var model = new SelectNewDepartmentViewModel
            {
                DoctorSelections = SelectNewDepartmentViewModel.Convert(doctors),
                Departments = departments
            };

            return View("~/Views/Departments/SelectNewDepartment.cshtml", model);

        }

        // POST: Departments/SelectNewDepartment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectNewDepartment(SelectNewDepartmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If the model state is invalid, return the model to the view to show validation errors
                return View(model);
            }

            var departmentIdToDelete = TempData["DepartmentIdToDelete"] as int?;

            if (departmentIdToDelete == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Update the selected doctors' department
            foreach (var doctorSelection in model.DoctorSelections.Where(d => d.IsSelected))
            {
                var doctor = await _context.Doctors.FindAsync(doctorSelection.Id);
                if (doctor != null)
                {
                    doctor.Department = model.NewDepartmentId; // Update the doctor to the new department
                }
            }

            // Save the changes
            await _context.SaveChangesAsync();

            // Now delete the department
            var department = await _context.Departments.FindAsync(departmentIdToDelete);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)); // Redirect back to the departments list
        }




        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
                return NotFound();

            return View(department);
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
