using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Pligrimage.Web.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Pligrimage.Services.Interface;
using Pligrimage.Entities;
using Novell.Directory.Ldap;

namespace Pligrimage.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAdminService _userService;

        public AccountController(IAdminService userService)
        {
            _userService = userService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string ReturnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.UserName = model.UserName?.Trim().ToUpper();

            // Basic injection-attempt guard
            if (string.IsNullOrEmpty(model.UserName) ||
                model.UserName.Contains("'") ||
                model.UserName.Contains("/") ||
                model.UserName.ToLower().Contains("select"))
            {
                ModelState.AddModelError("", "اسم المستخدم غير صالح");
                return View(model);
            }

            bool isAuthenticated = false;

            // Local admin bypass (development / emergency access)
            if (model.UserName == "ADMIN" && model.Password == "Oman")
            {
                isAuthenticated = true;
            }
            else
            {
                // LDAP authentication against Active Directory
                using var ldap = new LdapConnection();
                try
                {
                    ldap.Connect("10.22.8.8", 389);
                    ldap.Bind("ITS\\" + model.UserName, model.Password);
                    isAuthenticated = true;
                }
                catch (LdapException)
                {
                    ModelState.AddModelError("", "اسم المستخدم أو كلمة المرور غير صحيح");
                    return View(model);
                }
            }

            if (!isAuthenticated)
                return View(model);

            // ── User must exist in the application database ──────────────────
            var user = await _userService.GetUserByUserName(model.UserName);

            if (user == null)
            {
                ModelState.AddModelError("", " لم يتم تسجيل هذا المستخدم يرجى التواصل مع المستخدم المحلي ");
                return View(model);
            }

            if (!user.Active)
            {
                ModelState.AddModelError("", " تم إيقاف دخولك في النظام ،  يرجى التواصل مع مسؤول المستخدمين ");
                return View(model);
            }

            // Store username in the global constant (used by BaseEntity.CreateBy)
            PligrimageConstants.UserName = model.UserName;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("UserId",    user.UserId.ToString()),
                new Claim("FullName",  user.FullName ?? string.Empty),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToLocal(ReturnUrl);
        }

        public async Task<IActionResult> Logout()
        {
            return RedirectToAction(nameof(LogoutOfSystem));
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> LogoutOfSystem()
        {
            foreach (var cookie in Request.Cookies.Keys)
                Response.Cookies.Delete(cookie);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
