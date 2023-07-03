using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Application.Interfaces;

public interface IOrderRepository
{

    public Task<ICollection<Order>> GetAllAsync();
    public Task<ICollection<Order>> GetNotDeliveredOrdersAsync();
    public Task<ICollection<Order>> GetAllByUserAsync(string userId = null);
    public Task<Order> GetAsync(Guid orderId);
    public Task<Order> CreateAsync (CreatedOrder item, string userId = null);
}
