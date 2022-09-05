using DigitusTestCase.WebAPP.Models;
using DigitusTestCase.WebAPP.Services.UserService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Linq;
using System.Security.Cryptography;

namespace DigitusTestCase.WebAPP.Controllers
{
    public class ReportController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private UserService _userService;
        public ReportController(UserManager<ApplicationUser> userManager, UserService userService)
        {
            _userManager = userManager;
            _userService = userService;
        }

        public IActionResult ListingByTimeRange(string date)
        {
           
            return View(_userService.ListingByTimeRange(date));
        }
        public IActionResult SentVerificationCodeButDidNotRegisterAfter(string date)
        { 
            return View(_userService.SentVerificationCodeButDidNotRegisterAfter(data:date));
        }
        public  IActionResult CompleteLoginAverage(DateTime dateTime)
        { 
            return View(_userService.CompleteLoginAverage(dateTime:dateTime));
        }


    }
}
