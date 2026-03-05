using Hajj.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pligrimage.Data;
using Newtonsoft.Json.Serialization;
using HrmsHttpClient;
using System;
using Pligrimage.Web.Common.Mapper;
using AutoMapper;
 
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Hajj.Web
{
    public class Startup
    {
     

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddAutoMapper(typeof(Startup));

            services.AddDbContext<DbContext, AppDbContext>(c =>
            c.UseSqlServer(Configuration.GetConnectionString("AppConnection")));
            services.RegisterServices();



            //services.AddAutoMapper(options =>
            //{
            //    options.AddProfile<IdentityMapping>();
            //});


            //services
            //    .AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            //    .AddJsonOptions(option =>
            //    {
            //        option.SerializerSettings.ContractResolver = new DefaultContractResolver();
            //        // Added By Mohammed Alkaabi to avoid self referencing. 
            //        option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //    });


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                //options.Events.OnRedirectToLogin = (context) =>
                //{
                //    context.Response.StatusCode = 401;
                //    return Task.CompletedTask;
                //};

                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Home/Error";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(35);

            });
            //services.AddKendo();
           services.AddMvc(option =>option.EnableEndpointRouting=false);
            //services.AddKendo();
            services.Configure<HrmsApiConfig>(Configuration.GetSection("HrmsApi"));
            //services.AddHttpClient<IApiClient, ApiClient>(client =>
            //{
            //    client.BaseAddress = new Uri(Configuration["HrmsApiLink"]);
            //    client.DefaultRequestHeaders.Add("User-Agent", "HrmsClient");
            //});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            string sAppPath = env.ContentRootPath; //Application Base Path
            string swwwRootPath = env.WebRootPath;  //wwwroot folder path
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }

    
    }
}
