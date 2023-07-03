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
[Route("[controller]")]
public class AuthController : ControllerBase
{

    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;

    public AuthController(ILogger<AuthController> logger, ApplicationDbContext context, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpGet("/Account", Name = "GetUserInfo")]
    [Authorize]
    public async Task<ActionResult<ApplicationUser>> GetUserInfo()
    {
        var current_user = await _authService.GetCurrentUserInfo();
        return current_user == null ? Ok("U should authorize") : Ok(current_user);
    }

    [HttpGet("Logout", Name = "LogoutAsync")]
    [Authorize]
    public async Task<ActionResult<ApplicationUser>> LogoutAsync()
    {
        var db_user = await _authService.LogoutAsync();
        //return db_user == null ? BadRequest(ModelState) : Ok(db_user);
        return Ok();
    }

    [HttpPost("Register",Name = "RegisterAsync")]
    [AllowAnonymous]
    public async Task<ActionResult<ApplicationUser>> RegisterAsync(RegisterUser user)
    {
        var db_user = await _authService.RegisterAsync(user);
        return db_user == null ? BadRequest(ModelState) : Ok(db_user);
    }

    [HttpPost("Login",Name = "LoginAsync")]
    [AllowAnonymous]
    public async Task<ActionResult<ApplicationUser>> LoginAsync(LoginUser user)
    {
        var db_user = await _authService.LoginAsync(user);
        if (db_user == null) return BadRequest(ModelState);
        var tokenString = _authService.GenerateTokenString(user);
        var role = _authService.GetRole(db_user);
        
        return Ok(role.Result+"\nToken - " + tokenString);

    }

    [HttpDelete("DeleteAll", Name = "DeleteAll")]
    [AllowAnonymous]
    public async Task<ActionResult<bool>> DeleteAll()
    {
        var result = await _authService.DeleteAll();
        return result == false ? BadRequest(ModelState) : Ok(result);
    }

    [HttpDelete("DeleteAccount", Name = "DeleteAccount")]
    [Authorize]
    public async Task<ActionResult<bool>> DeleteAccount()
    {
        var current_user = await _authService.DeleteAccount();
        return current_user == null ? BadRequest(ModelState) : Ok(current_user);
    }

}
