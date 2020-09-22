using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using FinancialPortal.Models;
using Microsoft.AspNet.Identity.Owin;
using FinancialPortal.Helpers;

namespace FinancialPortal.Extensions
{
    public static class AuthorizeExtensions
    {
        public static async Task RefreshAuthentication(this HttpContextBase context, ApplicationUser user)
        {
            context.GetOwinContext().Authentication.SignOut();
            await context.GetOwinContext().Get<ApplicationSignInManager>().SignInAsync(user, isPersistent: false, rememberBrowser: false);
        }
        public static void AutoLogOut(this HttpContextBase context)
        {
            context.GetOwinContext().Authentication.SignOut();
        }

    }
}