using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HospitalDomain.Model;

namespace HospitalMVC.HospitalInfrastructure.Controllers
{
    public class RoomsController : Controller
    {
        private readonly HospitalContext _context;

        public RoomsController(HospitalContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            return View(await _context.Rooms.Include(r => r.AppointmentNavigation).ToListAsync());
        }

        // GET: Rooms/Create
        public IActionResult Create()
        {
            ViewData["Appointment"] = new SelectList(_context.Appointments, "Id", "Id");
            return View();
        }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Type,Capacity,Availability,Appointment")] Room room)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Appointment"] = new SelectList(_context.Appointments, "Id", "Id", room.Appointment);
                return View(room);
            }

            _context.Add(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return NotFound();

            ViewData["Appointment"] = new SelectList(_context.Appointments, "Id", "Id", room.Appointment);
            return View(room);
        }

        // POST: Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Capacity,Availability,Appointment")] Room room)
        {
            if (id != room.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(room);

            _context.Entry(room).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
