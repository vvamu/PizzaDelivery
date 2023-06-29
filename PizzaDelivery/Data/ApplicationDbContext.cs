using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Models;

namespace PizzaDelivery.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    //public static string DbPath
    //{
    //    get
    //    {
    //        var folder = Environment.SpecialFolder.LocalApplicationData;
    //        var path = Environment.GetFolderPath(folder);

    //        var directory = Directory.CreateDirectory(Path.Join(path, "LiteDb"));
    //        var db = Path.Join(directory.FullName, "LiteDb.db");

    //        return db;
    //    }
    //}
    //protected override void OnConfiguring(DbContextOptionsBuilder options)
    //    => options.UseSqlite($"DataSource={DbPath}");

    public DbSet<Pizza> Pizzas { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Promocode> Promocodes { get; set; }
    public DbSet<ShoppingCartItem> ShoopingCartPizzas { get; set; }
    public DbSet<ShoppingCart> ShoppingCart { get; set; }



}