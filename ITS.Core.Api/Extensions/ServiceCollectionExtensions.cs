using Hajj.Data;
using Hajj.Entities;
using Hajj.Services;
using ITS.Core.Abstractions.Trackable;
using ITS.Core.EF.Trackable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ITS.Core.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(
            this IServiceCollection services)
        {
            //services.AddScoped<DbContext, AppDbContext>();
            services.AddScoped<ITrackableRepository<Customer>, TrackableRepository<Customer>>();
            services.AddScoped<ITrackableRepository<Order>, TrackableRepository<Order>>();

            services.AddScoped<ICustomerService, CustomerService>();
            // and a lot more Services

            return services;
        }
    }
}
