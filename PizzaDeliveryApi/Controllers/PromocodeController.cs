
using Microsoft.AspNetCore.Authorization;
using PizzaDelivery.Application.Models;
using PizzaDelivery.Application.Helpers;
using Newtonsoft.Json;
using PizzaDelivery.Application.Services.Interfaces;

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
    public IActionResult GetAllAsync([FromQuery] QueryStringParameters parameters)
    {
        var items = _context.GetAllAsync(parameters);

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

    [HttpGet("{id}", Name = "GetOneAsync")]
    public async Task<ActionResult<Promocode>> GetOneAsync(Guid id)
    {
        var promocode = await _context.GetAsync(id);
        if (promocode == null) return BadRequest(ModelState);
        return Ok(promocode);
    }

    [HttpPost(Name = "CreatePromocodeAsync")]
    public async Task<ActionResult<Promocode>> CreateAsync(PromocodeCreateModel promocode)
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
