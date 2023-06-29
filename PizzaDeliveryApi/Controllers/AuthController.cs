using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Data;
using PizzaDelivery.Models;
using PizzaDelivery.Services.Interfaces;
using PizzaDelivery.ViewModels;

namespace PizzaDeliveryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly ILogger<AuthController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public AuthController(ILogger<AuthController> logger, ApplicationDbContext context, IAuthService authService)
        {
            _logger = logger;
            _context = context;
            _authService = authService;
        }

        public async Task<ActionResult<ApplicationUser>> RegisterAsync(LoginUser user)
        {

            var _user = await _authService.RegisterAsync(user);
            if (_user == null) { return BadRequest(ModelState); }
            return Ok(_user);

        }
        public async Task<ActionResult<ApplicationUser>> LoginAsync(LoginUser user)
        {
            var _user = await _authService.LoginAsync(user);
            if (_user == null) { return BadRequest(ModelState); }
            return Ok(_user);
        }
    }
}
