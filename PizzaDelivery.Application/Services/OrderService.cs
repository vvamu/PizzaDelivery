using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Domain.Models;
using PizzaDelivery.Helpers;
using PizzaDelivery.Models.Enums;
using PizzaDelivery.Application.Interfaces;
using System.Net;
using Microsoft.AspNetCore.Identity;
using PizzaDelivery.Domain.Models.User;

namespace PizzaDelivery.Application.Services;

public class OrderService :  IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;


    public OrderService(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager= signInManager;
    }
    private ApplicationUser CurrentUser
    {
        get
        {
            var username = _signInManager.Context.User.Identity.Name;
            var current_user = _userManager.Users.FirstOrDefault(x => x.UserName == username);
            return current_user == null ? throw new Exception("Unauthorized") : current_user;
        }
    }
    public async Task<ICollection<Order>> GetAllAsync()
    {
        var NotDeliveredOrderStatus = Enum.GetName(typeof(OrderStatus), 1);
        return await _context.Orders.OrderByDescending(o => o.OrderStatus == "NotDelivered").ThenByDescending(x => x.OrderDate).ToListAsync();
    }
    public async Task<ICollection<Order>> GetNotDeliveredOrdersAsync()
    {
        return await _context.Orders.OrderByDescending(x => x.OrderDate).Where(o => o.OrderStatus == "NotDelivered").ToListAsync();
    }

    public async Task<ICollection<Order>> GetAllByUserAsync(string userId = null)
    {
        if(string.IsNullOrEmpty(userId))
        {
            var username = _signInManager.Context.User.Identity.Name;
            var current_user = await _userManager.FindByNameAsync(username);
            userId = current_user.Id;
        }
        return await _context.Orders.Include(x => x.User).Where(x => x.User.Id == userId).ToListAsync();
    }
    public async Task<Order> GetAsync(Guid orderId)
    {
        var order = await _context.Orders.Include(x=>x.User).FirstOrDefaultAsync(x => x.Id == orderId);
        var userOrders = await GetAllByUserAsync();
        if (_signInManager.Context.User.IsInRole("User") && userOrders.Any(o => o.Id == orderId))
        {
            order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            return order;
        }
        throw new Exception("No Access");
    }
    public async Task<Order> CreateAsync(OrderCreationModel item)
    {
        var db_shoppingCart = await _context.ShoppingCart.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Id == CurrentUser.Id);
        var totalPrice = db_shoppingCart.TotalPrice;
        if (totalPrice == 0) throw new Exception("Add items to create order");

        var correctItem = new Order()
        {
            PaymentType = item.PaymentType,
            Address = item.Address,
            TotalPrice = totalPrice,
            DeliveryType = item.DeliveryType,
            Comment = item.Comment,
            OrderDate = item.OrderDate,
            UserId = CurrentUser.Id,
            OrderStatus = "NotDelivered"
        };

        await _context.Orders.AddAsync(correctItem);
        await _context.SaveChangesAsync();

        var shoppingCart = await _context.ShoppingCart.Include(x=>x.User).FirstOrDefaultAsync(x => x.UserId == CurrentUser.Id);


        if (shoppingCart != null)
        {
            var cartItems = await _context.ShoopingCartPizzas
                .Where(x => x.ShoppingCartId == shoppingCart.Id)
                .ToListAsync();
            List<OrderItem> orderItems= new List<OrderItem>();
            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem()
                {
                    PizzaId = cartItem.PizzaId,
                    Amount = cartItem.Amount,
                    OrderId = correctItem.Id
                };

                orderItems.Add(orderItem);
            }
            await _context.OrderItems.AddRangeAsync(orderItems); 
            _context.ShoopingCartPizzas.RemoveRange(cartItems);
        }
        await _context.SaveChangesAsync();

        return correctItem;
    }

    public async Task<Order> AddPromocodeToOrder(string promocodeValue, Guid orderId = default)
    {
        var order = await GetAsync(orderId);
        if (!(await GetNotDeliveredOrdersAsync()).Any(x => x.Id == order.Id)) throw new Exception("Order was delivered");
        var db_promocode = await CheckCorrectPromocode(promocodeValue);
        if (order.PromocodeId != default)
        {
            var existingPromoCode = await _context.Promocodes.FindAsync(order.PromocodeId);
            var total2 = order.TotalPrice / (decimal)((double)existingPromoCode.SalePercent / (double)100);
            order.TotalPrice = total2;
        }
        order.TotalPrice = (decimal)((double)order.TotalPrice * ((double)100 -(double)db_promocode.SalePercent) / (double)100);
        order.PromocodeId = db_promocode.Id;
        try
        {
            _context.Orders.Update(order);
        }
        catch { }
        await _context.SaveChangesAsync();
        order = await GetAsync(orderId);
        return order;
    }
    public async Task<Promocode> CheckCorrectPromocode(string promocodeValue)
    {
        var promocode = await _context.Promocodes.FirstOrDefaultAsync(x => x.Value == promocodeValue);
        return promocode == null ? throw new Exception("No such promocode in database") : promocode;
    }

    public async Task<ICollection<OrderItem>> GetAllOrderItems()
    {
        return await _context.OrderItems.Include(x=>x.Order).Include(x=>x.Pizza).ToListAsync();
    }

}
