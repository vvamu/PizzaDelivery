global using PizzaDelivery.Models;
global using PizzaDelivery.Domain.Models;
global using PizzaDelivery.Persistence;
global using PizzaDelivery.Domain.Models.User;


using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PizzaDelivery.Domain.Models;
using PizzaDelivery.Application.Services.Implementation;
using PizzaDelivery.Application.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

/*
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Sorce= ../LiteDb/LiteDb.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
*/


var connectionString = builder.Configuration.GetConnectionString("SqliteConnection");
builder.Services.AddDbContext<ApplicationDbContext>(
    options => {
        options.UseSqlite(connectionString);
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });

builder.Services.AddTransient<IPizzaService, PizzaService>();
builder.Services.AddTransient<IPromocodeService, PromocodeService>();


builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IShoppingCartService, ShoppingCartRepository>();
builder.Services.AddTransient<IAuthService, AuthService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages();


builder.Services.AddIdentity<ApplicationUser,IdentityRole>(options =>
    {
        options.Password.RequiredLength = 5;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

builder.Logging.AddConsole();


var app = builder.Build();

//var logger = app.Services.GetService<ILogger<RepeatingService>>();
//var service = new RepeatingService(logger,app.Services.GetService<IServiceScopeFactory>());
//service.StartAsync(CancellationToken.None);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
app.MapRazorPages();

app.Run();
