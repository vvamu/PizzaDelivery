using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Domain.Models;
using PizzaDelivery.Persistence.Configuration;

namespace PizzaDelivery.Persistence;

public class ApplicationDbContext : IdentityDbContext<Domain.Models.User.ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new OrderConfiguration());
        base.OnModelCreating(builder);
    }
    public DbSet<Pizza> Pizzas { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Promocode> Promocodes { get; set; }
    public DbSet<ShoppingCartItem> ShoopingCartPizzas { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<ShoppingCart> ShoppingCart { get; set; }



}