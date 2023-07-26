global using PizzaDelivery.Domain.Models;
global using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Serilog;
using PizzaDelivery.Application.Options;
using PizzaDelivery.Application.Helpers;
using PizzaDeliveryApi.Services;
using MongoDB.Driver;
using PizzaDelivery.Application.Services.Implementation;
using PizzaDelivery.Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using PizzaDeliveryApi.Controllers;
using EmailProvider.Options;
using EmailProvider.Interfaces;
using EmailProvider;
using GoogleProvider.Options;
using GoogleProvider.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using PizzaDelivery.Persistence;
//using MailKit;

#region Logging
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
.Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
   .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
#endregion

var services = builder.Services;


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
services.AddAutoMapper(typeof(MappingProfile).Assembly);


builder.Services.AddControllersWithViews().
    AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

#region Options pattern 
builder.Services.AddSingleton(configuration);
services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.OptionName));
services.Configure<AdminOptions>(configuration.GetSection(AdminOptions.OptionName));
services.Configure<ConnectionStringsOptions>(configuration.GetSection(ConnectionStringsOptions.OptionName));
services.Configure<MailOptions>(configuration.GetSection(MailOptions.OptionName));
services.Configure<GoogleOptions>(configuration.GetSection(MailOptions.OptionName));
#endregion
#region DBConfig
var connectionString = builder.Configuration.GetConnectionString("SqlServer");

builder.Services.AddDbContext<PizzaDelivery.Persistence.ApplicationDbContext>(
    options => {
        //options.UseNpgsql(connectionString);
        options.UseSqlServer(connectionString, builder =>
        {
            builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        });
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        options.EnableSensitiveDataLogging();
    });
#endregion

#region Own Services
//services.AddSingleton<IWebHostEnvironment>(provider => (IWebHostEnvironment)provider.GetRequiredService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>());

builder.Services.AddTransient<IPizzaService, PizzaService>();
builder.Services.AddTransient<IPromocodeService, PromocodeService>();


builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IShoppingCartService, ShoppingCartService>();
builder.Services.AddTransient<IAuthService, AuthService>();

builder.Services.Configure<PasswordHasherOptions>(options =>
    options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2);

services.AddTransient<IMailService, MailService>();
services.AddTransient<IExternalProvider, GoogleProvider.ExternalProvider>();


#endregion

#region Authentication Roles JWT
builder.Services.AddIdentity<PizzaDelivery.Domain.Models.User.ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<PizzaDelivery.Persistence.ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options=>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.LoginPath = "/external/google-login";
                })
                .AddGoogle(options =>
                 {
                     options.ClientId = builder.Configuration.GetSection("GoogleOath:ClientId").Value;
                     options.ClientSecret = builder.Configuration.GetSection("GoogleOath:ClientSecret").Value;
                 });
/* (options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        RequireExpirationTime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
        ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Key").Value))
    };
});*/

builder.Services.AddAuthorization(options =>
{


    //options.AddPolicy("User", builderPolicy =>
    //{
    //    builderPolicy.RequireClaim(ClaimTypes.Role, "User");
    //});

    //options.AddPolicy("Admin", builderPolicy =>
    //{
    //    builderPolicy.RequireClaim(ClaimTypes.Role, "Admin");
    //});


});

#endregion




services.AddHostedService<RepeatingService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseDeveloperExceptionPage();
//app.UseSerilogRequestLogging();


app.UseMiddleware<PizzaDelivery.Application.HandleExceptions.ExceptionHandlingMiddleware>();
app.UseRouting();

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.UseStatusCodePages();


#region Roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = roleManager.Roles.ToList();
    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }


    //var userManager = scope.ServiceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
    //var authService =
    //var admin = userManager.UserManager.FindByEmailAsync("admin@service.com") ?? userManager;

}
#endregion


app.Run();
