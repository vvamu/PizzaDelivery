using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaDelivery.Application.Services.Interfaces;
using PizzaDelivery.Domain.Models;
using PizzaDelivery.ViewModels;
using System.Diagnostics;
using GoogleProvider.Interfaces;
using System.Security.Claims;
using Google.Apis.Auth.OAuth2;
using DocumentFormat.OpenXml.InkML;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using ExternalProvider.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using PizzaDelivery.Application.Models;

namespace PizzaDelivery.Controllers;

public class HomeController : Controller
{
    private IPizzaService _pizzaService;
    private readonly IExternalProvider _externalProvider;
    private readonly IAuthService _authService;


    public static string AppId = "51701410";
    public static string AppKey = "CsgfrtmZMFW3ibq3SAib";
    public static string ServiceKey = "7e67f85f7e67f85f7e67f85f667d731efd77e677e67f85f1ac9c75a0bed2fb712f22ac8";
    public static string RedirectUrl = "/signin-vk-token";
    public HomeController(IExternalProvider googleProvider, IAuthService authService
        , IPizzaService pizzaService)
    {
        _externalProvider = googleProvider;
        _authService = authService;
        _pizzaService = pizzaService;



    }



    public async Task<IActionResult> Index()
    {
        var pizzas = await _pizzaService.GetAllAsync();
        return View(pizzas);
    }
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Index");
    }

    #region Default Login - Facebook, Google

    [HttpGet("google-login", Name = "ExternalLogin2")]
    [Authorize]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLogin(string provider = GoogleDefaults.AuthenticationScheme,
        string returnUrl = null)
    {

        var redirectUrl = Url.Action("ExternalLoginCallback", "Home",
                                   new { ReturnUrl = returnUrl });
        var properties = await _externalProvider.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }


    [AllowAnonymous]
    [HttpGet("google-response", Name = "ExternalLoginCallback2")]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");
        await _externalProvider.ExternalLoginCallback(returnUrl, remoteError);
        return RedirectToAction("Index");
    }
    #endregion


    #region Vk Login

    [HttpGet("vk-login", Name = "ExternalLogin3")]
    [Authorize]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginVk(string provider,
    string returnUrl = null)
    {
        var authenticationProperties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("ExternalLoginCallbackVk", "Home", null, Request.Scheme, Request.Host.ToString()),
            Items =
        {
            { "LoginProvider", "Vkontakte" }
        }
        };

        return Challenge(authenticationProperties, "Vkontakte");

    }

    [AllowAnonymous]
    //[HttpGet("signin-vk-token", Name = "ExternalLoginCallback3")]
    public async Task<IActionResult> ExternalLoginCallbackVk(string returnUrl = null, string remoteError = null)
    {
        var authenticateResult = await HttpContext.AuthenticateAsync("Vkontakte");

        if (authenticateResult.Succeeded)
        {
            
            var tokenEndpoint = "https://api.vk.com/method/users.get";
            var httpClient = new HttpClient();

            var accessToken = authenticateResult.Properties.Items.FirstOrDefault(x => x.Key == ".Token.access_token").Value;

            var requestUrl = $"{tokenEndpoint}?access_token={accessToken}&v=5.131";
            var response = await httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var Response = JsonConvert.DeserializeObject<VkontakteUserResponse>(content);
                var userVK = Response.Response.FirstOrDefault();

                await _authService.ExternalLogin("Vkontakte", userVK);

            }
        }
        return RedirectToAction("Index");

    }
    #endregion


    public async Task<string> GetUserId()
    {
        var httpClient = new HttpClient();
        var accessToken = HttpContext.Session.GetString("AccessToken");

        var requestUrl = $"https://api.vk.com/method/users.get?access_token={accessToken}&v=5.131";

        var response = await httpClient.GetAsync(requestUrl);
        var content = await response.Content.ReadAsStringAsync();

        var jsonResponse = JObject.Parse(content);
        var user = jsonResponse["response"].FirstOrDefault();
        var userId = user?["id"]?.ToString();
        return userId;
    }

    [Authorize(Policy = "VKPolicy")]
    public async Task<IActionResult> GetOnlineFriends()
    {
        var accessToken = HttpContext.Session.GetString("AccessToken");
        var requestUrl = $"https://api.vk.com/method/friends.get?access_token={accessToken}&v=5.131&online=1";

        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync(requestUrl);
            var content = await response.Content.ReadAsStringAsync();


            try
            {
                var jsonResponse = JObject.Parse(content);
                var friendIds = jsonResponse["response"]["items"].ToObject<List<int>>();

                // Make another API request to retrieve detailed information about the friends
                var friendDetailsUrl = $"https://api.vk.com/method/users.get?access_token={accessToken}&v=5.131&user_ids={string.Join(",", friendIds)}";
                var friendDetailsResponse = await httpClient.GetAsync(friendDetailsUrl);
                var friendDetailsContent = await friendDetailsResponse.Content.ReadAsStringAsync();
                var friendDetailsJsonResponse = JObject.Parse(friendDetailsContent);
                var friendDetails = friendDetailsJsonResponse["response"].ToObject<List<VkontakteUser>>();

                var userId = _authService.GetCurrentUser().ExternalConnections.FirstOrDefault(x=>x.Provider == "Vkontakte").ProviderUserId;

                //var userId = _externalProvider.GetCurrentUser().ExternalConnections.FirstOrDefault(x=>x.Provider == "Vkontakte").ProviderUserId;
                var friendOnlineDetailsUrl = $"https://api.vk.com/method/friends.getOnline?user_id={userId}&access_token={accessToken}&v=5.131";
                var friendOnlineDetailsResponse = await httpClient.GetAsync(friendDetailsUrl);
                var friendOnlineDetailsContent = await friendDetailsResponse.Content.ReadAsStringAsync();
                var friendOnlineDetailsJsonResponse = JObject.Parse(friendDetailsContent);
                var friendOnlineDetails = friendDetailsJsonResponse["response"].ToObject<List<VkontakteUser>>();

                return View("OnlineFriends", (friendDetails, friendOnlineDetails));
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index");

            }
            // Deserialize the response to retrieve the online friends


        }
    }


    #region Other

    
    public async Task<IActionResult> CreateDefaultPizza()
    {
        var pizza = new PizzaCreateModel()
        {
            Name = "Default",
            Desctiption = "Default",
            Ingridients = "Default",
            Price = 10m
        };
        await _pizzaService.CreateAsync(pizza);

        return View();
    }
    public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion



    
}


public class TokenResponse
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
}


