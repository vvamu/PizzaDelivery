using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PizzaDelivery.Helpers;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;
using StringLengthAttribute = System.ComponentModel.DataAnnotations.StringLengthAttribute;

namespace PizzaDelivery.Models;

public class ApplicationUser : IdentityUser
{

    public ApplicationUser() : base()
    {
        Orders = new List<Order>();
        ShoppingCart = new ShoppingCart() { User = this };
    }

    [Key]
    public override string Email { get; set; }

    public List<Order> Orders { get; set; }
    public Guid ShoppindCardId { get; set; }
    public ShoppingCart ShoppingCart { get; set; }


}
