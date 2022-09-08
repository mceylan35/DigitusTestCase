using DigitusTestCase.WebAPP.Models;
using DigitusTestCase.WebAPP.Services.UserService;
using DigitusTestCase.WebAPP.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;

namespace DigitusTestCase.WebAPP.Controllers
{
     [Authorize(Roles = "Admin")]
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
        
        public IActionResult CreateRole() => View();

        [HttpPost]
        public async Task<IActionResult> CreateRole([Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new ApplicationRole() { Name = name });
                if (result.Succeeded)
                    ViewBag.Message = "Role Created Successfully";
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }


        
     
      

    }
}
