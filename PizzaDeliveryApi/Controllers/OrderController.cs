using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using PizzaDelivery.Application.DTO;
using PizzaDelivery.Application.Helpers;
using PizzaDelivery.Application.Models;
using PizzaDelivery.Application.Services.Interfaces;

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
    public ActionResult GetAllOrdersByUser([FromQuery] QueryStringParameters queryStringParameters)
    {
        var items = _context.GetAllByUserAsync(queryStringParameters);

        var metadata = new
        {
            items.TotalCount,
            items.PageSize,
            items.CurrentPage,
            items.TotalPages,
            items.HasNext,
            items.HasPrevious
        };

        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        return Ok(items);

    }

    [HttpGet("All/", Name = "GetAllOrders")]
    [Authorize(Roles = "Admin")]
    public ActionResult GetAllOrders([FromQuery] QueryStringParameters queryStringParameters)
    {
        var items = _context.GetAllAsync(queryStringParameters);

        var metadata = new
        {
            items.TotalCount,
            items.PageSize,
            items.CurrentPage,
            items.TotalPages,
            items.HasNext,
            items.HasPrevious
        };

        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        return Ok(items);
    }

    [HttpGet("{orderId}", Name = "GetOrder")]
    [Authorize(Roles = "User,Admin")]
    public async Task<ActionResult<OrderDTO>> GetOrder(Guid orderId)
    {
        var order = await _context.GetAsync(orderId);
        return order == null ? BadRequest(ModelState) : Ok(order);
    }

    [HttpPost(Name = "CreateOrder")]
    [Authorize(Roles = "User")]
    public async Task<ActionResult<OrderDTO>> Create(OrderCreationModel order)
    {
        return Ok(await _context.CreateAsync(order));
    }
    [HttpPut("AddPromocode",Name = "AddPromocodeToOrder")]
    public async Task<ActionResult<OrderDTO>> AddPromocodeToOrder(string promocodeValue, Guid orderId)
    {
        var db_shoppingCart = await _context.AddPromocodeToOrder(promocodeValue,orderId);
        return db_shoppingCart == null ? BadRequest(db_shoppingCart) : Ok(db_shoppingCart);
    }
}
