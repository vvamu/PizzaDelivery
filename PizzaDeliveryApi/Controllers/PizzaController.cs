using PizzaDelivery.Domain.Models;
using PizzaDelivery.Application.Interfaces;
using PizzaDelivery.DomainRealize.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace PizzaDeliveryApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
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
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Pizza>> Create(Pizza pizza)
    {
        return Ok(await _context.CreateAsync(pizza));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Pizza>> Delete(Guid id)
    {
        var result = await _context.DeleteAsync(id);
        if (result == null) return BadRequest(ModelState);
        return Ok(result);
    }

    [HttpPut(Name = "UpdatePizza")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Pizza>> Put(Pizza pizza)
    {
        var result = await _context.UpdateAsync(pizza);
        if (result == null) return BadRequest(ModelState);
        return Ok(result);
    }
}
