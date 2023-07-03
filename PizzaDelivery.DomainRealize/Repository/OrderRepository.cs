using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Domain.Models;
using PizzaDelivery.Helpers;
using PizzaDelivery.Models.Enums;
using System.Net;

namespace PizzaDelivery.DomainRealize.Repository;

public class OrderRepository : IRepository<Order>
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
        return await _context.Orders.OrderByDescending(o => o.OrderStatus == "NotDelivered").ThenByDescending(x => x.OrderDate).ToListAsync();
    }
    public async Task<Order?> GetAsync(Guid orderId)
    {
        return await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
    }
    public async Task<Order?> CreateAsync(Order item)
    {
        if(item == null) throw new ArgumentNullException(nameof(item));
        await _context.Orders.AddAsync(item);
        var db_item = await _context.Orders.FirstOrDefaultAsync(x => x.Id == item.Id);
        if (db_item == null) throw new KeyNotFoundException();
        await SaveChangesAsync();
        return db_item;
    }

    public async Task<Order?> UpdateAsync(Order item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        var db_item = await _context.Orders.FirstOrDefaultAsync(x => x.Id == item.Id);
        if (db_item == null) throw new KeyNotFoundException();
        var promo = _context.Orders.Update(item);
        await _context.SaveChangesAsync();
        return promo.Entity;

    }

    public async Task<Order?> DeleteAsync(Guid id)
    {
        var db_item = await _context.Orders.FirstOrDefaultAsync(x=>x.Id == id);
        if (db_item == null) throw new KeyNotFoundException();
        var remotedItem = _context.Orders.Remove(db_item);
        await _context.SaveChangesAsync();
        return remotedItem.Entity;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
