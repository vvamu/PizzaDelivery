using PizzaDelivery.Application.DTO;
using PizzaDelivery.Application.Helpers;
using PizzaDelivery.Domain.Models;

namespace PizzaDelivery.Application.Services.Interfaces;

public interface IOrderService
{

    public PagedList<OrderDTO> GetAllAsync(QueryStringParameters ownerParameters);
    public PagedList<OrderDTO> GetAllByUserAsync(QueryStringParameters ownerParameters);
    public Task<OrderDTO> GetAsync(Guid orderId);
    public Task<OrderDTO> CreateAsync(OrderCreationModel item);
    public Task<OrderDTO> AddPromocodeToOrder(string promocodeValue, Guid orderId);

    public Task<ICollection<OrderItem>> GetAllOrderItems();
}
