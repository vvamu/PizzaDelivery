using Microsoft.AspNetCore.Authorization;
using PizzaDelivery.Application.Interfaces;
using PizzaDelivery.Application.Models;

namespace PizzaDeliveryApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{

    private readonly ILogger<OrdersController> _logger;
    private IOrderService _context;
    public OrdersController(ILogger<OrdersController> logger, IOrderService context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet(Name = "GetAllOrdersByUser")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<ICollection<Order>>> GetAllOrdersByUser(int page = 1, int pageSize = 5)
    {
        if (page < 1 || pageSize < 1) throw new ArgumentOutOfRangeException();
        var items = await _context.GetAllByUserAsync();
        var totalCount = items.Count;
        var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
        if (page > totalPages) throw new Exception("With this size of page the last page number - " + totalPages);
        var itemPerPage = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Ok(itemPerPage);
    }

    [HttpGet("All/", Name = "GetAllOrders")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ICollection<Order>>> GetAllOrders(int page = 1, int pageSize = 5)
    {
        if (page < 1 || pageSize < 1) throw new ArgumentOutOfRangeException();
        var items = await _context.GetAllAsync();
        var totalCount = items.Count;
        var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
        if (page > totalPages) throw new Exception("With this size of page the last page number - " + totalPages);
        var itemPerPage = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Ok(itemPerPage);
    }

    [HttpGet("{orderId}", Name = "GetOrder")]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult<Order>> GetOrder(Guid orderId)
    {
        var order = await _context.GetAsync(orderId);
        return order == null ? BadRequest(ModelState) : Ok(order);
    }

    [HttpPost(Name = "CreateOrder")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<Order>> Create(OrderCreationModel order)
    {
        return Ok(await _context.CreateAsync(order));
    }
    [HttpPut("AddPromocode",Name = "AddPromocodeToOrder")]
    public async Task<ActionResult<Order>> AddPromocodeToOrder(string promocodeValue, Guid orderId)
    {
        var db_shoppingCart = await _context.AddPromocodeToOrder(promocodeValue,orderId);
        return db_shoppingCart == null ? BadRequest(db_shoppingCart) : Ok(db_shoppingCart);
    }
}
