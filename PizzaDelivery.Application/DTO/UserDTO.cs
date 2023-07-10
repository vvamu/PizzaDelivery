using PizzaDelivery.Models.Interfaces;

namespace PizzaDelivery.Application.Models;

public class UserDTO : BaseModel
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
