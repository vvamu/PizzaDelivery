using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Data;

namespace PizzaDelivery.Services;

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

    [HttpGet]
    public async Task<ICollection<Pizza>> GetAllAsync()
    {
        return await _context.Pizzas.ToListAsync();

    }

    [HttpPost]
    public async Task<Pizza> CreateAsync(Pizza item)
    {
        if (item == null) return null;

        Guid newGuid = Guid.NewGuid();
        var correctItem = new Pizza()
        {
            Id = newGuid,
            Name = item.Name,
            Ingridients = item.Ingridients,
            Price = item.Price,
            Desctiption = item.Desctiption,
            ImagePath = item.ImagePath
        };
        await _context.Pizzas.AddAsync(correctItem);
        await _context.SaveChangesAsync();
        return correctItem;
    }
    public async Task<Pizza> UpdateAsync(Pizza item)
    {
        if (item == null) return null;
        var pizza = await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == item.Id);
        if (pizza == null) return null;

        var promo = _context.Pizzas.Update(item);
        await _context.SaveChangesAsync();

        return promo.Entity;
    }
    public async Task<Pizza> DeleteAsync(Guid id)
    {
        var pizza = await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == id);
        if (pizza == null) return null;
        var remotedPizza = _context.Pizzas.Remove(pizza);
        await _context.SaveChangesAsync();

        return remotedPizza.Entity;
    }
}
