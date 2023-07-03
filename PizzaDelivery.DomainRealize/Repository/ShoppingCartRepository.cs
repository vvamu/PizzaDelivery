//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using PizzaDelivery.Domain.Models;
//using System.Drawing;

//namespace PizzaDelivery.DomainRealize.Repository;


//public class ShoppingCartRepository : IRepository<ShoppingCart>
//{
//    private ApplicationDbContext _context;


//    public ShoppingCartRepository(ApplicationDbContext context)
//    {
//        _context = context;
//    }
//    public async Task<ShoppingCart?> GetAsync(Guid id)
//    {
//        return await _context.ShoppingCart.FirstOrDefaultAsync(x => x.Id == id);
//    }
//    public async Task<ICollection<ShoppingCart>> GetAllAsync()
//    {
//        return await _context.ShoppingCart.ToListAsync();
//    }
//    public async Task<ShoppingCart> CreateAsync(ShoppingCart item)
//    {
//        if (item == null) throw new ArgumentNullException(nameof(item));
//        await _context.ShoppingCart.AddAsync(item);
//        var db_item = await _context.ShoppingCart.FirstOrDefaultAsync(x => x.Id == item.Id);
//        if (db_item == null) throw new NotImplementedException();
//        await SaveChangesAsync();
//        return db_item;
//    }
//    public async Task<ShoppingCart> UpdateAsync(ShoppingCart item)
//    {
//        if (item == null) throw new ArgumentNullException(nameof(item));
//        var db_item = await _context.ShoppingCart.FirstOrDefaultAsync(x => x.Id == item.Id);
//        if (db_item == null) throw new KeyNotFoundException();
//        var promo = _context.ShoppingCart.Update(item);
//        await _context.SaveChangesAsync();
//        return promo.Entity;
//    }
//    public async Task<ShoppingCart> DeleteAsync(Guid id)
//    {
//        var db_item = await _context.ShoppingCart.FirstOrDefaultAsync(x => x.Id == id);
//        if (db_item == null) throw new KeyNotFoundException();
//        var remotedItem = _context.ShoppingCart.Remove(db_item);
//        await _context.SaveChangesAsync();
//        return remotedItem.Entity;
//    }
//    public async Task SaveChangesAsync()
//    {
//        await _context.SaveChangesAsync();
//    }
//}
