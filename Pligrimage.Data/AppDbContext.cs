using Microsoft.EntityFrameworkCore;
using Pligrimage.Data.Mappings;
using Pligrimage.Entities;
using Pligrimage.Entities.IdentityModels;
using System;

namespace Pligrimage.Data
{
    public class AppDbContext : DbContext
    {
        // ── Tenant resolver injected from DI ──────────────────────────────
        // Using a Func<int> so it reads the CURRENT tenant at query time,
        // not at DbContext construction time.
        private readonly Func<int> _getTenantId;
        private readonly Func<bool> _isSysAdmin;

        /// <summary>
        /// Production constructor — tenant resolved from HttpContext at runtime.
        /// </summary>
        public AppDbContext(DbContextOptions options,
                            Func<int>  getTenantId,
                            Func<bool> isSysAdmin)
            : base(options)
        {
            _getTenantId = getTenantId ?? (() => 0);
            _isSysAdmin  = isSysAdmin  ?? (() => true);
        }

        /// <summary>
        /// Fallback constructor for migrations and tooling (no tenant filtering).
        /// </summary>
        public AppDbContext(DbContextOptions options) : base(options)
        {
            _getTenantId = () => 0;
            _isSysAdmin  = () => true;
        }

        // ── DbSets ────────────────────────────────────────────────────────
        public DbSet<Parameter>            parameters            { get; set; }
        public DbSet<Category>             categories            { get; set; }
        public DbSet<Unit>                 units                 { get; set; }
        public DbSet<Buses>                Buses                 { get; set; }
        public DbSet<Flight>               Flights               { get; set; }
        public DbSet<Residences>           Residences            { get; set; }
        public DbSet<Document>             Documents             { get; set; }
        public DbSet<AlhajjMaster>         AlhajjMasters         { get; set; }
        public DbSet<PassengerSupervisors> PassengerSupervisors  { get; set; }
        public DbSet<Passenger>            Passengers            { get; set; }

        #region Identity
        public DbSet<User>                 Users                 { get; set; }
        public DbSet<Role>                 Roles                 { get; set; }
        public DbSet<UserRoles>            UserRoles             { get; set; }
        public DbSet<UserService>          UserService           { get; set; }
        public DbSet<Permission>           Permissions           { get; set; }
        public DbSet<ApplicationAuditorLog> ApplicationAuditorLogs { get; set; }
        public DbSet<AdminLogin>           AdminLogin            { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── Entity maps ───────────────────────────────────────────────
            modelBuilder.ApplyConfiguration(new FlightMap());
            modelBuilder.ApplyConfiguration(new BusesMap());
            modelBuilder.ApplyConfiguration(new CategoryMap());
            modelBuilder.ApplyConfiguration(new UnitMap());
            modelBuilder.ApplyConfiguration(new ParameterMap());
            modelBuilder.ApplyConfiguration(new DocumentMap());
            modelBuilder.ApplyConfiguration(new AlhajjMasterMap());
            modelBuilder.ApplyConfiguration(new PassengerSupervisorsMap());
            modelBuilder.ApplyConfiguration(new PassengerMap());
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new RoleMap());
            modelBuilder.ApplyConfiguration(new UserRolesMap());
            modelBuilder.ApplyConfiguration(new RolePermissionsMap());
            modelBuilder.ApplyConfiguration(new UserServiceMap());
            modelBuilder.ApplyConfiguration(new PermissionMap());

            // ── GLOBAL QUERY FILTERS — Multi-Tenancy ─────────────────────
            // These filters apply automatically to EVERY query on these tables.
            // SysAdmins (TenantId == 0) bypass all filters.
            // Unit admins see only their own unit's data.
            // IsDeleted=true records are always hidden (soft delete).

            modelBuilder.Entity<AlhajjMaster>()
                .HasQueryFilter(c =>
                    (_isSysAdmin() || c.TenantId == _getTenantId()) &&
                    !c.IsDeleted);

            modelBuilder.Entity<Passenger>()
                .HasQueryFilter(c =>
                    (_isSysAdmin() || c.TenantId == _getTenantId()) &&
                    !c.IsDeleted);

            // Users: SysAdmin sees all; others see only their own unit's users
            modelBuilder.Entity<User>()
                .HasQueryFilter(u =>
                    _isSysAdmin() || u.TenantId == _getTenantId());
        }
    }
}
