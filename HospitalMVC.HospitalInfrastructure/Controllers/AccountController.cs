using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HospitalDomain.Model;
using HospitalMVC;
using System.Threading.Tasks;
using HospitalDomain.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LibraryWebApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IdentityContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IdentityContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

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
                        return RedirectToAction("Login", "Account");
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
        public async Task<IActionResult> Logout()
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
            User user = await _context.Users.FindAsync(model.UserId);
            user?.UpdateUser(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }



    }
}
