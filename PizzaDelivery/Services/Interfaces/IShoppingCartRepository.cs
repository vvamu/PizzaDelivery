using PizzaDelivery.Models;

namespace PizzaDelivery.Services.Interfaces;

public interface IShoppingCartRepository
{
    public Task<ShoppingCart> GetShoppingCartAsync(Guid shoppingCartId);
    public Task<ICollection<ShoppingCartItem>> GetAllShoppingCartItemsAsync(Guid shoppingCartId);
    public Task<ShoppingCartItem> AddOneToShoppingCartAsync(Guid pizzaId);
    public Task<ShoppingCartItem> RemoveOneFromShoppingCartAsync(Guid shoppingCartItemId);
    public Task<ShoppingCartItem> UpdateItemInShoppingCartAsync(Guid pizzaId, int amount);
    public Task<ICollection<ShoppingCartItem>> ClearCartAsync(Guid shoppingCartId);
    public Task<ShoppingCart> UpdateShoppingCartTotal(Guid shoppingCartId);
    public Task<ShoppingCart> AddPromocodeToShoppingCart(Guid shoppingCartId, string promocodeValue);


}
