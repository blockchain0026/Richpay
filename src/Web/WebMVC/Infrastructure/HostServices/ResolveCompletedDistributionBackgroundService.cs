using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Distributions;
using Distributing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ordering.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.RunningAccounts;
using WebMVC.Applications.Queries.ShopAgents;
using WebMVC.Applications.Queries.Shops;
using WebMVC.Applications.Queries.TraderAgents;
using WebMVC.Applications.Queries.Traders;
using WebMVC.Data;
using WebMVC.Models.Queries;
using WebMVC.Settings;

namespace WebMVC.Infrastructure.HostServices
{
    public class ResolveCompletedDistributionBackgroundService : BackgroundService
    {
        private readonly ILogger<ResolveCompletedDistributionBackgroundService> _logger;
        private readonly ResolveCompletedDistributionSettings _settings;

        private readonly IServiceScopeFactory scopeFactory;
        private readonly List<RunningAccountRecord> _runningAccountRecordsToAdd;
        public ResolveCompletedDistributionBackgroundService(ILogger<ResolveCompletedDistributionBackgroundService> logger,
            IOptions<ResolveCompletedDistributionSettings> settings, IServiceScopeFactory scopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _runningAccountRecordsToAdd = new List<RunningAccountRecord>();
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

                await Task.Delay(_settings.CheckTime, stoppingToken);
            }

            _logger.LogDebug($"ResolveCompletedDistribution background task is stopping.");
        }


