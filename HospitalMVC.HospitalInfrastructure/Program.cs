using Microsoft.EntityFrameworkCore;
using HospitalDomain.Model;
using LibraryWebApplication;
using Microsoft.AspNetCore.Identity;
using HospitalMVC;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add support for HTTP Antiforgery token (for local development)
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Allow cookies over HTTP
});

// ✅ Allow SameSite cookies over HTTP (important for local dev)
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax; // Fixes the SameSite cookie issue
    options.Secure = CookieSecurePolicy.None; // Allow HTTP
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
});

// ✅ Add services
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<HospitalContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders(); // Add this if you need token generation (for password reset, etc.)

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ✅ Ensure HTTPS Redirection is only in production (fixes local HTTP issue)
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

// ✅ Ensure cookie policies are applied
app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseStaticFiles(); // Maps static assets (css, js, images, etc.)

app.Run();
