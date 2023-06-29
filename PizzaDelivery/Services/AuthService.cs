using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Services.Interfaces;
using PizzaDelivery.ViewModels;

namespace PizzaDelivery.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ApplicationUser> LoginAsync(LoginUser user)
        {
            var db_user = await _userManager.FindByEmailAsync(user.Email);
            if (db_user == null) return null;
            var checkpassword = await _signInManager.PasswordSignInAsync(db_user.Id, user.Password, false, false);
            if (!checkpassword.Succeeded) return null;
            

            return db_user;
        }
        public async Task<ApplicationUser> LogoutAsync()
        {
            //var user = await System.Security.Claims.ClaimsPrincipal();
            await _signInManager.SignOutAsync();
            return null;
        }

        public async Task<ApplicationUser> RegisterAsync(LoginUser user)
        {
            var applicationUser = new ApplicationUser()
            {
                Email= user.Email
            };
            var result = await _userManager.CreateAsync(applicationUser,user.Password);
            if (!result.Succeeded) return null;
            return applicationUser;

        }
    }
}
