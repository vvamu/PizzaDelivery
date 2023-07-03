//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using PizzaDelivery.Domain.Models.User;
//using PizzaDelivery.Persistence;
//using PizzaDelivery.Application.Interfaces;
//using PizzaDelivery.Application.Models;
//using Microsoft.AspNetCore.Authorization;

//namespace PizzaDeliveryApi.Controllers;

//[ApiController]
//[Authorize(Roles = "Admin")]
//[Route("[controller]")]
//public class UserController : ControllerBase
//{

//    private readonly ILogger<AuthController> _logger;
//    private readonly ApplicationDbContext _context;
//    private readonly IAuthService _authService;

//    public UserController(ILogger<AuthController> logger, ApplicationDbContext context, IAuthService authService)
//    {
//        _logger = logger;
//        _context = context;
//        _authService = authService;
//    }

//    [HttpGet("/Users",Name = "GetAllAsync")]
//    [Authorize(Roles = "Admin")]
//    public async Task<ActionResult<ApplicationUser>> GetAllUsersAsync()
//    {
//        return Ok(await _authService.GetAllAsync());
//    }

//    [HttpGet("/User/{id}", Name = "GetUserInfo")]
//    [Authorize(Roles = "Admin")]
//    public async Task<ActionResult<ApplicationUser>> GetUserInfo(string userid)
//    {
//        var db_user = await _context.Users.FindAsync(userid);
//        return db_user == null ? BadRequest(ModelState) : Ok(db_user);
//    }

//    [HttpDelete("/DeleteAccount/{id}", Name = "DeleteAccount")]
//    [Authorize(Roles = "Admin")]
//    public async Task<ActionResult<bool>> DeleteAccount(string userId)
//    {
//        var db_user = await _authService.DeleteAccount(userId);
//        if (db_user == null) return BadRequest("There is no such user in the database");
//        return db_user == null ? BadRequest(ModelState) : Ok(db_user);
//    }

//}
