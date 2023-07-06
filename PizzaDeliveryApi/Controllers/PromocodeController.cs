
using Microsoft.AspNetCore.Authorization;
using PizzaDelivery.Application.Models;
using PizzaDelivery.Application.Interfaces;

namespace OrderDeliveryApi.Controllers;

[ApiController]
[Route("[controller]")]
//[Authorize(Roles = "Admin")]
public class PromocodeController : ControllerBase
{

    private readonly ILogger<PromocodeController> _logger;

    private IPromocodeService _context;

    public PromocodeController(ILogger<PromocodeController> logger, IPromocodeService context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet(Name = "GetAllPromocodesAsync")]
    public async Task<ActionResult<ICollection<Promocode>>> GetAllAsync(int page = 1, int pageSize = 5)
    {
        if (page < 1 || pageSize < 1) throw new ArgumentOutOfRangeException();
        var items = await _context.GetAllAsync();
        var totalCount = items.Count;
        var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
        if (page > totalPages) throw new Exception("With this size of page the last page number - " + totalPages);
        var itemPerPage = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Ok(itemPerPage);
    }

    [HttpGet("{id}", Name = "GetOneAsync")]
    public async Task<ActionResult<Promocode>> GetOneAsync(Guid id)
    {
        var promocode = await _context.GetAsync(id);
        if (promocode == null) return BadRequest(ModelState);
        return Ok(promocode);
    }

    [HttpPost(Name = "CreatePromocodeAsync")]
    public async Task<ActionResult<Promocode>> CreateAsync(PromocodeCreationModel promocode)
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
