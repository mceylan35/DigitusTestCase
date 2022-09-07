using DigitusTestCase.WebAPP.Hubs;
using DigitusTestCase.WebAPP.Models;
using DigitusTestCase.WebAPP.Services.EmailService;
using DigitusTestCase.WebAPP.Services.UserService;
using DigitusTestCase.WebAPP.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration; 
using static Org.BouncyCastle.Math.EC.ECCurve;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.
services.AddControllersWithViews();
#region MongoDb Settings
var setting = builder.Configuration.GetSection(nameof(MongoDbConfig)).Get<MongoDbConfig>();
services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedEmail = true;
    options.Tokens.PasswordResetTokenProvider = "passwordReset";
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequiredLength = 0;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abc�defghi�jklmno�pqrs�tu�vwxyzABC�DEFGHI�JKLMNO�PQRS�TU�VWXYZ0123456789-._@+'#!/^%{}*";


}).AddDefaultTokenProviders().AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(setting.ConnectionString, setting.Name);

services.Configure<MongoDbConfig>(options =>
{
    options.ConnectionString = builder.Configuration.GetSection("MongoConnection:ConnectionString").Value;
    options.Host = builder.Configuration.GetSection("MongoConnection:Database").Value;

});
#endregion
services.AddScoped<IEmailService, EmailService>();
services.AddScoped<UserManager<ApplicationUser>>();
services.AddScoped<UserService>();
services.AddSignalR(); 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
 
app.MapHub<OnlineCountHub>("/onlinecount");
app.UseHttpsRedirection();
app.UseStaticFiles();
 
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
