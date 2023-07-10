using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Application.Models;

public class UserRegister
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
