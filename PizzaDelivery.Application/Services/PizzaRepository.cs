using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Application.Interfaces;
using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Application.Services;

public class PizzaRepository : IPizzaRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public PizzaRepository(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;

    }
    public async Task<Pizza?> GetAsync(Guid id)
    {
        return await _context.Pizzas.FirstOrDefaultAsync(x => x.Id == id);
    }
    public async Task<ICollection<Pizza>> GetAllAsync()
    {
        return await _context.Pizzas.ToListAsync();
    }
    public async Task<Pizza> CreateAsync(PizzaCreationModel item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        //if (item.ImageFile == null || item.ImageFile.Length < 0) throw new Exception("Error with file");
        Pizza db_item = null;
            var pizza = new Pizza()
            {
                Name = item.Name,
                Ingridients = item.Ingridients,
                Price = item.Price,
                Desctiption = item.Desctiption,
            };

        if (item.ImageFile != null && item.ImageFile.Length > 0)
        {
            string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(item.ImageFile.FileName);
            string filePath = System.IO.Path.Combine(_env.WebRootPath, "/PizzaDelivery.Domain/Images", fileName);

            // Save the image file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await item.ImageFile.CopyToAsync(fileStream);
            }
            pizza.ImageUrl = fileName;
        }

        await _context.Pizzas.AddAsync(pizza);
        await _context.SaveChangesAsync();

        db_item = await _context.Pizzas.FindAsync(pizza.Id);
        return db_item == null ? throw new NotImplementedException() : db_item;

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
