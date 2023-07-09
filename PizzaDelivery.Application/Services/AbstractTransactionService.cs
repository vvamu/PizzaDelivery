namespace PizzaDelivery.Application.Services;
public abstract class AbstractTransactionService
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

