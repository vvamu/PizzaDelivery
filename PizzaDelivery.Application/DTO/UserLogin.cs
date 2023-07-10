using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Application.Models;

public class UserLogin
{
    public string Email { get; set; }
    public string Password { get; set; }
}
