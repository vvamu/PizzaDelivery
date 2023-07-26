using PizzaDelivery.Application.DTO;
using PizzaDelivery.Application.Helpers;
using PizzaDelivery.Domain.Models.User;

namespace PizzaDelivery.Application.Services.Interfaces;

public interface IAuthService
{
    public PagedList<UserDTO> GetAllAsync(QueryStringParameters ownerParameters);
    public Task<ICollection<ApplicationUser>> GetAllAsync();

    public Task<UserDTO> GetCurrentUserInfo();
    public Task<UserDTO> LoginByEmailAsync(UserLogin user);
    public Task<UserDTO> LogoutAsync();
    public Task<UserDTO> RegisterAsync(UserRegister user);

    public Task<string> GetRole(string userEmail);
    public string GenerateTokenString(UserLogin user);

    public Task<bool> DeleteAll();
    public Task<UserDTO> DeleteAccount(string userId = null);



    public ApplicationUser GetCurrentUser();

    public Task<ApplicationUser> ExternalLogin(string provider, ExternalUser user);


}
