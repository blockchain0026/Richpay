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
    public class UpdateUserCacheBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public UpdateUserCacheBackgroundService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // This eShopOnContainers method is querying a database table
                // and publishing events into the Event Bus (RabbitMQ / ServiceBus)
                await UpdateUserNameInfosCache();

                await Task.Delay(60000, stoppingToken);
            }
        }

        private async Task UpdateUserNameInfosCache()
        {
            try
            {
                Console.WriteLine("Updating User Name Infos Cache...");
                using (var scope = scopeFactory.CreateScope())
                {
                    var userCacheService = scope.ServiceProvider.GetRequiredService<IUserCacheService>();
                    await userCacheService.UpdateNameInfos();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Catch Excpetion while Updating Commissions Cache... " + ex.Message);
            }
        }
    }
}
