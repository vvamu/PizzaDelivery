
using ExternalProvider.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PizzaDelivery.Domain.Models;
using PizzaDelivery.Domain.Models.User;

namespace GoogleProvider.Interfaces;

public interface IExternalProvider
{
    public Task<string> GetCurrentUserId(string accessToken);
    public Task<ICollection<AuthenticationScheme>> GetExternalAuthenticationSchemes();
    public Task<ApplicationUser> ExternalLoginCallback(string returnUrl = null, string remoteError = null);
    public Task<AuthenticationProperties> ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);

    public Task<ApplicationUser> ExternalLogin(string provider,VkontakteUser user);
    public Task<ExternalConnection> AddToUserExternalConnection(string userId, string provider, string externalUserId);


}