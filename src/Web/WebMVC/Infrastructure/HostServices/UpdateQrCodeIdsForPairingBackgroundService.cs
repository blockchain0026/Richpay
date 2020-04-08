using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.CacheServices;

namespace WebMVC.Infrastructure.HostServices
{
    public class UpdateQrCodeIdsForPairingBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public UpdateQrCodeIdsForPairingBackgroundService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // This eShopOnContainers method is querying a database table
                // and publishing events into the Event Bus (RabbitMQ / ServiceBus)
                await UpdateQrCodeIdsForPairing();

                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task UpdateQrCodeIdsForPairing()
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var qrCodeQueueService = scope.ServiceProvider.GetRequiredService<IQrCodeQueueService>();
                    await qrCodeQueueService.UpdateQrCodeIds();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Catch Exception while Updating Qr Code Ids For Pairing...");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
