using PizzaDelivery.Application.Interfaces;
using System.Transactions;


public abstract class AbstractTransactionService : ITransactionService
{
    private readonly ApplicationDbContext _db;
    public AbstractTransactionService(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task BeginTransactionAsync()
    {
        await _db.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _db.Database.CommitTransactionAsync();
    }

    public async Task RollbackAsync()
    {
        await _db.Database.BeginTransactionAsync();
    }

}

