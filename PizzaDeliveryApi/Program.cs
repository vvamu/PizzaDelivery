global using PizzaDelivery.Models;
global using PizzaDelivery.Services;
global using PizzaDelivery.Domain.Models;

global using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using PizzaDeliveryApi;
using PizzaDeliveryApi.Controllers;
using PizzaDelivery.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using PizzaDelivery.Domain.Models;


using Serilog;
using PizzaDelivery.Application.Services;


var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<WebApplication>();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
//var logger = loggerFactory.CreateLogger<WebApplication>();


#region DBConfig
var connectionString = builder.Configuration.GetConnectionString("SqliteConnection");
builder.Services.AddDbContext<PizzaDelivery.Persistence.ApplicationDbContext>(
    options => {
        options.UseSqlite(connectionString);
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        options.EnableSensitiveDataLogging();
    });
#endregion

#region Own Services
builder.Services.AddTransient<IPizzaRepository, PizzaRepository>();
builder.Services.AddTransient<IPromocodeRepository, PromocodeRepository>();


builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IShoppingCartRepository, ShoppingCartRepository>();
builder.Services.AddTransient<IAuthService, AuthService>();

builder.Services.Configure<PasswordHasherOptions>(options =>
    options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2
);

#endregion

#region Authentication Roles JWT
builder.Services.AddIdentity<PizzaDelivery.Domain.Models.User.ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<PizzaDelivery.Persistence.ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication();/* (options =>
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



IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.AddSingleton<IConfiguration>(configuration);


builder.Services.AddControllersWithViews().
    AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);


builder.Host.UseSerilog((context, configuration) =>
    configuration
    .WriteTo.Console()
    .MinimumLevel.Information());

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseExceptionHandler("/error-local-development");
}
else
{
    //app.UseExceptionHandler("/error");
}


//app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRouting();

app.UseHttpsRedirection();
app.UseDeveloperExceptionPage();


app.UseAuthentication();
app.UseAuthorization();
app.UseSerilogRequestLogging();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.UseStatusCodePages();

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

app.Run();
