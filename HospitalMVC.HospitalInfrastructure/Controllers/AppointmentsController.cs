using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HospitalDomain.Model;
using Microsoft.AspNetCore.Identity;
using Utils;
using Newtonsoft.Json;
using System.Text.Json;
using HospitalDomain.Utils;
using HospitalDomain.MailService;
using AspNetCoreGeneratedDocument;
using System.ComponentModel;
using DinkToPdf;
using Mono.TextTemplating;
using HospitalMVC.HospitalInfrastructure.Services.PdfService;
using DinkToPdf.Contracts;

namespace HospitalMVC.HospitalInfrastructure.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly HospitalContext _hospitalContext;
        private readonly IdentityContext _identityContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMailService _mailService;
        private readonly IConverter _converter;
        private readonly IWebHostEnvironment webHostEnvironment;

        public AppointmentsController(
            HospitalContext context,
            IdentityContext identityContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IMailService mailService,
            IConverter converter,
            IWebHostEnvironment webHostEnvironment)
        {
            _hospitalContext = context;
            _identityContext = identityContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _mailService = mailService;
            _converter = converter;
            this.webHostEnvironment = webHostEnvironment;
        }

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
                    .Where(a => a.DoctorNavigation.Email == email);
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

            List<AppointmentChangeHistoryModel> changes = await _hospitalContext
                .AppointmentChanges
                .Where(a => a.AppointmentId == id)
                .OrderByDescending(a => a.ChangeTime)
                .ToListAsync();

            ViewBag.ChangeHistory = changes;

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
            closedDates.Add(DateOnly.FromDateTime(DateTime.Now));

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
                .Where(a => a.Doctor == doctorId && a.Date == date && a.AppointmentState != 5)
                .Select(a => a.Time)
                .ToList();
            for (int i = 0; i < dateTimes.Count; i++)
            {
                possibleTimes.Remove(dateTimes[i]);
            }

            List<SelectListItem> selectTimes = new List<SelectListItem>();
            for (int i = 0; i < possibleTimes.Count; i++)
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
            for (int i = Constants.StartWork; i < Constants.EndWork; i++)
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
            AppointmentChangeHistoryModel historyModel = new AppointmentChangeHistoryModel(
                appointment,
                _hospitalContext,
                Constants.CreatedAppointment,
                User
            );

            await _hospitalContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!id.HasValue)
            {
                return BadRequest("Appointment ID is required.");
            }

            var appointment = await _hospitalContext.Appointments
                .Include(a => a.DoctorNavigation)
                .Include(a => a.PatientNavigation)
                .Include(a => a.RoomNavigation)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound("Appointment not found.");
            }

            var doctor = _hospitalContext.Doctors.FirstOrDefault(d => d.Id == appointment.Doctor);
            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }

            // Set up ViewBag for the datepicker and doctor
            ViewBag.DoctorId = doctor.Id;
            var closedDates = GetClossedDatesForDoctor(doctor.Id); // Assuming this method exists
            string json = JsonConvert.SerializeObject(closedDates.Select(d => d.ToString("yyyy-MM-dd")).ToArray());
            ViewBag.PossibleDatesJson = json;

            // Set the selected date for the view
            DateTime dateTime = appointment.Date.ToDateTime(TimeOnly.MinValue); // Convert DateOnly to DateTime
            DateOnly currentDay = DateOnly.FromDateTime(dateTime);
            ViewBag.SelectedDate = currentDay;

            //ViewBag.PossibleTimes = GetPossibleTimesForDoctor(doctor.Id, appointment.Date); // Optional, if you want initial times

            if (Utils.CheckRole.IsInRoles(User, new string[] { Constants.Admin, Constants.Manager }))
            {
                ViewBag.Patient = new SelectList(
                    _hospitalContext.Patients.Select(p => new { p.Id, Name = p.Name + " (" + p.Contacts + ")" }),
                    "Id", "Name", appointment.Patient);

                ViewBag.Room = new SelectList(_hospitalContext.Rooms, "Id", "Type", appointment.Room);
            }
            else if (Utils.CheckRole.IsInRoles(User, new string[] { Constants.User, Constants.Doctor }))
            {
                var patient = _hospitalContext.Patients.FirstOrDefault(p => p.Email == user.Email);
                if (patient == null)
                {
                    return NotFound("Patient not found.");
                }

                ViewBag.Patient = new SelectList(
                new List<object> { new { Id = patient.Id, Name = patient.Name } },
                "Id", "Name", patient.Id);
            }
            else
                return RedirectToAction("Index", "Home");

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

            ModelState.Remove("DoctorNavigation");
            ModelState.Remove("PatientNavigation");
            ModelState.Remove("RoomNavigation");

            TryValidateModel(ModelState);

            if (!ModelState.IsValid)
            {
                // Populate ViewData for the dropdowns in case of validation failure
                ViewData["Doctor"] = new SelectList(_hospitalContext.Doctors, "Id", "Contact", appointment.Doctor);
                ViewData["Patient"] = new SelectList(_hospitalContext.Patients, "Id", "Contacts", appointment.Patient);
                return View(appointment);
            }

            try
            {
                var existingAppointment = await _hospitalContext.Appointments
                    .Include(a => a.DoctorNavigation)
                    .Include(a => a.PatientNavigation)
                    .Include(a => a.RoomNavigation)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (existingAppointment == null)
                {
                    return NotFound();
                }

                FindChangesAndCreateHistory(appointment, existingAppointment);
                _hospitalContext.Entry(existingAppointment).CurrentValues.SetValues(appointment);

                existingAppointment.Doctor = appointment.Doctor;
                existingAppointment.Patient = appointment.Patient;
                existingAppointment.Room = appointment.Room;


                // Save changes
                await _hospitalContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
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
        }

        private void FindChangesAndCreateHistory(Appointment appointment, Appointment oldAppointment)
        {
            string changes = "";
            if (appointment.Time != oldAppointment.Time)
            {
                AppointmentChangeHistoryModel historyModel = new AppointmentChangeHistoryModel(
                    appointment,
                    _hospitalContext,
                    AppointmentHistoryAssigner.GetTransformedString(Constants.ChangeTime, oldAppointment.Time, appointment.Time),
                    User
                );

                changes += historyModel.ChangeInfo + "\n";
            }
            if (appointment.Date != oldAppointment.Date)
            {
                AppointmentChangeHistoryModel historyModel = new AppointmentChangeHistoryModel(
                    appointment,
                    _hospitalContext,
                    AppointmentHistoryAssigner.GetTransformedString(Constants.ChangeDate, oldAppointment.Date, appointment.Date),
                    User
                );
                changes += historyModel.ChangeInfo + "\n";
            }
            if (appointment.Doctor != oldAppointment.Doctor)
            {
                string oldDoctorName = _hospitalContext.Doctors.First(d => d.Id == oldAppointment.Doctor).Name;
                string newDoctorName = _hospitalContext.Doctors.First(d => d.Id == appointment.Doctor).Name;

                AppointmentChangeHistoryModel historyModel = new AppointmentChangeHistoryModel(
                    appointment,
                    _hospitalContext,
                    AppointmentHistoryAssigner.GetTransformedString(Constants.ChangeDoctor, oldDoctorName, newDoctorName),
                    User
                );
                changes += historyModel.ChangeInfo + "\n";
            }
            if (appointment.Reason != oldAppointment.Reason)
            {
                AppointmentChangeHistoryModel historyModel = new AppointmentChangeHistoryModel(
                    appointment,
                    _hospitalContext,
                    AppointmentHistoryAssigner.GetTransformedString(Constants.ChangedReason, oldAppointment.Reason, appointment.Reason),
                    User
                );
                changes += historyModel.ChangeInfo + "\n";
            }
            if (appointment.Room != oldAppointment.Room)
            {
                if (oldAppointment.Room != 0)
                {
                    Room oldRoomName = _hospitalContext.Rooms.FirstOrDefault(d => d.Id == oldAppointment.Room);
                    Room newRoomName = _hospitalContext.Rooms.FirstOrDefault(d => d.Id == appointment.Room);

                    if (oldRoomName != default && newRoomName != default)
                    {

                        AppointmentChangeHistoryModel historyModel = new AppointmentChangeHistoryModel(
                            appointment,
                            _hospitalContext,
                            AppointmentHistoryAssigner.GetTransformedString(Constants.ChangedRoom, oldRoomName.Type, newRoomName.Type),
                            User
                        );
                        changes += historyModel.ChangeInfo + "\n";
                    }
                }
                else
                {
                    Room newRoomName = _hospitalContext.Rooms.FirstOrDefault(d => d.Id == appointment.Room);

                    if (newRoomName != default)
                    {
                        AppointmentChangeHistoryModel historyModel = new AppointmentChangeHistoryModel(
                            appointment,
                            _hospitalContext,
                            AppointmentHistoryAssigner.GetTransformedString(Constants.SetRoom, "", newRoomName.Type),
                            User
                        );
                        changes += historyModel.ChangeInfo + "\n";
                    }
                }
            }
            if (appointment.Patient != oldAppointment.Patient)
            {
                string oldPatientName = _hospitalContext.Patients.First(d => d.Id == oldAppointment.Patient).Name;
                string newPatientName = _hospitalContext.Patients.First(d => d.Id == appointment.Patient).Name;

                AppointmentChangeHistoryModel historyModel = new AppointmentChangeHistoryModel(
                    appointment,
                    _hospitalContext,
                    AppointmentHistoryAssigner.GetTransformedString(Constants.ChangedPatient, oldPatientName, newPatientName),
                    User
                );
                changes += historyModel.ChangeInfo + "\n";
            }

            Patient patient = _hospitalContext.Patients.First(a => a.Id == appointment.Patient);
            Doctor doctor = _hospitalContext.Doctors.First(a => a.Id == appointment.Doctor);

            changes = appointment.ToString() + changes;

            SentMail("Appointment changes", changes, appointment);
        }

        private void SentMail(string subject, string message, Appointment appointment)
        {
            Patient patient = _hospitalContext.Patients.First(a => a.Id == appointment.Patient);
            Doctor doctor = _hospitalContext.Doctors.First(a => a.Id == appointment.Doctor);
            _mailService.SentMail(new Mail()
            {
                message = message,
                subject = subject,
                recieverName = patient.Name,
                recieverEmail = patient.Email
            });

            _mailService.SentMail(new Mail()
            {
                message = message,
                subject = subject,
                recieverName = doctor.Name,
                recieverEmail = doctor.Email
            });
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

            _hospitalContext.AppointmentChanges
                .RemoveRange(
                    _hospitalContext.AppointmentChanges.Where(c => c.AppointmentId == appointment.Id)
                );

            await _hospitalContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _hospitalContext.Appointments.Any(e => e.Id == id);
        }

        [HttpGet]
        public void Approve(int id)
        {
            var appointment = _hospitalContext.Appointments.First(a => a.Id == id);
            appointment.AppointmentState++;

            AppointmentChangeHistoryModel historyModel = new AppointmentChangeHistoryModel(
                appointment,
                _hospitalContext,
                Constants.ApprovedAppointment,
                User
            );

            SentMail($"Appointment on date {appointment.Date} at time {appointment.Time} approved!", "Your appointment was approved!", appointment);
            _hospitalContext.SaveChanges();
        }

        [HttpGet]
        public void Cancel(int id)
        {
            var appointment = _hospitalContext.Appointments.First(a => a.Id == id);
            appointment.AppointmentState = AppointmentStates.States.Find(a => a.Item2 == AppointmentStates.Canceled).Item1;

            AppointmentChangeHistoryModel historyModel = new AppointmentChangeHistoryModel(
                appointment,
                _hospitalContext,
                Constants.CanceledAppointment,
                User
            );

            SentMail($"Appointment on date {appointment.Date} at time {appointment.Time} canceled!", "Your appointment was cancel!", appointment);
            _hospitalContext.SaveChanges();
        }

        [HttpPost]
        public void UpdateAppointmentsState()
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            var appointments = _hospitalContext.Appointments
                .Where(ap => ap.Date == currentDate && ap.AppointmentState <= 3)
                .ToList();

            if (appointments.Count == 0) return;

            TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);
            TimeOnly timeIn2Hourd = currentTime.Add(TimeSpan.FromHours(2));
            TimeOnly startRange = timeIn2Hourd.Add(TimeSpan.FromMinutes(-3));
            TimeOnly endRange = timeIn2Hourd.Add(TimeSpan.FromMinutes(3));

            foreach (var app in appointments)
            {
                TimeOnly endTime = app.Time.Add(TimeSpan.FromHours(1));

                if (app.Time >= startRange && app.Time <= endRange) {
                    SentMail("Appointment reminder", $"Your appointemtn in 2 hours\nAppointment scheduled at {app.Date} {app.Time}", app);
                }

                if (app.Time <= currentTime && endTime > currentTime)
                {
                    app.AppointmentState = 3;
                    AppointmentChangeHistoryModel model = new AppointmentChangeHistoryModel(
                        app,
                        _hospitalContext,
                        Constants.AttendentedAppointment);
                }
                else if (endTime < currentTime)
                {
                    app.AppointmentState = 4;
                    AppointmentChangeHistoryModel model = new AppointmentChangeHistoryModel(
                    app,
                    _hospitalContext,
                    Constants.CompletedAppointment);
                }
            }

            _hospitalContext.Appointments.UpdateRange(appointments);
            _hospitalContext.SaveChanges();
        }

        [HttpPost]
        public void AddReceipt(int appId, string[] names, string[] description)
        {
            Appointment app = _hospitalContext.Appointments.First(a => a.Id == appId);
            ReceiptModel.CreateReceipt(app, names, description, _hospitalContext);
        }

        [HttpPost]
        public void DownloadReceipt(int appId)
        {
            Appointment app = _hospitalContext.Appointments.First(a => a.Id == appId);
            Patient patient = _hospitalContext.Patients.First(p => p.Id == app.Patient);
            Doctor doctor = _hospitalContext.Doctors.First(p => p.Id == app.Doctor);
            DateTime date = DateTime.Parse(app.Date.ToString());

            ReceiptModel receiptModel = app.ReceiptNavigation;
            if (receiptModel == null)
            {
                receiptModel = _hospitalContext.Receipts.First(r => r.Id == app.ReceiptId);
            }

            List<ReceiptRecord> records = _hospitalContext.ReceiptRecords
                .Where(r => r.ReceiptId == receiptModel.Id)
                .ToList();

            string[] names = records.Select(r => r.Name).ToArray();
            string[] descriptions = records.Select(r => r.Description).ToArray();

            DataContainer dataContainer = new DataContainer()
            {
                receiptId = receiptModel.Id,
                names = names,
                descriptions = descriptions,
                patient = patient,
                doctor = doctor,
                createTime = date
            };

            GenerateAndDownloadPDF(dataContainer);
        }

        private bool GenerateAndDownloadPDF(DataContainer dataContainer)
        {
            string download = Environment.ExpandEnvironmentVariables("%userprofile%/downloads/");

            string fileName = $"Receipt_{dataContainer.patient.Name.Trim()}_{DateTime.Now.Ticks}.pdf";
            string filePath = Path.Combine(download, fileName);

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report",
                Out = filePath
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = PDFTemplateGenerator.Generate(dataContainer, webHostEnvironment),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            _converter.Convert(pdf);

            string htmlString = PDFTemplateGenerator.Generate(dataContainer, webHostEnvironment);
            return true;
        }

        public class ReceiptRecordDto
        {
            public string[] Names { get; set; }
            public string[] Descriptions { get; set; }
        }

        [HttpGet]
        public JsonResult RecordInformation(int appId)
        {
            Appointment appointment = _hospitalContext.Appointments.First(a => a.Id == appId);
            ReceiptModel receiptModel = _hospitalContext.Receipts.First(a => a.Id == appointment.ReceiptId);
            ReceiptRecord[] receiptRecords = _hospitalContext.ReceiptRecords
                .Where(r => r.ReceiptId == appointment.ReceiptId)
                .ToArray();

            var result = new ReceiptRecordDto
            {
                Names = receiptRecords.Select(r => r.Name ?? string.Empty).ToArray(),
                Descriptions = receiptRecords.Select(r => r.Description ?? string.Empty).ToArray()
            };
            JsonResult json = Json(result);
            return json;
        }
    }
}
