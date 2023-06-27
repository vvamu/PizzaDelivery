using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Data;
using PizzaDelivery.Models;

namespace PizzaDelivery.Services;


public class PromocodeRepository : IRepository<Promocode>
{
    private ApplicationDbContext _context;

    public PromocodeRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Promocode> GetAsync(Guid id)
    {
        return await _context.Promocodes.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ICollection<Promocode>> GetAllAsync()
    {
        return await _context.Promocodes.ToListAsync();

    }
    public async Task<Promocode> CreateAsync(Promocode item)
    {
        await _context.Promocodes.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }
    public async Task<Promocode> UpdateAsync(Promocode item)
    {
        await Task.CompletedTask;
        var promo = _context.Promocodes.Update(item);
        await _context.SaveChangesAsync();

        return promo.Entity;
    }
    public async Task<Promocode> DeleteAsync(Guid id)
    {
        var promocode = await _context.Promocodes.FirstOrDefaultAsync(x => x.Id == id);
        var promo = _context.Promocodes.Remove(promocode);
        await _context.SaveChangesAsync();

        return promo.Entity;
    }




}
