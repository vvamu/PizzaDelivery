using PizzaDelivery.Models;

namespace PizzaDelivery.Services.Interfaces;

public interface IOrderRepository
{

    public Task<ICollection<Order>> GetAllAsync();
    public Task<ICollection<Order>> GetNotDeliveredOrdersAsync();
    public Task<ICollection<Order>> GetAllByUserAsync(string userId);
    public Task<Order> GetAsync(Guid orderId);
    public Task<Order> CreateAsync (CreatedOrder item, string userId);
}
