using System.Net;
using BugTrackerPetProj.Interfaces;
using BugTrackerPetProj.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation()
    .AddJsonOptions(jsonOptions =>
    {
        jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration["ConnectionStrings:DefaultConnection"]));
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddScoped<IRepository, SqliteRepository>();
builder.Services.AddTransient<IEmailService, GmailService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IEncryptionService, ASCIIEncryptionService>();

builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleOAuth:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleOAuth:ClientSecret"];
});


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

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

