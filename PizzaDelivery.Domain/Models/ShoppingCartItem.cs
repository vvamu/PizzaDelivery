using PizzaDelivery.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.Domain.Models;

public class ShoppingCartItem : BaseModel
{
    public ShoppingCartItem()
    {
        Amount = 1;
    }

    public Guid ShoppingCartId { get; set; }

    public ShoppingCart ShoppingCart { get; set; }

    public Guid PizzaId { get; set; }

    public Pizza Pizza { get; set; }

    [Range(0, 20, ErrorMessage = "The field {0} must be greater than {1}.")]
    public int Amount { get; set; }

}
