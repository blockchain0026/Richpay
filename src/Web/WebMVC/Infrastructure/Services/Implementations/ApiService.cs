using Distributing.Domain.Model.Commissions;
using Distributing.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Domain.Model.Orders;
using Ordering.Domain.Model.Shared;
using Ordering.Domain.Model.ShopApis;
using Ordering.Infrastructure;
using Pairing.Domain.Model.QrCodes;
using Pairing.Domain.Model.ShopGateways;
using Pairing.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.CacheServices;
using WebMVC.Applications.Queries.Orders;
using WebMVC.Applications.Queries.QrCodes;
using WebMVC.Applications.Queries.ShopGateways;
using WebMVC.Data;
using WebMVC.Extensions;
using WebMVC.Infrastructure.ApiClients;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;

namespace WebMVC.Infrastructure.Services.Implementations
{
    public class ApiService : IApiService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IServiceScopeFactory scopeFactory;

        private readonly IShopGatewayQueries _shopGatewayQueries;
        private readonly IQrCodeQueries _qrCodeQueries;
        private readonly IOrderQueries _orderQueries;

        private readonly IOrderRepository _orderRepository;
        private readonly ICommissionRepository _commissionRepository;
        private readonly IShopApiRepository _shopApiRepository;
        private readonly IQrCodeRepository _qrCodeRepository;

        private readonly IDateTimeService _orderingDateTimeService;

        private readonly IPairingDomainService _pairingDomainService;

        private readonly OrderingContext _orderingContext;
        private readonly PairingContext _pairingContext;
        private readonly DistributingContext _distributingContext;
        private readonly ApplicationDbContext _applicationDbContext;

        private readonly ICommissionCacheService _commissionCacheService;

        private readonly INewPayApiClient _newPayApiClient;

