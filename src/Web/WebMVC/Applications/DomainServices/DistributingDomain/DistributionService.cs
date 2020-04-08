using Distributing.Domain.Exceptions;
using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Distributions;
using Distributing.Domain.Model.Frozens;
using Distributing.Domain.Model.Shared;
using Ordering.Domain.Model.Orders;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.CacheServices;
using WebMVC.Applications.Queries.Orders;
using WebMVC.Applications.Queries.QrCodes;
using WebMVC.Applications.Queries.RunningAccounts;

namespace WebMVC.Applications.DomainServices.DistributingDomain
{
    public class DistributionService : IDistributionService
    {
        private readonly ICommissionRepository _commissionRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IDistributionRepository _distributionRepository;
        private readonly IRunningAccountQueries _runningAccountQueries;
        private readonly ICommissionCacheService _commissionCacheService;

        public DistributionService(ICommissionRepository commissionRepository, IBalanceRepository balanceRepository, IDistributionRepository distributionRepository, IRunningAccountQueries runningAccountQueries, ICommissionCacheService commissionCacheService)
        {
            _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _distributionRepository = distributionRepository ?? throw new ArgumentNullException(nameof(distributionRepository));
            _runningAccountQueries = runningAccountQueries ?? throw new ArgumentNullException(nameof(runningAccountQueries));
            _commissionCacheService = commissionCacheService ?? throw new ArgumentNullException(nameof(commissionCacheService));
        }


        /// <summary>
        /// This function is called when qr code is paired.
        /// TO DO: Append all data needed to commission.
        /// 
        /// If the order's finished, then retrieve the data and distriube money by calling DistributeFrom function.
        /// 
        /// #There might be problems if someone disable it's account during the process.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="rateType"></param>
        /// <param name="dateTimeService"></param>
        /// <returns></returns>
        public async Task OrderCreated(Distributing.Domain.Model.Distributions.Order order, RateType rateType, IDateTimeService dateTimeService)
        {
            //Validating the shop, traders and agents is ready for new order.
            /*var availableTraders = await GetAvailableTradersForNewOrder(order.Amount, order.ShopId, rateType);

            var traderId = availableTraders.Where(id => id == order.TraderId).SingleOrDefault();

            if (string.IsNullOrEmpty(traderId))
            {
                throw new DistributingDomainException("The trader's account must be enabled before creating order." + ". At DistributionService.OrderCreated()");
            }*/
            //

            #region Freeze trader's balance
            //The QrCodePairedWithOrderDomainEventHandler already handle the trader balance updating process.

            /*var traderBalance = await _balanceRepository.GetByUserIdAsync(order.TraderId);
            traderBalance.NewOrder(order, dateTimeService);
            _balanceRepository.Update(traderBalance);*/

            #endregion

            //await this.DistributeDivends(order, rateType);

            //Save the change on balance updating and new frozen.
            await _commissionRepository.UnitOfWork.SaveEntitiesAsync();

        }



        /// <summary>
        /// This function is called by OrderPaymentConfirmedByTraderDomainEventHandler.
        /// TO DO: Create distribution.
        ///        Update trader's Qr Codes and VMs.
        ///        Update running account records VMs.
        ///        Delele trader's frozen for the order because the order is success, so the frozen amount is no need any more.
        /// RETURN:Distributions of Trader and Shop.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="dateTimeService"></param>
        /// <param name="qrCodeId">
        /// This param is required for reset QrCode staus at BalanceDistributedDomainEventHandler.
        /// </param>
        /// <returns></returns>
        public async Task<List<decimal>> DistributeFrom(Distributing.Domain.Model.Distributions.Order order, IDateTimeService dateTimeService, int qrCodeId, RateType rateType)
        {
            #region Distribute the dividends.
            //Find all qr codes and update balance.
            //--> The ResolveCompletedDistributionBackgroundService will take care of above concerns.
            var distributedAmounts = await this.DistributeDivends(order, rateType);

            #endregion



            #region Delete traders frozen record
            /*var frozen = await _frozenRepository.GetByOrderTrackingNumberAsync(order.TrackingNumber);
            if (frozen == null)
            {
                throw new DistributingDomainException("No frozen found by order tracking number provided" + ". At DistributionService.DistributeFrom()");
            }
            _frozenRepository.Delete(frozen);*/
            #endregion

            return distributedAmounts;

            //Let event handler do the saves.
            //await _distributionRepository.UnitOfWork.SaveEntitiesAsync();
        }



