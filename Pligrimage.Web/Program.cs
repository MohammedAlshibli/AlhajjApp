using Hajj.Web.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pligrimage.Data;
using Pligrimage.Web.Infrastructure;
using HrmsHttpClient;
using System;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// ── Database ─────────────────────────────────────────────────────────────────
// ── Tenant isolation services ─────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantService, HttpContextTenantService>();

builder.Services.AddDbContext<DbContext, AppDbContext>((sp, options) => {
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AppConnection"),
        sql => sql.EnableRetryOnFailure());
    // Tenant funcs are passed here so Global Query Filters in DbContext can use them.
    // The DbContext constructor reads these at query time (not at construction time).
});

// Required so AppServiceLocator can resolve AppDbContext in static extension methods
// Resolve AppDbContext with tenant-aware funcs injected
builder.Services.AddScoped<AppDbContext>(sp => {
    var tenantSvc = sp.GetRequiredService<ITenantService>();
    var dbOptions = sp.GetRequiredService<DbContextOptions<AppDbContext>>();
    return new AppDbContext(
        dbOptions,
        () => tenantSvc.GetCurrentTenantId(),
        () => tenantSvc.IsSysAdmin());
});

// ── AutoMapper ────────────────────────────────────────────────────────────────
builder.Services.AddAutoMapper(
    typeof(Program).Assembly,
    typeof(Pligrimage.Web.Common.Mapper.IdentityMapping).Assembly);

// ── Application services (DI registrations) ──────────────────────────────────
builder.Services.RegisterServices();

// ── HRMS API config ──────────────────────────────────────────────────────────
builder.Services.Configure<HrmsApiConfig>(
    builder.Configuration.GetSection("HrmsApi"));

// ── Cookie Authentication ─────────────────────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath       = "/Account/Login";
        options.LogoutPath      = "/Account/Logout";
        options.AccessDeniedPath = "/Home/Error";
        options.Cookie.HttpOnly  = true;
        options.ExpireTimeSpan   = TimeSpan.FromMinutes(35);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// ── MVC + Razor Views ─────────────────────────────────────────────────────────
builder.Services.Configure<HajjSettings>(
    builder.Configuration.GetSection("HajjSettings"));

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver =
            new Newtonsoft.Json.Serialization.DefaultContractResolver();
        options.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// DataTables, Toastr, Select2, and Flatpickr are loaded via CDN in _Layout.cshtml

// ── Localization (used by CultureLocalizer) ───────────────────────────────────
builder.Services.AddLocalization(opts => opts.ResourcesPath = "Resources");
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded    = _ => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

// ── IDbContextFactory (for any code that needs short-lived contexts) ──────────
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AppConnection"),
        sql => sql.EnableRetryOnFailure()),
    ServiceLifetime.Scoped);

// ────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Initialise the static service locator ASAP after the app is built ────────
AppServiceLocator.Initialize(app.Services);

// ── Seed test data (safe: uses MERGE / existence checks — runs every startup) ─
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    Pligrimage.Data.AppDbContextSeed.Seed(db);
}

// ── Middleware pipeline ───────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

var supportedCultures = new[] { new CultureInfo("ar-SA"), new CultureInfo("en-US") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture  = new RequestCulture("ar-SA"),
    SupportedCultures      = supportedCultures,
    SupportedUICultures    = supportedCultures
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
