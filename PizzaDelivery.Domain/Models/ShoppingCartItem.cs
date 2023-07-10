using PizzaDelivery.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Domain.Models;

public class ShoppingCartItem : BaseModel
{
    public Guid ShoppingCartId { get; set; }
    public ShoppingCart ShoppingCart { get; set; }
    public Guid PizzaId { get; set; }
    public Pizza Pizza { get; set; }
    public int Amount { get; set; } = 1;

}
