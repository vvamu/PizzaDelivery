using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Data;
using PizzaDelivery.Models;

namespace PizzaDeliveryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationUserController : ControllerBase
    {

        private readonly ILogger<ApplicationUserController> _logger;
        private readonly ApplicationDbContext _context;

        public ApplicationUserController(ILogger<ApplicationUserController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet(Name = "GetUsers")]
        public async Task<List<ApplicationUser>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpPost(Name = "CreateUser")]
        public async Task<List<ApplicationUser>> Create(ApplicationUser user)
        {
            //var _user = new ApplicationUser()
            //{
            //    UserName = user.UserName,
            //    Email = user.Email,
            //    PasswordHash = _context.Users.SHA256(user.PasswordHash)
            //};

            return null;
        }
    }
}
