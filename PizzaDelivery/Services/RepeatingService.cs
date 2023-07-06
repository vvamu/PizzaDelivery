using PizzaDelivery.Domain.Models;
using PizzaDelivery.Application.Interfaces;


namespace PizzaDelivery.Services;

//public class RepeatingService : BackgroundService
//{
//    private readonly PeriodicTimer _timer = new(TimeSpan.FromHours(1));
//    private readonly ILogger<RepeatingService> _logger;
//    private readonly IServiceScopeFactory _serviceScopeFactory;

//    public RepeatingService(ILogger<RepeatingService> logger, IServiceScopeFactory serviceScopeFactory)
//    {
//        _logger = logger;
//        _serviceScopeFactory = serviceScopeFactory;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        while (!stoppingToken.IsCancellationRequested && await _timer.WaitForNextTickAsync(stoppingToken))
//        {
//            await DoWorkAsync();
//            await Task.Delay(1000, stoppingToken);
//        }
//    }

//    private async Task DoWorkAsync()
//    {
//        using (var scope = _serviceScopeFactory.CreateScope())
//        {
//            var promocodeRepository = scope.ServiceProvider.GetRequiredService<IPromocodeService>();

//            var expiredPromocode = (await promocodeRepository.GetAllAsync())
//                .FirstOrDefault(x => x.ExpireDate == DateTime.Now);

//            if (expiredPromocode != null)
//            {
//                await promocodeRepository.DeleteAsync(expiredPromocode.Id);
//                await promocodeRepository.SaveChangesAsync();
//            }
//        }
//    }
//}
