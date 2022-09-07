using DigitusTestCase.WebAPP.Extensions;
using DigitusTestCase.WebAPP.Models;
using DigitusTestCase.WebAPP.Services.EmailService;
using DigitusTestCase.WebAPP.Services.UserService;
using DigitusTestCase.WebAPP.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IEmailService _emailService;
        private readonly UserService _userService;
        public AuthenticationController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailService emailService, UserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _userService = userService;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            user.SurName = "Ceylan";
            user.Name = "Mehmet";
           
           // user.MailCode = "asdas";
          //  if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    Email = user.Email,
                    Name = user.Name,
                    Surname = user.SurName,
                    SendVerificationCodeDate = DateTime.UtcNow,
                    UserName = "mceylan35" + new Random().Next(5,1500)
                      
                };

                IdentityResult result = await _userService.CreateAsync(appUser, user.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.TryAddModelError(error.Code, error.Description);
                    }
                    return View(user);
                }
                var token = await _userService.GenerateEmailConfirmationTokenAsync(appUser);
                 
                string confirmationLink= Url.Action("ConfirmEmail","Authentication",new {token,email=user.Email},Request.Scheme);
                
                sendVerificationEmail(user, confirmationLink);
                appUser.EmailConfirmed = true;
                await _userService.UpdateAsync(appUser);
               



            }
            return View(user);
        }
        private void sendVerificationEmail(User user,string link )
        {
            string message;
            
            var verifyUrl = $"{link}";
            message = $@"<p>Please click the below link to verify your email address:</p>
                        <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
             
            _emailService.Send(
                userEmail: user.Email, 
                confirmationLink: $@"<h4>Verify Email</h4>
                        <p>Thanks for registering!</p>
                        {message}"
            );
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
            var user = await _userService.FindByEmailAsync(email);
            if (user==null)
            {
                ModelState.AddModelError("", "User Not Found");
                return View();
            }
            var token = await _userService.GeneratePasswordResetTokenAsync(user);

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
            var user =await _userService.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("","User Not Found");
            }
            var result = await _userService.ResetPasswordAsync(user,token, newPassword);
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

        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userService.FindByEmailAsync(email);
            if (user is null)
            {
                return View("Error");
            }
            var result = await _userService.ConfirmEmailAsync(user, token);
            return View(result.Succeeded?nameof(ConfirmEmail):"Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.FindByIdAsync(model.Email);
                if (user==null || (await _userService.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                var code = await _userService.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallBackLink(user.Id.ToString(), code, Request.Scheme);
                _emailService.Send(model.Email, $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
                 
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
