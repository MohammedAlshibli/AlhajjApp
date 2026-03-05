using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pligrimage.Entities.IdentityModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Pligrimage.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Pligrimage.Entities;

namespace Pligrimage.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public string LoggedUserName()
        {
            var user = HttpContext.User.Identity.GetUser();
           
            if (!user.Active)
            {
                LogoutOfSystem();

            }

            return user.UserName;

        }



     

        public User userLogged()
        {
            return HttpContext.User.Identity.GetUser();

        }

        public IActionResult LogoutOfSystem()

        {
            try
            {
                foreach (var cookies in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookies);

                }
                HttpContext.SignOutAsync();
                // await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return RedirectToAction("Login", "Account");
            }
            catch (Exception)
            {

                throw;
            }

        }


        public bool UserIsSysAdmin()
        {
            return HttpContext.User.IsSysAdmin();
        }

    }
}
