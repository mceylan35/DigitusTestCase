using DigitusTestCase.WebAPP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DigitusTestCase.WebAPP.Controllers
{
    public class AuthenticationController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        public AuthenticationController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager; 
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    UserName = user.Name,
                    Email = user.Email
                     
                    
                   
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                    ViewBag.Message = "User Created Successfully";
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email,string password,string returnUrl)
        {
            ApplicationUser appUser = await _userManager.FindByEmailAsync(email);
            if (appUser != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(appUser, password, false, false);
                if (signInResult.Succeeded)
                {
                    return Redirect(returnUrl ?? "/");
                }
            }
            ModelState.AddModelError(nameof(email), "Invalid Email or Password");

            return View();
        }
    
        [HttpPost]
        public async Task<IActionResult> ResetPasswordIntent(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user==null)
            {
                ModelState.AddModelError("", "User Not Found");
                return View();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            string url =  Url.Action(action: "ResetPassword", 
                controller: "Authentication", 
                values: new { email = email, token = token });
            string message = "Click on this link to reset password of your account: ";
            StringBuilder stringBuilder= new StringBuilder();
            stringBuilder.Append(message);
            stringBuilder.Append(url);
            // _emailSenderService.Send(email, "Ok", stringBuilder.ToString());
            return RedirectToAction("", "");
             

        }
        public IActionResult ResetPassword(string email, string token)
        { 
            //email token view yolla
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email, string token,string newPassword)
        {
            var user =await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("","User Not Found");
            }
            var result = await _userManager.ResetPasswordAsync(user,token, newPassword);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Error");
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
           
        }


        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
