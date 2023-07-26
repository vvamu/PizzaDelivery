using PizzaDelivery.Domain.Models.User;
using PizzaDelivery.Models.Interfaces;
using System.Security.Claims;
using System.Security.Principal;

namespace PizzaDelivery.Domain.Models;
public class ExternalConnection  : BaseModel
{
    public string Provider { get; set; }
    public string ProviderUserId { get; set; }
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    // Other properties specific to the external connection
}