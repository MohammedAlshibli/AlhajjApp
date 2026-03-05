using ITS.Core.Abstractions;
using ITS.Core.Abstractions.Trackable;
using ITS.Core.EF;
using ITS.Core.EF.Trackable;
using Microsoft.Extensions.DependencyInjection;
using Pligrimage.Entities;
using Pligrimage.Services;
using Pligrimage.Services.Interface;
using Pligrimage.Services.Implementation;
using Microsoft.EntityFrameworkCore;
using Pligrimage.Data;
using HrmsHttpClient.HrmsClientApi;
using Pligrimage.Entities.IdentityModels;
using Pligrimage.Utilities;

namespace Hajj.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(
            this IServiceCollection services)
        {
            //services.AddScoped<DbContext, AppDbContext>();

            services.AddScoped<ITrackableRepository<Parameter>, TrackableRepository<Parameter>>();
            services.AddScoped<ITrackableRepository<Category>, TrackableRepository<Category>>();
            services.AddScoped<ITrackableRepository<Unit>, TrackableRepository<Unit>>();
            services.AddScoped<ITrackableRepository<Buses>, TrackableRepository<Buses>>();
            services.AddScoped<ITrackableRepository<Flight>, TrackableRepository<Flight>>();
            services.AddScoped<ITrackableRepository<Document>, TrackableRepository<Document>>();
            services.AddScoped<ITrackableRepository<AlhajjMaster>, TrackableRepository<AlhajjMaster>>();
            services.AddScoped<ITrackableRepository<Passenger>, TrackableRepository<Passenger>>();
            services.AddScoped<ITrackableRepository<PassengerSupervisors>, TrackableRepository<PassengerSupervisors>>();
            services.AddScoped<ITrackableRepository<Residences>, TrackableRepository<Residences>>();
            services.AddScoped<ITrackableRepository<User>, TrackableRepository<User>>();
            services.AddScoped<ITrackableRepository<Role>, TrackableRepository<Role>>();
            services.AddScoped<ITrackableRepository<Permission>, TrackableRepository<Permission>>();





            //services.addscoped<itrackablerepository<Unit>, trackablerepository<Unit>>();






            services.AddScoped<IUnitOfWork, UnitOfWork>();



            services.AddScoped<IParameterService, ParameterService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IUnitServcie, UnitService>();
            services.AddScoped<IBusServcie, BusService>();
            services.AddScoped<IFlightServcie, FlightServcie>();
            services.AddScoped<IDocumentServcie, DocumentServcie>();
            services.AddScoped<IAlHajjMasterServcie, AlHajjMasterServcie>();
            services.AddScoped<IPassengerService, PassengerService>();
            services.AddScoped<IPassengerSupervisorService, PassengerSupervisorService>();
            services.AddScoped<IResidenceService, ResidenceService>();
            services.AddScoped<IEmployeeClient, EmployeeClient>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<CultureLocalizer, CultureLocalizer>();
            services.AddScoped<IAlHajjMasterServcieTst, AlHajjMasterServcieTst>();
            services.AddScoped<AppDbContext, AppDbContext>();



            // and a lot more Services


            return services;
        }
    }
}
