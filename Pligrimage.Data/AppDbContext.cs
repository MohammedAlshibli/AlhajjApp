using Microsoft.EntityFrameworkCore;
using Pligrimage.Data.Mappings;
using Pligrimage.Entities;
using Pligrimage.Entities.IdentityModels;

namespace Pligrimage.Data
{
    public class AppDbContext : DbContext
    {
        //public AppDbContext() { }

        public AppDbContext(DbContextOptions options) : base(options) { }



        public DbSet<Parameter> parameters { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Unit> units { get; set; }
        public DbSet<Buses> Buses { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Residences> Residences { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<AlhajjMaster> AlhajjMasters { get; set; }
        public DbSet<PassengerSupervisors> PassengerSupervisors { get; set; }
        public DbSet<Passenger> Passengers { get; set; }

        #region IdentityAuthentication
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<UserService> UserService { get; set; }


        public DbSet<Permission> Permissions { get; set; }

        public DbSet<ApplicationAuditorLog> ApplicationAuditorLogs { get; set; }

        //public DbSet<UserFeedback> UserFeedback { get; set; }

        public DbSet<AdminLogin> AdminLogin { get; set; }
        #endregion



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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


        }







    }
}
