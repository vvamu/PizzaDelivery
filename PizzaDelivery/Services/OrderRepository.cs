using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Data;
using System.Net;

namespace PizzaDelivery.Services;

public class OrderRepository : IRepository<Order>
{
    private ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Order> GetAsync(Guid id)
    {
        return await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ICollection<Order>> GetAllAsync()
    {
        return await _context.Orders.ToListAsync();

    }
    public async Task<Order> CreateAsync(Order item)
    {
        if (item == null) return null;
        //if(item.Promocode ==  null) throw new HttpResponseException(HttpStatusCode.NotFound);

        Guid newGuid = Guid.NewGuid();
        
        var correctItem = new Order()
        {
            Id = newGuid,
            PaymentType = item.PaymentType,
            OrderTotal = item.OrderTotal,
            Address = item.Address,
            DeliveryType= item.DeliveryType,
            Comment= item.Comment,
            Promocode= item.Promocode,
            User =item.User

        };

        await _context.Orders.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }
    public async Task<Order> UpdateAsync(Order item)
    {
        await Task.CompletedTask;
        var promo = _context.Orders.Update(item);
        await _context.SaveChangesAsync();

        return promo.Entity;
    }
    public async Task<Order> DeleteAsync(Guid id)
    {
        var promocode = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
        var promo = _context.Orders.Remove(promocode);
        await _context.SaveChangesAsync();

        return promo.Entity;
    }
}
