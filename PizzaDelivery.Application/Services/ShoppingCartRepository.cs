using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using PizzaDelivery.Domain.Models;
using PizzaDelivery.Application.Interfaces;
using System.Drawing;
using PizzaDelivery.Domain.Models.User;
using PizzaDelivery.DomainRealize.Repository;

namespace PizzaDelivery.Application.Services;


public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    //private readonly PizzaDelivery.DomainRealize.Interfaces.IRepository<ShoppingCart> _shoppingCartRepository;

    private ApplicationUser CurrentUser
    {
        get
        {
            var username = _signInManager.Context.User.Identity.Name;
            var current_user =  _userManager.Users.FirstOrDefault(x=>x.UserName == username);
            return current_user == null ? throw new Exception("Unauthorized") : current_user;
        }
    }
    public ShoppingCartRepository(ApplicationDbContext context,
        //PizzaDelivery.DomainRealize.Interfaces.IRepository<ShoppingCart> shoppingCartRep,
        UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        //_shoppingCartRepository = shoppingCartRep;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    public async Task<ShoppingCart> GetShoppingCartAsync(Guid shoppingCartId = default)
    {
        if (shoppingCartId == default)
        {
            var current_user = CurrentUser;
            shoppingCartId = _context.ShoppingCart.Include(x=>x.User).Where(x=>x.User.Id == current_user.Id).FirstOrDefault().Id;
        }
        var shoppingCarts = await _context.ShoppingCart.Include(x => x.ShoppingCartItems).Where(x => x.Id == shoppingCartId).ToListAsync();
        if (shoppingCarts.Count > 1) throw new Exception("More than 1 shopping cart");
        var shoppingCart = shoppingCarts.FirstOrDefault();

        return shoppingCart == null ? throw new Exception("Error with shopping cart") : shoppingCart;
    }
    public async Task<ShoppingCartItem> GetShoppingCartItemByIdAsync(Guid shoppingCartItemId)
    {
        var shoppingCart = await _context.ShoopingCartPizzas.Include(x => x.ShoppingCart).FirstOrDefaultAsync(x => x.Id == shoppingCartItemId);
        return shoppingCart == null ? throw new Exception("No shopping cart item") : shoppingCart;
    }
    public async Task<ShoppingCartItem> AddOneToShoppingCartAsync(Guid pizzaId)
    {
        return await UpdateItemInShoppingCartAsync(pizzaId);
    }

    public async Task<ShoppingCartItem> RemoveOneFromShoppingCartAsync(Guid shoppingCartItemId)
    {
        var db_shoppingCartItem = await GetShoppingCartItemByIdAsync(shoppingCartItemId);
        if (db_shoppingCartItem.Amount == 1)
        {
            _context.ShoopingCartPizzas.Remove(db_shoppingCartItem);
        }
        else
        {
            db_shoppingCartItem.Amount -= 1;
            _context.ShoopingCartPizzas.Update(db_shoppingCartItem);
        }
        await _context.SaveChangesAsync();
        await UpdateShoppingCartTotal();

        return db_shoppingCartItem;
    }

    public async Task<ShoppingCartItem> UpdateItemInShoppingCartAsync(Guid pizzaId, int amount = 1)
    {
        var userShoppingCart = await GetShoppingCartAsync();
        var shoppingCartItems = await GetAllShoppingCartItemsAsync(userShoppingCart.Id);
        var existingPizzaInCart = shoppingCartItems.FirstOrDefault(x => x.Pizza.Id == pizzaId);
        
        if (existingPizzaInCart == null)
        {
            var db_pizza = await GetPizzaById(pizzaId);
            existingPizzaInCart = new ShoppingCartItem()
            {
                Amount = amount,
                PizzaId = db_pizza.Id,
                ShoppingCartId = userShoppingCart.Id,
            };
            await _context.ShoopingCartPizzas.AddAsync(existingPizzaInCart);
        }
        else 
        {
            if (amount == 1) existingPizzaInCart.Amount += amount;
            else existingPizzaInCart.Amount = amount;
            _context.ShoopingCartPizzas.Update(existingPizzaInCart);
        }
        
        await _context.SaveChangesAsync();
        await UpdateShoppingCartTotal();

        return existingPizzaInCart;
    }
    public async Task<ICollection<ShoppingCartItem>> ClearCartAsync(Guid shoppingCartId = default)
    {
        if (shoppingCartId == default)
        {
            var username = _signInManager.Context.User.Identity.Name;
            var current_user = await _userManager.FindByNameAsync(username);
            shoppingCartId = current_user.ShoppindCardId;
        }
        var db_shoppingCartItems = await GetAllShoppingCartItemsAsync();
        _context.ShoopingCartPizzas.RemoveRange(db_shoppingCartItems);
        await _context.SaveChangesAsync();
        await UpdateShoppingCartTotal();
        return db_shoppingCartItems;
    }
    public async Task<ICollection<ShoppingCartItem>> GetAllShoppingCartItemsAsync(Guid shoppingCartId = default)
    {
        var db_shoppingCart = await GetShoppingCartAsync();
        var shoppingCartPizzas = _context.ShoopingCartPizzas
                                                        .Include(x => x.ShoppingCart)
                                                        .Include(x => x.Pizza)
                                                        .Where(x => x.ShoppingCart.Id == db_shoppingCart.Id)
                                                        .ToList();

        return shoppingCartPizzas;
    }
    public async Task<ShoppingCart> UpdateShoppingCartTotal(Guid shoppingCartId = default)
    {
        var db_shoppingCart = await GetShoppingCartAsync();
        var shoppingCartItems = await GetAllShoppingCartItemsAsync();
        var total = shoppingCartItems.Select(c => c.Pizza.Price * c.Amount).Sum();
        db_shoppingCart.TotalPrice = total;

        var updatedShoppingCart = new ShoppingCart
        {
            Id = db_shoppingCart.Id,
            TotalPrice = total,
            ShoppingCartItems = shoppingCartItems,
            UserId= db_shoppingCart.UserId,
        };


        _context.ShoppingCart.Update(db_shoppingCart);
        await _context.SaveChangesAsync();
        return db_shoppingCart;
    }
    public async Task<ShoppingCart> AddPromocodeToShoppingCart(string promocodeValue, Guid shoppingCartId = default)
    {
        var db_shoppingCart = await GetShoppingCartAsync();
        var db_promocode = await CheckCorrectPromocode(promocodeValue);
        db_shoppingCart.TotalPrice *= db_promocode.SalePercent / 100;
        _context.ShoppingCart.Update(db_shoppingCart);
        await _context.SaveChangesAsync();
        return db_shoppingCart;
    }

    public async Task<Promocode> CheckCorrectPromocode(string promocodeValue)
    {
        var promocode = await _context.Promocodes.FirstOrDefaultAsync(x => x.Value == promocodeValue);
        return promocode == null ? throw new Exception("No such promocode in database") : promocode;
    }

    private async Task<Pizza> GetPizzaById(Guid pizzaId)
    {
        var db_pizza = await _context.Pizzas.FirstOrDefaultAsync(x=>x.Id== pizzaId);
        return db_pizza == null ? throw new Exception("No such Pizza in database") : db_pizza;
    }

}
