using PizzaDelivery.Domain.Models;
using PizzaDelivery.Application.Interfaces;

using Microsoft.AspNetCore.Authorization;
using PizzaDelivery.Application.Models;
using System.Net;

namespace PizzaDeliveryApi.Controllers;

[ApiController]
[Route("Pizzas")]
[Authorize]
public class PizzaController : ControllerBase
{

    private readonly ILogger<PizzaController> _logger;

    private IPizzaService _context;

    public PizzaController(ILogger<PizzaController> logger, IPizzaService context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet(Name = "GetAllPizzas")]
    public async Task<ActionResult<ICollection<Pizza>>> GetAll(int page = 1, int pageSize = 5)
    {
        if (page < 1 || pageSize < 1) throw new ArgumentOutOfRangeException();
        var items = await _context.GetAllAsync();
        var totalCount = items.Count;
        var totalPages = (int)Math.Ceiling((decimal)totalCount/ pageSize);
        if(page > totalPages) throw new Exception("With this size of page the last page number - " + totalPages);
        var itemPerPage = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        
        return Ok(itemPerPage);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Pizza>> GetOne([FromForm] Guid id)
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
    public async Task<ActionResult<Pizza>> Create([FromForm]PizzaCreationModel pizza)
    {
        return Ok(await _context.CreateAsync(pizza));
    }


    //public HttpResponseMessage GetImageWithModel(int id)
    //{
    //    // Retrieve your model data
    //    YourModel model = YourDataFetchingLogic(id);

    //    // Check if the model exists
    //    if (model == null)
    //    {
    //        // Return a Not Found response if the model doesn't exist
    //        return Request.CreateResponse(HttpStatusCode.NotFound);
    //    }

    //    // Read the image file
    //    byte[] imageBytes = File.ReadAllBytes(model.ImagePath);

    //    // Create a response message
    //    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

    //    // Set the content of the response
    //    response.Content = new ByteArrayContent(imageBytes);

    //    // Set the content type header
    //    response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // Replace with your desired content type

    //    // Attach your model data to the response
    //    response.Content.Headers.Add("X-ModelData", YourModelDataSerializationLogic(model)); // Replace with your serialization logic

    //    return response;
    //}

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
