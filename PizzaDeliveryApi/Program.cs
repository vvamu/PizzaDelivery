global using PizzaDelivery.Models;
global using PizzaDelivery.Services;
global using Microsoft.AspNetCore.Mvc;



using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Data;
using PizzaDeliveryApi;
using PizzaDeliveryApi.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = builder.Configuration.GetConnectionString("SqliteConnection");
//var c = new Microsoft.Data.Sqlite.SqliteConnection($"DataSource={ApplicationDbContext.DbPath}");
builder.Services.AddDbContext<ApplicationDbContext>(
    options => {
        options.UseSqlite(connectionString);
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });

builder.Services.AddTransient<IRepository<Pizza>, PizzaRepository>();
builder.Services.AddTransient<IRepository<Order>, OrderRepository>();
builder.Services.AddTransient<IRepository<Promocode>, PromocodeRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDeveloperExceptionPage();
app.UseAuthorization();

app.MapControllers();

app.Run();
