using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using PizzaDelivery.Application.Services.Interfaces;
using Serilog;
using System.Net.Mail;
using System.Net;

namespace PizzaDeliveryApi.Services;

public class RepeatingService : BackgroundService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromHours(1));
    private readonly ILogger<RepeatingService> _logger; // Use Microsoft.Extensions.Logging.ILogger instead of Serilog.ILogger
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RepeatingService(ILogger<RepeatingService> logger,  IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("Start check expired promocodes");
        Log.Information("Start check inactive pizzas a month");


        while (!stoppingToken.IsCancellationRequested)
        {
            await DeletingExpiredPromocodes();
            await DeleteInactivePizzasAsync();

            await Task.Delay(1000, stoppingToken);

        }
        if(stoppingToken.IsCancellationRequested)
        {
            Log.Information("Finish check expired promocodes");
            Log.Information("Finish check inactive pizzas a month");
        }
    }

    private async Task DeletingExpiredPromocodes()
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {

            var promocodeRepository = scope.ServiceProvider.GetRequiredService<IPromocodeService>();
            var expiredPromocode = (await promocodeRepository.GetAllAsync())
                .FirstOrDefault(x => x.ExpireDate >= DateTime.Now);

            if (expiredPromocode != null)
            {
                _logger.LogInformation("Promocode " + expiredPromocode.Value + " expired");
                //expiredPromocode.Expired= true;
                await promocodeRepository.DeleteAsync(expiredPromocode.Id);
                await promocodeRepository.SaveChangesAsync();
            }
        }
    }

    private async Task DeleteInactivePizzasAsync()
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {

            var pizzaService = scope.ServiceProvider.GetRequiredService<IPizzaService>();
            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

            var orderItems = await orderService.GetAllOrderItems();

            foreach(var orderItem in orderItems)
            {
                Log.Information("Pizza " + orderItem.Pizza.Name + " not delivered by 1 month and was inactive");
                var inactiveDate = orderItem.Order.OrderDate.AddMonths(1);
                if (inactiveDate >= DateTime.Now)
                    pizzaService.DeleteAsync(orderItem.PizzaId);
            }

            
        }
    }
}


