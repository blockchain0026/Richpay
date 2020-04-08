using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Settings;

namespace WebMVC.Infrastructure.HostServices
{
    public class ClearCompletedOrderBackgroundService : BackgroundService
    {
        private readonly ILogger<ClearCompletedOrderBackgroundService> _logger;
        private readonly ClearCompletedOrderSettings _settings;

        private readonly IServiceScopeFactory scopeFactory;

        public ClearCompletedOrderBackgroundService(ILogger<ClearCompletedOrderBackgroundService> logger,
            IOptions<ClearCompletedOrderSettings> settings, IServiceScopeFactory scopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        //private readonly IOrderRepository _orderRepository;


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"ClearCompletedOrderBackgroundService is starting.");

            stoppingToken.Register(() =>
                _logger.LogDebug($" ClearCompletedOrder background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {

                // This eShopOnContainers method is querying a database table
                // and publishing events into the Event Bus (RabbitMQ / ServiceBus)
                await ResolveCompletedOrders();

                await Task.Delay(_settings.CheckTime, stoppingToken);
            }

            _logger.LogDebug($"ClearCompletedOrder background task is stopping.");
        }

        private async Task ResolveCompletedOrders()
        {
            _logger.LogDebug($"Clearing Completed Order: Start clearing...");

            try
            {

                using (var scope = scopeFactory.CreateScope())
                {
                    var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

                    await orderRepository.DeleteFinishedOrder();
                    await orderRepository.UnitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"Clearing Completed Order: Catch exception:" + ex.ToString());
            }

            _logger.LogDebug($"Clearing Completed Order: Finished");
        }
    }
}
