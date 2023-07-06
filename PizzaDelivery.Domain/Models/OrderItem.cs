using PizzaDelivery.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Domain.Models;

public class OrderItem : BaseModel 
{
    public OrderItem()
    {
        Amount = 1;
    }

    public Guid OrderId { get; set; }

    public Order Order { get; set; }

    public Guid PizzaId { get; set; }

    public Pizza Pizza { get; set; }

    [Range(0, 20, ErrorMessage = "The field {0} must be greater than {1}.")]
    public int Amount { get; set; }

}
