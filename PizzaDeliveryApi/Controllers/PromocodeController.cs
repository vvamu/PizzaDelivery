using Microsoft.AspNetCore.Mvc;

namespace OrderDeliveryApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PromocodeController : ControllerBase
{

    private readonly ILogger<PromocodeController> _logger;

    private IRepository<Promocode> _context;

    public PromocodeController(ILogger<PromocodeController> logger, IRepository<Promocode> context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet(Name = "GetAllPromocodes")]
    public async Task<ActionResult<ICollection<Promocode>>> GetAll()
    {
        var h = _context.GetAllAsync();
        return Ok(h);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Promocode>> GetOne(Guid id)
    {
        var promocode = await _context.GetAsync(id);
        if (promocode == null) return BadRequest(ModelState);
        return Ok(promocode);
    }

    [HttpPost(Name = "CreatePromocode")]
    public async Task<ActionResult<Promocode>> Create(Promocode promocode)
    {
        return Ok(await _context.CreateAsync(promocode));
    }

    [HttpDelete(Name = "DeletePromocode")]
    public async Task<ActionResult<Promocode>> Delete(Promocode promocode)
    {
        var result = await _context.DeleteAsync(promocode.Id);
        if (result == null) return BadRequest(ModelState);
        return Ok(result);
    }

    [HttpPut(Name = "UpdatePromocode")]
    public async Task<ActionResult<Promocode>> Put(Promocode promocode)
    {
        var result = await _context.UpdateAsync(promocode);
        if (result == null) return BadRequest(ModelState);
        return Ok(result);
    }
}
