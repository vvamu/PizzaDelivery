using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Application.Interfaces;

public interface IOrderService
{

    public Task<ICollection<Order>> GetAllAsync();
    public Task<ICollection<Order>> GetNotDeliveredOrdersAsync();
    public Task<ICollection<Order>> GetAllByUserAsync(string userId = null);
    public Task<Order> GetAsync(Guid orderId);
    public Task<Order> CreateAsync (OrderCreationModel item);
    public Task<Order> AddPromocodeToOrder(string promocodeValue, Guid orderId);

    public Task<ICollection<OrderItem>> GetAllOrderItems();
}
