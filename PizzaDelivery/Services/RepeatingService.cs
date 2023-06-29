using PizzaDelivery.Services.Interfaces;

namespace PizzaDelivery.Services;

public class RepeatingService : BackgroundService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromHours(1));
    private readonly ILogger<RepeatingService> _logger;
    private readonly IRepository<Promocode> _promocodeRepository;

    public RepeatingService(ILogger<RepeatingService> logger, IRepository<Promocode> promocodeRepository)
    {
        _logger = logger;
        _promocodeRepository= promocodeRepository;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //_logger.LogInformation("RepeatingService started.");

        while (!stoppingToken.IsCancellationRequested 
          && await _timer.WaitForNextTickAsync(stoppingToken))
        {
            await DoWorkAsync();
            await Task.Delay(1000, stoppingToken);
        }
        _logger.LogInformation("RepeatingService stopped.");

    }
    private async Task DoWorkAsync()
    {
        //Console.WriteLine(DateTime.Now.ToString("G"));
       var expiredPromocode = _promocodeRepository.GetAllAsync().Result.FirstOrDefault(x=>x.ExpireDate >= DateTime.Now);
       if(expiredPromocode != null)
        {
            await _promocodeRepository.DeleteAsync(expiredPromocode.Id);
            await _promocodeRepository.SaveChangesAsync();
        }

    }
}
