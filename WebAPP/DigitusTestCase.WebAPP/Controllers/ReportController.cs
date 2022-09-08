using DigitusTestCase.WebAPP.Models;
using DigitusTestCase.WebAPP.Services.UserService;
using DigitusTestCase.WebAPP.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;

namespace DigitusTestCase.WebAPP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private UserService _userService;
        public ReportController(UserManager<ApplicationUser> userManager, UserService userService)
        {
            _userManager = userManager;
            _userService = userService;
        }
        public IActionResult Index()
        {
            ReportViewModel reportViewModel = new ReportViewModel();
            reportViewModel.CompleteLoginAverage = (int)_userService.CompleteLoginAverage(dateTime: DateTime.Now.AddDays(2)); 
            reportViewModel.SentVerificationCodeButDidNotRegisterAfter = _userService.SentVerificationCodeButDidNotRegisterAfter(date: DateTime.Now.AddDays(1));
            reportViewModel.ListingByTimeRange = _userService.ListingByTimeRange(date: DateTime.Now.AddDays(1));
            return View(reportViewModel);
        }
        public IActionResult ListingByTimeRange(string date)
        {
           
            return Json(_userService.ListingByTimeRange(DateTime.Parse(date)));
        }
        public IActionResult SentVerificationCodeButDidNotRegisterAfter(string date)
        { 
            return Json(_userService.SentVerificationCodeButDidNotRegisterAfter(date: DateTime.Parse(date)));
        }
        public  IActionResult CompleteLoginAverage(string dateTime)
        { 
            return Json(_userService.CompleteLoginAverage(dateTime: DateTime.Parse(dateTime)));
        }


    }
}
