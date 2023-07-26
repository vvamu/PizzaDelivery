using PizzaDelivery.Domain.Models;

using Microsoft.AspNetCore.Authorization;
using PizzaDelivery.Application.Models;
using System.Net;
using Newtonsoft.Json;
using PizzaDelivery.Application.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;
using PizzaDelivery.Application.Services.Interfaces;
using Serilog;

namespace PizzaDeliveryApi.Controllers;

[ApiController]
[Route("Pizzas")]
public class PizzaController : ControllerBase
{

    private readonly ILogger<PizzaController> _logger;

    private IPizzaService _context;

    public PizzaController(ILogger<PizzaController> logger, IPizzaService context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet(Name = "GetPizzas")]
    public IActionResult GetPizzas([FromQuery] QueryStringParameters parameters)
    {
        Log.Warning("Auth");

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

    [HttpGet("{id}")]
    public async Task<ActionResult<Pizza>> GetOne(Guid id)
    {
        var pizza = await _context.GetAsync(id);
        if (pizza == null) return BadRequest(ModelState);
        return Ok(pizza);
    }

    [HttpGet("image/{pizzaId}")]
    public async Task<ActionResult> GetImage([FromRoute]Guid pizzaId)
    {
        var imageBytes = await _context.GetImageBytesAsync(pizzaId);
        var pizza = await _context.GetAsync(pizzaId);
        return File(imageBytes, "image/" + pizza.ImageMime.Replace(".", ""));
    }

    [HttpPost(Name = "CreatePizza")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Pizza>> Create([FromForm]PizzaCreateModel pizza)
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
    public async Task<ActionResult<Pizza>> Put([FromForm] Pizza pizza)
    {
        var result = await _context.UpdateAsync(pizza);
        if (result == null) return BadRequest(ModelState);
        return Ok(result);
    }
}
