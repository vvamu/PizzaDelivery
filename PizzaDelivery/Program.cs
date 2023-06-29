global using PizzaDelivery.Models;
global using PizzaDelivery.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using PizzaDelivery.Data;
using Microsoft.Extensions.Options;
using PizzaDelivery.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

/*
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Sorce= ../LiteDb/LiteDb.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
*/


var connectionString = builder.Configuration.GetConnectionString("SqliteConnection");
//var c = new Microsoft.Data.Sqlite.SqliteConnection($"DataSource={ApplicationDbContext.DbPath}");
builder.Services.AddDbContext<ApplicationDbContext>(
    options => {
        options.UseSqlite(connectionString);
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });


builder.Services.AddTransient<IRepository<Pizza>, PizzaRepository>();
builder.Services.AddTransient<IRepository<Promocode>, PromocodeRepository>();


builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IShoppingCartRepository, ShoppingCartRepository>();
builder.Services.AddTransient<IAuthService, AuthService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 5;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();

builder.Logging.AddConsole();


var app = builder.Build();

var logger = app.Services.GetService<ILogger<RepeatingService>>();
var service = new RepeatingService(logger,app.Services.GetService<IRepository<Promocode>>());
service.StartAsync(CancellationToken.None);

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
