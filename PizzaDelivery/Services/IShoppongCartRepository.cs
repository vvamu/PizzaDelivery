using PizzaDelivery.Models;

namespace PizzaDelivery.Services;

public interface IShoppongCartRepository 
{
    public Task<ShoppingCart> GetCart(IServiceProvider services);

    public Task<ICollection<Pizza>> AddToShoppingCartAsync(ShoppingCart shoppingCart, Pizza pizza, int amount);
    public Task<Pizza> AddOneToShoppingCartAsync(ShoppingCart shoppingCart, Pizza pizza);

    public Task<Pizza> RemoveOneFromShoppingCartAsync(ShoppingCart shoppingCart, Pizza pizza);
    public Task ClearCartAsync();
    public Task<List<ShoopingCartPizzas>> GetShoppingCartPizzasAsync();
    public decimal GetShoppingCartTotal();

}
