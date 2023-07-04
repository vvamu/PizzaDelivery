using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Application.Interfaces;
using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Application.Services;


public class PromocodeRepository : IPromocodeRepository
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
    public async Task<Promocode> CreateAsync(PromocodeCreationModel item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        var promocode = new Promocode()
        {
            Value = item.Value,
            ExpireDate = item.ExpireDate,
            SalePercent = item.SalePercent,
        };
        await _context.Promocodes.AddAsync(promocode);
        await _context.SaveChangesAsync();
        var db_item = await _context.Promocodes.FindAsync(promocode.Id);
        return db_item == null ? throw new NotImplementedException() : db_item;
    }
    public async Task<Promocode> UpdateAsync(Promocode item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        var db_item = await _context.Promocodes.FirstOrDefaultAsync(x => x.Id == item.Id);
        if (db_item == null) throw new KeyNotFoundException();
        var promo = _context.Promocodes.Update(item);
        await _context.SaveChangesAsync();
        return promo.Entity;
    }
    public async Task<Promocode> DeleteAsync(Guid id)
    {
        var db_item = await _context.Promocodes.FirstOrDefaultAsync(x => x.Id == id);
        if (db_item == null) throw new KeyNotFoundException();
        var remotedItem = _context.Promocodes.Remove(db_item);
        await _context.SaveChangesAsync();
        return remotedItem.Entity;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }


}
