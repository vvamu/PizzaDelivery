using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using PizzaDelivery.Domain.Models;
using System.Drawing;
using PizzaDelivery.Domain.Models.User;
using PizzaDelivery.Application.Services.Interfaces;
using AutoMapper;
using PizzaDelivery.Application.DTO;

namespace PizzaDelivery.Application.Services.Implementation;


public class ShoppingCartService : IShoppingCartService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMapper _mapper;
    private ApplicationUser CurrentUser
    {
        get
        {
            var user = _userManager.SupportsUserClaim;

            var username = _signInManager.Context.User.Identity.Name;
            var current_user = _userManager.Users.FirstOrDefault(x => x.UserName == username);
            return current_user == null ? throw new Exception("Unauthorized") : current_user;
        }
    }

    public ShoppingCartService(ApplicationDbContext context,
        UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        IMapper mapper)
    {
        _context = context;
        //_shoppingCartRepository = shoppingCartRep;
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
    }

    public async Task<ShoppingCartDTO> GetShoppingCartAsyncDTO()
    {
        var current_user = CurrentUser;
        var shoppingCartId = _context.ShoppingCart.Include(x => x.User).Where(x => x.User.Id == current_user.Id).FirstOrDefault().Id;

        var shoppingCarts = await _context.ShoppingCart.Include(x => x.ShoppingCartItems).Where(x => x.Id == shoppingCartId).ToListAsync();
        if (shoppingCarts.Count > 1) throw new Exception("More than 1 shopping cart");
        var shoppingCart = shoppingCarts.FirstOrDefault();

        return shoppingCart == null ? throw new Exception("Error with shopping cart") : _mapper.Map<ShoppingCartDTO>(shoppingCart);
    }
    public async Task<ShoppingCart> GetShoppingCartAsync()
    {
        var current_user = CurrentUser;
        var shoppingCartId = _context.ShoppingCart.Include(x => x.User).Where(x => x.User.Id == current_user.Id).FirstOrDefault().Id;

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
    public async Task<ShoppingCartItemDTO> AddOneToShoppingCartAsyncDTO(Guid pizzaId)
    {
        return await UpdateItemInShoppingCartAsyncDTO(pizzaId);
    }

    public async Task<ShoppingCartItem> AddOneToShoppingCartAsync(Guid pizzaId)
    {
        return await UpdateItemInShoppingCartAsync(pizzaId);
    }

    public async Task<ShoppingCartItemDTO> RemoveOneFromShoppingCartAsyncDTO(Guid shoppingCartItemId)
    {

        var item = RemoveOneFromShoppingCartAsync(shoppingCartItemId);
        return _mapper.Map<ShoppingCartItemDTO>(item);
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
    public async Task<ShoppingCartItemDTO> UpdateItemInShoppingCartAsyncDTO(Guid pizzaId, int amount = 1)
    {
        var item = await UpdateItemInShoppingCartAsync(pizzaId, amount);
        return _mapper.Map<ShoppingCartItemDTO>(item);
    }

    public async Task<ShoppingCartItem> UpdateItemInShoppingCartAsync(Guid pizzaId, int amount = 1)
    {
        if(amount<0) throw new ArgumentException(nameof(amount));
        var userShoppingCart = await GetShoppingCartAsync();
        var shoppingCartItems = await GetAllShoppingCartItemsAsync();
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
    public async Task<ICollection<ShoppingCartItem>> ClearCartAsync()
    {
        var db_shoppingCartItems = await GetAllShoppingCartItemsAsync();
        _context.ShoopingCartPizzas.RemoveRange(db_shoppingCartItems);
        await _context.SaveChangesAsync();
        await UpdateShoppingCartTotal();
        db_shoppingCartItems = await GetAllShoppingCartItemsAsync();

        return db_shoppingCartItems;
    }

    public async Task<ICollection<ShoppingCartItemDTO>> GetAllShoppingCartItemsAsyncDTO()
    {

        var shoppingCartPizzasDTO = (await GetAllShoppingCartItemsAsync()).Select(x => _mapper.Map<ShoppingCartItemDTO>(x)).ToList();
        return shoppingCartPizzasDTO;
    }
    public async Task<ICollection<ShoppingCartItem>> GetAllShoppingCartItemsAsync()
    {
        var db_shoppingCart = await GetShoppingCartAsync();
        var shoppingCartPizzas = _context.ShoopingCartPizzas
                                                        .Include(x => x.ShoppingCart)
                                                        .Include(x => x.Pizza)
                                                        .Where(x => x.ShoppingCart.Id == db_shoppingCart.Id)
                                                        .ToList();

        return shoppingCartPizzas;
    }
    public async Task<ShoppingCart> UpdateShoppingCartTotal()
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
            UserId = db_shoppingCart.UserId,
        };

        try
        {
            _context.ShoppingCart.Update(updatedShoppingCart);
        }
        catch (Exception ex) { }
        await _context.SaveChangesAsync();
        return db_shoppingCart;
    }
    private async Task<Pizza> GetPizzaById(Guid pizzaId)
    {
        var db_pizza = await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == pizzaId);
        return db_pizza == null ? throw new Exception("No such Pizza in database") : db_pizza;
    }

}
