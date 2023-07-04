using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Application.Interfaces;

public interface IShoppingCartRepository
{
    public Task<ShoppingCart> GetShoppingCartAsync();
    public Task<ICollection<ShoppingCartItem>> GetAllShoppingCartItemsAsync();
    public Task<ShoppingCartItem> AddOneToShoppingCartAsync(Guid pizzaId);
    public Task<ShoppingCartItem> RemoveOneFromShoppingCartAsync(Guid shoppingCartItemId);
    public Task<ShoppingCartItem> UpdateItemInShoppingCartAsync(Guid pizzaId, int amount);
    public Task<ICollection<ShoppingCartItem>> ClearCartAsync();
    public Task<ShoppingCart> UpdateShoppingCartTotal();

}
