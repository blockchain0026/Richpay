using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Model.Orders;
using Ordering.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Infrastructure.Services;

namespace WebMVC.Infrastructure.HostServices
{
    public class ConfirmAwaitingPaymentOrderBackgroundService : BackgroundService
    {
        private readonly ILogger<ClearCompletedOrderBackgroundService> _logger;

        private readonly IServiceScopeFactory scopeFactory;

        public ConfirmAwaitingPaymentOrderBackgroundService(ILogger<ClearCompletedOrderBackgroundService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"ClearCompletedOrderBackgroundService is starting.");

            stoppingToken.Register(() =>
                _logger.LogDebug($" ClearCompletedOrder background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"ClearCompletedOrder task doing background work.");

                // This eShopOnContainers method is querying a database table
                // and publishing events into the Event Bus (RabbitMQ / ServiceBus)
                await ConfirmOrders();

                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogDebug($"ClearCompletedOrder background task is stopping.");
        }

        private async Task ConfirmOrders()
        {
            return;
            using (var scope = scopeFactory.CreateScope())
            {
                var orderingContext = scope.ServiceProvider.GetRequiredService<OrderingContext>();
                var apiService = scope.ServiceProvider.GetRequiredService<IApiService>();

                var awaitingPaymentOrderIds = orderingContext.Orders
                    .Include(o => o.OrderStatus)
                    .Where(o => o.OrderStatus.Id == OrderStatus.AwaitingPayment.Id)
                    .Select(o => o.Id);

                var tasks = new List<Task>();

                foreach (var orderId in awaitingPaymentOrderIds)
                {
                    tasks.Add(Task.Run(() => apiService.ConfirmOrder(orderId)));
                    //await apiService.ConfirmOrder(orderId);
                }

                await Task.WhenAll(tasks);
            }
        }
    }
}
