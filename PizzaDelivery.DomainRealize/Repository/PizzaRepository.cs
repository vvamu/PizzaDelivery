using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.DomainRealize.Repository;

public class PizzaRepository : IRepository<Pizza>
{
    private ApplicationDbContext _context;

    public PizzaRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Pizza?> GetAsync(Guid id)
    {
        return await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<ICollection<Pizza>> GetAllAsync()
    {
        return await _context.Pizzas.ToListAsync();
    }
    public async Task<Pizza> CreateAsync(Pizza item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        await _context.Pizzas.AddAsync(item);
        var db_item = await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == item.Id);
        if (db_item == null) throw new NotImplementedException();
        await SaveChangesAsync();
        return db_item;
    }
    public async Task<Pizza> UpdateAsync(Pizza item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        var db_item = await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == item.Id);
        if (db_item == null) throw new KeyNotFoundException();
        var promo = _context.Pizzas.Update(item);
        await _context.SaveChangesAsync();
        return promo.Entity;
    }
    public async Task<Pizza> DeleteAsync(Guid id)
    {
        var db_item = await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == id);
        if (db_item == null) throw new KeyNotFoundException();
        var remotedItem = _context.Pizzas.Remove(db_item);
        await _context.SaveChangesAsync();
        return remotedItem.Entity;
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
