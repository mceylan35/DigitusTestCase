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
             
           
           // user.MailCode = "asdas";
          //  if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    Email = user.Email,
                    Name = user.Name,
                    Surname = user.SurName, 
                    UserName=user.UserName,
                      
                      
                };

                IdentityResult result = await _userService.CreateAsync(appUser, user.Password);
                await _userService.AddToRoleAsync(appUser, "Admin");
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
                

                if (result.Succeeded)
                    ViewBag.Message = "User Created Successfully";



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
        public async Task<IActionResult> Login([Required]string email, [Required] string password,string returnUrl)
        {
            ApplicationUser appUser = await _userManager.FindByEmailAsync(email);
            if (appUser != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(appUser, password, true, true);
                if (signInResult.Succeeded)
                {
                   // await _signInManager.SignInAsync(signInResult, isPersistent: false);
                    return Redirect(returnUrl ?? "/");
                }
                if (signInResult.IsNotAllowed)
                {
                    ModelState.AddModelError(nameof(email), "Email Not Confirmed");
                }
            }else
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



        //[HttpGet]
      //  [AllowAnonymous]
        public IActionResult ResetPassword(string code)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
         

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        /*
        public IActionResult ResetPassword(string email, string token)
        { 
           
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
        */
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userService.FindByEmailAsync(email);
            if (user is null)
            {
                return View("Error");
            }
            user.MailConfirmationDate = DateTime.Now;
            await _userService.UpdateAsync(user);
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
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user==null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallBackLink(user.Id.ToString(), code, Request.Scheme);
                _emailService.Send(email, $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
                 
            }
            return View(email);
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
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
