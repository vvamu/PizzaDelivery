using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Application.Interfaces;

public interface IShoppingCartRepository
{
    public Task<ShoppingCart> GetShoppingCartAsync(Guid shoppingCartId = default);
    public Task<ICollection<ShoppingCartItem>> GetAllShoppingCartItemsAsync(Guid shoppingCartId = default);
    public Task<ShoppingCartItem> AddOneToShoppingCartAsync(Guid pizzaId);
    public Task<ShoppingCartItem> RemoveOneFromShoppingCartAsync(Guid shoppingCartItemId);
    public Task<ShoppingCartItem> UpdateItemInShoppingCartAsync(Guid pizzaId, int amount);
    public Task<ICollection<ShoppingCartItem>> ClearCartAsync(Guid shoppingCartId = default);
    public Task<ShoppingCart> UpdateShoppingCartTotal(Guid shoppingCartId = default);
    public Task<ShoppingCart> AddPromocodeToShoppingCart(string promocodeValue, Guid shoppingCartId = default);


}
