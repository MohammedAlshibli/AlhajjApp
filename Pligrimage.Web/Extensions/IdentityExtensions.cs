using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pligrimage.Data;
using Pligrimage.Entities.IdentityModels;
using Pligrimage.Web.Infrastructure;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Pligrimage.Web.Extensions
{
    public static class IdentityExtendedMethods4Principal
    {
        public static User GetUser(this IIdentity identity)
        {
            if (identity == null || !identity.IsAuthenticated)
                return null;

            var ci = identity as ClaimsIdentity;
            var userName = ci?.FindFirstValue(ClaimTypes.Name);
            return string.IsNullOrEmpty(userName) ? null : GetUserModel(userName);
        }

        public static string GetFullName(this IIdentity identity)
        {
            if (identity == null || !identity.IsAuthenticated)
                return string.Empty;

            var fullNameClaim = ((ClaimsIdentity)identity).FindFirst("FullName");
            return fullNameClaim?.Value ?? string.Empty;
        }

        public static int GetIntUserId(this IIdentity identity)
        {
            if (identity == null || !identity.IsAuthenticated)
                return 0;

            var ci = identity as ClaimsIdentity;
            var userName = ci?.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName)) return 0;

            return GetUserModel(userName)?.UserId ?? 0;
        }

        public static bool HasPermission(this IPrincipal principal, string controllerName, string actionName)
        {
            if (principal?.Identity == null || !principal.Identity.IsAuthenticated)
                return false;

            var ci = principal.Identity as ClaimsIdentity;
            var userName = ci?.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName)) return false;

            var user = GetUserModel(userName);
            return user?.IsPermissionInUserRoles(controllerName, actionName) ?? false;
        }

        public static bool IsSysAdmin(this IPrincipal principal)
        {
            if (principal?.Identity == null || !principal.Identity.IsAuthenticated)
                return false;

            var ci = principal.Identity as ClaimsIdentity;
            var userName = ci?.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName)) return false;

            return GetUserModel(userName)?.IsSysAdmin ?? false;
        }

        public static bool IsActive(this IPrincipal principal)
        {
            if (principal?.Identity == null || !principal.Identity.IsAuthenticated)
                return false;

            var ci = principal.Identity as ClaimsIdentity;
            var userName = ci?.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName)) return false;

            return GetUserModel(userName)?.Active ?? false;
        }

        /// <summary>
        /// Loads a user with roles/permissions from the database using the DI container.
        /// Previously used a hardcoded connection string — now reads from appsettings.json.
        /// </summary>
        private static User GetUserModel(string userName)
        {
            try
            {
                using var scope = AppServiceLocator.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                return db.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                            .ThenInclude(r => r.RolePermissions)
                                .ThenInclude(rp => rp.Permission)
                    .SingleOrDefault(u => u.UserName == userName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[IdentityExtensions] GetUserModel failed for '{userName}': {ex.Message}");
                return null;
            }
        }

        public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            if (identity == null) return string.Empty;
            return identity.FindFirst(claimType)?.Value ?? string.Empty;
        }
    }
}
