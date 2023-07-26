using PizzaDelivery.Domain.Models.User;
using PizzaDelivery.Persistence;
using PizzaDelivery.Application.Models;
using Microsoft.AspNetCore.Authorization;
using PizzaDelivery.Application.Helpers;
using PizzaDelivery.Application.Services.Interfaces;
using Serilog;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;
using System.Security.Claims;

namespace PizzaDeliveryApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{

    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("Account", Name = "GetOwnUserInfoAsync")]
    [Authorize]
    public async Task<ActionResult<UserDTO>> GetOwnUserInfoAsync()
    {
        //await _defalutDbContent.GenerateAll();
        var current_user = await _authService.GetCurrentUserInfo();
        return current_user == null ? Ok("U should authorize") : Ok(current_user);
    }

    [HttpGet("Logout", Name = "LogoutAsync")]
    [Authorize]
    public async Task<ActionResult<UserDTO>> LogoutAsync()
    {
        //_logger.Information("Auth");
        //_logger.LogDebug("aaa");
        var db_user = await _authService.LogoutAsync();
        return db_user == null ? BadRequest(ModelState) : Ok(db_user);
    }

    [HttpPost("Register",Name = "RegisterAsync")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDTO>> RegisterAsync(UserRegister user)
    {
        var db_user = await _authService.RegisterAsync(user);
        return db_user == null ? BadRequest(ModelState) : Ok(db_user);
    }

    [HttpPost("Login",Name = "LoginByEmailAsync")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDTO>> LoginByEmailAsync(UserLogin user)
    {
        var db_user = await _authService.LoginByEmailAsync(user);
        if (db_user == null) return BadRequest(ModelState);
        var tokenString = _authService.GenerateTokenString(user);
        var role = _authService.GetRole(db_user.Email);
        
        return Ok(role.Result+"\nToken - " + tokenString);

    }

    [HttpDelete("DeleteAccount", Name = "DeleteOwnAccountAsync")]
    [Authorize]
    public async Task<ActionResult<bool>> DeleteOwnAccountAsync()
    {
        var current_user = await _authService.DeleteAccount();
        return current_user == null ? BadRequest(ModelState) : Ok(current_user);
    }

    

}

