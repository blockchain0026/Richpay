using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebWorkerService.CacheServices;

namespace WebWorkerService.BackgroundServices
{

    public class UpdateCommissionCacheBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public UpdateCommissionCacheBackgroundService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // This eShopOnContainers method is querying a database table
                // and publishing events into the Event Bus (RabbitMQ / ServiceBus)
                await UpdateCommissionsCache();

                await Task.Delay(60000, stoppingToken);
            }
        }

        private async Task UpdateCommissionsCache()
        {
            try
            {
                Console.WriteLine("Updating Commissions Cache...");
                using (var scope = scopeFactory.CreateScope())
                {
                    var commissionCacheService = scope.ServiceProvider.GetRequiredService<ICommissionCacheService>();
                    await commissionCacheService.UpdateCommissions();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Catch Excpetion while Updating Commissions Cache... " + ex.Message);
            }
        }
    }

}
