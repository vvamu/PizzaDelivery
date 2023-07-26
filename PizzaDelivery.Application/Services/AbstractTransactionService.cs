using Microsoft.EntityFrameworkCore.Storage;

namespace PizzaDelivery.Application.Services;
public abstract class AbstractTransactionService
{
    private readonly ApplicationDbContext _db;
    public AbstractTransactionService(ApplicationDbContext db)
    {
        _db = db;
    }
    public IExecutionStrategy CreateExecutionStrategy()
    {
        return _db.Database.CreateExecutionStrategy();
    }
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _db.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync(IDbContextTransaction transaction)
    {
        await transaction.CommitAsync();
    }

    public async Task RollbackAsync(IDbContextTransaction transaction)
    {
        await transaction.RollbackAsync();
    }

}

