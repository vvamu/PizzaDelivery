//using PizzaDelivery.Services.Interfaces;

//namespace PizzaDeliveryApi.Controllers;

//[ApiController]
//[Route("[controller]")]
//public class OrderController : ControllerBase
//{

//    private readonly ILogger<OrderController> _logger;

//    private IRepository<Order> _context;

//    public OrderController(ILogger<OrderController> logger, IRepository<Order> context)
//    {
//        _logger = logger;
//        _context = context;
//    }

//    [HttpGet(Name = "GetAllOrders")]
//    public async Task<ActionResult<ICollection<Order>>> GetAll()
//    {
//        var h = _context.GetAllAsync();
//        return Ok(h);
//    }

//    [HttpGet("{id}")]
//    public async Task<ActionResult<Order>> GetOne(Guid id)
//    {
//        var order = await _context.GetAsync(id);
//        if (order == null) return BadRequest(ModelState);
//        return Ok(order);
//    }

//    [HttpPost(Name = "CreateOrder")]
//    public async Task<ActionResult<Order>> Create(CreatedOrder order)
//    {
//        return Ok(await _context.CreateAsync(order));
//    }

//    [HttpDelete(Name = "DeleteOrder")]
//    public async Task<ActionResult<Order>> Delete(Order order)
//    {
//        var result = await _context.DeleteAsync(order.Id);
//        if (result == null) return BadRequest(ModelState);
//        return Ok(result);
//    }

//    [HttpPut(Name = "UpdateOrder")]
//    public async Task<ActionResult<Order>> Put(Order order)
//    {
//        var result = await _context.UpdateAsync(order);
//        if (result == null) return BadRequest(ModelState);
//        return Ok(result);
//    }
//}
