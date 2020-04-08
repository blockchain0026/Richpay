using Distributing.Domain.Exceptions;
using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Commissions;
using Distributing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Model.Orders;
using Ordering.Infrastructure;
using Pairing.Domain.Model.QrCodes;
using Pairing.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Data;
using WebMVC.Models.Queries;
using WebWorkerService.CacheServices;

namespace WebWorkerService.BackgroundServices
{
    public class MarkOrderAsExpiredBackgroundService : BackgroundService
    {
        private readonly ILogger<MarkOrderAsExpiredBackgroundService> _logger;

        private readonly IServiceScopeFactory scopeFactory;

        public MarkOrderAsExpiredBackgroundService(ILogger<MarkOrderAsExpiredBackgroundService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"MarkOrderAsExpiredBackgroundService is starting.");

            stoppingToken.Register(() =>
                _logger.LogDebug($" MarkOrderAsExpiredBackgroundService background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"MarkOrderAsExpired task doing background work.");

                await ResolveExpiredOrders();

                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogDebug($"MarkOrderAsExpired background task is stopping.");
        }

        private async Task ResolveExpiredOrders()
        {
            try
            {
                Console.WriteLine("Resolving Expired Orders: Start... Date:" + DateTime.UtcNow);

                using (var scope = scopeFactory.CreateScope())
                {
                    var orderingContext = scope.ServiceProvider.GetRequiredService<OrderingContext>();

                    //Check db connection.
                    if (!await orderingContext.Database.CanConnectAsync())
                    {
                        return;
                    }

                    var currentDateTime = DateTime.UtcNow;

                    var toMarkExpiredOrdersId = orderingContext.Orders
                        .AsNoTracking()
                        .Include(o => o.OrderStatus)
                        .Where(o => !o.IsExpired
                        && o.OrderStatus.Id != OrderStatus.Success.Id
                        && currentDateTime > o.DateCreated.AddSeconds((int)o.ExpirationTimeInSeconds))
                        .Select(o => o.Id);
                    var tasks = new List<Task>();

                    foreach (var orderId in toMarkExpiredOrdersId)
                    {
                        tasks.Add(Task.Run(() => MarkOrderAsExpired(orderId)));
                    }

                    await Task.WhenAll(tasks);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Catch Exception When Resolving Expired Orders... Exception Description: " + ex.Message);
            }
        }

        private async Task MarkOrderAsExpired(int orderId)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var orderingContext = scope.ServiceProvider.GetRequiredService<OrderingContext>();
                var pairingContext = scope.ServiceProvider.GetRequiredService<PairingContext>();
                var distributingContext = scope.ServiceProvider.GetRequiredService<DistributingContext>();
                var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var commissionCacheService = scope.ServiceProvider.GetRequiredService<ICommissionCacheService>();
                var userCacheService = scope.ServiceProvider.GetRequiredService<IUserCacheService>();

                var balanceRepository = scope.ServiceProvider.GetRequiredService<IBalanceRepository>();
                var qrCodeRepository = scope.ServiceProvider.GetRequiredService<IQrCodeRepository>();

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

                                                    //Return amount to trader if the order is paired and the order IS NOT A TEST ORDER.
                                                    if (!order.IsTestOrder && order.GetOrderStatus.Id == OrderStatus.AwaitingPayment.Id)
                                                    {
                                                        //Distribute the dividends.
                                                        RateType rateType = null;
                                                        if (order.GetOrderPaymentChannel.Name == OrderPaymentChannel.Alipay.Name)
                                                        {
                                                            rateType = RateType.Alipay;
                                                        }
                                                        else if (order.GetOrderPaymentChannel.Name == OrderPaymentChannel.Wechat.Name)
                                                        {
                                                            rateType = RateType.Wechat;
                                                        }
                                                        else
                                                        {
                                                            throw new Exception("Unrecognized rate type.");
                                                        }

                                                        await this.AddFailedRunningAccountRecords(
                                                            new Distributing.Domain.Model.Distributions.Order(
                                                                order.TrackingNumber,
                                                                order.ShopInfo.ShopOrderId,
                                                                order.Amount,
                                                                0, //Set to 0  because failed to proccess payment.
                                                                order.ShopInfo.ShopId,
                                                                order.PayeeInfo.TraderId,
                                                                order.DateCreated
                                                                ),
                                                            rateType,
                                                            commissionCacheService,
                                                            applicationContext,
                                                            userCacheService
                                                            );



                                                        #region Unfreeze trader frozen balance
                                                        var amountAvailable = await balanceRepository.UpdateBalanceForDistributeByUserId(
                                                            order.PayeeInfo.TraderId, order.Amount);
                                                        #endregion

                                                        //Update available balance of the trader's QR code.
                                                        //Prevent qr code that have inefficient balance pair with order.
                                                        var availableBalance = amountAvailable;
                                                        //Update target QR code pairing data and status.
                                                        var userQrCode = await qrCodeRepository.GetByQrCodeIdForFinishingOrderAsync((int)order.PayeeInfo.QrCodeId);
                                                        userQrCode.OrderFailed(
                                                            new Pairing.Domain.Model.QrCodes.Order(
                                                                order.TrackingNumber,
                                                                order.ShopInfo.ShopOrderId,
                                                                order.GetOrderPaymentChannel.Name,
                                                                order.GetOrderPaymentScheme.Name,
                                                                order.Amount,
                                                                order.ShopInfo.ShopId,
                                                                order.ShopInfo.RateRebateShop,
                                                                order.DateCreated,
                                                                DateTime.UtcNow
                                                                )
                                                            );

                                                        userQrCode.BalanceUpdated(availableBalance);

                                                        //Use direct modify approach.
                                                        await qrCodeRepository.UpdateUserQrCodesBalanceWhenPaired(
                                                            order.PayeeInfo.TraderId, availableBalance, userQrCode.Id);


                                                        //Update Qr code entry.
                                                        await UpdateQrCodeEntryForOrderCreated(
                                                            applicationContext,
                                                            userQrCode.Id,
                                                            userQrCode.AvailableBalance,
                                                            userQrCode.GetPairingStatus.Name,
                                                            userQrCode.PairingStatusDescription,
                                                            userQrCode.DateLastTraded?.ToString("yyyy/MM/dd HH:mm:ss")
                                                            );
                                                    }


                                                    //Update order vm.
                                                    Console.WriteLine("Resolving Expired Orders: Update order view model... Date:" + DateTime.UtcNow);
                                                    UpdateOrderEntryToExpired(
                                                        applicationContext,
                                                        order.Id,
                                                        order.GetOrderStatus.Name,
                                                        order.OrderStatusDescription);

                                                    Console.WriteLine("Resolving Expired Orders: Save Changes... Date:" + DateTime.UtcNow);
                                                    //Save changes.
                                                    await orderingContext.SaveChangesAsync();
                                                    await pairingContext.SaveChangesAsync();
                                                    await distributingContext.SaveChangesAsync();

                                                    // Saves all view model created at previous processes.
                                                    await applicationContext.SaveChangesAsync();

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

                                                    orderingContext.RollbackTransaction(false);
                                                    pairingContext.RollbackTransaction(false);
                                                    distributingContext.RollbackTransaction(false);
                                                    applicationContext.RollbackTransaction();

                                                    //saved = false;

                                                    //throw; //Throw the exception and catch by the catch block out side the tranasaction, then sleep for random secs.
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine("Catch Exception When Resolving Expired Orders... Date:" + DateTime.UtcNow);
                                                    Console.WriteLine("Exception Description: " + ex.Message);

                                                    orderingContext.RollbackTransaction(false);
                                                    pairingContext.RollbackTransaction(false);
                                                    distributingContext.RollbackTransaction(false);
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

        private async Task AddFailedRunningAccountRecords(Distributing.Domain.Model.Distributions.Order order, RateType rateType,
            ICommissionCacheService commissionCacheService, ApplicationDbContext appContext, IUserCacheService userCacheService)
        {
            #region Store order details to shop
            var shopCommission = commissionCacheService.GetCommissionInfoByUserId(order.ShopId, rateType);
            if (shopCommission == null)
            {
                throw new DistributingDomainException("No distribution found by given shop id. At DistributionService.OrderCreated");
            }

            var shopOrder = new Distributing.Domain.Model.Distributions.Order(
                    order.TrackingNumber,
                    order.ShopOrderId,
                    order.Amount,
                    0,
                    order.ShopId,
                    order.TraderId,
                    order.DateCreated
                    );

            //Create Running Account Record VM.
            appContext.RunningAccountRecords.Add(
                await MapFromOrderInfo(shopOrder, shopCommission.UserId, userCacheService));
            #endregion


            #region Store order details to shop agents

            var uplineShopAgentCommission = commissionCacheService.GetCommissionInfoByCommissionId(shopCommission.UplineCommissionId ?? default, rateType);

            //For UI Search Purpose.
            string downlineUserId = shopCommission.UserId;

            while (true)
            {
                if (uplineShopAgentCommission == null)
                {
                    break;
                }

                var currentCommission = uplineShopAgentCommission;

                var newOrder = new Distributing.Domain.Model.Distributions.Order(
                    order.TrackingNumber,
                    order.ShopOrderId,
                    order.Amount,
                    0,
                    order.ShopId,
                    order.TraderId,
                    order.DateCreated
                    );

                //Create Running Account Record VM.
                appContext.RunningAccountRecords.Add(
                    await MapFromOrderInfo(newOrder, currentCommission.UserId, userCacheService));

                downlineUserId = currentCommission.UserId;

                uplineShopAgentCommission = commissionCacheService.GetCommissionInfoByCommissionId(currentCommission.UplineCommissionId ?? default, rateType);
            }

            #endregion


            #region Store order details to trader

            var traderCommission = commissionCacheService.GetCommissionInfoByUserId(order.TraderId, rateType);
            if (traderCommission == null)
            {
                throw new DistributingDomainException("No distribution found by given trader id. At DistributionService.OrderCreated");
            }

            var traderOrder = new Distributing.Domain.Model.Distributions.Order(
                order.TrackingNumber,
                order.ShopOrderId,
                order.Amount,
                0,
                order.ShopId,
                order.TraderId,
                order.DateCreated
                );

            //Create Running Account Record VM.
            appContext.RunningAccountRecords.Add(
                await MapFromOrderInfo(traderOrder, traderCommission.UserId, userCacheService));
            #endregion


            #region Store order details to trader agents

            var uplineTraderAgentCommission = commissionCacheService.GetCommissionInfoByCommissionId(traderCommission.UplineCommissionId ?? default, rateType);

            //For UI Search Purpose.
            downlineUserId = traderCommission.UserId;

            while (true)
            {
                if (uplineTraderAgentCommission == null)
                {
                    break;
                }

                var currentCommission = uplineTraderAgentCommission;

                var newOrder = new Distributing.Domain.Model.Distributions.Order(
                    order.TrackingNumber,
                    order.ShopOrderId,
                    order.Amount,
                    0,
                    order.ShopId,
                    order.TraderId,
                    order.DateCreated
                    );

                //Create Running Account Record VM.
                appContext.RunningAccountRecords.Add(
                    await MapFromOrderInfo(newOrder, currentCommission.UserId, userCacheService));

                downlineUserId = currentCommission.UserId;

                uplineTraderAgentCommission = commissionCacheService.GetCommissionInfoByCommissionId(currentCommission.UplineCommissionId ?? default, rateType);
            }
            #endregion
        }


        private async Task<RunningAccountRecord> MapFromOrderInfo(Distributing.Domain.Model.Distributions.Order orderInfo,
        string userId, IUserCacheService userCacheService)
        {
            var user = userCacheService.GetNameInfoByUserId(userId);

            var shopUser = userCacheService.GetNameInfoByUserId(orderInfo.ShopId);

            var traderUser = userCacheService.GetNameInfoByUserId(orderInfo.TraderId);

            //Build view model.
            var runningAccountRecord = new RunningAccountRecord
            {
                UserId = userId,
                UserName = user.UserName,
                UserFullName = user.FullName,

                //Set downline data if user has downline.
                //DownlineUserId = downlineUserId,
                //DownlineUserName = downlineUserName,
                // = downlineFullName,

                OrderTrackingNumber = orderInfo.TrackingNumber,
                ShopOrderId = orderInfo.ShopOrderId,

                //The running account record is created by domain event handler 
                //if the order is success, then the commission amount will be non-zero.
                Status = orderInfo.CommissionAmount > 0 ? "Success" : "Failed",

                Amount = orderInfo.Amount,

                //The commission amount is the expected amount distributed to the user.
                CommissionAmount = orderInfo.CommissionAmount,

                //The amount distributed to user.
                DistributedAmount = orderInfo.CommissionAmount,

                ShopId = orderInfo.ShopId,
                ShopUserName = shopUser.UserName,
                ShopFullName = shopUser.FullName,

                TraderId = orderInfo.TraderId,
                TraderUserName = traderUser?.UserName,
                TraderFullName = traderUser?.FullName,


                DateCreated = orderInfo.DateCreated
            };

            return runningAccountRecord;
        }

        private async Task UpdateQrCodeEntryForOrderCreated(ApplicationDbContext applicationDbContext,
            int qrCodeId, decimal availableBalance,
            string pairingStatus, string pairingStatusDescription, string dateLastTraded = null)
        {

            var toUpdate = new QrCodeEntry
            {
                QrCodeId = qrCodeId,
                //PairingInfo = originalQrCode.PairingInfo
            };

            applicationDbContext.QrCodeEntrys.Attach(toUpdate);
            //_applicationContext.Entry(toUpdate).Reference(b => b.PairingInfo).IsModified = true;
            applicationDbContext.Entry(toUpdate).Property(b => b.PairingStatus).IsModified = true;
            applicationDbContext.Entry(toUpdate).Property(b => b.PairingStatusDescription).IsModified = true;
            applicationDbContext.Entry(toUpdate).Property(b => b.AvailableBalance).IsModified = true;
            applicationDbContext.Entry(toUpdate).Property(b => b.DateLastTraded).IsModified = true;

            //Update available balance.
            toUpdate.AvailableBalance = availableBalance;

            //Update pairing status.
            toUpdate.PairingStatus = pairingStatus;
            toUpdate.PairingStatusDescription = pairingStatusDescription;

            //Update last traded date.
            if (!string.IsNullOrEmpty(dateLastTraded))
            {
                toUpdate.DateLastTraded = dateLastTraded;
            }
        }

        private void UpdateOrderEntryToExpired(ApplicationDbContext applicationDbContext,
            int orderId, string orderStatus, string orderStatusDescription)
        {
            var toUpdate = new OrderEntry
            {
                OrderId = orderId
            };

            applicationDbContext.OrderEntrys.Attach(toUpdate);
            applicationDbContext.Entry(toUpdate).Property(b => b.IsExpired).IsModified = true;
            applicationDbContext.Entry(toUpdate).Property(b => b.OrderStatus).IsModified = true;
            applicationDbContext.Entry(toUpdate).Property(b => b.OrderStatusDescription).IsModified = true;

            toUpdate.IsExpired = true;
            toUpdate.OrderStatus = orderStatus;
            toUpdate.OrderStatusDescription = orderStatusDescription;
        }

    }
}
