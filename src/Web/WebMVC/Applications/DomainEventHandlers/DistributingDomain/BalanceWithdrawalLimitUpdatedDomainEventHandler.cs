using Distributing.Domain.Events;
using Distributing.Domain.Model.Commissions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.ShopAgents;
using WebMVC.Applications.Queries.Shops;
using WebMVC.Applications.Queries.TraderAgents;
using WebMVC.Applications.Queries.Traders;

namespace WebMVC.Applications.DomainEventHandlers.DistributingDomain
{

    /// <summary>
    /// Update view model when balance created.
    /// </summary>
    public class BalanceWithdrawalLimitUpdatedDomainEventHandler
         : INotificationHandler<BalanceWithdrawalLimitUpdatedDomainEvent>
    {
        private readonly ITraderAgentQueries _traderAgentQueries;
        private readonly ITraderQueries _traderQueries;
        private readonly IShopAgentQueries _shopAgentQueries;
        private readonly IShopQueries _shopQueries;

        public BalanceWithdrawalLimitUpdatedDomainEventHandler(ITraderAgentQueries traderAgentQueries, ITraderQueries traderQueries, IShopAgentQueries shopAgentQueries, IShopQueries shopQueries)
        {
            _traderAgentQueries = traderAgentQueries ?? throw new ArgumentNullException(nameof(traderAgentQueries));
            _traderQueries = traderQueries ?? throw new ArgumentNullException(nameof(traderQueries));
            _shopAgentQueries = shopAgentQueries ?? throw new ArgumentNullException(nameof(shopAgentQueries));
            _shopQueries = shopQueries ?? throw new ArgumentNullException(nameof(shopQueries));
        }

        public async Task Handle(BalanceWithdrawalLimitUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var balance = domainEvent.Balance;
            if (balance.GetUserType.Id == UserType.TraderAgent.Id)
            {
                var traderAgentVm = await _traderAgentQueries.GetTraderAgent(balance.UserId);

                if (traderAgentVm != null)
                {
                    traderAgentVm.Balance.WithdrawalLimit.DailyAmountLimit = (int)balance.WithdrawalLimit.DailyAmountLimit;
                    traderAgentVm.Balance.WithdrawalLimit.DailyFrequencyLimit = (int)balance.WithdrawalLimit.DailyFrequencyLimit;
                    traderAgentVm.Balance.WithdrawalLimit.EachAmountUpperLimit = (int)balance.WithdrawalLimit.EachAmountUpperLimit;
                    traderAgentVm.Balance.WithdrawalLimit.EachAmountLowerLimit = (int)balance.WithdrawalLimit.EachAmountLowerLimit;
                }

                _traderAgentQueries.Update(traderAgentVm);

                await _traderAgentQueries.SaveChangesAsync();
            }
            else if (balance.GetUserType.Id == UserType.Trader.Id)
            {
                var traderVm = await _traderQueries.GetTrader(balance.UserId);

                if (traderVm != null)
                {
                    traderVm.Balance.WithdrawalLimit.DailyAmountLimit = (int)balance.WithdrawalLimit.DailyAmountLimit;
                    traderVm.Balance.WithdrawalLimit.DailyFrequencyLimit = (int)balance.WithdrawalLimit.DailyFrequencyLimit;
                    traderVm.Balance.WithdrawalLimit.EachAmountUpperLimit = (int)balance.WithdrawalLimit.EachAmountUpperLimit;
                    traderVm.Balance.WithdrawalLimit.EachAmountLowerLimit = (int)balance.WithdrawalLimit.EachAmountLowerLimit;

                    _traderQueries.Update(traderVm);
                    await _traderQueries.SaveChangesAsync();
                }
            }
            else if (balance.GetUserType.Id == UserType.ShopAgent.Id)
            {
                var shopAgentVm = await _shopAgentQueries.GetShopAgent(balance.UserId);

                if (shopAgentVm != null)
                {
                    shopAgentVm.Balance.WithdrawalLimit.DailyAmountLimit = (int)balance.WithdrawalLimit.DailyAmountLimit;
                    shopAgentVm.Balance.WithdrawalLimit.DailyFrequencyLimit = (int)balance.WithdrawalLimit.DailyFrequencyLimit;
                    shopAgentVm.Balance.WithdrawalLimit.EachAmountUpperLimit = (int)balance.WithdrawalLimit.EachAmountUpperLimit;
                    shopAgentVm.Balance.WithdrawalLimit.EachAmountLowerLimit = (int)balance.WithdrawalLimit.EachAmountLowerLimit;

                    _shopAgentQueries.Update(shopAgentVm);
                    await _shopAgentQueries.SaveChangesAsync();
                }
            }
            else if (balance.GetUserType.Id == UserType.Shop.Id)
            {
                var shopVm = await _shopQueries.GetShop(balance.UserId);

                if (shopVm != null)
                {
                    shopVm.Balance.WithdrawalLimit.DailyAmountLimit = (int)balance.WithdrawalLimit.DailyAmountLimit;
                    shopVm.Balance.WithdrawalLimit.DailyFrequencyLimit = (int)balance.WithdrawalLimit.DailyFrequencyLimit;
                    shopVm.Balance.WithdrawalLimit.EachAmountUpperLimit = (int)balance.WithdrawalLimit.EachAmountUpperLimit;
                    shopVm.Balance.WithdrawalLimit.EachAmountLowerLimit = (int)balance.WithdrawalLimit.EachAmountLowerLimit;

                    _shopQueries.Update(shopVm);
                    await _shopQueries.SaveChangesAsync();
                }
            }
        }
    }
}
