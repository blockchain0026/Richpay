using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Distributions;
using Distributing.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Model.Orders;
using Ordering.Domain.Model.Shared;
using Ordering.Infrastructure;
using Pairing.Domain.Model.CloudDevices;
using Pairing.Domain.Model.QrCodes;
using Pairing.Domain.Model.ShopGateways;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using Pairing.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.CacheServices;
using WebMVC.Applications.Queries.Balances;
using WebMVC.Applications.Queries.Commission;
using WebMVC.Applications.Queries.Orders;
using WebMVC.Applications.Queries.QrCodes;
using WebMVC.Applications.Queries.RunningAccounts;
using WebMVC.Applications.Queries.ShopGateways;
using WebMVC.Data;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;

namespace WebMVC.Infrastructure.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IOrderQueries _orderQueries;
        private readonly IRunningAccountQueries _runningAccountQueries;
        private readonly IShopGatewayQueries _shopGatewayQueries;
        private readonly IQrCodeQueries _qrCodeQueries;

        private readonly IOrderRepository _orderRepository;
        private readonly IShopSettingsRepository _shopSettingsRepository;
        private readonly ICommissionRepository _commissionRepository;
        private readonly IQrCodeRepository _qrCodeRepository;

        private readonly ISystemConfigurationService _systemConfigurationService;

        private readonly IDateTimeService _orderingDateTimeService;
        private readonly IPairingDomainService _pairingDomainService;

        private readonly OrderingContext _orderingContext;
        private readonly PairingContext _pairingContext;
        private readonly DistributingContext _distributingContext;
        private readonly ApplicationDbContext _applicationDbContext;

        private readonly ICommissionCacheService _commissionCacheService;
        private readonly IOrderDailyCacheService _orderDailyCacheService;

        public OrderService(UserManager<ApplicationUser> userManager, IOrderQueries orderQueries, IRunningAccountQueries runningAccountQueries, IShopGatewayQueries shopGatewayQueries, IQrCodeQueries qrCodeQueries, IOrderRepository orderRepository, IShopSettingsRepository shopSettingsRepository, ICommissionRepository commissionRepository, IQrCodeRepository qrCodeRepository, ISystemConfigurationService systemConfigurationService, IDateTimeService orderingDateTimeService, IPairingDomainService pairingDomainService, OrderingContext orderingContext, PairingContext pairingContext, DistributingContext distributingContext, ApplicationDbContext applicationDbContext, ICommissionCacheService commissionCacheService, IOrderDailyCacheService orderDailyCacheService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _runningAccountQueries = runningAccountQueries ?? throw new ArgumentNullException(nameof(runningAccountQueries));
            _shopGatewayQueries = shopGatewayQueries ?? throw new ArgumentNullException(nameof(shopGatewayQueries));
            _qrCodeQueries = qrCodeQueries ?? throw new ArgumentNullException(nameof(qrCodeQueries));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _shopSettingsRepository = shopSettingsRepository ?? throw new ArgumentNullException(nameof(shopSettingsRepository));
            _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
            _qrCodeRepository = qrCodeRepository ?? throw new ArgumentNullException(nameof(qrCodeRepository));
            _systemConfigurationService = systemConfigurationService ?? throw new ArgumentNullException(nameof(systemConfigurationService));
            _orderingDateTimeService = orderingDateTimeService ?? throw new ArgumentNullException(nameof(orderingDateTimeService));
            _pairingDomainService = pairingDomainService ?? throw new ArgumentNullException(nameof(pairingDomainService));
            _orderingContext = orderingContext ?? throw new ArgumentNullException(nameof(orderingContext));
            _pairingContext = pairingContext ?? throw new ArgumentNullException(nameof(pairingContext));
            _distributingContext = distributingContext ?? throw new ArgumentNullException(nameof(distributingContext));
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _commissionCacheService = commissionCacheService ?? throw new ArgumentNullException(nameof(commissionCacheService));
            _orderDailyCacheService = orderDailyCacheService ?? throw new ArgumentNullException(nameof(orderDailyCacheService));
        }




        #region Query

        public async Task<OrderEntry> GetOrderById(string searchByUserId, int orderId)
        {
            //Validate user is exist.
            /*var user = await ValidateUserExist(searchByUserId).ConfigureAwait(false);

            await ValidateUserPermissionOnOrderAsync(user, orderId).ConfigureAwait(false);
            */
            //Only manager can get order. (shop need to use api to get order)


            //Get order data.
            var result = await _orderQueries.GetOrderEntryAsync(
                orderId
                ).ConfigureAwait(false);


            if (result is null)
            {
                throw new InvalidOperationException("查无订单数据。");
            }

            return result;
        }

        public async Task<List<OrderEntry>> GetPlatformOrderEntrys(int pageIndex, int take, string searchByUserId, string searchString = "", string sortField = "",
            DateTime? from = null, DateTime? to = null,
            string orderStatus = null, string orderPaymentChannel = null, string orderPaymentScheme = null,
            string direction = SortDirections.Descending)
        {
            var user = await ValidateUserExist(searchByUserId).ConfigureAwait(false);

            //If the order status provided is failed, then search expired orders.
            bool? isExpired = null;
            if (orderStatus == "Failed")
            {
                isExpired = true;
                //No need to specified ordre status.
                orderStatus = string.Empty;
            }
            else if (!string.IsNullOrEmpty(orderStatus))
            {
                //If the order status is specified, then search the in-expired order.
                isExpired = false;
            }

            //Only manager can view order list.
            if (user.BaseRoleType == BaseRoleType.Manager)
            {
                return await _orderQueries.GetOrderEntrysAsync(
                    pageIndex,
                    take,
                    searchString,
                    sortField,
                    from,
                    to,
                    OrderType.ShopToPlatform.Name,
                    orderStatus,
                    orderPaymentChannel,
                    orderPaymentScheme,
                    isExpired,
                    direction
                    ).ConfigureAwait(false);
            }
            else if (user.BaseRoleType == BaseRoleType.Shop)
            {
                return await _orderQueries.GetOrderEntrysByShopIdAsync(
                    user.Id,
                    pageIndex,
                    take,
                    searchString,
                    sortField,
                    from,
                    to,
                    OrderType.ShopToPlatform.Name,
                    orderStatus,
                    orderPaymentChannel,
                    orderPaymentScheme,
                    isExpired,
                    direction
                    ).ConfigureAwait(false);
            }
            else if (user.BaseRoleType == BaseRoleType.Trader)
            {
                return await _orderQueries.GetOrderEntrysByTraderIdAsync(
                    user.Id,
                    pageIndex,
                    take,
                    searchString,
                    sortField,
                    from,
                    to,
                    OrderType.ShopToPlatform.Name,
                    orderStatus,
                    orderPaymentChannel,
                    orderPaymentScheme,
                    isExpired,
                    direction
                    ).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidOperationException("用户没有订单列表权限");
            }
        }

        public async Task<OrderSumData> GetPlatformOrderEntrysTotalSumData(string searchByUserId, string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string orderStatus = null, string orderPaymentChannel = null, string orderPaymentScheme = null)
        {
            var user = await ValidateUserExist(searchByUserId).ConfigureAwait(false);
            //If the order status provided is failed, then search expired orders.
            bool isExpired = false;
            if (orderStatus == "Failed")
            {
                isExpired = true;
                //No need to specified ordre status.
                orderStatus = string.Empty;
            }
            if (user.BaseRoleType == BaseRoleType.Manager)
            {
                return await _orderQueries.GetOrderEntrysTotalSumDataAsync(
                    searchString,
                    from,
                    to,
                    OrderType.ShopToPlatform.Name,
                    orderStatus,
                    orderPaymentChannel,
                    orderPaymentScheme,
                    isExpired
                    );
            }
            else if (user.BaseRoleType == BaseRoleType.Shop)
            {
                return await _orderQueries.GetOrderEntrysTotalSumDataByShopIdAsync(
                    user.Id,
                    searchString,
                    from,
                    to,
                    OrderType.ShopToPlatform.Name,
                    orderStatus,
                    orderPaymentChannel,
                    orderPaymentScheme,
                    isExpired
                    );
            }
            else if (user.BaseRoleType == BaseRoleType.Trader)
            {
                return await _orderQueries.GetOrderEntrysTotalSumDataByTraderIdAsync(
                    user.Id,
                    searchString,
                    from,
                    to,
                    OrderType.ShopToPlatform.Name,
                    orderStatus,
                    orderPaymentChannel,
                    orderPaymentScheme,
                    isExpired
                    );
            }
            else
            {
                throw new InvalidOperationException("用户没有订单列表权限");
            }
        }

        public async Task<List<RunningAccountRecord>> GetRunningAccountRecordsByUserIdAsync(string searchByUserId, string userId, int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null, string status = null,
            string direction = SortDirections.Descending)
        {
            var searchByUser = await ValidateUserExist(searchByUserId).ConfigureAwait(false);

            await this.ValidateUserPermissionOnRunningAccountAsync(searchByUser, userId);

            var runningAccountRecords = await this._runningAccountQueries.GetRunningAccountRecordsByUserIdAsync(
                userId,
                pageIndex,
                take,
                searchString,
                sortField,
                from,
                to,
                status,
                direction
                );

            return runningAccountRecords;
        }

        public async Task<RunningAccountRecordSumData> GetRunningAccountRecordsTotalSumDataByUserIdAsync(string searchByUserId, string userId, string searchString = null,
            DateTime? from = null, DateTime? to = null, string status = null)
        {
            var searchByUser = await ValidateUserExist(searchByUserId).ConfigureAwait(false);

            await this.ValidateUserPermissionOnRunningAccountAsync(searchByUser, userId);

            var runningAccountRecordsSumData = await _runningAccountQueries.GetRunningAccountRecordsTotalSumDataByUserIdAsync(
                userId,
                searchString,
                from,
                to,
                status
                );

            return runningAccountRecordsSumData;
        }

        #endregion


        #region Command
        public async Task<int?> CreateOrderToPlatform(string createByUserId, string shopId, string shopOrderId, decimal shopOrderAmount, string shopReturnUrl, string shopOkReturnUrl,
            OrderGatewayType orderGatewayType)
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
                                                            await _shopSettingsRepository.UnitOfWork.SaveEntitiesAsync();

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
                return orderCreatedId;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> CreateTestOrderToPlatform(string createByUserId, decimal orderAmount,
            int qrCodeId)
        {
            string orderTrackingNumber = string.Empty;

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
                    var applicationStrategy = _applicationDbContext.Database.CreateExecutionStrategy();

                    await orderingStrategy.ExecuteAsync(async () =>
                    {
                        await applicationStrategy.ExecuteAsync(async () =>
                        {
                            using (var orderingTransaction = await _orderingContext.BeginTransactionAsync())
                            {
                                using (var applicationTransaction = await _applicationDbContext.BeginTransactionAsync())
                                {
                                    try
                                    {
                                        //Get qr code.
                                        var qrCode = await _pairingContext.QrCodes
                                        .AsNoTracking()
                                        .Where(q => q.Id == qrCodeId)
                                        .FirstOrDefaultAsync();

                                        Ordering.Domain.Model.Orders.Order order = null;

                                        if (qrCode != null)
                                        {
                                            order = Ordering.Domain.Model.Orders.Order.FromAdminToPlatform(
                                                180,
                                                OrderPaymentChannel.FromName(qrCode.GetPaymentChannel.Name),
                                                OrderPaymentScheme.FromName(qrCode.GetPaymentScheme.Name),
                                                orderAmount,
                                                this._orderingDateTimeService,
                                                alipayPagePreference
                                                );

                                            //If success paired, 
                                            //mark order as paired,
                                            order.PairedByPlatform(
                                                qrCode.UserId,
                                                qrCode.Id,
                                                0M,
                                                _orderingDateTimeService
                                                );
                                            paired = true;
                                        }
                                        else
                                        {
                                            throw new Exception("找无此二维码。");
                                        }

                                        var orderCreated = _orderRepository.Add(order);

                                        //Save changes.
                                        await _orderRepository.UnitOfWork.SaveChangesAsync();

                                        //Create view data here because it may slow down the performence if create in domain handler.
                                        var orderVM = await _orderQueries.MapFromEntity(orderCreated);

                                        var createdOrderVM = _orderQueries.Add(orderVM);

                                        //Set order tracking number for later process;
                                        orderTrackingNumber = orderCreated.TrackingNumber;


                                        // Saves all view model created at previous processes.
                                        // The Queries all belongs to one db ccontext, so only need to save once.
                                        await _orderQueries.SaveChangesAsync();

                                        //Commit Transaction.
                                        //If there is any error, transaction will rollback, and throw the error after the rollback.
                                        await _orderingContext.CommitTransactionOnlyAsync(orderingTransaction);
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
                                        _applicationDbContext.RollbackTransaction();

                                        saved = false;
                                        paired = false;

                                        throw; //Throw the exception and catch by the catch block out side the tranasaction, then sleep for random secs.
                                    }
                                    catch
                                    {
                                        _orderingContext.RollbackTransaction();
                                        _applicationDbContext.RollbackTransaction();

                                        saved = true;
                                        paired = false;

                                        throw;
                                    }
                                    finally
                                    {

                                        _orderingContext.DisposeTransaction();
                                        _applicationDbContext.DisposeTransaction();
                                    }
                                }
                            }
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
                string localUrl = "pay?ordertrackingnumber=" + orderTrackingNumber;
                return localUrl;
            }
            else
            {
                return null;
            }
        }

        public async Task DeleteAllTestOrders(string deleteByUserId)
        {
            var deleteByUser = await this.ValidateUserExist(deleteByUserId);

            //Only manager can delete test orders.
            if (deleteByUser is null || deleteByUser.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("用户没有此权限。");
            }

            //Delete order entites.
            var testOrders = await this._orderingContext.Orders
                .Where(o => o.IsTestOrder)
                .ToListAsync();

            if (testOrders.Any())
            {
                this._orderingContext.Orders.RemoveRange(testOrders);
                //Delete order entries (View Model).
                foreach (var order in testOrders)
                {
                    var orderEntryToDelete = await _applicationDbContext.OrderEntrys
                        .Where(o => o.OrderId == order.Id)
                        .FirstOrDefaultAsync();
                    if (orderEntryToDelete is null)
                    {
                        throw new KeyNotFoundException("找无订单。");
                    }
                    _applicationDbContext.Remove(orderEntryToDelete);
                }
            }

            await _orderingContext.SaveChangesAsync();
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task ConfirmOrderById(int orderId, string confirmedByTraderId)
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
                                                                confirmedByTraderId,
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

        public async Task ConfirmOrderByTrackingNumber(string orderTrackingNumber, string confirmedByTraderId)
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
                                                        var order = await this._orderRepository.GetByOrderTrackingNumberAsync(orderTrackingNumber);
                                                        if (order == null)
                                                        {
                                                            throw new KeyNotFoundException("找无订单");
                                                        }

                                                        if (order.GetOrderType.Id == OrderType.ShopToPlatform.Id)
                                                        {
                                                            //Confirm.
                                                            order.ConfirmPaymentByTrader(
                                                                confirmedByTraderId,
                                                                this._orderingDateTimeService
                                                                );
                                                            _orderRepository.Update(order);
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
                catch
                {
                    //Errors occur for other reasons.
                    //Throw errors and finished the thread.
                    saved = true;

                    throw;
                }
            }
        }

        public async Task ConfirmOrderByAdmin(int orderId)
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


        public async Task ForceConfirmOrder(int orderId, string forcedByAdminId)
        {
            //Checking the order is exist.
            var order = await this._orderRepository.GetByOrderIdAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("找无订单");
            }

            //Force Confirm.
            order.ForcedConfirmation(
                forcedByAdminId,
                string.Empty);

            _orderRepository.Update(order);
            await _orderRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task ForceConfirmOrders(List<int> orderIds, string forcedByAdminId)
        {
            foreach (var orderId in orderIds)
            {
                //Checking the order is exist.
                var order = await this._orderRepository.GetByOrderIdAsync(orderId);
                if (order == null)
                {
                    throw new KeyNotFoundException("找无订单");
                }

                //Force Confirm.
                order.ForcedConfirmation(
                    forcedByAdminId,
                    string.Empty);

                _orderRepository.Update(order);
            }

            await _orderRepository.UnitOfWork.SaveEntitiesAsync();
        }

        #endregion


        #region Custom Function
        private async Task<ApplicationUser> ValidateUserExist(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("No user found by given user id.");
            }

            return user;
        }

        private async Task<OrderEntry> ValidateUserPermissionOnOrderAsync(ApplicationUser user, int orderId)
        {
            throw new NotImplementedException();

            if (user == null)
            {
                throw new KeyNotFoundException("The user must be provided.");
            }

            //Get order to do permission check later.
            var orderEntry = await _orderQueries.GetOrderEntryAsync(orderId).ConfigureAwait(false);
            if (orderEntry == null)
            {
                throw new InvalidOperationException("查无订单。");
            }

            //Check user permission.
            if (user.BaseRoleType == BaseRoleType.Manager)
            {
            }
            else if (user.BaseRoleType == BaseRoleType.TraderAgent)
            {
                /*if (orderEntry.UplineUserId != user.Id)
                {
                    throw new InvalidOperationException("代理只能查看直属交易员的二维码。");
                }*/
            }
            else if (user.BaseRoleType == BaseRoleType.Trader)
            {
                //if (orderEntry.UserId != user.Id)
                //{
                //    throw new InvalidOperationException("交易员只能查看自己的二维码。");
                //}
            }
            else
            {
                throw new InvalidOperationException("用户没有二维码权限。");
            }

            return orderEntry;
        }

        private int GetRandomSkipNumber()
        {
            Random random = new Random();
            return random.Next(0, 200);
        }


        private void SleepForRandomSeconds(int min, int max)
        {
            Random random = new Random();
            Thread.Sleep(random.Next(min, max) * 1000);
        }


        private async Task ValidateUserPermissionOnRunningAccountAsync(ApplicationUser searchByUser, string userId)
        {
            //Validating user.
            if (searchByUser.BaseRoleType == BaseRoleType.Manager)
            {
            }
            else if (searchByUser.BaseRoleType == BaseRoleType.Shop)
            {
                if (searchByUser.Id != userId)
                {
                    throw new InvalidOperationException("商户无法查看他人流水。");
                }
            }
            else if (searchByUser.BaseRoleType == BaseRoleType.ShopAgent)
            {
                if (searchByUser.Id != userId)
                {
                    var user = await ValidateUserExist(userId).ConfigureAwait(false);

                    if (!await IsAboveAgentChainAsync(searchByUser, user))
                    {
                        throw new InvalidOperationException("商户代理只能查看自身以及旗下代理和交易员的流水。");
                    }
                }
            }
            else if (searchByUser.BaseRoleType == BaseRoleType.Trader)
            {
                if (searchByUser.Id != userId)
                {
                    throw new InvalidOperationException("交易员无法查看他人流水。");
                }
            }
            else if (searchByUser.BaseRoleType == BaseRoleType.TraderAgent)
            {
                {
                    if (searchByUser.Id != userId)
                    {
                        var user = await ValidateUserExist(userId).ConfigureAwait(false);
                        if (!await IsAboveAgentChainAsync(searchByUser, user))
                        {
                            throw new InvalidOperationException("交易员代理只能查看自身以及旗下代理和交易员的流水。");
                        }
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("用户身份无法辨识。");
            }
        }

        private void ValidateUserPermissionOnCreateOrder(ApplicationUser createByUser, string shopId)
        {
            //If the order isn't create by manager, check the order is create by shop,
            //and the owner is him.
            if (createByUser.BaseRoleType != BaseRoleType.Manager)
            {
                if (createByUser.BaseRoleType == BaseRoleType.Shop)
                {
                    if (createByUser.Id != shopId)
                    {
                        throw new InvalidOperationException($"无效操作，商户不能为其他商户建立订单。");
                    }
                }
                else
                {
                    throw new InvalidOperationException("用户没有此权限。");
                }
            }
        }


        private async Task<bool> IsAboveAgentChainAsync(ApplicationUser agent, ApplicationUser bottomUser)
        {
            if (bottomUser == null || agent == null)
            {
                throw new ArgumentNullException("Invalid user.");
            }
            /*if (string.IsNullOrEmpty(bottomUser.UplineId))
            {
                return false;
            }*/

            var isAboveAgentChain = false;
            var currentUser = await _userManager.Users
                    .Where(u => u.Id == bottomUser.Id)
                    .Select(u => new
                    {
                        u.Id,
                        u.UplineId
                    })
                    .FirstOrDefaultAsync();

            while (!isAboveAgentChain)
            {
                //Check the current user's id is equal to trader agent's id.
                if (agent.Id == currentUser.Id)
                {
                    isAboveAgentChain = true;
                    break;
                }


                var uplineId = currentUser.UplineId;
                if (string.IsNullOrEmpty(uplineId))
                {
                    break;
                }

                //Get the upline user.
                var uplineUser = await _userManager.Users
                    .Where(u => u.Id == uplineId)
                    .Select(u => new
                    {
                        u.Id,
                        u.UplineId
                    })
                    .FirstOrDefaultAsync();

                //Set current user and check at the next loop.
                currentUser = uplineUser;
            }

            return isAboveAgentChain;
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

        private async Task DeleteWithoutSaveEntities(int orderId)
        {
            throw new NotImplementedException();
            //Checking the qr code is exist.
            var order = await this._orderRepository.GetByOrderIdAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException("No qr code found by given QR code id.");
            }

            //Checking the qr code is not approved yet.
            //if (order.IsApproved)
            //{
            //    throw new InvalidOperationException("无法拒绝审核已审核的二维码。");
            //}

            //Delete qr code.
            _orderRepository.Delete(order);


            //Delete qr code view data.
            var orderEntry = await this._orderQueries.GetOrderEntryAsync(orderId);
            if (orderEntry != null)
            {
                this._orderQueries.Delete(orderEntry);
            }
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

        #endregion
    }
}
