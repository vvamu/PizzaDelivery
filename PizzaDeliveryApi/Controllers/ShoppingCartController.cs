using Microsoft.AspNetCore.Authorization;
using PizzaDelivery.Application.DTO;
using PizzaDelivery.Application.Models;
using PizzaDelivery.Application.Services.Interfaces;
using PizzaDelivery.Domain.Models;

namespace PizzaDeliveryApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "User")]

public class ShoppingCartController : ControllerBase
{
    private readonly ILogger<ShoppingCartController> _logger;
    private IShoppingCartService _context;
    public ShoppingCartController(ILogger<ShoppingCartController> logger, IShoppingCartService context)
    {
        _logger = logger;
        _context = context;
    }
    [HttpGet(Name = "GetShoppingCartAsync")]
    public async Task<ActionResult<ShoppingCartDTO>> GetShoppingCartAsync()
    {
        await _context.UpdateShoppingCartTotal();
        var db_shoppingCart = await _context.GetShoppingCartAsyncDTO();
        return db_shoppingCart == null ? BadRequest(db_shoppingCart) : Ok(db_shoppingCart);
    }

    [HttpGet("Items", Name = "GetAllShoppingCartItemsAsync")]
    public async Task<ActionResult<ICollection<ShoppingCartItemDTO>>> GetAllShoppingCartItemsAsync()
    {
        await _context.UpdateShoppingCartTotal();
        var shoppingCartItems = await _context.GetAllShoppingCartItemsAsyncDTO();
        return shoppingCartItems == null ? Ok("Shopping cart empty") : Ok(shoppingCartItems);
    }

    [HttpPost("Items/{pizzaId}", Name = "AddOneToShoppingCartAsync")]
    public async Task<ActionResult<ShoppingCartItemDTO>> AddOneToShoppingCartAsync(Guid pizzaId)
    {
        var db_shoppingCartItem = await _context.AddOneToShoppingCartAsyncDTO(pizzaId);
        return db_shoppingCartItem == null ? BadRequest(db_shoppingCartItem) : Ok(db_shoppingCartItem);
    }
    [HttpDelete("Items/{shoppingCartItemId}", Name = "RemoveOneFromShoppingCartAsync")]
    public async Task<ActionResult<ShoppingCartItemDTO>> RemoveOneFromShoppingCartAsync(Guid shoppingCartItemId)
    {
        var db_shoppingCartItem = await _context.RemoveOneFromShoppingCartAsyncDTO(shoppingCartItemId);
        return db_shoppingCartItem == null ? BadRequest(db_shoppingCartItem) : Ok(db_shoppingCartItem);
    }
    [HttpPut("Items/{pizzaId}",Name = "UpdateItemInShoppingCartAsync")]
    public async Task<ActionResult<ShoppingCartItemDTO>> UpdateItemInShoppingCartAsync(Guid pizzaId, [FromQuery] int amount)
    {
        var db_shoppingCartItem = await _context.UpdateItemInShoppingCartAsyncDTO(pizzaId, amount);
        return db_shoppingCartItem == null ? BadRequest(db_shoppingCartItem) : Ok(db_shoppingCartItem);
    }
    [HttpDelete("Clear",Name = "ClearCartAsync")]
    public async Task<ActionResult<ICollection<ShoppingCartItem>>> ClearCartAsync()
    {
        var db_shoppingCartItems = await _context.ClearCartAsync();
        return db_shoppingCartItems == null ? BadRequest(db_shoppingCartItems) : Ok(db_shoppingCartItems);
    }



}
