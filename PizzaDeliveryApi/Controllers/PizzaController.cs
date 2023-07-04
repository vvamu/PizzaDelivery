using PizzaDelivery.Domain.Models;
using PizzaDelivery.Application.Interfaces;

using Microsoft.AspNetCore.Authorization;
using PizzaDelivery.Application.Models;

namespace PizzaDeliveryApi.Controllers;

[ApiController]
[Route("Pizzas")]
[Authorize]
public class PizzaController : ControllerBase
{

    private readonly ILogger<PizzaController> _logger;

    private IPizzaRepository _context;

    public PizzaController(ILogger<PizzaController> logger, IPizzaRepository context)
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
    public async Task<ActionResult<Pizza>> Create(PizzaCreationModel pizza)
    {
        return Ok(await _context.CreateAsync(pizza));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Pizza>> Delete(Guid id)
    {
        var result = await _context.DeleteAsync(id);
        return result == null ? BadRequest(ModelState) : Ok(result);
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
