using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Server.HttpSys;
using System.Security.Principal;

namespace PizzaDelivery.Domain.Models.User;

public class ApplicationUser : IdentityUser
{
    public List<Order> Orders { get; set; } = new List<Order>();
    public Guid ShoppindCardId { get; set; }
    public ShoppingCart ShoppingCart { get; set; }
    public string OwnHashedPassword { get; set; }
    public ICollection<ExternalConnection> ExternalConnections { get; set; } = new List<ExternalConnection>();

}


