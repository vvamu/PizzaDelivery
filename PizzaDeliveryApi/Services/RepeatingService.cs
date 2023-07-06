using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using PizzaDelivery.Application.Interfaces;


namespace PizzaDeliveryApi.Services;

public class RepeatingService : BackgroundService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromHours(1));
    private readonly Serilog.ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RepeatingService(Serilog.ILogger logger,  IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {        

        while (!stoppingToken.IsCancellationRequested && await _timer.WaitForNextTickAsync(stoppingToken))
        {
            await DeletingExpiredPromocodes();
            await DeleteInactivePizzasAsync();
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task DeletingExpiredPromocodes()
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            _logger.Information("Hello");
            var promocodeRepository = scope.ServiceProvider.GetRequiredService<IPromocodeService>();

            var expiredPromocode = (await promocodeRepository.GetAllAsync())
                .FirstOrDefault(x => x.ExpireDate >= DateTime.Now);

            
            if (expiredPromocode != null)
            {
                _logger.Debug("Promocode " + expiredPromocode.Value + " expired");
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
                _logger.Debug("Pizza " + orderItem.Pizza.Name + " not delivered by 1 month and was inactive");
                var inactiveDate = orderItem.Order.OrderDate.AddMonths(1);
                if (inactiveDate >= DateTime.Now)
                    pizzaService.DeleteAsync(orderItem.PizzaId);
            }

            
        }
    }
}
