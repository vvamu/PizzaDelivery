using System.Security.Claims;
using System;
using PizzaDelivery.Domain.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using GoogleProvider.Interfaces;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Persistence;
using PizzaDelivery.Domain.Models;
using System.Runtime.ConstrainedExecution;
using ExternalProvider.Models;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.Core;
using Newtonsoft.Json.Linq;

namespace GoogleProvider;

public class ExternalProvider : IExternalProvider
{ 
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public ExternalProvider(SignInManager<ApplicationUser> signInNanager, UserManager<ApplicationUser> userManager,
        ApplicationDbContext db)
    {
        _signInManager= signInNanager;
        _userManager= userManager;
        _db= db;


    }

    public async Task RefreshToken()
    {
        //var clientId = Configuration["VkOauth:AppId"];
        //var redirectUri = "your_redirect_uri";
        //var authorizationUrl = $"https://oauth.vk.com/authorize?client_id={clientId}&redirect_uri={redirectUri}&response_type=token";

        //return Redire(authorizationUrl);
    }

    public async Task<string> GetCurrentUserId(string accessToken)
    {
        var httpClient = new HttpClient();
        var getUserDetailsUrl = $"https://api.vk.com/method/users.get?access_token={accessToken}&v=5.131";
        var getUserDetailsResponse = await httpClient.GetAsync(getUserDetailsUrl);
        var getUserDetailsContent = await getUserDetailsResponse.Content.ReadAsStringAsync();
        var getUserDetailsJsonResponse = JObject.Parse(getUserDetailsContent);
        var vkUser = getUserDetailsJsonResponse["response"].ToObject<List<VkontakteUser>>();
        var numericUserId = vkUser[0].Id;
        return numericUserId;
    }
    public async Task<AuthenticationProperties> ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
    {
        return _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
    }
    public async Task<ApplicationUser> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        if (remoteError != null)
        {
            throw new Exception($"Error from external provider: {remoteError}");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            throw new Exception("Error loading external login information.");
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
            info.ProviderKey, isPersistent: false, bypassTwoFactor: false);
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        var existedUser = await _userManager.FindByEmailAsync(email);

        if (!signInResult.Succeeded)
        {
            if (existedUser == null)
            {
                existedUser = new ApplicationUser
                {
                    UserName = info.Principal.FindFirstValue(ClaimTypes.Name),
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                    OwnHashedPassword = PizzaDelivery.Application.Helpers.HashProvider.ComputeHash("default"),
                };

                await _userManager.CreateAsync(existedUser);
            }
        }

        var user = existedUser;
        var userSchemes = user.ExternalConnections.Select(x => x.Provider);
        
        if (!userSchemes.Contains(info.Principal.Identity.AuthenticationType))
        {

            var externalConnection = new PizzaDelivery.Domain.Models.ExternalConnection()
            {
                Provider = info.LoginProvider,
                ProviderUserId = user.Id
            };
            user.ExternalConnections.Add(externalConnection);


            await _db.SaveChangesAsync();
        }


        await _signInManager.SignInAsync(user, isPersistent: false);
        await _db.SaveChangesAsync();

        return user ?? throw new Exception("Error with register user");
    }

    public async Task<ApplicationUser> ExternalLogin(string provider, VkontakteUser vkUser)
    {
        var externalCollections = await _db.Users.SelectMany(x => x.ExternalConnections).ToListAsync();
        var externalUser = externalCollections.FirstOrDefault(x => x.ProviderUserId == vkUser.Id);
        ApplicationUser? user = null;
        if (externalUser != null)
        {
            user = await _userManager.FindByIdAsync(externalUser.UserId) ?? throw new Exception("User not fount");
        }
        else
        {
            user = new ApplicationUser
            {
                UserName = user.UserName ?? user.Id,
                Email = user.Email ?? user.Id + "@example.com",
                OwnHashedPassword = PizzaDelivery.Application.Helpers.HashProvider.ComputeHash("default"),
            };

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            externalUser = await AddToUserExternalConnection(user.Id,provider,user.Id);
            user.ExternalConnections.Add(externalUser);

            await _userManager.AddToRoleAsync(user, "User");

        }
        await _db.SaveChangesAsync();
        user = await _db.Users.Include(x => x.ExternalConnections).
            FirstOrDefaultAsync(x => x.Id == user.Id);


        await _signInManager.SignInAsync(user,false);
        await _db.SaveChangesAsync();


        return user;

    }
    public async Task<ExternalConnection> AddToUserExternalConnection(string userId,string provider, string externalUserId)
    {
       
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId) ?? throw new Exception("User not found");

        var externalCollections = await _db.Users.SelectMany(x=>x.ExternalConnections).ToListAsync();
        var externalUser = externalCollections.FirstOrDefault(x => x.ProviderUserId == externalUserId) ?? 
            throw new Exception("Account was connected to other user");

        var externalUserConnection =  user.ExternalConnections.FirstOrDefault(x=>x.Provider == provider);
        if (externalUserConnection != null)
        {
            externalUserConnection.ProviderUserId = externalUserId;
        }
        else
        {
            externalUserConnection = new ExternalConnection()
            {
                Provider = provider,
                ProviderUserId = externalUserId,
                UserId = userId,
            };
            _db.ExternalConnections.Add(externalUserConnection);
        }
        return externalUserConnection;

    }
    public async Task<ICollection<AuthenticationScheme>> GetExternalAuthenticationSchemes()
    {
        var ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        return ExternalLogins;
    }

}