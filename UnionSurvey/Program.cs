using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Tls;
using UnionSurvey.Business;
using UnionSurvey.Business.Services;
using UnionSurvey.Common;
using UnionSurvey.Data.IRepository;
using UnionSurvey.Data.Models;
using UnionSurvey.Data.Repository;
using UnionSurvey.Hubs;
using UnionSurvey.HostedServices;
using UnionSurvey.Middleware;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Org.BouncyCastle.Asn1.X509.Qualified;
using UnionSurvey.Services;

var builder = WebApplication.CreateBuilder(args);

// Scheduled Jobs
builder.Services.AddHostedService<ScheduledJobService>();

// Database and Repositories
builder.Services.AddDbContext<SurveyUnionContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddDataProtection()
//    .SetApplicationName("UnionSurvey")
//    .PersistKeysToDbContext<SurveyUnionContext>();


// Dependency Injection for Services and Repositories
builder.Services
    .AddTransient<IAppUserSerice, AppUserSerice>()
    .AddTransient<ISurveyService, SurveyService>()
    .AddTransient<IQuestionService, QuestionService>()
    .AddTransient<IFinancialService, FinancialService>()
    .AddTransient<IUserSessionService, UserSessionService>()

    .AddTransient(typeof(IRepository<>), typeof(Repository<>))
    .AddTransient<IAppUserRepo, AppUserRepo>()
    .AddTransient<IQuestionRepo, QuestionRepo>()
    .AddTransient<ISurveyRepo, SurveyRepo>()
    .AddTransient<ISurveyUnionConfigRepo, SurveyUnionConfigRepo>()
    .AddTransient<ITransactionRepo, TransactionRepo>()
    .AddTransient<SensitiveDataService, SensitiveDataService>();

// Identity Services
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<SurveyUnionContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true; // Optional: Format JSON for readability
    });


// Session Services
//builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax; // Cookie is sent with same-site requests and top-level cross-site navigations
});


// Authentication and Authorization
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index"; // Path to your login page
        options.AccessDeniedPath = "/Login/AccessDenied"; // Path to your access denied page
    });


builder.Services.AddAuthorization();

//HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add custom Identity services
builder.Services
    .AddIdentityCore<IdentityUser>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<SurveyUnionContext>()
    .AddSignInManager<SignInManager<IdentityUser>>(); ;




// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Service Locator
var serviceScopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
ServiceAccessor.Configure(serviceScopeFactory);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Add Session middleware
app.UseSession();

// Ensure authentication and authorization middlewares are called before session validation
app.UseAuthentication();
app.UseAuthorization();



// Add Custom Middleware
app.UseMiddleware<SessionValidationMiddleware>();

// SignalR Configuration
app.MapHub<ChatHub>("/chatHub");

// Configure HttpContextHelper
HttpContextHelper.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

// Route Configuration
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
