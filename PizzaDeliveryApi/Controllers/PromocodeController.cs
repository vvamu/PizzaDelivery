using PizzaDelivery.DomainRealize.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace OrderDeliveryApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Admin")]
public class PromocodeController : ControllerBase
{

    private readonly ILogger<PromocodeController> _logger;

    private IRepository<Promocode> _context;

    public PromocodeController(ILogger<PromocodeController> logger, IRepository<Promocode> context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet(Name = "GetAllPromocodesAsync")]
    public async Task<ActionResult<ICollection<Promocode>>> GetAllAsync()
    {
        var h = _context.GetAllAsync();
        return Ok(h);
    }

    [HttpGet("{id}", Name = "GetOneAsync")]
    public async Task<ActionResult<Promocode>> GetOneAsync(Guid id)
    {
        var promocode = await _context.GetAsync(id);
        if (promocode == null) return BadRequest(ModelState);
        return Ok(promocode);
    }

    [HttpPost(Name = "CreatePromocodeAsync")]
    public async Task<ActionResult<Promocode>> CreateAsync(Promocode promocode)
    {
        return Ok(await _context.CreateAsync(promocode));
    }

    [HttpDelete(Name = "DeletePromocodeAsync")]
    public async Task<ActionResult<Promocode>> DeleteAsync(Guid id)
    {
        var result = await _context.DeleteAsync(id);
        if (result == null) return BadRequest(ModelState);
        return Ok(result);
    }

    [HttpPut(Name = "UpdatePromocodeAsync")]
    public async Task<ActionResult<Promocode>> PutAsync(Promocode promocode)
    {
        var result = await _context.UpdateAsync(promocode);
        if (result == null) return BadRequest(ModelState);
        return Ok(result);
    }
}
