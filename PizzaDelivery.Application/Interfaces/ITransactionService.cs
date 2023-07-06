using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaDelivery.Application.Interfaces;

public interface ITransactionService
{
    public Task BeginTransactionAsync();
    public Task CommitAsync();
    public Task RollbackAsync();

}
