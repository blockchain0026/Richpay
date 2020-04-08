using Distributing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Model.Orders;
using Ordering.Infrastructure;
using Pairing.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Orders;
using WebMVC.Data;

namespace WebMVC.Infrastructure.HostServices
{
    public class ResolveExpiredOrderBackgroundService : BackgroundService
    {
        private readonly ILogger<ResolveExpiredOrderBackgroundService> _logger;

        private readonly IServiceScopeFactory scopeFactory;

        public ResolveExpiredOrderBackgroundService(ILogger<ResolveExpiredOrderBackgroundService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"ResolveExpiredOrderBackgroundService is starting.");

            stoppingToken.Register(() =>
                _logger.LogDebug($" ResolveExpiredOrderBackgroundService background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"ResolveExpiredOrders task doing background work.");

                await ResolveExpiredOrders();

                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogDebug($"ResolveExpiredOrders background task is stopping.");
        }

        private async Task ResolveExpiredOrders()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var orderingContext = scope.ServiceProvider.GetRequiredService<OrderingContext>();
                var pairingContext = scope.ServiceProvider.GetRequiredService<PairingContext>();
                var distributingContext = scope.ServiceProvider.GetRequiredService<DistributingContext>();
                var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var orderQueries = scope.ServiceProvider.GetRequiredService<IOrderQueries>();

                var currentDateTime = DateTime.UtcNow;

                var toMarkExpiredOrdersId = orderingContext.Orders
                    .AsNoTracking()
                    .Include(o => o.OrderStatus)
                    .Where(o => !o.IsExpired
                    && o.OrderStatus.Id != OrderStatus.Success.Id
                    && currentDateTime > o.DateCreated.AddSeconds((int)o.ExpirationTimeInSeconds))
                    .Select(o => o.Id);

                foreach (var orderId in toMarkExpiredOrdersId)
                {
                    // To Platform order: Find qr code to pairing.
                    // To Fourth Party order: Send request to fourth party.
                    var orderingStrategy = orderingContext.Database.CreateExecutionStrategy();
                    var pairingStrategy = pairingContext.Database.CreateExecutionStrategy();
                    var distribuingStrategy = distributingContext.Database.CreateExecutionStrategy();
                    var applicationStrategy = applicationContext.Database.CreateExecutionStrategy();

                    await orderingStrategy.ExecuteAsync(async () =>
                    {
                        await pairingStrategy.ExecuteAsync(async () =>
                        {
                            await distribuingStrategy.ExecuteAsync(async () =>
                            {
                                await applicationStrategy.ExecuteAsync(async () =>
                                {

                                    using (var orderingTransaction = await orderingContext.BeginTransactionAsync())
                                    {
                                        using (var pairingTransaction = await pairingContext.BeginTransactionAsync())
                                        {
                                            using (var distributingTransaction = await distributingContext.BeginTransactionAsync())
                                            {
                                                using (var applicationTransaction = await applicationContext.BeginTransactionAsync())
                                                {
                                                    try
                                                    {
                                                        var order = await orderingContext.Orders
                                                        .Where(o => o.Id == orderId)
                                                        .FirstOrDefaultAsync();

                                                        //Mark order as expired.
                                                        Console.WriteLine("Resolving Expired Orders: Mark Order as Expired... Date:" + DateTime.UtcNow);
                                                        order.Expired();

                                                        //Update order vm.
                                                        Console.WriteLine("Resolving Expired Orders: Update order view model... Date:" + DateTime.UtcNow);
                                                        orderQueries.UpdateOrderEntryToExpired(
                                                                        order.Id,
                                                                        order.GetOrderStatus.Name,
                                                                        order.OrderStatusDescription);

                                                        Console.WriteLine("Resolving Expired Orders: Save Changes... Date:" + DateTime.UtcNow);
                                                        //Save changes and execute domain event.
                                                        await orderingContext.SaveEntitiesAsync();

                                                        // Saves all view model created at previous processes.
                                                        // The queries all belongs to one db ccontext, so only need to save once.
                                                        await orderQueries.SaveChangesAsync();

                                                        //Commit Transaction.
                                                        //If there is any error, transaction will rollback, and throw the error after the rollback.
                                                        await orderingContext.CommitTransactionOnlyAsync(orderingTransaction);
                                                        await pairingContext.CommitTransactionOnlyAsync(pairingTransaction);
                                                        await distributingContext.CommitTransactionOnlyAsync(distributingTransaction);
                                                        await applicationContext.CommitTransactionOnlyAsync(applicationTransaction);

                                                        //--- If no concurrency conflics happened, set Saved to true and ---//
                                                        //saved = true;

                                                        Console.WriteLine("Resolving Expired Orders Success! Date:" + DateTime.UtcNow);
                                                    }
                                                    catch (DbUpdateConcurrencyException)
                                                    {
                                                        Console.WriteLine("Catch DbUpdateConcurrencyException When Resolving Expired Orders... Date:" + DateTime.UtcNow);

                                                        //If the transaction failed because of concurrencies and conflicts,

                                                        orderingContext.RollbackTransaction();
                                                        pairingContext.RollbackTransaction();
                                                        distributingContext.RollbackTransaction();
                                                        applicationContext.RollbackTransaction();

                                                        //saved = false;

                                                        //throw; //Throw the exception and catch by the catch block out side the tranasaction, then sleep for random secs.
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Console.WriteLine("Catch Exception When Resolving Expired Orders... Date:" + DateTime.UtcNow);
                                                        Console.WriteLine("Exception Description: " + ex.Message);

                                                        orderingContext.RollbackTransaction();
                                                        pairingContext.RollbackTransaction();
                                                        distributingContext.RollbackTransaction();
                                                        applicationContext.RollbackTransaction();

                                                        //throw;
                                                    }
                                                    finally
                                                    {
                                                        orderingContext.DisposeTransaction();
                                                        pairingContext.DisposeTransaction();
                                                        distributingContext.DisposeTransaction();
                                                        applicationContext.DisposeTransaction();
                                                    }
                                                }
                                            }
                                        }
                                    }

                                });
                            });
                        });
                    });
                }


            }
        }
    }
}
