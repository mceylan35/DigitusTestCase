using DigitusTestCase.WebAPP.Models;
using DigitusTestCase.WebAPP.Services.UserService;
using DigitusTestCase.WebAPP.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitusTestCase.WebAPP.Controllers
{
    public class RoleController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserService _userService;
        public RoleController(UserManager<ApplicationUser>  userManager, RoleManager<ApplicationRole> roleManager, UserService userService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _userManager = userService;
        }
        public IActionResult Index()
        {
            return View(_userService.Users);
        }

        public async Task<ActionResult> AddToRole(string roleName, string userName)
        {
            var user = await _userService.FindByNameAsync(userName);
            if (!await _roleManager.RoleExistsAsync(roleName)) 
            {
                var rol = new ApplicationRole();
                rol.Name = roleName;
                await _roleManager.CreateAsync(rol);
            }
            if (user is null) return NotFound();


            await _userService.AddToRoleAsync(user, roleName);
            await _userService.AddClaimAsync(user, new System.Security.Claims.Claim(ClaimTypes.Role, roleName));
            return Redirect($"/role/edit/{userName}");
        }
        public async Task<ActionResult> CheckInRole(string roleName, string userName)
        {
            var user = await _userService.FindByNameAsync(userName);
            if (!await _roleManager.RoleExistsAsync(userName)) 
            {
                var rol = new ApplicationRole();
                rol.Name = roleName;
                await _roleManager.CreateAsync(rol);
            }
            var res = await _userManager.IsInRoleAsync(user, roleName);

            return Content(res.ToString());

        }

        public async Task<ActionResult> Edit(string id)
        {
            var user = await _userManager.FindByNameAsync(id);

            if (user == null) return NotFound();

            var model = new UserViewModel
            {
                Id = user.Id.ToString(),
                AccessFailedCount = user.AccessFailedCount,
                ConcurrencyStamp = user.ConcurrencyStamp,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                NormalizedEmail = user.NormalizedEmail,
                NormalizedUserName = user.NormalizedUserName,
                PasswordHash = user.PasswordHash,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                SecurityStamp = user.SecurityStamp,
                TwoFactorEnabled = user.TwoFactorEnabled,
                UserName = user.UserName
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null) return NotFound();

            user.AccessFailedCount = (int)model.AccessFailedCount;
            user.ConcurrencyStamp = (string)model.ConcurrencyStamp;
            user.Email = (string)model.Email;
            user.EmailConfirmed = model.EmailConfirmed;
            user.LockoutEnabled = (bool)model.LockoutEnabled;
            user.LockoutEnd = model.LockoutEnd;
            user.PhoneNumber = model.PhoneNumber;
            user.PhoneNumberConfirmed = model.PhoneNumberConfirmed;
            user.SecurityStamp = model.SecurityStamp;
            user.TwoFactorEnabled = model.TwoFactorEnabled;
            user.UserName = model.UserName;

            await _userManager.UpdateAsync(user);
            return Redirect("/user");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await _userService.DeleteAsync(await _userService.FindByIdAsync(id));
            return Redirect("/user");
        }

    }
}
