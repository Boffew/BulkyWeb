using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Bulky.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Add database service
builder.Services.AddDbContext<ApplicationDBContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//Add Identity service
//Change AddDefaultIdentity to AddIdentity to customize the role fucntion
builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<ApplicationDBContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{

    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
//Add razer pages handle so the identity can work
builder.Services.AddRazorPages();

//Dependency Inversion 
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender,EmailSender>();
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
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
