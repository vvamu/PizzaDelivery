//using Microsoft.AspNetCore.Authorization;
//using PizzaDelivery.Application.Interfaces;
//using PizzaDelivery.Application.Models;

//namespace PizzaDeliveryApi.Controllers;

//[ApiController]
//[Route("[controller]")]
//[Authorize]
//public class OrderController : ControllerBase
//{

//    private readonly ILogger<OrderController> _logger;
//    private IOrderRepository _context;
//    public OrderController(ILogger<OrderController> logger, IOrderRepository context)
//    {
//        _logger = logger;
//        _context = context;
//    }

//    [HttpGet("/User/",Name = "GetAllOrdersByUser")]
//    [Authorize(Roles = "User")]
//    public async Task<ActionResult<ICollection<Order>>> GetAllOrdersByUser()
//    {
//        var h = _context.GetAllAsync();
//        return Ok(h);
//    }

//    [HttpGet("/All/", Name = "GetAllOrders")]
//    [Authorize(Roles = "Admin")]
//    public async Task<ActionResult<ICollection<Order>>> GetAll()
//    {
//        var h = _context.GetAllAsync();
//        return Ok(h);
//    }

//    [HttpGet("/User/",Name ="{id}")]
//    [Authorize(Roles = "User")]
//    public async Task<ActionResult<Order>> GetOwn(Guid id)
//    {
//        var order = await _context.GetAsync(id);
//        var userOrders = await _context.GetAllByUserAsync();


//        return order == null ? BadRequest(order) : Ok(order);
//    }

//    [HttpPost("/User/",Name = "CreateOrder")]
//    [Authorize(Roles = "User")]
//    public async Task<ActionResult<Order>> Create(CreatedOrder order)
//    {
//        return Ok(await _context.CreateAsync(order));
//    }


//    //[HttpPut("/AddPromocode",Name = "UpdateOrder")]
//    //public async Task<ActionResult<Order>> Put(Order order)
//    //{
//    //    var result = await _context.(order);
//    //    if (result == null) return BadRequest(ModelState);
//    //    return Ok(result);
//    //}
//}
