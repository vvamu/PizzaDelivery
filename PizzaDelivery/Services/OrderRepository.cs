using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Data;
using PizzaDelivery.Helpers;
using PizzaDelivery.Models.Enums;
using PizzaDelivery.Services.Interfaces;
using System.Net;

namespace PizzaDelivery.Services;

public class OrderRepository : IOrderRepository
{
    private ApplicationDbContext _context;
    private IRepository<Promocode> _promocodesRepository;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ICollection<Order>> GetAllAsync()
    {
        var NotDeliveredOrderStatus = Enum.GetName(typeof(OrderStatus), 1);
        return await _context.Orders.OrderByDescending(o => o.OrderStatus == "NotDelivered").ThenByDescending(x=>x.OrderDate).ToListAsync();
    }
    public async Task<ICollection<Order>> GetNotDeliveredOrdersAsync()
    {
        return await _context.Orders.OrderByDescending(x => x.OrderDate).Where(o => o.OrderStatus == "NotDelivered").ToListAsync();
    }

    public async Task<ICollection<Order>> GetAllByUserAsync(string userId)
    {
        return await _context.Orders.Include(x=>x.User).Where(x=>x.User.Id== userId).ToListAsync();
    }
    public async Task<Order> GetAsync(Guid orderId)
    {
        return await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
    }
    public async Task<Order> CreateAsync(CreatedOrder item, string userId)
    {
        var db_user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        var db_shoppingCart = await _context.ShoppingCart.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Id == userId);
        var totalPrice = db_shoppingCart.TotalPrice;

        if (item == null || db_user == null) return null;

        var correctItem = new Order()
        {
            PaymentType = item.PaymentType,
            Address = item.Address,
            TotalPrice = totalPrice,
            DeliveryType= item.DeliveryType,
            Comment= item.Comment,
            User = db_user,
            OrderDate= item.OrderDate,
        };

        await _context.Orders.AddAsync(correctItem);
        await _context.SaveChangesAsync();
        return correctItem;
    }


}
