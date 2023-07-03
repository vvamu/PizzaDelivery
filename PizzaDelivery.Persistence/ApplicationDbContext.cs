using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Persistence;

public class ApplicationDbContext : IdentityDbContext<Domain.Models.User.ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Pizza> Pizzas { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Promocode> Promocodes { get; set; }
    public DbSet<ShoppingCartItem> ShoopingCartPizzas { get; set; }
    public DbSet<ShoppingCart> ShoppingCart { get; set; }



}