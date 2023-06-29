using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;

namespace PizzaDelivery.Services.Interfaces;

public interface IAuthService
{
    public Task<ApplicationUser> LoginAsync(LoginUser user);
    public Task<ApplicationUser> LogoutAsync();
    public Task<ApplicationUser> RegisterAsync(LoginUser user);
}
