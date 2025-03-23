using HospitalDomain.Model;
using HospitalDomain.ViewModel;
using HospitalMVC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Utils;

namespace HospitalDomain.Controllers
{
    [Authorize(Roles = "admin")]
    public class RolesController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<User> _userManager;
        HospitalContext _hospitalContext;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, HospitalContext hospitalContext)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _hospitalContext = hospitalContext;
        }
        public IActionResult Index() => View(_roleManager.Roles.ToList());
        public IActionResult UserList() => View(_userManager.Users.ToList());

        public async Task<IActionResult> Edit(string userId)
        {
            // отримуємо користувача
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                //список ролей користувача
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }

            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            User user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);

                var role = roles[0];
                if (role == Constants.Doctor && _hospitalContext.Doctors.FirstOrDefault(d => d.Email == user.Email) == default)
                {
                    List<int> ids = _hospitalContext.Doctors
                    .Select(d => d.Id)
                    .OrderBy(id => id) 
                    .ToList();

                    Patient patient = _hospitalContext.Patients.First(p => p.Email == user.Email);

                    int id = Util.GetId(ids);
                    _hospitalContext.Patients.Remove(patient);
                    _hospitalContext.Doctors.Add(new Doctor(user, id));
                    await _hospitalContext.SaveChangesAsync();
                }
                else if (role != Constants.User)
                {
                    Patient patient = _hospitalContext.Patients.First(p => p.Email == user.Email);
                    _hospitalContext.Patients.Remove(patient);
                    await _hospitalContext.SaveChangesAsync();
                } 

                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                return RedirectToAction("UserList");
            }

            return NotFound();
        }

    }
}

