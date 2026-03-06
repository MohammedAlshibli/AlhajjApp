namespace Pligrimage.Web.Infrastructure
{
    /// <summary>
    /// Resolves the TenantId for the currently authenticated user.
    /// Injected into AppDbContext so Global Query Filters can use it.
    /// </summary>
    public interface ITenantService
    {
        /// <summary>
        /// Returns the UnitId of the logged-in user's unit.
        /// Returns 0 for SysAdmins (bypasses all tenant filters).
        /// </summary>
        int GetCurrentTenantId();

        /// <summary>True when the current user is a SysAdmin (sees all tenants).</summary>
        bool IsSysAdmin();
    }
}
