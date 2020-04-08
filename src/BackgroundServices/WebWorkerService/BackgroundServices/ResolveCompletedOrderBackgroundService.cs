using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Model.Orders;
using Ordering.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebWorkerService.ApiClients;

namespace WebWorkerService.BackgroundServices
{
    public class ResolveCompletedOrderBackgroundService : BackgroundService
    {
        private readonly ILogger<ResolveCompletedOrderBackgroundService> _logger;
        private readonly IReturnOrderApiClient _returnOrderApiClient;

        private readonly IServiceScopeFactory scopeFactory;

        public ResolveCompletedOrderBackgroundService(ILogger<ResolveCompletedOrderBackgroundService> logger, IReturnOrderApiClient returnOrderApiClient, IServiceScopeFactory scopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _returnOrderApiClient = returnOrderApiClient ?? throw new ArgumentNullException(nameof(returnOrderApiClient));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"ResolveCompletedOrderBackgroundService is starting.");

            stoppingToken.Register(() =>
                _logger.LogDebug($" ResolveCompletedOrderBackgroundService background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                await ResolveCompletedOrders();

                await Task.Delay(10000, stoppingToken);
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
                    var orderingContext = scope.ServiceProvider.GetRequiredService<OrderingContext>();
                    //Check db connection.
                    if (!await orderingContext.Database.CanConnectAsync())
                    {
                        return;
                    }
                    var orders = orderingContext
                            .Orders
                            .Include(o => o.OrderStatus)
                            .Where(o => o.OrderStatus.Id == OrderStatus.Success.Id || o.IsExpired);

                    foreach (var order in orders)
                    {
                        if (!order.IsTestOrder)
                        {
                            await orderingContext.Entry(order)
                               .Reference(b => b.ShopInfo).LoadAsync();

                            string url = string.Empty;
                            string orderTrackingNumber = string.Empty;
                            string shopOrderId = string.Empty;
                            string shopId = string.Empty;
                            string dateCreated = string.Empty;
                            string dateConfirmed = string.Empty;
                            int orderAmount = 0;
                            int orderStatus = 0;

                            //Assign request value.
                            url = order.ShopInfo.ShopOkReturnUrl;
                            orderTrackingNumber = order.TrackingNumber;
                            shopOrderId = order.ShopInfo.ShopOrderId;
                            shopId = order.ShopInfo.ShopId;
                            dateCreated = order.DateCreated.ToString("yyyy/MM/dd HH:mm:ss");
                            dateConfirmed = order.DatePaymentRecieved?.ToString("yyyy/MM/dd HH:mm:ss");
                            orderAmount = (int)order.Amount;
                            orderStatus = order.GetOrderStatus.Id;

                            //Send order info to shop.
                            await _returnOrderApiClient.ReturnOrderResult(
                                url,
                                orderTrackingNumber,
                                shopOrderId,
                                shopId,
                                dateCreated,
                                dateConfirmed,
                                orderAmount,
                                orderStatus
                                );
                        }
                    }
                    
                    orderingContext.RemoveRange(orders);
                    await orderingContext.SaveChangesAsync();
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
