using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Data;
using PizzaDelivery.Models;
using PizzaDelivery.Services.Interfaces;
using System.Drawing;

namespace PizzaDelivery.Services;


public class ShoppingCartRepository : IShoppingCartRepository
{
    private ApplicationDbContext _context;

    public ShoppingCartRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ShoppingCart> GetShoppingCartAsync(Guid shoppingCartId)
    {
        return await _context.ShoppingCart.Include(x => x.ShoppingCartItems).FirstOrDefaultAsync(x => x.Id == shoppingCartId);
    }

    #region 

    #endregion
    public async Task<ShoppingCartItem> AddOneToShoppingCartAsync(Guid pizzaId)
    {
        var userShoppingCart = await _context.Users.Include(x => x.ShoppingCart).Select(x => x.ShoppingCart).FirstOrDefaultAsync();
        var db_pizza = await _context.Pizzas.FindAsync(pizzaId);

        if (db_pizza == null) return null;

        return await UpdateItemInShoppingCartAsync(pizzaId);
    }

    public async Task<ShoppingCartItem> RemoveOneFromShoppingCartAsync(Guid shoppingCartItemId)
    {

        var db_shoppingCartItem = await _context.ShoopingCartPizzas.Include(x=>x.ShoppingCart).FirstOrDefaultAsync(x=>x.Id==shoppingCartItemId);
        if (db_shoppingCartItem == null) return null;

        if (db_shoppingCartItem.Amount == 1)
        {
            _context.ShoopingCartPizzas.Remove(db_shoppingCartItem);        
        }
        else
            db_shoppingCartItem.Amount -= 1;
        await _context.SaveChangesAsync();
        await UpdateShoppingCartTotal(db_shoppingCartItem.ShoppingCart.Id);

        return db_shoppingCartItem;
    }

    public async Task<ShoppingCartItem> UpdateItemInShoppingCartAsync(Guid pizzaId, int amount = 1)
    {
        var userShoppingCart = await _context.Users.Include(x => x.ShoppingCart).Select(x => x.ShoppingCart).FirstOrDefaultAsync();

        var existingPizzaInCart = GetAllShoppingCartItemsAsync(userShoppingCart.Id).Result.Where(x => Convert.ToBoolean(x.Pizza.Id.CompareTo(pizzaId))).FirstOrDefault();
        if (existingPizzaInCart == null)
        {
            var db_pizza = await _context.Pizzas.FindAsync(pizzaId);
            if (db_pizza == null) return null;
            existingPizzaInCart = new ShoppingCartItem()
            {
                Amount = amount,
                Pizza = db_pizza,
                ShoppingCart = userShoppingCart
            };
            await _context.ShoopingCartPizzas.AddAsync(existingPizzaInCart);
        }
        else
        {
            if (amount == 1) existingPizzaInCart.Amount += amount;
            else existingPizzaInCart.Amount = amount;
        }
        await _context.SaveChangesAsync();
        await UpdateShoppingCartTotal(userShoppingCart.Id);

        return existingPizzaInCart;
    }
    public async Task<ICollection<ShoppingCartItem>> ClearCartAsync(Guid shoppingCartId)
    {
        var db_shoppingCartItems = await GetAllShoppingCartItemsAsync(shoppingCartId);
        _context.ShoopingCartPizzas.RemoveRange(db_shoppingCartItems);
        await _context.SaveChangesAsync();
        await UpdateShoppingCartTotal(shoppingCartId);
        return db_shoppingCartItems;
    }


    public async Task<ICollection<ShoppingCartItem>> GetAllShoppingCartItemsAsync(Guid shoppingCartId)
    {
        var db_shoppingCart = await GetShoppingCartAsync(shoppingCartId);
        if (db_shoppingCart == null) return null;
        var shoppingCartPizzas = _context.ShoopingCartPizzas.Include(x => x.ShoppingCart).Include(x=>x.Pizza).
            Where(x => Convert.ToBoolean(x.ShoppingCart.Id.CompareTo(db_shoppingCart.Id)));
        
        return (ICollection<ShoppingCartItem>)shoppingCartPizzas;
    }



    public async Task<ShoppingCart> UpdateShoppingCartTotal(Guid shoppingCartId)
    {

        var db_shoppingCart = await GetShoppingCartAsync(shoppingCartId);
        if (db_shoppingCart == null) return null;

        var total = GetAllShoppingCartItemsAsync(shoppingCartId).Result.ToList()
    .Select(c => c.Pizza.Price * c.Amount).Sum();
        db_shoppingCart.TotalPrice = total;
        await _context.SaveChangesAsync();

        return db_shoppingCart;
    }




    public async Task<ShoppingCart> AddPromocodeToShoppingCart(Guid shoppingCartId, string promocodeValue)
    {
        var db_shoppingCart = await _context.ShoppingCart.Where(x => x.Id == shoppingCartId).FirstOrDefaultAsync();
        if (db_shoppingCart == null) return null;
        var db_promocode = await CheckCorrectPromocode(promocodeValue);
        db_shoppingCart.TotalPrice *= db_promocode.SalePercent / 100;
        await _context.SaveChangesAsync();
        return db_shoppingCart;

    }

    public async Task<Promocode> CheckCorrectPromocode(string promocodeValue)
    {
        return await _context.Promocodes.FirstOrDefaultAsync(x => x.Value == promocodeValue);
    }

}
