using PizzaDelivery.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Domain.Models;

public class OrderItem : BaseModel 
{
    public Guid OrderId { get; set; }

    public Order Order { get; set; }

    public Guid PizzaId { get; set; }

    public Pizza Pizza { get; set; }
    public int Amount { get; set; } = 1;

}
