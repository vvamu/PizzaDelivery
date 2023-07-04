using Microsoft.AspNetCore.Authorization;
using PizzaDelivery.Application.Interfaces;
using PizzaDelivery.Application.Models;
using PizzaDelivery.Domain.Models;

namespace PizzaDeliveryApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "User")]

public class ShoppingCartController : ControllerBase
{
    private readonly ILogger<ShoppingCartController> _logger;
    private IShoppingCartRepository _context;
    public ShoppingCartController(ILogger<ShoppingCartController> logger, IShoppingCartRepository context)
    {
        _logger = logger;
        _context = context;
    }
    [HttpGet(Name = "GetShoppingCartAsync")]
    public async Task<ActionResult<ShoppingCart>> GetShoppingCartAsync()
    {
        await _context.UpdateShoppingCartTotal();
        var db_shoppingCart = await _context.GetShoppingCartAsync();
        return db_shoppingCart == null ? BadRequest(db_shoppingCart) : Ok(db_shoppingCart);
    }

    [HttpGet("Items", Name = "GetAllShoppingCartItemsAsync")]
    public async Task<ActionResult<ICollection<ShoppingCartItem>>> GetAllShoppingCartItemsAsync()
    {
        await _context.UpdateShoppingCartTotal();
        var shoppingCartItems = await _context.GetAllShoppingCartItemsAsync();
        return shoppingCartItems == null ? Ok("Shopping cart empty") : Ok(shoppingCartItems);
    }

    [HttpPost("Items/{pizzaId}", Name = "AddOneToShoppingCartAsync")]
    public async Task<ActionResult<ShoppingCartItem>> AddOneToShoppingCartAsync(Guid pizzaId)
    {
        var db_shoppingCartItem = await _context.AddOneToShoppingCartAsync(pizzaId);
        return db_shoppingCartItem == null ? BadRequest(db_shoppingCartItem) : Ok(db_shoppingCartItem);
    }
    [HttpDelete("Items/{shoppingCartItemId}", Name = "RemoveOneFromShoppingCartAsync")]
    public async Task<ActionResult<ShoppingCartItem>> RemoveOneFromShoppingCartAsync(Guid shoppingCartItemId)
    {
        var db_shoppingCartItem = await _context.RemoveOneFromShoppingCartAsync(shoppingCartItemId);
        return db_shoppingCartItem == null ? BadRequest(db_shoppingCartItem) : Ok(db_shoppingCartItem);
    }
    [HttpPut("Items/{shoppingCartItemId}",Name = "UpdateItemInShoppingCartAsync")]
    public async Task<ActionResult<ShoppingCartItem>> UpdateItemInShoppingCartAsync(Guid pizzaId, int amount)
    {
        var db_shoppingCartItem = await _context.UpdateItemInShoppingCartAsync(pizzaId, amount);
        return db_shoppingCartItem == null ? BadRequest(db_shoppingCartItem) : Ok(db_shoppingCartItem);
    }
    [HttpDelete("Clear",Name = "ClearCartAsync")]
    public async Task<ActionResult<ICollection<ShoppingCartItem>>> ClearCartAsync()
    {
        var db_shoppingCartItems = await _context.ClearCartAsync();
        return db_shoppingCartItems == null ? BadRequest(db_shoppingCartItems) : Ok(db_shoppingCartItems);
    }



}
