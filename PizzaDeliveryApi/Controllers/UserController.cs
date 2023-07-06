using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Domain.Models.User;
using PizzaDelivery.Persistence;
using PizzaDelivery.Application.Interfaces;
using PizzaDelivery.Application.Models;
using Microsoft.AspNetCore.Authorization;
using FuzzySharp;

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
    public async Task<ActionResult<ApplicationUser>> GetAllUsersAsync(int page = 1, int pageSize = 5)
    {
        if (page < 1 || pageSize < 1) throw new ArgumentOutOfRangeException();
        var items = await _authService.GetAllAsync();
        var totalCount = items.Count;
        var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
        if (page > totalPages) throw new Exception("With this size of page the last page number - " + totalPages);
        var itemPerPage = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        return Ok(itemPerPage);

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

    [HttpGet("{searchName}", Name = "SearchUsers")]
    public async Task<ActionResult<ICollection<string>>> SearchUsers(string searchName)
    {
        // Retrieve all users from the database
        var allUsersNames = await _context.Users.Select(x=>x.UserName).ToListAsync();

        // Use FuzzySharp to find fuzzy matches
        var searchResults = Process.ExtractTop(searchName, allUsersNames, limit: 5);

        // Return the search results
        return Ok(searchResults.Select(x=>x.Value).ToList());
    }
}
