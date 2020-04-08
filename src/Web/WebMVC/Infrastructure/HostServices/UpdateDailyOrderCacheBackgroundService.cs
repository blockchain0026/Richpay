using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.CacheServices;

namespace WebMVC.Infrastructure.HostServices
{
    public class UpdateDailyOrderCacheBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public UpdateDailyOrderCacheBackgroundService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // This eShopOnContainers method is querying a database table
                // and publishing events into the Event Bus (RabbitMQ / ServiceBus)
                await UpdateDailyOrderCache();

                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task UpdateDailyOrderCache()
        {
            Console.WriteLine("Updating Daily Order Cache...");
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var orderDailyCacheService = scope.ServiceProvider.GetRequiredService<IOrderDailyCacheService>();
                    await orderDailyCacheService.UpdateRecentOrderEntries();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Catch Exception while updating daily order cache... " + ex.Message);
            }
        }
    }
}
