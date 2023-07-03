using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace PizzaDelivery.Domain.Models.User;

public class ApplicationUser : IdentityUser
{

    public ApplicationUser() : base()
    {
        Orders = new List<Order>();
        ShoppingCart = new ShoppingCart() { User = this };
    }
    public List<Order> Orders { get; set; }
    public Guid ShoppindCardId { get; set; }
    public ShoppingCart ShoppingCart { get; set; }
    public string OwnHashedPassword { get; set; }


}
