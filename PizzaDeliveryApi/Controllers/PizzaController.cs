namespace PizzaDeliveryApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PizzaController : ControllerBase
{

    private readonly ILogger<PizzaController> _logger;

    private IRepository<Pizza> _context;

    public PizzaController(ILogger<PizzaController> logger, IRepository<Pizza> context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet(Name = "GetAllPizzas")]
    public async Task<ActionResult<ICollection<Pizza>>> GetAll()
    {
        var h = _context.GetAllAsync();
        return Ok(h);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Pizza>> GetOne(Guid id)
    {
        var pizza = await _context.GetAsync(id);
        if (pizza == null) return BadRequest(ModelState);
        return Ok(pizza);
    }

    [HttpPost(Name = "CreatePizza")]
    public async Task<ActionResult<Pizza>> Create(Pizza pizza)
    {
        return Ok(await _context.CreateAsync(pizza));
    }

    [HttpDelete(Name = "DeletePizza")]
    public async Task<ActionResult<Pizza>> Delete(Pizza pizza)
    {
        var result = await _context.DeleteAsync(pizza.Id);
        if (result == null) return BadRequest(ModelState);
        return Ok(result);
    }

    [HttpPut(Name = "UpdatePizza")]
    public async Task<ActionResult<Pizza>> Put(Pizza pizza)
    {
        var result = await _context.UpdateAsync(pizza);
        if (result == null) return BadRequest(ModelState);
        return Ok(result);
    }
}
