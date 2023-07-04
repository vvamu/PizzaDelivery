using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Domain.Models.User;
using PizzaDelivery.Persistence;
using PizzaDelivery.Application.Interfaces;
using PizzaDelivery.Application.Models;
using Microsoft.AspNetCore.Authorization;

namespace PizzaDeliveryApi.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("[controller]")]
public class UserController : ControllerBase
{

    private readonly ILogger<AuthController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;

    public UserController(ILogger<AuthController> logger, ApplicationDbContext context, IAuthService authService)
    {
        _logger = logger;
        _context = context;
        _authService = authService;
    }

    [HttpGet(Name = "GetAllUsersAsync")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApplicationUser>> GetAllUsersAsync()
    {
        return Ok(await _authService.GetAllAsync());
    }

    [HttpGet("{userId}", Name = "GetUserInfoAsync")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApplicationUser>> GetUserInfoAsync(string userId)
    {
        var db_user = await _context.Users.FindAsync(userId);
        return db_user == null ? BadRequest(ModelState) : Ok(db_user);
    }

    [HttpDelete("{userId}", Name = "DeleteAccountAsync")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<bool>> DeleteAccountAsync(string userId)
    {
        var db_user = await _authService.DeleteAccount(userId);
        if (db_user == null) return BadRequest("There is no such user in the database");
        return db_user == null ? BadRequest(ModelState) : Ok(db_user);
    }

}
