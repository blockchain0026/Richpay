using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Distributions;
using Distributing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebWorkerService.BackgroundServices
{
    public class ResolveCompletedDistributionBackgroundService : BackgroundService
    {
        private readonly ILogger<ResolveCompletedDistributionBackgroundService> _logger;

        private readonly IServiceScopeFactory scopeFactory;
        public ResolveCompletedDistributionBackgroundService(ILogger<ResolveCompletedDistributionBackgroundService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        //private readonly IOrderRepository _orderRepository;


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"ResolveCompletedDistributionBackgroundService is starting.");

            stoppingToken.Register(() =>
                _logger.LogDebug($" ResolveCompletedDistribution background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"ResolveCompletedDistribution task doing background work.");

                // This eShopOnContainers method is querying a database table
                // and publishing events into the Event Bus (RabbitMQ / ServiceBus)
                await ResolveCompletedDistributions();

                await Task.Delay(30000, stoppingToken);
            }

            _logger.LogDebug($"ResolveCompletedDistribution background task is stopping.");
        }


        private async Task ResolveCompletedDistributions()
        {
            Console.WriteLine("Start Resolving Completed Distributions... Date:" + DateTime.UtcNow);
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var distributingContext = scope.ServiceProvider.GetRequiredService<DistributingContext>();

                    //Check db connection.
                    if (!await distributingContext.Database.CanConnectAsync())
                    {
                        return;
                    }

                    Console.WriteLine("Resolving Completed Distributions In Process... Date:" + DateTime.UtcNow);


                    //Get recently created distributions.
                    Console.WriteLine("Resolving Completed Distributions: Get recently created distributions... Date:" + DateTime.UtcNow);

                    var distributionGroups = distributingContext.Distributions
                        .AsNoTracking()
                        .AsEnumerable()
                        .GroupBy(d => d.UserId);

                    Console.WriteLine("Resolving Completed Distributions: Resolve each user's distribution... Date:" + DateTime.UtcNow);
                    if (!distributionGroups.Any())
                    {
                        Console.WriteLine("Resolving Completed Distributions Success! --No distributions to resovle-- Date:" + DateTime.UtcNow);
                        return;
                    }
                    var tasks = new List<Task>();

                    //Resolve each user's distribution.
                    foreach (var distributions in distributionGroups)
                    {
                        tasks.Add(Task.Run(() => ResolveUserDistributions(distributions)));
                    };

                    await Task.WhenAll(tasks);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Catch Exception When Resolving Completed Distributions... Date:" + DateTime.UtcNow);
                Console.WriteLine("Exception Description: " + ex.Message);
            }
        }

        private async Task ResolveUserDistributions(IGrouping<string, Distribution> distributions)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var distributingContext = scope.ServiceProvider.GetRequiredService<DistributingContext>();

                var balanceRepository = scope.ServiceProvider.GetRequiredService<IBalanceRepository>();

                Console.WriteLine("Resolving User Distributions In Process... Date:" + DateTime.UtcNow);

                //Get recently created distributions.
                Console.WriteLine("Resolving User Distributions: Get recently created distributions... Date:" + DateTime.UtcNow);

                // To Platform order: Find qr code to pairing.
                // To Fourth Party order: Send request to fourth party.
                var distribuingStrategy = distributingContext.Database.CreateExecutionStrategy();

                await distribuingStrategy.ExecuteAsync(async () =>
                {
                    using (var distributingTransaction = await distributingContext.BeginTransactionAsync())
                    {
                        try
                        {
                            var userId = distributions.Key;

                            //Sum the distributed amount.
                            var totalDistributedAmount = distributions.Sum(d => d.DistributedAmount);
                            await balanceRepository.UpdateBalanceForDistributeByUserId(
                                userId,
                                totalDistributedAmount
                                );


                            //Delete distributions.
                            Console.WriteLine("Resolving User Distributions: Delete distributions... Date:" + DateTime.UtcNow);
                            distributingContext.Distributions.RemoveRange(distributions);


                            Console.WriteLine("Resolving User Distributions: Save Changes... Date:" + DateTime.UtcNow);
                            //Save changes for balance entities.
                            await distributingContext.SaveChangesAsync();

                            //Commit Transaction.
                            //If there is any error, transaction will rollback, and throw the error after the rollback.
                            await distributingContext.CommitTransactionOnlyAsync(distributingTransaction);


                            //--- If no concurrency conflics happened, set Saved to true and ---//
                            //saved = true;

                            Console.WriteLine("Resolving User Distributions Success! Date:" + DateTime.UtcNow);
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            Console.WriteLine("Catch DbUpdateConcurrencyException When Resolving User Distributions... Date:" + DateTime.UtcNow);

                            //If the transaction failed because of concurrencies and conflicts,
                            //then re-pairing again.

                            distributingContext.RollbackTransaction(false);

                            //saved = false;

                            //throw; //Throw the exception and catch by the catch block out side the tranasaction, then sleep for random secs.
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Catch Exception When Resolving User Distributions... Date:" + DateTime.UtcNow);
                            Console.WriteLine("Exception Description: " + ex.Message);

                            distributingContext.RollbackTransaction(false);

                            //saved = true;
                            //throw;
                        }
                        finally
                        {
                            distributingContext.DisposeTransaction();
                        }
                    }
                });

            }
        }

    }
}
