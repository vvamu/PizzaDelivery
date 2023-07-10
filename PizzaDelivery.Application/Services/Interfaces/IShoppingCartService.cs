using PizzaDelivery.Application.DTO;
using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Application.Services.Interfaces;

public interface IShoppingCartService
{
    public Task<ShoppingCartDTO> GetShoppingCartAsyncDTO();
    public Task<ShoppingCart> GetShoppingCartAsync();
    public Task<ICollection<ShoppingCartItemDTO>> GetAllShoppingCartItemsAsyncDTO();
    public Task<ICollection<ShoppingCartItem>> GetAllShoppingCartItemsAsync();

    public Task<ShoppingCartItemDTO> AddOneToShoppingCartAsyncDTO(Guid pizzaId);
    public Task<ShoppingCartItem> AddOneToShoppingCartAsync(Guid pizzaId);

    public Task<ShoppingCartItemDTO> RemoveOneFromShoppingCartAsyncDTO(Guid shoppingCartItemId);
    public Task<ShoppingCartItemDTO> UpdateItemInShoppingCartAsyncDTO(Guid pizzaId, int amount);


    public Task<ShoppingCartItem> RemoveOneFromShoppingCartAsync(Guid shoppingCartItemId);
    public Task<ShoppingCartItem> UpdateItemInShoppingCartAsync(Guid pizzaId, int amount);
    public Task<ICollection<ShoppingCartItem>> ClearCartAsync();
    public Task<ShoppingCart> UpdateShoppingCartTotal();

}
