using DigitusTestCase.WebAPP.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DigitusTestCase.WebAPP.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string ResetPasswordCallBackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AuthenticationController.ResetPassword),
                controller: "Account",
                values: new { userId, code },
                protocol: scheme);
        }
    }
}
