using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HospitalDomain.Model;
using HospitalMVC;
using HospitalDomain.ViewModel;
using System.Security.Claims;

namespace LibraryWebApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IdentityContext _identityContext;
        private readonly HospitalContext _hospitalContext;

        private static AccountController instance;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IdentityContext context, HospitalContext hospitalContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _identityContext = context;
            _hospitalContext = hospitalContext;


            instance = this;
        }

        public static AccountController GetInstance()
        {
            return instance;
        }

        public async Task<User> GetAuthorizedUser() => await _userManager.GetUserAsync(User);
        public async Task<bool> IsUserInRole(User user, string role) => await _userManager.IsInRoleAsync(user, role);

        /// <summary>
        /// Displays the registration form.
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Handles user registration.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevents CSRF attacks
        public async Task<IActionResult> Index(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User
            {
                Email = model.Email,
                UserName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Passworld);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ModelState.Remove("ReturnUrl");
            TryValidateModel(ModelState);
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильний логін чи (та) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(AccountViewModel model)
        {
            // видаляємо автентифікаційні куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "No account found with this email.");
                return View(model);
            }

            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token }, Request.Scheme);

            // TODO: Send email with resetLink
            System.Diagnostics.Debug.WriteLine($"Reset Link: {resetLink}");

            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User
            {
                UserName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth ?? DateTime.MinValue
            };

            List<int> ids = _hospitalContext.Patients.Select(a => a.Id).ToList();
            int patientId = Utils.Util.GetId(ids);

            var patient = new Patient()
            {
                Id = patientId,
                Name = model.FullName,
                DateOfBirth = model.DateOfBirth,
                Contacts = model.PhoneNumber,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Passworld);


            if (result.Succeeded)
            {
                _hospitalContext.Patients.Add(patient);
                _identityContext.Users.Add(user);

                await _userManager.AddToRoleAsync(user, "user");
                await _hospitalContext.SaveChangesAsync();
                await _identityContext.SaveChangesAsync();
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccountDisplay()
        {
            var user = await _userManager.GetUserAsync(User); // Get the current logged-in user

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new AccountViewModel
            {
                FullName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                ProfilePictureUrl = user.ProfilePictureUrl,
                UserId = user.Id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(AccountViewModel model, IFormFile profilePicture)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            User user = await _identityContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(); // Handle case where user doesn't exist
            }

            // Update scalar properties from the model
            user.UserName = model.FullName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.DateOfBirth = model.DateOfBirth;
            user.Address = model.Address;

            // Handle profile picture upload if a file was provided
            if (profilePicture != null && profilePicture.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profilePicture.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/css/images/profiles", fileName);

                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                // Update the ProfilePictureUrl with the new file name
                user.ProfilePictureUrl = fileName;
            }

            // Save changes to the database
            await _identityContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RemovePhoto(AccountViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/css/images/profiles", user.ProfilePictureUrl);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            user.ProfilePictureUrl = null;
            await _identityContext.SaveChangesAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessButtonClicks(AccountViewModel model, string action)
        {
/*            if (action == "save")
            {
                return await UpdateProfile(model);
            } */

            if (action == "logout")
            {
                return await Logout(model);
            }


            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(AccountViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var account = await _userManager.GetUserAsync(User);

            Patient patient = _hospitalContext.Patients.First(p => p.Email == account.Email);

            var appointments = _hospitalContext.Appointments
                .Where(a => a.Patient == patient.Id)
                .ToList();

            if ((appointments == null || appointments.Count == 0) == false)
            {
                _hospitalContext.Appointments.RemoveRange(appointments);
            }
            _hospitalContext.Patients.Remove(patient);
            _identityContext.Users.Remove(user);

            await _hospitalContext.SaveChangesAsync();
            await _identityContext.SaveChangesAsync();
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
