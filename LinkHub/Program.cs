using AspNetCoreHero.ToastNotification;
using LinkHub.Data;
using LinkHub.Models;
using LinkHub.Repositories;
using LinkHub.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = Environment.GetEnvironmentVariable("ConnectionString");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

var keyPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"C:\keys" : "/var/keys";
if (!Directory.Exists(keyPath))
{
	Directory.CreateDirectory(keyPath);
}
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(keyPath));

//Adiciona a dependÍncia do Toastify
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
});

builder.Services.AddScoped<LdapAuthentication>();
builder.Services.AddScoped<LdapSyncService>();
builder.Services.AddScoped<ImageStorage>();
builder.Services.AddScoped<ILinkRepository, LinkRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPageRepository, PageRepository>();
builder.Services.AddScoped<ILdapSettingsRepository, LdapSettingsRepository>();
builder.Services.AddScoped<IUserPagePermissionRepository, UserPagePermissionRepository>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Account/Login";
	});

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();

var app = builder.Build();

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

await CreateUserProfilesAsync(app);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

async Task CreateUserProfilesAsync(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    if (scopedFactory != null)
    {
        using (var scope = scopedFactory.CreateScope())
        {
            var userRoleInitial = scope.ServiceProvider.GetService<ISeedUserRoleInitial>();

            if (userRoleInitial != null)
            {
                await userRoleInitial.SeedRolesAsync();
                await userRoleInitial.SeedUsersAsync();
            }            
        }
    }
}