        /// <summary>
        /// When an order's cancellation is reviewed by an admin,
        /// mark the processing orders under associated commissions as canceled,
        /// and unfreeze the trader's frozen balance.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="rateType"></param>
        /// <param name="dateTimeService"></param>
        /// <returns></returns>
        public async Task OrderCanceled(Distributing.Domain.Model.Distributions.Order order, IDateTimeService dateTimeService, RateType rateType)
        {

            #region Add Running Account Records
            await this.AddFailedRunningAccountRecords(order, rateType);
            #endregion

            await _distributionRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task<CommissionInfo> GetUplineCommissionInfoFrom(CommissionInfo commissionInfo, RateType rateType)
        {
            /*if (commission.UplineCommissionId == null)
            {
                throw new DistributingDomainException("The commission provided has no upline" + ". At DistributionService.GetUplineCommissionFrom()");
            }

            var result = await _commissionRepository.GetByCommissionIdAsync((int)commission.UplineCommissionId);

            if (result == null)
            {
                throw new DistributingDomainException("No upline commission found" + ". At DistributionService.GetUplineCommissionFrom()");
            }*/

            if (commissionInfo.UplineCommissionId != null)
            {
                return await _commissionRepository.GetCommissionInfoByCommissionIdAsync((int)commissionInfo.UplineCommissionId, rateType);
            }

            return null;
        }


        private async Task<List<decimal>> DistributeDivends(Distributing.Domain.Model.Distributions.Order order, RateType rateType)
        {
            #region Store order details to shop
            var shopCommission = _commissionCacheService.GetCommissionInfoByUserId(order.ShopId, rateType);
            if (shopCommission == null)
            {
                throw new DistributingDomainException("No distribution found by given shop id. At DistributionService.OrderCreated");
            }
            var shopRebateRate = shopCommission.Rate;

            /*var balanceId = await _balanceRepository.GetBalanceIdByUserIdAsync(order.ShopId);
            if (balanceId == null)
            {
                throw new DistributingDomainException("No balance found by given shop id. At DistributionService.OrderCreated");
            }*/

            //The distributed amount is total order amount minus rebate amount.
            decimal shopDistributedAmount = order.Amount - order.Amount * (decimal)shopRebateRate;

            var shopOrder = new Distributing.Domain.Model.Distributions.Order(
                    order.TrackingNumber,
                    order.ShopOrderId,
                    order.Amount,
                    shopDistributedAmount,
                    order.ShopId,
                    order.TraderId,
                    order.DateCreated
                    );

            var shopDistribution = Distribution.FromLiquidation(
                shopOrder,
                shopCommission.UserId,
                //(int)balanceId,
                shopCommission.BalanceId,
                shopCommission.Id,
                null
                );

            _distributionRepository.Add(shopDistribution);

            //Create Running Account Record VM.
            _runningAccountQueries.Add(
                await _runningAccountQueries.MapFromOrderInfo(shopOrder, shopCommission.UserId));
            #endregion


            #region Store order details to shop agents

            var uplineShopAgentCommission = _commissionCacheService.GetCommissionInfoByCommissionId(shopCommission.UplineCommissionId ?? default, rateType);

            decimal downlineCommissionRate = shopRebateRate;

            //For UI Search Purpose.
            string downlineUserId = shopCommission.UserId;

            while (true)
            {
                if (uplineShopAgentCommission == null)
                {
                    break;
                }

                var currentCommission = uplineShopAgentCommission;
                //ValidateCommissionStatus(currentCommission); The agent doens't need to be enabled.

                decimal currentCommissionRate = currentCommission.Rate;
                if (currentCommissionRate >= downlineCommissionRate)
                {
                    throw new DistributingDomainException("Invalid value of commission's rates" + ". At DistributionService.OrderCreated()");
                }


                /*var shopAgentBalanceId = await _balanceRepository.GetBalanceIdByUserIdAsync(currentCommission.UserId);
                if (shopAgentBalanceId == null)
                {
                    throw new DistributingDomainException("No balance found by given user id. At DistributionService.OrderCreated");
                }*/

                //Shop agent's commission amount is order amount * ( downline's rate - shop agent's rate )
                decimal currentCommissionAmount = order.Amount * (downlineCommissionRate - currentCommissionRate);

                var newOrder = new Distributing.Domain.Model.Distributions.Order(
                    order.TrackingNumber,
                    order.ShopOrderId,
                    order.Amount,
                    currentCommissionAmount,
                    order.ShopId,
                    order.TraderId,
                    order.DateCreated
                    );

                //Create distribution
                var shopAgentDistribution = Distribution.FromCommission(
                    newOrder,
                    currentCommission.UserId,
                    //(int)shopAgentBalanceId,
                    currentCommission.BalanceId,
                    currentCommission.Id,
                    downlineUserId
                    );
                _distributionRepository.Add(shopAgentDistribution);

                //Create Running Account Record VM.
                _runningAccountQueries.Add(
                    await _runningAccountQueries.MapFromOrderInfo(newOrder, currentCommission.UserId));

                downlineCommissionRate = currentCommissionRate;
                downlineUserId = currentCommission.UserId;

                uplineShopAgentCommission = _commissionCacheService.GetCommissionInfoByCommissionId(currentCommission.UplineCommissionId ?? default, rateType);
            }

            #endregion


            #region Store order details to trader

            var traderCommission = _commissionCacheService.GetCommissionInfoByUserId(order.TraderId, rateType);
            if (traderCommission == null)
            {
                throw new DistributingDomainException("No distribution found by given trader id. At DistributionService.OrderCreated");
            }

            /*var traderBalanceId = await _balanceRepository.GetBalanceIdByUserIdAsync(order.TraderId);
            if (traderBalanceId == null)
            {
                throw new DistributingDomainException("No balance found by given trader id. At DistributionService.OrderCreated");
            }*/

            decimal traderCommissionRate = traderCommission.Rate;
            decimal traderDistributedAmount = order.Amount * traderCommissionRate;

            var traderOrder = new Distributing.Domain.Model.Distributions.Order(
                order.TrackingNumber,
                order.ShopOrderId,
                order.Amount,
                traderDistributedAmount,
                order.ShopId,
                order.TraderId,
                order.DateCreated
                );

            var traderDistribution = Distribution.FromCommission(
                traderOrder,
                traderCommission.UserId,
                //(int)traderBalanceId,
                traderCommission.BalanceId,
                traderCommission.Id,
                null
                );

            _distributionRepository.Add(traderDistribution);

            //Create Running Account Record VM.
            _runningAccountQueries.Add(
                await _runningAccountQueries.MapFromOrderInfo(traderOrder, traderCommission.UserId));
            #endregion


            #region Store order details to trader agents

            var uplineTraderAgentCommission = _commissionCacheService.GetCommissionInfoByCommissionId(traderCommission.UplineCommissionId ?? default, rateType);

            downlineCommissionRate = traderCommissionRate;

            //For UI Search Purpose.
            downlineUserId = traderCommission.UserId;

            while (true)
            {
                if (uplineTraderAgentCommission == null)
                {
                    break;
                }

                var currentCommission = uplineTraderAgentCommission;
                //ValidateCommissionStatus(currentCommission); The agent doens't need to be enabled.

                /*var traderAgentBalanceId = await _balanceRepository.GetBalanceIdByUserIdAsync(currentCommission.UserId);
                if (traderAgentBalanceId == null)
                {
                    throw new DistributingDomainException("No balance found by given trader agent id. At DistributionService.OrderCreated");
                }*/

                decimal currentCommissionRate = uplineTraderAgentCommission.Rate;
                if (currentCommissionRate <= downlineCommissionRate)
                {
                    throw new DistributingDomainException("Invalid value of commission's rates" + ". At DistributionService.OrderCreated()");
                }

                decimal currentCommissionAmount = order.Amount * (currentCommissionRate - downlineCommissionRate);

                var newOrder = new Distributing.Domain.Model.Distributions.Order(
                    order.TrackingNumber,
                    order.ShopOrderId,
                    order.Amount,
                    currentCommissionAmount,
                    order.ShopId,
                    order.TraderId,
                    order.DateCreated
                    );

                //Create distribution.
                var traderAgentDistribution = Distribution.FromCommission(
                    newOrder,
                    currentCommission.UserId,
                    //(int)traderAgentBalanceId,
                    currentCommission.BalanceId,
                    currentCommission.Id,
                    downlineUserId
                    );
                _distributionRepository.Add(traderAgentDistribution);

                //Create Running Account Record VM.
                _runningAccountQueries.Add(
                    await _runningAccountQueries.MapFromOrderInfo(newOrder, currentCommission.UserId));

                downlineCommissionRate = currentCommissionRate;
                downlineUserId = currentCommission.UserId;

                uplineTraderAgentCommission = _commissionCacheService.GetCommissionInfoByCommissionId(currentCommission.UplineCommissionId ?? default, rateType);
            }
            #endregion

            return new List<decimal>() { shopDistributedAmount, traderDistributedAmount };
        }

        private async Task AddFailedRunningAccountRecords(Distributing.Domain.Model.Distributions.Order order, RateType rateType)
        {
            #region Store order details to shop
            var shopCommission = _commissionCacheService.GetCommissionInfoByUserId(order.ShopId, rateType);
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
            _runningAccountQueries.Add(
                await _runningAccountQueries.MapFromOrderInfo(shopOrder, shopCommission.UserId));
            #endregion


            #region Store order details to shop agents

            var uplineShopAgentCommission = _commissionCacheService.GetCommissionInfoByCommissionId(shopCommission.UplineCommissionId ?? default, rateType);

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
                _runningAccountQueries.Add(
                    await _runningAccountQueries.MapFromOrderInfo(newOrder, currentCommission.UserId));

                downlineUserId = currentCommission.UserId;

                uplineShopAgentCommission = _commissionCacheService.GetCommissionInfoByCommissionId(currentCommission.UplineCommissionId ?? default, rateType);
            }

            #endregion


            #region Store order details to trader

            var traderCommission = _commissionCacheService.GetCommissionInfoByUserId(order.TraderId, rateType);
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
            _runningAccountQueries.Add(
                await _runningAccountQueries.MapFromOrderInfo(traderOrder, traderCommission.UserId));
            #endregion


            #region Store order details to trader agents

            var uplineTraderAgentCommission = _commissionCacheService.GetCommissionInfoByCommissionId(traderCommission.UplineCommissionId ?? default, rateType);

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
                _runningAccountQueries.Add(
                    await _runningAccountQueries.MapFromOrderInfo(newOrder, currentCommission.UserId));

                downlineUserId = currentCommission.UserId;

                uplineTraderAgentCommission = _commissionCacheService.GetCommissionInfoByCommissionId(currentCommission.UplineCommissionId ?? default, rateType);
            }
            #endregion
        }
    }
}
