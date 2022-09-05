using DigitusTestCase.WebAPP.Models;
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
        public ReportController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult ListingByTimeRange(string date)
        {
            var oneDate = DateTime.UtcNow.AddDays(-1);
            _userManager.Users.Count(i => i.CreatedOn > oneDate && i.CreatedOn<DateTime.UtcNow);
            return View();
        }
        public IActionResult SentVerificationCodeButDidNotRegisterAfter(string date)
        {
             
           var count= _userManager.Users.Count(i => i.EmailConfirmed == false && i.CreatedOn.AddDays(1) > DateTime.UtcNow);

            return View(count);
        }
        public  IActionResult CompleteLoginAverage(DateTime dateTime)
        {
            
            var userList = _userManager.Users.Where(c => c.EmailConfirmed == true && c.CreatedOn.Date== dateTime.Date);
            List<double> dateBetween = new List<double>();
            foreach (var item in userList)
            {
                TimeSpan timeSpan = (DateTime)item.SendVerificationCodeDate - item.CreatedOn;
                
                dateBetween.Add(timeSpan.TotalSeconds);
            }

          
            return View(dateBetween.Average());
        }


    }
}