        private async Task ResolveCompletedDistributions()
        {
            Console.WriteLine("Start Resolving Completed Distributions... Date:" + DateTime.UtcNow);

            using (var scope = scopeFactory.CreateScope())
            {
                var distributingContext = scope.ServiceProvider.GetRequiredService<DistributingContext>();

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

        private async Task ResolveUserDistributions(IGrouping<string,Distribution> distributions)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var distributingContext = scope.ServiceProvider.GetRequiredService<DistributingContext>();
                var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var runningAccountRecordQueries = scope.ServiceProvider.GetRequiredService<IRunningAccountQueries>();

                /*var traderQueries = scope.ServiceProvider.GetRequiredService<ITraderQueries>();
                var traderAgentQueries = scope.ServiceProvider.GetRequiredService<ITraderAgentQueries>();
                var shopQueries = scope.ServiceProvider.GetRequiredService<IShopQueries>();
                var shopAgentQueries = scope.ServiceProvider.GetRequiredService<IShopAgentQueries>();
                */
                var balanceRepository = scope.ServiceProvider.GetRequiredService<IBalanceRepository>();

                Console.WriteLine("Resolving Completed Distributions In Process... Date:" + DateTime.UtcNow);


                //Get recently created distributions.
                Console.WriteLine("Resolving Completed Distributions: Get recently created distributions... Date:" + DateTime.UtcNow);

                // To Platform order: Find qr code to pairing.
                // To Fourth Party order: Send request to fourth party.
                var distribuingStrategy = distributingContext.Database.CreateExecutionStrategy();
                var applicationStrategy = applicationContext.Database.CreateExecutionStrategy();

                await distribuingStrategy.ExecuteAsync(async () =>
                {
                    await applicationStrategy.ExecuteAsync(async () =>
                    {
                        using (var distributingTransaction = await distributingContext.BeginTransactionAsync())
                        {
                            using (var applicationTransaction = await applicationContext.BeginTransactionAsync())
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
                                    Console.WriteLine("Resolving Completed Distributions: Delete distributions... Date:" + DateTime.UtcNow);
                                    distributingContext.Distributions.RemoveRange(distributions);


                                    Console.WriteLine("Resolving Completed Distributions: Save Changes... Date:" + DateTime.UtcNow);
                                    //Save changes for balance entities.
                                    await distributingContext.SaveChangesAsync();

                                    // Saves all view model created at previous processes.
                                    // The queries all belongs to one db ccontext, so only need to save once.
                                    await runningAccountRecordQueries.SaveChangesAsync();

                                    //Commit Transaction.
                                    //If there is any error, transaction will rollback, and throw the error after the rollback.
                                    await distributingContext.CommitTransactionOnlyAsync(distributingTransaction);
                                    await applicationContext.CommitTransactionOnlyAsync(applicationTransaction);


                                    //--- If no concurrency conflics happened, set Saved to true and ---//
                                    //saved = true;

                                    Console.WriteLine("Resolving Completed Distributions Success! Date:" + DateTime.UtcNow);
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    Console.WriteLine("Catch DbUpdateConcurrencyException When Resolving Completed Distributions... Date:" + DateTime.UtcNow);

                                    //If the transaction failed because of concurrencies and conflicts,
                                    //then re-pairing again.

                                    distributingContext.RollbackTransaction();
                                    applicationContext.RollbackTransaction();

                                    //saved = false;

                                    //throw; //Throw the exception and catch by the catch block out side the tranasaction, then sleep for random secs.
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Catch Exception When Resolving Completed Distributions... Date:" + DateTime.UtcNow);
                                    Console.WriteLine("Exception Description: " + ex.Message);

                                    distributingContext.RollbackTransaction();
                                    applicationContext.RollbackTransaction();

                                    //saved = true;
                                    //throw;
                                }
                                finally
                                {
                                    distributingContext.DisposeTransaction();
                                    applicationContext.DisposeTransaction();
                                }
                            }
                        }
                    });
                });

            }
        }
        private async Task ResolveCompletedDistributions_Deprcated6()
        {
            Console.WriteLine("Start Resolving Completed Distributions... Date:" + DateTime.UtcNow);

            using (var scope = scopeFactory.CreateScope())
            {
                var distributingContext = scope.ServiceProvider.GetRequiredService<DistributingContext>();
                var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var runningAccountRecordQueries = scope.ServiceProvider.GetRequiredService<IRunningAccountQueries>();

                var traderQueries = scope.ServiceProvider.GetRequiredService<ITraderQueries>();
                var traderAgentQueries = scope.ServiceProvider.GetRequiredService<ITraderAgentQueries>();
                var shopQueries = scope.ServiceProvider.GetRequiredService<IShopQueries>();
                var shopAgentQueries = scope.ServiceProvider.GetRequiredService<IShopAgentQueries>();

                var balanceRepository = scope.ServiceProvider.GetRequiredService<IBalanceRepository>();

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

                //Resolve each user's distribution.
                foreach (var distributions in distributionGroups)
                {
                    // To Platform order: Find qr code to pairing.
                    // To Fourth Party order: Send request to fourth party.
                    var distribuingStrategy = distributingContext.Database.CreateExecutionStrategy();
                    var applicationStrategy = applicationContext.Database.CreateExecutionStrategy();

                    await distribuingStrategy.ExecuteAsync(async () =>
                    {
                        await applicationStrategy.ExecuteAsync(async () =>
                        {
                            using (var distributingTransaction = await distributingContext.BeginTransactionAsync())
                            {
                                using (var applicationTransaction = await applicationContext.BeginTransactionAsync())
                                {
                                    try
                                    {
                                        var userId = distributions.Key;
                                        /*var balance = await distributingContext.Balances
                                        .Where(b => b.UserId == userId)
                                        .FirstOrDefaultAsync();

                                        if (balance != null)
                                        {
                                            //Sum the distributed amount.
                                            var totalDistributedAmount = distributions.Sum(d => d.DistributedAmount);

                                            //Distribute to balance.
                                            balance.Distribute(totalDistributedAmount);
                                        }*/
                                        //Sum the distributed amount.
                                        var totalDistributedAmount = distributions.Sum(d => d.DistributedAmount);
                                        await balanceRepository.UpdateBalanceForDistributeByUserId(
                                            userId,
                                            totalDistributedAmount
                                            );



                                        //Delete distributions.
                                        Console.WriteLine("Resolving Completed Distributions: Delete distributions... Date:" + DateTime.UtcNow);
                                        distributingContext.Distributions.RemoveRange(distributions);


                                        Console.WriteLine("Resolving Completed Distributions: Save Changes... Date:" + DateTime.UtcNow);
                                        //Save changes for balance entities.
                                        await distributingContext.SaveChangesAsync();

                                        // Saves all view model created at previous processes.
                                        // The queries all belongs to one db ccontext, so only need to save once.
                                        await shopAgentQueries.SaveChangesAsync();

                                        //Commit Transaction.
                                        //If there is any error, transaction will rollback, and throw the error after the rollback.
                                        await distributingContext.CommitTransactionOnlyAsync(distributingTransaction);
                                        await applicationContext.CommitTransactionOnlyAsync(applicationTransaction);


                                        //--- If no concurrency conflics happened, set Saved to true and ---//
                                        //saved = true;

                                        Console.WriteLine("Resolving Completed Distributions Success! Date:" + DateTime.UtcNow);
                                    }
                                    catch (DbUpdateConcurrencyException)
                                    {
                                        Console.WriteLine("Catch DbUpdateConcurrencyException When Resolving Completed Distributions... Date:" + DateTime.UtcNow);

                                        //If the transaction failed because of concurrencies and conflicts,
                                        //then re-pairing again.

                                        distributingContext.RollbackTransaction();
                                        applicationContext.RollbackTransaction();

                                        //saved = false;

                                        //throw; //Throw the exception and catch by the catch block out side the tranasaction, then sleep for random secs.
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Catch Exception When Resolving Completed Distributions... Date:" + DateTime.UtcNow);
                                        Console.WriteLine("Exception Description: " + ex.Message);

                                        distributingContext.RollbackTransaction();
                                        applicationContext.RollbackTransaction();

                                        //saved = true;
                                        //throw;
                                    }
                                    finally
                                    {
                                        distributingContext.DisposeTransaction();
                                        applicationContext.DisposeTransaction();
                                    }
                                }
                            }
                        });
                    });
                };
            }
        }


        private async Task ResolveCompletedDistributions_Deprecated4()
        {
            Console.WriteLine("Start Resolving Completed Distributions... Date:" + DateTime.UtcNow);

            using (var scope = scopeFactory.CreateScope())
            {
                var distributingContext = scope.ServiceProvider.GetRequiredService<DistributingContext>();
                var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var runningAccountRecordQueries = scope.ServiceProvider.GetRequiredService<IRunningAccountQueries>();

                var traderQueries = scope.ServiceProvider.GetRequiredService<ITraderQueries>();
                var traderAgentQueries = scope.ServiceProvider.GetRequiredService<ITraderAgentQueries>();
                var shopQueries = scope.ServiceProvider.GetRequiredService<IShopQueries>();
                var shopAgentQueries = scope.ServiceProvider.GetRequiredService<IShopAgentQueries>();

                Console.WriteLine("Resolving Completed Distributions In Process... Date:" + DateTime.UtcNow);

                // To Platform order: Find qr code to pairing.
                // To Fourth Party order: Send request to fourth party.
                var distribuingStrategy = distributingContext.Database.CreateExecutionStrategy();
                var applicationStrategy = applicationContext.Database.CreateExecutionStrategy();

                await distribuingStrategy.ExecuteAsync(async () =>
                {
                    await applicationStrategy.ExecuteAsync(async () =>
                    {
                        using (var distributingTransaction = await distributingContext.BeginTransactionAsync())
                        {
                            using (var applicationTransaction = await applicationContext.BeginTransactionAsync())
                            {
                                try
                                {
                                    //Initialize a list for later use for Remove Range.
                                    var runningAccountRecordsToAdd = new List<RunningAccountRecord>();
                                    var tempDistributionsToDelete = new List<Distribution>();

                                    //Get recently created distributions.
                                    Console.WriteLine("Resolving Completed Distributions: Get recently created distributions... Date:" + DateTime.UtcNow);

                                    var distributions = distributingContext.Distributions;

                                    //Resolve each user's distribution.


                                    var balanceDictionary = new Dictionary<string, decimal>();
                                    if (!await distributions.AnyAsync())
                                    {
                                        Console.WriteLine("No distributions to resolve... Date:" + DateTime.UtcNow);
                                        return;
                                    }
                                    Parallel.ForEach(distributions, distribution =>
                                    {
                                        var userId = distribution.UserId;

                                        //decimal balanceToAddSum = 0;

                                        var distributedAmount = distribution.DistributedAmount;

                                        //Increase user balance to add.
                                        if (balanceDictionary.ContainsKey(userId))
                                        {
                                            balanceDictionary[userId] += distributedAmount;
                                        }
                                        else
                                        {
                                            balanceDictionary.Add(userId, distributedAmount);
                                        }


                                        //Add new running account record to list.
                                        runningAccountRecordsToAdd.Add(runningAccountRecordQueries.MapFromData(
                                            distribution.Order.TrackingNumber,
                                            distribution.Order.ShopId,
                                            distribution.Order.ShopOrderId,
                                            distribution.Order.TraderId,
                                            distribution.Order.Amount,
                                            distribution.Order.DateCreated,
                                            distributedAmount,
                                            userId
                                            ));

                                        //tempDistributionsToDelete.Add(distribution);
                                    });

                                    //Distribute the amount to user's balance.
                                    Console.WriteLine("Resolving Completed Distributions: Distribute the amount to user's balance... Date:" + DateTime.UtcNow);

                                    foreach (var balanceToAdd in balanceDictionary)
                                    {
                                        var balance = distributingContext.Balances
                                       .Where(b => b.UserId == balanceToAdd.Key)
                                       .FirstOrDefault();

                                        if (balance == null)
                                        {
                                            throw new KeyNotFoundException("No balance found by distribution's user id. At ResolveCompletedDistributionBackgroundTaskService()");
                                        }
                                        var userId = balanceToAdd.Key;
                                        var totalDistributedAmount = balanceToAdd.Value;

                                        balance.Distribute(totalDistributedAmount);

                                        //Update balance view model.
                                        Console.WriteLine("Resolving Completed Distributions: Update balance view model... Date:" + DateTime.UtcNow);
                                        if (balance.GetUserType.Name == UserType.Trader.Name)
                                        {
                                            traderQueries.UpdateBalance(userId, totalDistributedAmount);
                                        }
                                        else if (balance.GetUserType.Name == UserType.TraderAgent.Name)
                                        {
                                            traderAgentQueries.UpdateBalance(userId, totalDistributedAmount);
                                        }
                                        else if (balance.GetUserType.Name == UserType.Shop.Name)
                                        {
                                            shopQueries.UpdateBalance(userId, totalDistributedAmount);
                                        }
                                        else if (balance.GetUserType.Name == UserType.ShopAgent.Name)
                                        {
                                            shopAgentQueries.UpdateBalance(userId, totalDistributedAmount);
                                        }
                                        else
                                        {
                                            throw new Exception("Unrecognized user type. At ResolveCompletedDistributionBackgroundTaskService.");
                                        }
                                    }


                                    //Delete distributions. (No need anymore)
                                    Console.WriteLine("Resolving Completed Distributions: Delete distributions... Date:" + DateTime.UtcNow);

                                    runningAccountRecordQueries.AddRange(runningAccountRecordsToAdd);
                                    distributingContext.Distributions.RemoveRange(distributions);

                                    Console.WriteLine("Resolving Completed Distributions: Save Changes... Date:" + DateTime.UtcNow);
                                    //Save changes for balance entities.
                                    await distributingContext.SaveChangesAsync();

                                    // Saves all view model created at previous processes.
                                    // The queries all belongs to one db ccontext, so only need to save once.
                                    await shopAgentQueries.SaveChangesAsync();

                                    //Commit Transaction.
                                    //If there is any error, transaction will rollback, and throw the error after the rollback.
                                    await distributingContext.CommitTransactionOnlyAsync(distributingTransaction);
                                    await applicationContext.CommitTransactionOnlyAsync(applicationTransaction);


                                    //--- If no concurrency conflics happened, set Saved to true and ---//
                                    //saved = true;

                                    Console.WriteLine("Resolving Completed Distributions Success! Date:" + DateTime.UtcNow);
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    Console.WriteLine("Catch DbUpdateConcurrencyException When Resolving Completed Distributions... Date:" + DateTime.UtcNow);

                                    //If the transaction failed because of concurrencies and conflicts,
                                    //then re-pairing again.

                                    distributingContext.RollbackTransaction();
                                    applicationContext.RollbackTransaction();

                                    //saved = false;

                                    //throw; //Throw the exception and catch by the catch block out side the tranasaction, then sleep for random secs.
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Catch Exception When Resolving Completed Distributions... Date:" + DateTime.UtcNow);
                                    Console.WriteLine("Exception Description: " + ex.Message);

                                    distributingContext.RollbackTransaction();
                                    applicationContext.RollbackTransaction();

                                    //saved = true;
                                    //throw;
                                }
                                finally
                                {
                                    distributingContext.DisposeTransaction();
                                    applicationContext.DisposeTransaction();
                                }

                            }
                        }
                    });
                });


            }
        }

        private async Task ResolveCompletedDistributions_Deprecated3()
        {
            Console.WriteLine("Start Resolving Completed Distributions... Date:" + DateTime.UtcNow);

            using (var scope = scopeFactory.CreateScope())
            {
                var distributingContext = scope.ServiceProvider.GetRequiredService<DistributingContext>();
                var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var runningAccountRecordQueries = scope.ServiceProvider.GetRequiredService<IRunningAccountQueries>();

                var traderQueries = scope.ServiceProvider.GetRequiredService<ITraderQueries>();
                var traderAgentQueries = scope.ServiceProvider.GetRequiredService<ITraderAgentQueries>();
                var shopQueries = scope.ServiceProvider.GetRequiredService<IShopQueries>();
                var shopAgentQueries = scope.ServiceProvider.GetRequiredService<IShopAgentQueries>();

                Console.WriteLine("Resolving Completed Distributions In Process... Date:" + DateTime.UtcNow);

                // To Platform order: Find qr code to pairing.
                // To Fourth Party order: Send request to fourth party.
                var distribuingStrategy = distributingContext.Database.CreateExecutionStrategy();
                var applicationStrategy = applicationContext.Database.CreateExecutionStrategy();

                await distribuingStrategy.ExecuteAsync(async () =>
                {
                    await applicationStrategy.ExecuteAsync(async () =>
                    {
                        using (var distributingTransaction = await distributingContext.BeginTransactionAsync())
                        {
                            using (var applicationTransaction = await applicationContext.BeginTransactionAsync())
                            {
                                try
                                {
                                    //Initialize a list for later use for Remove Range.
                                    var runningAccountRecordsToAdd = new List<RunningAccountRecord>();
                                    var tempDistributionsToDelete = new List<Distribution>();

                                    //Get recently created distributions.
                                    Console.WriteLine("Resolving Completed Distributions: Get recently created distributions... Date:" + DateTime.UtcNow);

                                    var distributionGroups = distributingContext.Distributions
                                    //.Include(d => d.Order)
                                    /*.Select(u => new
                                    {
                                        u.Id,
                                        u.UserId,
                                        u.DistributedAmount,
                                        OrderTrackingNumber = u.Order.TrackingNumber,
                                        ShopId = u.Order.ShopId,
                                        ShopOrderId = u.Order.ShopOrderId,
                                        TraderId = u.Order.TraderId,
                                        Amount = u.Order.Amount,
                                        DateCreated = u.Order.DateCreated
                                    })*/
                                    .AsEnumerable()
                                    .GroupBy(d => d.UserId)
                                    .Take(5);

                                    Console.WriteLine("Resolving Completed Distributions: Resolve each user's distribution... Date:" + DateTime.UtcNow);
                                    //Resolve each user's distribution.
                                    foreach (var distributions in distributionGroups)
                                    {
                                        var userId = distributions.Key;
                                        decimal balanceToAddSum = 0;
                                        Parallel.ForEach(distributions, distribution =>
                                        {
                                            var distributedAmount = distribution.DistributedAmount;

                                            //Increase balance to add.
                                            balanceToAddSum += distribution.DistributedAmount;

                                            //Add new running account record to list.
                                            /*runningAccountRecordsToAdd.Add(runningAccountRecordQueries.MapFromData(
                                                distribution.Order.TrackingNumber,
                                                distribution.Order.ShopId,
                                                distribution.Order.ShopOrderId,
                                                distribution.Order.TraderId,
                                                distribution.Order.Amount,
                                                distribution.Order.DateCreated,
                                                distributedAmount,
                                                userId
                                                ));*/
                                            /*runningAccountRecordsToAdd.Add(await runningAccountRecordQueries.MapFromDataAsync(
                                                distribution.Order.TrackingNumber,
                                                distribution.Order.ShopId,
                                                distribution.Order.ShopOrderId,
                                                distribution.Order.TraderId,
                                                distribution.Order.Amount,
                                                distribution.Order.DateCreated,
                                                userId
                                                ));*/


                                            //tempDistributionsToDelete.Add(distribution);
                                            //Delete Distribution.
                                            /*var toDelete = new Distribution(distribution.Id);
                                            distributingContext.Distributions.Attach(toDelete);
                                            distributingContext.Entry(toDelete).State = EntityState.Deleted;*/
                                        });

                                        //Distribute the amount to user's balance.
                                        Console.WriteLine("Resolving Completed Distributions: Distribute the amount to user's balance... Date:" + DateTime.UtcNow);
                                        var balance = distributingContext.Balances
                                        .Where(b => b.UserId == userId)
                                        .FirstOrDefault();

                                        if (balance == null)
                                        {
                                            throw new KeyNotFoundException("No balance found by distribution's user id. At ResolveCompletedDistributionBackgroundTaskService()");
                                        }

                                        balance.Distribute(balanceToAddSum);

                                        //Update balance view model.
                                        Console.WriteLine("Resolving Completed Distributions: Update balance view model... Date:" + DateTime.UtcNow);
                                        if (balance.GetUserType.Name == UserType.Trader.Name)
                                        {
                                            traderQueries.UpdateBalance(distributions.Key, balanceToAddSum);
                                        }
                                        else if (balance.GetUserType.Name == UserType.TraderAgent.Name)
                                        {
                                            traderAgentQueries.UpdateBalance(distributions.Key, balanceToAddSum);
                                        }
                                        else if (balance.GetUserType.Name == UserType.Shop.Name)
                                        {
                                            shopQueries.UpdateBalance(distributions.Key, balanceToAddSum);
                                        }
                                        else if (balance.GetUserType.Name == UserType.ShopAgent.Name)
                                        {
                                            shopAgentQueries.UpdateBalance(distributions.Key, balanceToAddSum);
                                        }
                                        else
                                        {
                                            throw new Exception("Unrecognized user type. At ResolveCompletedDistributionBackgroundTaskService.");
                                        }

                                        //distributingContext.Entry(balance).State = EntityState.Modified;

                                        //Delete distributions.
                                        distributingContext.Distributions.RemoveRange(distributions);
                                        Console.WriteLine("Resolving Completed Distributions: Delete distributions... Date:" + DateTime.UtcNow);
                                    };
                                    //runningAccountRecordQueries.AddRange(runningAccountRecordsToAdd);
                                    //distributingContext.Distributions.RemoveRange(tempDistributionsToDelete);

                                    Console.WriteLine("Resolving Completed Distributions: Save Changes... Date:" + DateTime.UtcNow);
                                    //Save changes for balance entities.
                                    await distributingContext.SaveChangesAsync();

                                    // Saves all view model created at previous processes.
                                    // The queries all belongs to one db ccontext, so only need to save once.
                                    await shopAgentQueries.SaveChangesAsync();

                                    //Commit Transaction.
                                    //If there is any error, transaction will rollback, and throw the error after the rollback.
                                    await distributingContext.CommitTransactionOnlyAsync(distributingTransaction);
                                    await applicationContext.CommitTransactionOnlyAsync(applicationTransaction);


                                    //--- If no concurrency conflics happened, set Saved to true and ---//
                                    //saved = true;

                                    Console.WriteLine("Resolving Completed Distributions Success! Date:" + DateTime.UtcNow);
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    Console.WriteLine("Catch DbUpdateConcurrencyException When Resolving Completed Distributions... Date:" + DateTime.UtcNow);

                                    //If the transaction failed because of concurrencies and conflicts,
                                    //then re-pairing again.

                                    distributingContext.RollbackTransaction();
                                    applicationContext.RollbackTransaction();

                                    //saved = false;

                                    //throw; //Throw the exception and catch by the catch block out side the tranasaction, then sleep for random secs.
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Catch Exception When Resolving Completed Distributions... Date:" + DateTime.UtcNow);
                                    Console.WriteLine("Exception Description: " + ex.Message);

                                    distributingContext.RollbackTransaction();
                                    applicationContext.RollbackTransaction();

                                    //saved = true;
                                    //throw;
                                }
                                finally
                                {
                                    distributingContext.DisposeTransaction();
                                    applicationContext.DisposeTransaction();
                                }

                            }
                        }
                    });
                });


            }
        }

        private async Task ResolveCompletedDistributions_Deprecated2()
        {
            Console.WriteLine("Start Resolving Completed Distributions... Date:" + DateTime.UtcNow);

            using (var scope = scopeFactory.CreateScope())
            {
                var distributingContext = scope.ServiceProvider.GetRequiredService<DistributingContext>();
                var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var runningAccountRecordQueries = scope.ServiceProvider.GetRequiredService<IRunningAccountQueries>();

                var traderQueries = scope.ServiceProvider.GetRequiredService<ITraderQueries>();
                var traderAgentQueries = scope.ServiceProvider.GetRequiredService<ITraderAgentQueries>();
                var shopQueries = scope.ServiceProvider.GetRequiredService<IShopQueries>();
                var shopAgentQueries = scope.ServiceProvider.GetRequiredService<IShopAgentQueries>();

                Console.WriteLine("Resolving Completed Distributions In Process... Date:" + DateTime.UtcNow);

                // To Platform order: Find qr code to pairing.
                // To Fourth Party order: Send request to fourth party.
                var distribuingStrategy = distributingContext.Database.CreateExecutionStrategy();
                var applicationStrategy = applicationContext.Database.CreateExecutionStrategy();

                await distribuingStrategy.ExecuteAsync(async () =>
                {
                    await applicationStrategy.ExecuteAsync(async () =>
                    {
                        using (var distributingTransaction = await distributingContext.BeginTransactionAsync())
                        {
                            using (var applicationTransaction = await applicationContext.BeginTransactionAsync())
                            {
                                try
                                {
                                    //Initialize a list for later use for Remove Range.
                                    //var runningAccountRecordsToAdd = new List<RunningAccountRecord>();
                                    var tempDistributionsToDelete = new List<Distribution>();

                                    //Get recently created distributions.
                                    Console.WriteLine("Resolving Completed Distributions: Get recently created distributions... Date:" + DateTime.UtcNow);

                                    var distributionGroups = distributingContext.Distributions
                                    .Include(d => d.Order)
                                    .Select(u => new
                                    {
                                        u.Id,
                                        u.UserId,
                                        u.DistributedAmount,
                                        OrderTrackingNumber = u.Order.TrackingNumber,
                                        ShopId = u.Order.ShopId,
                                        ShopOrderId = u.Order.ShopOrderId,
                                        TraderId = u.Order.TraderId,
                                        Amount = u.Order.Amount,
                                        DateCreated = u.Order.DateCreated
                                    })
                                    .AsEnumerable()
                                    .GroupBy(d => d.UserId);

                                    //Resolve each user's distribution.
                                    foreach (var distributions in distributionGroups)
                                    {
                                        var userId = distributions.Key;

                                        //var tempRunningAcountRecords = await runningAccountRecordQueries.GetTempByUserIdAsync(userId);

                                        //var tempRunningAccountRecords = new List<TempRunningAccountRecord>();

                                        decimal balanceToAddSum = 0;

                                        //Sum the balance to add and get running account records.
                                        Console.WriteLine("Resolving Completed Distributions: Sum the balance to add and get running account records... Date:" + DateTime.UtcNow);
                                        foreach (var distribution in distributions)
                                        {
                                            var distributedAmount = distribution.DistributedAmount;

                                            //Increase balance to add.
                                            balanceToAddSum += distribution.DistributedAmount;

                                            //Add new running account record to list.
                                            _runningAccountRecordsToAdd.Add(await runningAccountRecordQueries.MapFromDataAsync(
                                                distribution.OrderTrackingNumber,
                                                distribution.ShopId,
                                                distribution.ShopOrderId,
                                                distribution.TraderId,
                                                distribution.Amount,
                                                distribution.DateCreated,
                                                distributedAmount,
                                                userId
                                                ));
                                            /*runningAccountRecordsToAdd.Add(await runningAccountRecordQueries.MapFromDataAsync(
                                                distribution.Order.TrackingNumber,
                                                distribution.Order.ShopId,
                                                distribution.Order.ShopOrderId,
                                                distribution.Order.TraderId,
                                                distribution.Order.Amount,
                                                distribution.Order.DateCreated,
                                                userId
                                                ));*/


                                            //tempDistributionsToDelete.Add(distribution);
                                            //Delete Distribution.
                                            var toDelete = new Distribution(distribution.Id);
                                            distributingContext.Distributions.Attach(toDelete);
                                            distributingContext.Entry(toDelete).State = EntityState.Deleted;
                                            //tempDistributionsToDelete.Add(distribution);
                                        }

                                        //Distribute the amount to user's balance.
                                        Console.WriteLine("Resolving Completed Distributions: Distribute the amount to user's balance... Date:" + DateTime.UtcNow);
                                        var balance = await distributingContext.Balances
                                        .Where(b => b.UserId == userId)
                                        .FirstOrDefaultAsync();

                                        if (balance == null)
                                        {
                                            throw new KeyNotFoundException("No balance found by distribution's user id. At ResolveCompletedDistributionBackgroundTaskService()");
                                        }

                                        balance.Distribute(balanceToAddSum);

                                        //Update balance view model.
                                        Console.WriteLine("Resolving Completed Distributions: Update balance view model... Date:" + DateTime.UtcNow);
                                        if (balance.GetUserType.Name == UserType.Trader.Name)
                                        {
                                            await traderQueries.UpdateBalanceAsync(userId, balanceToAddSum);
                                        }
                                        else if (balance.GetUserType.Name == UserType.TraderAgent.Name)
                                        {
                                            await traderAgentQueries.UpdateBalanceAsync(userId, balanceToAddSum);
                                        }
                                        else if (balance.GetUserType.Name == UserType.Shop.Name)
                                        {
                                            await shopQueries.UpdateBalanceAsync(userId, balanceToAddSum);
                                        }
                                        else if (balance.GetUserType.Name == UserType.ShopAgent.Name)
                                        {
                                            await shopAgentQueries.UpdateBalanceAsync(userId, balanceToAddSum);
                                        }
                                        else
                                        {
                                            throw new Exception("Unrecognized user type. At ResolveCompletedDistributionBackgroundTaskService.");
                                        }

                                        //Delete distributions. (No need anymore)
                                        Console.WriteLine("Resolving Completed Distributions: Delete distributions... Date:" + DateTime.UtcNow);
                                    }

                                    //runningAccountRecordQueries.AddRange(runningAccountRecordsToAdd);
                                    //distributingContext.Distributions.RemoveRange(tempDistributionsToDelete);

                                    Console.WriteLine("Resolving Completed Distributions: Save Changes... Date:" + DateTime.UtcNow);
                                    //Save changes for balance entities.
                                    await distributingContext.SaveChangesAsync();

                                    // Saves all view model created at previous processes.
                                    // The queries all belongs to one db ccontext, so only need to save once.
                                    await shopAgentQueries.SaveChangesAsync();

                                    //Commit Transaction.
                                    //If there is any error, transaction will rollback, and throw the error after the rollback.
                                    await distributingContext.CommitTransactionOnlyAsync(distributingTransaction);
                                    await applicationContext.CommitTransactionOnlyAsync(applicationTransaction);


                                    //--- If no concurrency conflics happened, set Saved to true and ---//
                                    //saved = true;

                                    Console.WriteLine("Resolving Completed Distributions Success! Date:" + DateTime.UtcNow);
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    Console.WriteLine("Catch DbUpdateConcurrencyException When Resolving Completed Distributions... Date:" + DateTime.UtcNow);

                                    //If the transaction failed because of concurrencies and conflicts,
                                    //then re-pairing again.

                                    distributingContext.RollbackTransaction();
                                    applicationContext.RollbackTransaction();

                                    //saved = false;

                                    //throw; //Throw the exception and catch by the catch block out side the tranasaction, then sleep for random secs.
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Catch Exception When Resolving Completed Distributions... Date:" + DateTime.UtcNow);
                                    Console.WriteLine("Exception Description: " + ex.Message);

                                    distributingContext.RollbackTransaction();
                                    applicationContext.RollbackTransaction();

                                    //saved = true;
                                    //throw;
                                }
                                finally
                                {
                                    distributingContext.DisposeTransaction();
                                    applicationContext.DisposeTransaction();
                                }

                            }
                        }
                    });
                });


            }
        }

        private async Task ResolveCompletedDistributions_Deprecated()
        {
            return;

            Console.WriteLine("Start Resolving Completed Distributions... Date:" + DateTime.UtcNow);

            using (var scope = scopeFactory.CreateScope())
            {
                var distributingContext = scope.ServiceProvider.GetRequiredService<DistributingContext>();
                var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var runningAccountRecordQueries = scope.ServiceProvider.GetRequiredService<IRunningAccountQueries>();

                var traderQueries = scope.ServiceProvider.GetRequiredService<ITraderQueries>();
                var traderAgentQueries = scope.ServiceProvider.GetRequiredService<ITraderAgentQueries>();
                var shopQueries = scope.ServiceProvider.GetRequiredService<IShopQueries>();
                var shopAgentQueries = scope.ServiceProvider.GetRequiredService<IShopAgentQueries>();

                Console.WriteLine("Resolving Completed Distributions In Process... Date:" + DateTime.UtcNow);

                // To Platform order: Find qr code to pairing.
                // To Fourth Party order: Send request to fourth party.
                var distribuingStrategy = distributingContext.Database.CreateExecutionStrategy();
                var applicationStrategy = applicationContext.Database.CreateExecutionStrategy();

                await distribuingStrategy.ExecuteAsync(async () =>
                {
                    await applicationStrategy.ExecuteAsync(async () =>
                    {
                        using (var distributingTransaction = await distributingContext.BeginTransactionAsync())
                        {
                            using (var applicationTransaction = await applicationContext.BeginTransactionAsync())
                            {
                                try
                                {
                                    //Initialize a list for later use for Remove Range.
                                    var tempRunningRecordsToDelete = new List<TempRunningAccountRecord>();
                                    var tempDistributionsToDelete = new List<Distribution>();

                                    //Get recently created distributions.
                                    Console.WriteLine("Resolving Completed Distributions: Get recently created distributions... Date:" + DateTime.UtcNow);

                                    var distributionGroups = distributingContext.Distributions
                                    .Include(d => d.Order)
                                    .Select(u => new
                                    {
                                        u.Id,
                                        u.UserId,
                                        u.DistributedAmount,
                                        OrderTrackingNumber = u.Order.TrackingNumber
                                    })
                                    .AsEnumerable()
                                    .GroupBy(d => d.UserId);

                                    //Resolve each user's distribution.
                                    foreach (var distributions in distributionGroups)
                                    {
                                        var userId = distributions.Key;

                                        //var tempRunningAcountRecords = await runningAccountRecordQueries.GetTempByUserIdAsync(userId);

                                        //var tempRunningAccountRecords = new List<TempRunningAccountRecord>();

                                        decimal balanceToAddSum = 0;

                                        //Sum the balance to add and get running account records.
                                        Console.WriteLine("Resolving Completed Distributions: Sum the balance to add and get running account records... Date:" + DateTime.UtcNow);
                                        foreach (var distribution in distributions)
                                        {
                                            var distributedAmount = distribution.DistributedAmount;

                                            //Increase balance to add.
                                            balanceToAddSum += distribution.DistributedAmount;

                                            //Get corresponding temp running account records.
                                            var tempRunningAccountRecord = await runningAccountRecordQueries.GetTempByUserIdAndOrderTrackingNumberAsync(
                                                userId,
                                                distribution.OrderTrackingNumber
                                                );

                                            //Add to list.
                                            //tempRunningAccountRecords.Add(tempRunningAccountRecord);
                                            if (tempRunningAccountRecord != null)
                                            {
                                                //Update running account records.
                                                runningAccountRecordQueries.UpdateRunningRecordsToCompleted(
                                                    tempRunningAccountRecord.Id,
                                                    OrderStatus.Success.Name,
                                                    distributedAmount);

                                                //Delete temp running account record.
                                                //runningAccountRecordQueries.DeleteTemp(tempRunningAccountRecord);
                                                tempRunningRecordsToDelete.Add(tempRunningAccountRecord);
                                            }


                                            //Delete Distribution.

                                            var toDelete = new Distribution(distribution.Id);

                                            distributingContext.Distributions.Attach(toDelete);
                                            tempDistributionsToDelete.Add(toDelete);
                                            //distributingContext.Entry(toDelete).State = EntityState.Deleted;
                                        }

                                        //Distribute the amount to user's balance.
                                        Console.WriteLine("Resolving Completed Distributions: Distribute the amount to user's balance... Date:" + DateTime.UtcNow);
                                        var balance = await distributingContext.Balances
                                        .Where(b => b.UserId == userId)
                                        .FirstOrDefaultAsync();

                                        if (balance == null)
                                        {
                                            throw new KeyNotFoundException("No balance found by distribution's user id. At ResolveCompletedDistributionBackgroundTaskService()");
                                        }

                                        balance.Distribute(balanceToAddSum);

                                        //Update balance view model.
                                        Console.WriteLine("Resolving Completed Distributions: Update balance view model... Date:" + DateTime.UtcNow);
                                        if (balance.GetUserType.Name == UserType.Trader.Name)
                                        {
                                            await traderQueries.UpdateBalanceAsync(userId, balanceToAddSum);
                                        }
                                        else if (balance.GetUserType.Name == UserType.TraderAgent.Name)
                                        {
                                            await traderAgentQueries.UpdateBalanceAsync(userId, balanceToAddSum);
                                        }
                                        else if (balance.GetUserType.Name == UserType.Shop.Name)
                                        {
                                            await shopQueries.UpdateBalanceAsync(userId, balanceToAddSum);
                                        }
                                        else if (balance.GetUserType.Name == UserType.ShopAgent.Name)
                                        {
                                            await shopAgentQueries.UpdateBalanceAsync(userId, balanceToAddSum);
                                        }
                                        else
                                        {
                                            throw new Exception("Unrecognized user type. At ResolveCompletedDistributionBackgroundTaskService.");
                                        }

                                        //Delete distributions. (No need anymore)
                                        Console.WriteLine("Resolving Completed Distributions: Delete distributions... Date:" + DateTime.UtcNow);
                                        //distributingContext.Distributions.RemoveRange();
                                    }

                                    runningAccountRecordQueries.DeleteTempRange(tempRunningRecordsToDelete);
                                    distributingContext.Distributions.RemoveRange(tempDistributionsToDelete);

                                    Console.WriteLine("Resolving Completed Distributions: Save Changes... Date:" + DateTime.UtcNow);
                                    //Save changes for balance entities.
                                    await distributingContext.SaveChangesAsync();

                                    // Saves all view model created at previous processes.
                                    // The queries all belongs to one db ccontext, so only need to save once.
                                    await shopAgentQueries.SaveChangesAsync();

                                    //Commit Transaction.
                                    //If there is any error, transaction will rollback, and throw the error after the rollback.
                                    await distributingContext.CommitTransactionOnlyAsync(distributingTransaction);
                                    await applicationContext.CommitTransactionOnlyAsync(applicationTransaction);


                                    //--- If no concurrency conflics happened, set Saved to true and ---//
                                    //saved = true;

                                    Console.WriteLine("Resolving Completed Distributions Success! Date:" + DateTime.UtcNow);
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    Console.WriteLine("Catch DbUpdateConcurrencyException When Resolving Completed Distributions... Date:" + DateTime.UtcNow);

                                    //If the transaction failed because of concurrencies and conflicts,
                                    //then re-pairing again.

                                    distributingContext.RollbackTransaction();
                                    applicationContext.RollbackTransaction();

                                    //saved = false;

                                    //throw; //Throw the exception and catch by the catch block out side the tranasaction, then sleep for random secs.
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Catch Exception When Resolving Completed Distributions... Date:" + DateTime.UtcNow);
                                    Console.WriteLine("Exception Description: " + ex.Message);

                                    distributingContext.RollbackTransaction();
                                    applicationContext.RollbackTransaction();

                                    //saved = true;
                                    //throw;
                                }
                                finally
                                {
                                    distributingContext.DisposeTransaction();
                                    applicationContext.DisposeTransaction();
                                }

                            }
                        }
                    });
                });


            }
        }

    }
}
