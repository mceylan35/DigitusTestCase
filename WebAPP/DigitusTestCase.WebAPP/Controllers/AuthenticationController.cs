using DigitusTestCase.WebAPP.Models;
using DigitusTestCase.WebAPP.Services.EmailService;
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
        public AuthenticationController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            user.SurName = "asd";
            user.MailCode = "asdas";
          //  if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    Email = user.Email,
                    Name = user.Name,
                    Surname = user.SurName,
                    SendVerificationCodeDate = DateTime.UtcNow
                      
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, user.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.TryAddModelError(error.Code, error.Description);
                    }
                    return View(user);
                }
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                 
                string confirmationLink= Url.Action("ConfirmEmail","Authentication",new {token,email=user.Email},Request.Scheme);
                
                sendVerificationEmail(user, confirmationLink);
                
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
                to: user.Email,
                subject: "Sign-up Verification API - Verify Email",
                html: $@"<h4>Verify Email</h4>
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

        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded?nameof(ConfirmEmail):"Error");
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
