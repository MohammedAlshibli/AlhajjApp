using Microsoft.AspNetCore.Http;
using Pligrimage.Data;
using Pligrimage.Web.Extensions;
using System;

namespace Pligrimage.Web.Infrastructure
{
    /// <summary>
    /// Reads TenantId from the currently authenticated user stored in HttpContext.
    /// The TenantId equals the user's MainUnitId (their military unit's UnitId).
    /// </summary>
    public class HttpContextTenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextTenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCurrentTenantId()
        {
            try
            {
                var identity = _httpContextAccessor.HttpContext?.User?.Identity;
                if (identity == null || !identity.IsAuthenticated)
                    return BaseEntity.SysAdminTenantId; // unauthenticated = no filter

                var user = identity.GetUser();
                if (user == null) return BaseEntity.SysAdminTenantId;

                // SysAdmin sees everything — return 0 to bypass filters
                if (user.IsSysAdmin) return BaseEntity.SysAdminTenantId;

                // User's TenantId = their assigned unit
                return user.TenantId > 0 ? user.TenantId
                       : user.MainUnitId ?? BaseEntity.SysAdminTenantId;
            }
            catch (Exception)
            {
                return BaseEntity.SysAdminTenantId;
            }
        }

        public bool IsSysAdmin()
        {
            try
            {
                var identity = _httpContextAccessor.HttpContext?.User?.Identity;
                if (identity == null || !identity.IsAuthenticated) return false;
                var user = identity.GetUser();
                return user?.IsSysAdmin ?? false;
            }
            catch { return false; }
        }
    }

    // ── Convenience alias so BaseEntity.SysAdminTenantId is accessible here ──
    internal static class BaseEntity
    {
        internal static int SysAdminTenantId => 0;
    }
}
