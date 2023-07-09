using PizzaDelivery.Application.Helpers;
using PizzaDelivery.Domain.Models.User;

namespace PizzaDelivery.Application.Services.Interfaces;

public interface IAuthService
{

    public PagedList<ApplicationUser> GetAllAsync(QueryStringParameters ownerParameters);
    public Task<ICollection<ApplicationUser>> GetAllAsync();

    public Task<ApplicationUser> GetCurrentUserInfo();
    public Task<ApplicationUser> LoginAsync(LoginUser user);
    public Task<ApplicationUser> LogoutAsync();
    public Task<ApplicationUser> RegisterAsync(RegisterUser user);

    public Task<string> GetRole(ApplicationUser user);
    public string GenerateTokenString(LoginUser user);

    public Task<bool> DeleteAll();
    public Task<ApplicationUser> DeleteAccount(string userId = null);

}
