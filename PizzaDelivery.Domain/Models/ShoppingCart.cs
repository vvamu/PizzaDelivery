using PizzaDelivery.Domain.Models.User;
using PizzaDelivery.Models.Interfaces;

namespace PizzaDelivery.Domain.Models;

public class ShoppingCart : BaseModel
{
    public decimal TotalPrice { get; set; }
    public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

}
