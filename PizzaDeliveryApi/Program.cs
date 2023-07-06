global using PizzaDelivery.Domain.Models;
global using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Serilog;
using PizzaDelivery.Application.Services;
using PizzaDelivery.Application.Options;
using PizzaDelivery.Application.Helpers;
using PizzaDeliveryApi.Services;
using MongoDB.Driver;

//var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
//var logger = loggerFactory.CreateLogger<WebApplication>();

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllersWithViews().
    AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

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
services.AddSingleton<IWebHostEnvironment>(provider => (IWebHostEnvironment)provider.GetRequiredService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>());

builder.Services.AddTransient<IPizzaService, PizzaService>();
builder.Services.AddTransient<IPromocodeService, PromocodeService>();


builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IShoppingCartService, ShoppingCartRepository>();
builder.Services.AddTransient<IAuthService, AuthService>();

builder.Services.AddTransient<IDefalutDbContent,DefaultDbContent>();

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


#region Options pattern 
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
.Build();

builder.Services.AddSingleton(configuration);
services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.OptionName));
services.Configure<AdminOptions>(configuration.GetSection(AdminOptions.OptionName));
services.Configure<ConnectionStringsOptions>(configuration.GetSection(ConnectionStringsOptions.OptionName));
#endregion


var log = new LoggerConfiguration()
    .WriteTo.MongoDBBson("mongodb://localhost:27017/serilog")
    .CreateLogger();


try
{

    // Log a test message
    log.Information("Test log message");

    // Flush the log events to ensure they are written to MongoDB
    log.Dispose();
}
catch (Exception ex)
{
    // Handle connection error
    Console.WriteLine($"Failed to connect to MongoDB: {ex.Message}");
}

//builder.Host.UseSerilog((context, configuration) =>
//    configuration
//    .MinimumLevel.Information()
//    .ReadFrom.Configuration(context.Configuration)) ;

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseMiddleware<PizzaDelivery.Application.HandleExceptions.ExceptionHandlingMiddleware>();
app.UseRouting();

app.UseHttpsRedirection();
app.UseDeveloperExceptionPage();


app.UseAuthentication();
app.UseAuthorization();
//app.UseSerilogRequestLogging();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.UseStatusCodePages();


var repitingService = new RepeatingService(log, app.Services.GetService<IServiceScopeFactory>());
repitingService.StartAsync(CancellationToken.None);

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
