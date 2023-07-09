using PizzaDelivery.Application.Helpers;
using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Application.Services.Interfaces;

public interface IOrderService
{

    public PagedList<Order> GetAllAsync(QueryStringParameters ownerParameters);
    public PagedList<Order> GetAllByUserAsync(QueryStringParameters ownerParameters);
    public Task<ICollection<Order>> GetNotDeliveredOrdersAsync();
    public Task<Order> GetAsync(Guid orderId);
    public Task<Order> CreateAsync(OrderCreationModel item);
    public Task<Order> AddPromocodeToOrder(string promocodeValue, Guid orderId);

    public Task<ICollection<OrderItem>> GetAllOrderItems();
}