        public ApiService(UserManager<ApplicationUser> userManager, IServiceScopeFactory scopeFactory, IShopGatewayQueries shopGatewayQueries, IQrCodeQueries qrCodeQueries, IOrderQueries orderQueries, IOrderRepository orderRepository, ICommissionRepository commissionRepository, IShopApiRepository shopApiRepository, IQrCodeRepository qrCodeRepository, IDateTimeService orderingDateTimeService, IPairingDomainService pairingDomainService, OrderingContext orderingContext, PairingContext pairingContext, DistributingContext distributingContext, ApplicationDbContext applicationDbContext, ICommissionCacheService commissionCacheService, INewPayApiClient newPayApiClient)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _shopGatewayQueries = shopGatewayQueries ?? throw new ArgumentNullException(nameof(shopGatewayQueries));
            _qrCodeQueries = qrCodeQueries ?? throw new ArgumentNullException(nameof(qrCodeQueries));
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
            _shopApiRepository = shopApiRepository ?? throw new ArgumentNullException(nameof(shopApiRepository));
            _qrCodeRepository = qrCodeRepository ?? throw new ArgumentNullException(nameof(qrCodeRepository));
            _orderingDateTimeService = orderingDateTimeService ?? throw new ArgumentNullException(nameof(orderingDateTimeService));
            _pairingDomainService = pairingDomainService ?? throw new ArgumentNullException(nameof(pairingDomainService));
            _orderingContext = orderingContext ?? throw new ArgumentNullException(nameof(orderingContext));
            _pairingContext = pairingContext ?? throw new ArgumentNullException(nameof(pairingContext));
            _distributingContext = distributingContext ?? throw new ArgumentNullException(nameof(distributingContext));
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _commissionCacheService = commissionCacheService ?? throw new ArgumentNullException(nameof(commissionCacheService));
            _newPayApiClient = newPayApiClient ?? throw new ArgumentNullException(nameof(newPayApiClient));
        }

        public async Task<string> GenerateShopApiKey(string shopId, string generateByUserId)
        {
            //If this is generate by user, check his is generate for his shop.
            var generateByUser = await _userManager.Users
                .Where(u => u.Id == generateByUserId)
                .FirstOrDefaultAsync();
            if (generateByUser is null)
            {
                throw new KeyNotFoundException("No shop found by given user Id.");
            }

            if (generateByUser.BaseRoleType != BaseRoleType.Manager)
            {
                if (generateByUser.BaseRoleType != BaseRoleType.Shop)
                {
                    throw new InvalidOperationException("没有生成权限。");
                }
                else
                {
                    if (generateByUser.Id != shopId)
                    {
                        throw new InvalidOperationException("没有生成权限。");
                    }
                }
            }

            //Get the shop api.
            var shopApi = await _shopApiRepository.GetByShopIdAsync(shopId);
            if (shopApi is null)
            {
                throw new KeyNotFoundException("查无API实体。");
            }

            shopApi.GenerateNewApiKey();

            //Save Changes.
            await this._shopApiRepository.UnitOfWork.SaveEntitiesAsync();

            //Return new api key.
            return shopApi.ApiKey;
        }

        public async Task AddIpToWhitelist(string shopId, List<string> ips, string addByUserId)
        {
            //If this is add by user, check the IP is add for his shop.
            var addByUser = await _userManager.Users
                .Where(u => u.Id == addByUserId)
                .FirstOrDefaultAsync();
            if (addByUser is null)
            {
                throw new KeyNotFoundException("No user found by given user Id.");
            }

            if (addByUser.BaseRoleType != BaseRoleType.Manager)
            {
                if (addByUser.BaseRoleType != BaseRoleType.Shop)
                {
                    throw new InvalidOperationException("没有生成权限。");
                }
                else
                {
                    if (addByUser.Id != shopId)
                    {
                        throw new InvalidOperationException("没有生成权限。");
                    }
                }
            }

            //Get the shop api.
            var shopApi = await _shopApiRepository.GetByShopIdAsync(shopId);
            if (shopApi is null)
            {
                throw new KeyNotFoundException("查无API实体。");
            }

            //Update whole list.
            shopApi.UpdateWholeIpWhitelists(ips);

            //Save Changes.
            await this._shopApiRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task<string> CreateOrderAsync(string shopId, string shopOrderId, decimal shopOrderAmount, string shopReturnUrl, string shopOkReturnUrl, OrderGatewayType orderGatewayType)
        {
            RateType rateType = null;
            int orderCreatedId = default(int);
            string orderTrackingNumber = string.Empty;


            //Validate the shop is enabled.
            GetGatewayInfo(
                orderGatewayType,
                out rateType,
                out OrderPaymentChannel orderPaymentChannel,
                out OrderPaymentScheme orderPaymentScheme);

            var shopCommission = _commissionCacheService.GetCommissionInfoByUserId(
                shopId,
                rateType);

            if (!shopCommission.IsEnabled)
            {
                throw new Exception("商户已被停用");
            }

            var toppestCommission = _commissionCacheService.GetToppestCommissionInfoByCommission(shopCommission, rateType);


            //Create order by gateway type.
            AlipayPagePreference alipayPagePreference = new AlipayPagePreference(
                30,
                true,
                true,
                true,
                true,
                true
                );


            var paired = false;
            var saved = false;
            while (!saved)
            {
                try
                {
                    // To Platform order: Find qr code to pairing.
                    // To Fourth Party order: Send request to fourth party.
                    var orderingStrategy = _orderingContext.Database.CreateExecutionStrategy();
                    var pairingStrategy = _pairingContext.Database.CreateExecutionStrategy();
                    var distribuingStrategy = _distributingContext.Database.CreateExecutionStrategy();
                    var applicationStrategy = _applicationDbContext.Database.CreateExecutionStrategy();

                    await orderingStrategy.ExecuteAsync(async () =>
                    {
                        await pairingStrategy.ExecuteAsync(async () =>
                        {
                            await distribuingStrategy.ExecuteAsync(async () =>
                            {
                                await applicationStrategy.ExecuteAsync(async () =>
                                {
                                    using (var orderingTransaction = await _orderingContext.BeginTransactionAsync())
                                    {
                                        using (var pairingTransaction = await _pairingContext.BeginTransactionAsync())
                                        {
                                            using (var distributingTransaction = await _distributingContext.BeginTransactionAsync())
                                            {
                                                using (var applicationTransaction = await _applicationDbContext.BeginTransactionAsync())
                                                {
                                                    try
                                                    {
                                                        var order = Ordering.Domain.Model.Orders.Order.FromShopToPlatform(
                                                                       180,
                                                                       shopId,
                                                                       shopOrderId,
                                                                       shopReturnUrl,
                                                                       shopOkReturnUrl,
                                                                       shopCommission.Rate,
                                                                       toppestCommission.Rate,
                                                                       shopOrderAmount,
                                                                       orderPaymentChannel,
                                                                       orderPaymentScheme,
                                                                       this._orderingDateTimeService,
                                                                       alipayPagePreference
                                                                       );
                                                        //Pair with QR code.
                                                        var pairedQrCode = await _pairingDomainService.PairFrom(
                                                            new Pairing.Domain.Model.QrCodes.Order(
                                                                order.TrackingNumber,
                                                                order.ShopInfo.ShopOrderId,
                                                                order.GetOrderPaymentChannel.Name,
                                                                order.GetOrderPaymentScheme.Name,
                                                                order.Amount,
                                                                order.ShopInfo.ShopId,
                                                                order.ShopInfo.RateRebateFinal,
                                                                order.DateCreated,
                                                                null
                                                                ),
                                                            false //Pairing mode may change depends on Business.
                                                            );

                                                        if (pairedQrCode != null)
                                                        {
                                                            //order.Expired();//For test.

                                                            //If success paired, 
                                                            //mark order as paired,
                                                            order.PairedByPlatform(
                                                                pairedQrCode.UserId,
                                                                pairedQrCode.Id,
                                                                pairedQrCode.MinCommissionRate - 0.001M,
                                                                _orderingDateTimeService
                                                                );
                                                            paired = true;


                                                            //Save changes on qrcode, execute Pairing Domain event handler.
                                                            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();

                                                            //The distribution process was moved into trader confirming process.
                                                            //So we only need to save entities change that made in previous work (eg. created frozen).
                                                            await _commissionRepository.UnitOfWork.SaveEntitiesAsync();
                                                        }
                                                        else
                                                        {
                                                            //If failed to paired,
                                                            //mark order as Expired.
                                                            order.Expired();
                                                        }

                                                        var orderCreated = _orderRepository.Add(order);

                                                        //Save changes. (No need to execute domain event)
                                                        await _orderRepository.UnitOfWork.SaveEntitiesAsync();

                                                        //Create view data here because it may slow down the performence if create in domain handler.
                                                        var orderVM = await _orderQueries.MapFromEntity(orderCreated);

                                                        var createdOrderVM = _orderQueries.Add(orderVM);

                                                        //Set order id for later process;
                                                        orderCreatedId = orderCreated.Id;
                                                        orderTrackingNumber = orderCreated.TrackingNumber;


                                                        // Saves all view model created at previous processes.
                                                        // The Queries all belongs to one db ccontext, so only need to save once.
                                                        await _orderQueries.SaveChangesAsync();

                                                        //Commit Transaction.
                                                        //If there is any error, transaction will rollback, and throw the error after the rollback.
                                                        await _orderingContext.CommitTransactionOnlyAsync(orderingTransaction);
                                                        await _pairingContext.CommitTransactionOnlyAsync(pairingTransaction);
                                                        await _distributingContext.CommitTransactionOnlyAsync(distributingTransaction);
                                                        await _applicationDbContext.CommitTransactionOnlyAsync(applicationTransaction);


                                                        //--- If no concurrency conflics happened, set Saved to true and ---//
                                                        saved = true;
                                                    }
                                                    catch (DbUpdateConcurrencyException ex)
                                                    {
                                                        Console.WriteLine("Catch DbUpdateConcurrencyException When Create Order To Platform:");
                                                        Console.WriteLine(ex.ToString());
                                                        //If the transaction failed because of concurrencies and conflicts,
                                                        //then re-pairing again.

                                                        _orderingContext.RollbackTransaction();
                                                        _pairingContext.RollbackTransaction();
                                                        _distributingContext.RollbackTransaction();
                                                        _applicationDbContext.RollbackTransaction();

                                                        saved = false;
                                                        paired = false;

                                                        throw; //Throw the exception and catch by the catch block out side the tranasaction, then sleep for random secs.
                                                    }
                                                    catch
                                                    {
                                                        _orderingContext.RollbackTransaction();
                                                        _pairingContext.RollbackTransaction();
                                                        _distributingContext.RollbackTransaction();
                                                        _applicationDbContext.RollbackTransaction();

                                                        saved = true;
                                                        paired = false;

                                                        throw;
                                                    }
                                                    finally
                                                    {

                                                        _orderingContext.DisposeTransaction();
                                                        _pairingContext.DisposeTransaction();
                                                        _distributingContext.DisposeTransaction();
                                                        _applicationDbContext.DisposeTransaction();
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
                catch (DbUpdateConcurrencyException)
                {
                    //Sleep for random seconds.
                    //this.SleepForRandomSeconds(1, 25);
                }
                catch
                {
                    //Errors occur for other reasons.
                    //Throw errors and finished the thread.
                    saved = true;
                    paired = false;

                    throw;
                }
            }
            if (paired)
            {
                return orderTrackingNumber;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> CreateOrderAndConfirmAsync(string shopId, string shopOrderId, decimal shopOrderAmount, string shopReturnUrl, string shopOkReturnUrl, OrderGatewayType orderGatewayType)
        {
            RateType rateType = null;
            int orderCreatedId = default(int);
            string orderTrackingNumber = string.Empty;


            //Validate the shop is enabled.
            GetGatewayInfo(
                orderGatewayType,
                out rateType,
                out OrderPaymentChannel orderPaymentChannel,
                out OrderPaymentScheme orderPaymentScheme);

            var shopCommission = _commissionCacheService.GetCommissionInfoByUserId(
                shopId,
                rateType);

            if (!shopCommission.IsEnabled)
            {
                throw new Exception("商户已被停用");
            }

            var toppestCommission = _commissionCacheService.GetToppestCommissionInfoByCommission(shopCommission, rateType);


            //Create order by gateway type.
            AlipayPagePreference alipayPagePreference = new AlipayPagePreference(
                30,
                true,
                true,
                true,
                true,
                true
                );


            var paired = false;
            var saved = false;
            while (!saved)
            {
                try
                {
                    // To Platform order: Find qr code to pairing.
                    // To Fourth Party order: Send request to fourth party.
                    var orderingStrategy = _orderingContext.Database.CreateExecutionStrategy();
                    var pairingStrategy = _pairingContext.Database.CreateExecutionStrategy();
                    var distribuingStrategy = _distributingContext.Database.CreateExecutionStrategy();
                    var applicationStrategy = _applicationDbContext.Database.CreateExecutionStrategy();

                    await orderingStrategy.ExecuteAsync(async () =>
                    {
                        await pairingStrategy.ExecuteAsync(async () =>
                        {
                            await distribuingStrategy.ExecuteAsync(async () =>
                            {
                                await applicationStrategy.ExecuteAsync(async () =>
                                {
                                    using (var orderingTransaction = await _orderingContext.BeginTransactionAsync())
                                    {
                                        using (var pairingTransaction = await _pairingContext.BeginTransactionAsync())
                                        {
                                            using (var distributingTransaction = await _distributingContext.BeginTransactionAsync())
                                            {
                                                using (var applicationTransaction = await _applicationDbContext.BeginTransactionAsync())
                                                {
                                                    try
                                                    {
                                                        var order = Ordering.Domain.Model.Orders.Order.FromShopToPlatform(
                                                                       180,
                                                                       shopId,
                                                                       shopOrderId,
                                                                       shopReturnUrl,
                                                                       shopOkReturnUrl,
                                                                       shopCommission.Rate,
                                                                       toppestCommission.Rate,
                                                                       shopOrderAmount,
                                                                       orderPaymentChannel,
                                                                       orderPaymentScheme,
                                                                       this._orderingDateTimeService,
                                                                       alipayPagePreference
                                                                       );
                                                        //Pair with QR code.
                                                        var pairedQrCode = await _pairingDomainService.PairFrom(
                                                            new Pairing.Domain.Model.QrCodes.Order(
                                                                order.TrackingNumber,
                                                                order.ShopInfo.ShopOrderId,
                                                                order.GetOrderPaymentChannel.Name,
                                                                order.GetOrderPaymentScheme.Name,
                                                                order.Amount,
                                                                order.ShopInfo.ShopId,
                                                                order.ShopInfo.RateRebateFinal,
                                                                order.DateCreated,
                                                                null
                                                                ),
                                                            false //Pairing mode may change depends on Business.
                                                            );

                                                        if (pairedQrCode != null)
                                                        {
                                                            //order.Expired();//For test.

                                                            //If success paired, 
                                                            //mark order as paired,
                                                            order.PairedByPlatform(
                                                                pairedQrCode.UserId,
                                                                pairedQrCode.Id,
                                                                pairedQrCode.MinCommissionRate - 0.001M,
                                                                _orderingDateTimeService
                                                                );
                                                            paired = true;


                                                            //Save changes on qrcode, execute Pairing Domain event handler.
                                                            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();

                                                            //The distribution process was moved into trader confirming process.
                                                            //So we only need to save entities change that made in previous work (eg. created frozen).
                                                            await _commissionRepository.UnitOfWork.SaveEntitiesAsync();
                                                        }
                                                        else
                                                        {
                                                            //If failed to paired,
                                                            //mark order as Expired.
                                                            order.Expired();
                                                        }

                                                        var orderCreated = _orderRepository.Add(order);

                                                        //Save changes. (No need to execute domain event)
                                                        await _orderRepository.UnitOfWork.SaveEntitiesAsync();

                                                        //Create view data here because it may slow down the performence if create in domain handler.
                                                        var orderVM = await _orderQueries.MapFromEntity(orderCreated);

                                                        var createdOrderVM = _orderQueries.Add(orderVM);

                                                        //Set order id for later process;
                                                        orderCreatedId = orderCreated.Id;
                                                        orderTrackingNumber = orderCreated.TrackingNumber;


                                                        // Saves all view model created at previous processes.
                                                        // The Queries all belongs to one db ccontext, so only need to save once.
                                                        await _orderQueries.SaveChangesAsync();

                                                        //Commit Transaction.
                                                        //If there is any error, transaction will rollback, and throw the error after the rollback.
                                                        await _orderingContext.CommitTransactionOnlyAsync(orderingTransaction);
                                                        await _pairingContext.CommitTransactionOnlyAsync(pairingTransaction);
                                                        await _distributingContext.CommitTransactionOnlyAsync(distributingTransaction);
                                                        await _applicationDbContext.CommitTransactionOnlyAsync(applicationTransaction);


                                                        //--- If no concurrency conflics happened, set Saved to true and ---//
                                                        saved = true;
                                                    }
                                                    catch (DbUpdateConcurrencyException ex)
                                                    {
                                                        Console.WriteLine("Catch DbUpdateConcurrencyException When Create Order To Platform:");
                                                        Console.WriteLine(ex.ToString());
                                                        //If the transaction failed because of concurrencies and conflicts,
                                                        //then re-pairing again.

                                                        _orderingContext.RollbackTransaction();
                                                        _pairingContext.RollbackTransaction();
                                                        _distributingContext.RollbackTransaction();
                                                        _applicationDbContext.RollbackTransaction();

                                                        saved = false;
                                                        paired = false;

                                                        throw; //Throw the exception and catch by the catch block out side the tranasaction, then sleep for random secs.
                                                    }
                                                    catch
                                                    {
                                                        _orderingContext.RollbackTransaction();
                                                        _pairingContext.RollbackTransaction();
                                                        _distributingContext.RollbackTransaction();
                                                        _applicationDbContext.RollbackTransaction();

                                                        saved = true;
                                                        paired = false;

                                                        throw;
                                                    }
                                                    finally
                                                    {

                                                        _orderingContext.DisposeTransaction();
                                                        _pairingContext.DisposeTransaction();
                                                        _distributingContext.DisposeTransaction();
                                                        _applicationDbContext.DisposeTransaction();
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
                catch (DbUpdateConcurrencyException)
                {
                    //Sleep for random seconds.
                    //this.SleepForRandomSeconds(1, 25);
                }
                catch
                {
                    //Errors occur for other reasons.
                    //Throw errors and finished the thread.
                    saved = true;
                    paired = false;

                    throw;
                }
            }
            if (paired)
            {
                await this.ConfirmOrder(orderCreatedId);

                return orderTrackingNumber;
            }
            else
            {
                return null;
            }
        }

        public async Task ConfirmOrder(int orderId)
        {
            var saved = false;
            while (!saved)
            {
                try
                {
                    // To Platform order: Find qr code to pairing.
                    // To Fourth Party order: Send request to fourth party.
                    var orderingStrategy = _orderingContext.Database.CreateExecutionStrategy();
                    var pairingStrategy = _pairingContext.Database.CreateExecutionStrategy();
                    var distribuingStrategy = _distributingContext.Database.CreateExecutionStrategy();
                    var applicationStrategy = _applicationDbContext.Database.CreateExecutionStrategy();

                    await orderingStrategy.ExecuteAsync(async () =>
                    {
                        await pairingStrategy.ExecuteAsync(async () =>
                        {
                            await distribuingStrategy.ExecuteAsync(async () =>
                            {
                                await applicationStrategy.ExecuteAsync(async () =>
                                {
                                    using (var orderingTransaction = await _orderingContext.BeginTransactionAsync())
                                    {
                                        using (var pairingTransaction = await _pairingContext.BeginTransactionAsync())
                                        {
                                            using (var distributingTransaction = await _distributingContext.BeginTransactionAsync())
                                            {
                                                using (var applicationTransaction = await _applicationDbContext.BeginTransactionAsync())
                                                {
                                                    try
                                                    {
                                                        //Checking the order is exist.
                                                        var order = await this._orderRepository.GetForConfirmPaymentByOrderIdAsync(orderId);
                                                        if (order == null)
                                                        {
                                                            throw new KeyNotFoundException("找无订单");
                                                        }

                                                        if (order.GetOrderType.Id == OrderType.ShopToPlatform.Id)
                                                        {
                                                            //Confirm.
                                                            order.ConfirmPaymentByTrader(
                                                                order.PayeeInfo?.TraderId,
                                                                this._orderingDateTimeService
                                                                );
                                                            //_orderRepository.Update(order);
                                                            await _orderRepository.UnitOfWork.SaveEntitiesAsync();
                                                        }

                                                        // Saves all view model created at previous processes.
                                                        // The queries all belongs to one db ccontext, so only need to save once.
                                                        await _qrCodeQueries.SaveChangesAsync();

                                                        //Commit Transaction.
                                                        //If there is any error, transaction will rollback, and throw the error after the rollback.
                                                        await _orderingContext.CommitTransactionOnlyAsync(orderingTransaction);
                                                        await _pairingContext.CommitTransactionOnlyAsync(pairingTransaction);
                                                        await _distributingContext.CommitTransactionOnlyAsync(distributingTransaction);
                                                        await _applicationDbContext.CommitTransactionOnlyAsync(applicationTransaction);


                                                        //--- If no concurrency conflics happened, set Saved to true and ---//
                                                        saved = true;
                                                    }
                                                    catch (DbUpdateConcurrencyException)
                                                    {
                                                        //If the transaction failed because of concurrencies and conflicts,
                                                        //then re-pairing again.

                                                        _orderingContext.RollbackTransaction();
                                                        _pairingContext.RollbackTransaction();
                                                        _distributingContext.RollbackTransaction();
                                                        _applicationDbContext.RollbackTransaction();

                                                        saved = false;

                                                        throw; //Throw the exception and catch by the catch block out side the tranasaction, then sleep for random secs.
                                                    }
                                                    catch
                                                    {
                                                        _orderingContext.RollbackTransaction();
                                                        _pairingContext.RollbackTransaction();
                                                        _distributingContext.RollbackTransaction();
                                                        _applicationDbContext.RollbackTransaction();

                                                        saved = true;
                                                        throw;
                                                    }
                                                    finally
                                                    {

                                                        _orderingContext.DisposeTransaction();
                                                        _pairingContext.DisposeTransaction();
                                                        _distributingContext.DisposeTransaction();
                                                        _applicationDbContext.DisposeTransaction();
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
                catch (DbUpdateConcurrencyException)
                {
                    //Sleep for random seconds.
                    this.SleepForRandomSeconds(1, 25);
                }
                catch
                {
                    //Errors occur for other reasons.
                    //Throw errors and finished the thread.
                    saved = true;

                    throw;
                }
            }
        }

        public async Task ConfirmOrder_Deprecated(int orderId)
        {
            string url = string.Empty;
            string orderTrackingNumber = string.Empty;
            string shopOrderId = string.Empty;
            string shopId = string.Empty;
            string dateCreated = string.Empty;
            string dateConfirmed = string.Empty;
            int orderAmount = 0;
            int orderStatus = 0;

            var saved = false;
            while (!saved)
            {
                using (var innerScope = scopeFactory.CreateScope())
                {
                    try
                    {

                        var orderingContext = innerScope.ServiceProvider.GetRequiredService<OrderingContext>();
                        var pairingContext = innerScope.ServiceProvider.GetRequiredService<PairingContext>();
                        var distributingContext = innerScope.ServiceProvider.GetRequiredService<DistributingContext>();
                        var applicationDbContext = innerScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        // the code that you want to measure comes here

                        // To Platform order: Find qr code to pairing.
                        // To Fourth Party order: Send request to fourth party.
                        var orderingStrategy = orderingContext.Database.CreateExecutionStrategy();
                        var pairingStrategy = pairingContext.Database.CreateExecutionStrategy();
                        var distribuingStrategy = distributingContext.Database.CreateExecutionStrategy();
                        var applicationStrategy = applicationDbContext.Database.CreateExecutionStrategy();

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
                                                    using (var applicationTransaction = await applicationDbContext.BeginTransactionAsync())
                                                    {
                                                        try
                                                        {
                                                            //Checking the order is exist.
                                                            var order = await this._orderRepository.GetByOrderIdAsync(orderId);
                                                            if (order == null)
                                                            {
                                                                throw new KeyNotFoundException("找无订单");
                                                            }

                                                            if (order.GetOrderType.Id == OrderType.ShopToPlatform.Id)
                                                            {
                                                                //Confirm.
                                                                order.ConfirmPaymentByTrader(
                                                                    order.PayeeInfo?.TraderId,
                                                                    this._orderingDateTimeService
                                                                    );
                                                                //_orderRepository.Update(order);
                                                                await _orderRepository.UnitOfWork.SaveEntitiesAsync();
                                                            }

                                                            //The order is finished, and view data has updated, so delete the order entity to improve creating/confirming performance.
                                                            /*_orderRepository.Delete(order);
                                                            await _orderRepository.UnitOfWork.SaveChangesAsync();*/

                                                            // Saves all view model created at previous processes.
                                                            // The queries all belongs to one db ccontext, so only need to save once.
                                                            await _qrCodeQueries.SaveChangesAsync();

                                                            //Commit Transaction.
                                                            //If there is any error, transaction will rollback, and throw the error after the rollback.
                                                            await orderingContext.CommitTransactionOnlyAsync(orderingTransaction);
                                                            await pairingContext.CommitTransactionOnlyAsync(pairingTransaction);
                                                            await distributingContext.CommitTransactionOnlyAsync(distributingTransaction);
                                                            await applicationDbContext.CommitTransactionOnlyAsync(applicationTransaction);


                                                            //--- If no concurrency conflics happened, set Saved to true and ---//
                                                            saved = true;

                                                            //Assign request value.
                                                            url = order.ShopInfo.ShopOkReturnUrl;
                                                            orderTrackingNumber = order.TrackingNumber;
                                                            shopOrderId = order.ShopInfo.ShopOrderId;
                                                            shopId = order.ShopInfo.ShopId;
                                                            dateCreated = order.DateCreated.ToFullString();
                                                            dateConfirmed = order.DatePaymentRecieved?.ToFullString();
                                                            orderAmount = (int)order.Amount;
                                                            orderStatus = order.GetOrderStatus.Id;
                                                        }
                                                        catch (DbUpdateConcurrencyException)
                                                        {
                                                            //If the transaction failed because of concurrencies and conflicts,
                                                            //then re-pairing again.

                                                            orderingContext.RollbackTransaction();
                                                            pairingContext.RollbackTransaction();
                                                            distributingContext.RollbackTransaction();
                                                            applicationDbContext.RollbackTransaction();

                                                            saved = false;

                                                            throw; //Throw the exception and catch by the catch block out side the tranasaction, then sleep for random secs.
                                                        }
                                                        catch
                                                        {
                                                            orderingContext.RollbackTransaction();
                                                            pairingContext.RollbackTransaction();
                                                            distributingContext.RollbackTransaction();
                                                            applicationDbContext.RollbackTransaction();

                                                            saved = true;
                                                            throw;
                                                        }
                                                        finally
                                                        {

                                                            orderingContext.DisposeTransaction();
                                                            pairingContext.DisposeTransaction();
                                                            distributingContext.DisposeTransaction();
                                                            applicationDbContext.DisposeTransaction();
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
                    catch (DbUpdateConcurrencyException)
                    {
                        //Sleep for random seconds.
                        this.SleepForRandomSeconds(1, 25);
                    }
                    catch (Exception ex)
                    {
                        //Errors occur for other reasons.
                        //Throw errors and finished the thread.
                        saved = true;
                        Console.WriteLine("Catch Exceptions While Confirm order:" + ex.Message);

                        //throw;
                    }
                    await _newPayApiClient.ReturnOrderResult(
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

        }



        private async Task<ShopGatewayEntry> GetShopGatewayByOrderGatewayType(string shopId, OrderGatewayType orderGatewayType)
        {
            ShopGatewayEntry shopGatewayEntry = null;
            if (orderGatewayType == OrderGatewayType.AlipayBarcode)
            {
                shopGatewayEntry = await _shopGatewayQueries.GetMatchedShopGatewayAsync(
                    shopId,
                    Pairing.Domain.Model.QrCodes.PaymentChannel.Alipay.Name,
                    Pairing.Domain.Model.QrCodes.PaymentScheme.Barcode.Name
                    );

            }
            else if (orderGatewayType == OrderGatewayType.AlipayMerchant)
            {
                shopGatewayEntry = await _shopGatewayQueries.GetMatchedShopGatewayAsync(
                    shopId,
                    Pairing.Domain.Model.QrCodes.PaymentChannel.Alipay.Name,
                    Pairing.Domain.Model.QrCodes.PaymentScheme.Barcode.Name
                    );
            }
            else if (orderGatewayType == OrderGatewayType.AlipayTransaction)
            {
                shopGatewayEntry = await _shopGatewayQueries.GetMatchedShopGatewayAsync(
                    shopId,
                    Pairing.Domain.Model.QrCodes.PaymentChannel.Alipay.Name,
                    Pairing.Domain.Model.QrCodes.PaymentScheme.Transaction.Name
                    );
            }
            else if (orderGatewayType == OrderGatewayType.AlipayBank)
            {
                shopGatewayEntry = await _shopGatewayQueries.GetMatchedShopGatewayAsync(
                    shopId,
                    Pairing.Domain.Model.QrCodes.PaymentChannel.Alipay.Name,
                    Pairing.Domain.Model.QrCodes.PaymentScheme.Bank.Name
                    );
            }
            else if (orderGatewayType == OrderGatewayType.AlipayEnvelop)
            {
                shopGatewayEntry = await _shopGatewayQueries.GetMatchedShopGatewayAsync(
                    shopId,
                    Pairing.Domain.Model.QrCodes.PaymentChannel.Alipay.Name,
                    Pairing.Domain.Model.QrCodes.PaymentScheme.Envelop.Name
                    );
            }
            else if (orderGatewayType == OrderGatewayType.AlipayEnvelopPassword)
            {
                shopGatewayEntry = await _shopGatewayQueries.GetMatchedShopGatewayAsync(
                    shopId,
                    Pairing.Domain.Model.QrCodes.PaymentChannel.Alipay.Name,
                    Pairing.Domain.Model.QrCodes.PaymentScheme.EnvelopPassword.Name
                    );
            }
            else if (orderGatewayType == OrderGatewayType.WechatBarcode)
            {
                shopGatewayEntry = await _shopGatewayQueries.GetMatchedShopGatewayAsync(
                    shopId,
                    Pairing.Domain.Model.QrCodes.PaymentChannel.Wechat.Name,
                    Pairing.Domain.Model.QrCodes.PaymentScheme.Barcode.Name
                    );
            }
            else if (orderGatewayType == OrderGatewayType.WechatMerchant)
            {
                shopGatewayEntry = await _shopGatewayQueries.GetMatchedShopGatewayAsync(
                    shopId,
                    Pairing.Domain.Model.QrCodes.PaymentChannel.Wechat.Name,
                    Pairing.Domain.Model.QrCodes.PaymentScheme.Merchant.Name
                    );
            }
            else
            {
                throw new InvalidOperationException("无法辨识通道。");
            }

            return shopGatewayEntry;
        }

        private bool GetGatewayInfo(OrderGatewayType orderGatewayType,
            out RateType rateType,
            out OrderPaymentChannel orderPaymentChannel,
            out OrderPaymentScheme orderPaymentScheme)
        {
            if (orderGatewayType == OrderGatewayType.AlipayBarcode)
            {
                rateType = RateType.Alipay;
                orderPaymentChannel = OrderPaymentChannel.Alipay;
                orderPaymentScheme = OrderPaymentScheme.Barcode;

            }
            else if (orderGatewayType == OrderGatewayType.AlipayMerchant)
            {
                rateType = RateType.Alipay;
                orderPaymentChannel = OrderPaymentChannel.Alipay;
                orderPaymentScheme = OrderPaymentScheme.Merchant;
            }
            else if (orderGatewayType == OrderGatewayType.AlipayTransaction)
            {
                rateType = RateType.Alipay;
                orderPaymentChannel = OrderPaymentChannel.Alipay;
                orderPaymentScheme = OrderPaymentScheme.Transaction;
            }
            else if (orderGatewayType == OrderGatewayType.AlipayBank)
            {
                rateType = RateType.Alipay;
                orderPaymentChannel = OrderPaymentChannel.Alipay;
                orderPaymentScheme = OrderPaymentScheme.Bank;
            }
            else if (orderGatewayType == OrderGatewayType.AlipayEnvelop)
            {
                rateType = RateType.Alipay;
                orderPaymentChannel = OrderPaymentChannel.Alipay;
                orderPaymentScheme = OrderPaymentScheme.Envelop;
            }
            else if (orderGatewayType == OrderGatewayType.AlipayEnvelopPassword)
            {
                rateType = RateType.Alipay;
                orderPaymentChannel = OrderPaymentChannel.Alipay;
                orderPaymentScheme = OrderPaymentScheme.EnvelopPassword;
            }
            else if (orderGatewayType == OrderGatewayType.WechatBarcode)
            {
                rateType = RateType.Wechat;
                orderPaymentChannel = OrderPaymentChannel.Wechat;
                orderPaymentScheme = OrderPaymentScheme.Barcode;
            }
            else if (orderGatewayType == OrderGatewayType.WechatMerchant)
            {
                rateType = RateType.Wechat;
                orderPaymentChannel = OrderPaymentChannel.Wechat;
                orderPaymentScheme = OrderPaymentScheme.Merchant;
            }
            else
            {
                throw new InvalidOperationException("无法辨识通道。");
            }

            return true;
        }

        private int GetRandomSkipNumber()
        {
            Random random = new Random();
            return random.Next(0, 100);
        }

        private void SleepForRandomSeconds(int min, int max)
        {
            Random random = new Random();
            Thread.Sleep(random.Next(min, max) * 1000);
        }
    }
}
