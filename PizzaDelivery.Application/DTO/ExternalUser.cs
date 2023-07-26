

using PizzaDelivery.Models.Interfaces;

namespace PizzaDelivery.Application.DTO;

public class ExternalUser 
{
    public string Id { get; set; }
    public string? Email { get; set; }
    public string? First_Name { get; set; }
    public string? Last_Name { get; set; }
    public bool? Can_Access_Closed { get; set; }
    public bool? Is_Closed { get; set; }

}
