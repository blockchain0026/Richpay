using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Orders;
using WebMVC.Hubs;
using Z.EntityFramework.Plus;

namespace WebMVC.Infrastructure.HostServices
{
    public class OrderStatisticSenderBroadcasterBackgroundService : BackgroundService
    {
        private readonly ILogger<OrderStatisticSenderBroadcasterBackgroundService> _logger;

        private readonly IServiceScopeFactory scopeFactory;
        private readonly IHubContext<OrderStatisticHub> _orderStatisticHubContext;

        public OrderStatisticSenderBroadcasterBackgroundService(ILogger<OrderStatisticSenderBroadcasterBackgroundService> logger, IServiceScopeFactory scopeFactory, IHubContext<OrderStatisticHub> orderStatisticHubContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _orderStatisticHubContext = orderStatisticHubContext ?? throw new ArgumentNullException(nameof(orderStatisticHubContext));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"OrderStatisticSenderBroadcasterBackgroundService is starting.");

            //var options = new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(25) };
            //QueryCacheManager.DefaultMemoryCacheEntryOptions = options;

            while (!stoppingToken.IsCancellationRequested)
            {
                // This eShopOnContainers method is querying a database table
                // and publishing events into the Event Bus (RabbitMQ / ServiceBus)
                await BroadcastOrderStatistic();

                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task BroadcastOrderStatistic()
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var orderQueries = scope.ServiceProvider.GetRequiredService<IOrderQueries>();

                    var orderStatistic = await orderQueries.GetOrderEntriesStatisticAsync(null, null, null);
                    //var orderStatistic = await orderQueries.GetOrderEntriesStatisticAsync(null, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

                    await _orderStatisticHubContext.Clients.All.SendAsync("RefreshData", new object[] { orderStatistic });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Broadcast Order Errors: " + ex.Message);
            }
        }

    }
}
