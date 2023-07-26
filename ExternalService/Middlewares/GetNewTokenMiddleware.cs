using DocumentFormat.OpenXml.Vml;
using GoogleProvider.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalProvider.Middlewares;

public class RefreshVkTokenMiddleware : BackgroundService
{
    private readonly IExternalProvider _vkontakteApiService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RefreshVkTokenMiddleware(IExternalProvider vkontakteApiService, IHttpContextAccessor httpContextAccessor)
    {
        _vkontakteApiService = vkontakteApiService;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

            var accessToken = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            var expiresTime = _httpContextAccessor.HttpContext.Session.GetString("ExpiresTime");

            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(expiresTime) && int.TryParse(expiresTime, out var expiresIn))
            {
                var refreshTime = TimeSpan.FromMinutes(0.8 * expiresIn);

                if (refreshTime > TimeSpan.Zero && refreshTime < TimeSpan.FromMinutes(expiresIn))
                {
                    await Task.Delay(refreshTime, stoppingToken);

                    //var refreshedToken = await _vkontakteApiService.RefreshTokenAsync(accessToken);
                    //_httpContextAccessor.HttpContext.Session.SetString("AccessToken", refreshedToken);
                }
            }
        }
    }
}
