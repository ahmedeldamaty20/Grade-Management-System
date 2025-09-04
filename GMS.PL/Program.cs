using GMS.BLL.Interfaces;
using GMS.BLL.Services;
using GMS.DAL.Data;
using GMS.DAL.Interfaces;
using GMS.DAL.Models;
using GMS.DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ICourseRepo, CourseRepo>();

builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<ISessionRepo, SessionRepo>();

builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

// Configure ASP.NET Core Identity for authentication and authorization
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
	// Password requirements
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireUppercase = true;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequiredLength = 6;
	options.Password.RequiredUniqueChars = 1;

	// Lockout settings
	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
	options.Lockout.MaxFailedAccessAttempts = 5;
	options.Lockout.AllowedForNewUsers = true;

	// User settings
	options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
	options.User.RequireUniqueEmail = true;

	// Sign-in settings
	options.SignIn.RequireConfirmedEmail = false;
	options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure authentication cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
	options.Cookie.HttpOnly = true;
	options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
	options.LoginPath = "/Account/Login";
	options.AccessDeniedPath = "/Account/AccessDenied";
	options.SlidingExpiration = true;
});

var app = builder.Build();

#region Update Database With Using Way And Seeding Data

// We Said To Update Database You Should Do Two Things (1. Create Instance From DbContext 2. Migrate It)

// To Ask Clr To Create Instance Explicitly From Any Class
//    1 ->  Create Scope (Life Time Per Request)
using var scope = app.Services.CreateScope();
//    2 ->  Bring Service Provider Of This Scope
var services = scope.ServiceProvider;

// --> Bring Object Of StoreContext For Update His Migration
var AppContext = services.GetRequiredService<ApplicationDbContext>();
// --> Bring Object Of UserManager & RoleManager For Seeding Data
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
// --> Bring Object Of ILoggerFactory For Good Show Error In Console
var loggerFactory = services.GetRequiredService<ILoggerFactory>();

try
{
	await AppContext.Database.MigrateAsync();

	var isDatabaseCreated = AppContext.Users.Any();

	if (!isDatabaseCreated)
	{
		// Create roles if they don't exist
		await ApplicationDbContextSeeding.CreateRolesAsync(roleManager);

		// Create default super admin user
		await ApplicationDbContextSeeding.CreateDefaultSuperAdminAsync(userManager);
	}
}
catch (Exception ex)
{
	var logger = loggerFactory.CreateLogger<Program>();
	logger.LogError(ex, "an error has been occured during apply the migration!");
}
#endregion

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
