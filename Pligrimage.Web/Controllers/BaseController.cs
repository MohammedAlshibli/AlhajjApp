using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pligrimage.Entities;
using Pligrimage.Entities.IdentityModels;
using Pligrimage.Web.Extensions;
using Pligrimage.Web.Infrastructure;
using System;

namespace Pligrimage.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        // ── Identity helpers ─────────────────────────────────────────────
        public string LoggedUserName()
        {
            var user = HttpContext.User.Identity.GetUser();
            if (user != null && !user.Active)
                LogoutOfSystem();
            return user?.UserName ?? string.Empty;
        }

        public User userLogged() =>
            HttpContext.User.Identity.GetUser();

        public bool UserIsSysAdmin() =>
            HttpContext.User.IsSysAdmin();

        // ── Tenant helpers ───────────────────────────────────────────────
        /// <summary>
        /// Returns the TenantId of the currently logged-in user.
        /// Use this to stamp every new entity before Insert().
        /// </summary>
        public int CurrentTenantId()
        {
            var user = HttpContext.User.Identity.GetUser();
            if (user == null)                return BaseEntity.SysAdminTenantId;
            if (user.IsSysAdmin)             return BaseEntity.SysAdminTenantId;
            if (user.TenantId > 0)           return user.TenantId;
            return user.MainUnitId ?? BaseEntity.SysAdminTenantId;
        }

        /// <summary>
        /// Stamps TenantId + CreateBy + CreateOn on a new BaseEntity before saving.
        /// Call this instead of setting those fields individually in each controller.
        /// </summary>
        protected void StampNew(BaseEntity entity)
        {
            entity.TenantId = CurrentTenantId();
            entity.CreateBy = LoggedUserName();
            entity.CreateOn = DateTime.Now;
            entity.IsDeleted = false;
        }

        /// <summary>
        /// Stamps UpdatedBy + UpdatedOn on an existing BaseEntity before saving.
        /// </summary>
        protected void StampUpdate(BaseEntity entity)
        {
            entity.UpdatedBy = LoggedUserName();
            entity.UpdatedOn = DateTime.Now;
        }

        /// <summary>
        /// Soft-deletes an entity — never hard delete audit records.
        /// </summary>
        protected void StampDelete(BaseEntity entity)
        {
            entity.IsDeleted = true;
            entity.DeletedBy = LoggedUserName();
            entity.DeletedOn = DateTime.Now;
            entity.UpdatedBy = LoggedUserName();
            entity.UpdatedOn = DateTime.Now;
        }

        // ── Session ──────────────────────────────────────────────────────
        public IActionResult LogoutOfSystem()
        {
            try
            {
                foreach (var cookie in Request.Cookies.Keys)
                    Response.Cookies.Delete(cookie);
                HttpContext.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
