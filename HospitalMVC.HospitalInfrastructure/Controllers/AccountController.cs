using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HospitalDomain.Model;
using HospitalMVC;
using System.Threading.Tasks;
using HospitalDomain.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                UserName = model.Email
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
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
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
            //user.PatientNavigation = patient;
            var result = await _userManager.CreateAsync(user, model.Passworld);


            if (result.Succeeded)
            {
                _hospitalContext.Patients.Add(patient);

                await _hospitalContext.SaveChangesAsync();
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
                FullName = user.FullName,
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
        public async Task<IActionResult> UpdateProfile(AccountViewModel model)
        {
            User user = await _identityContext.Users.FindAsync(model.UserId);
            user?.UpdateUser(model);
            await _identityContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ProcessButtonClicks(AccountViewModel model, string action)
        {
            if (action == "save")
            {
                return await UpdateProfile(model);
            } 

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

            //Patient patient = user.PatientNavigation;
            //_hospitalContext.Appointments.RemoveRange(patient.Appointments);
            //_hospitalContext.Patients.Remove(patient);
            _identityContext.Users.Remove(user);

            return RedirectToAction("Index", "Home");
        }
    }
}
