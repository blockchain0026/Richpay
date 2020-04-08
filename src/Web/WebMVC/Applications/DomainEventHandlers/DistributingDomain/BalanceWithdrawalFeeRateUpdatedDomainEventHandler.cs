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
    /// Update view model: update trader agent's rate.
    /// </summary>
    public class BalanceWithdrawalFeeRateUpdatedDomainEventHandler
            : INotificationHandler<BalanceWithdrawalFeeRateUpdatedDomainEvent>
    {
        private readonly ITraderAgentQueries _traderAgentQueries;
        private readonly ITraderQueries _traderQueries;
        private readonly IShopAgentQueries _shopAgentQueries;
        private readonly IShopQueries _shopQueries;

        public BalanceWithdrawalFeeRateUpdatedDomainEventHandler(ITraderAgentQueries traderAgentQueries, ITraderQueries traderQueries, IShopAgentQueries shopAgentQueries, IShopQueries shopQueries)
        {
            _traderAgentQueries = traderAgentQueries ?? throw new ArgumentNullException(nameof(traderAgentQueries));
            _traderQueries = traderQueries ?? throw new ArgumentNullException(nameof(traderQueries));
            _shopAgentQueries = shopAgentQueries ?? throw new ArgumentNullException(nameof(shopAgentQueries));
            _shopQueries = shopQueries ?? throw new ArgumentNullException(nameof(shopQueries));
        }

        public async Task Handle(BalanceWithdrawalFeeRateUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            //Update balance.
            if (domainEvent.Balance.GetUserType.Id == UserType.TraderAgent.Id)
            {
                var traderAgentVM = await _traderAgentQueries.GetTraderAgent(domainEvent.Balance.UserId);
                if (traderAgentVM != null)
                {
                    traderAgentVM.Balance.WithdrawalCommissionRateInThousandth = (int)(domainEvent.WithdrawalCommissionRate * 1000);
                    _traderAgentQueries.Update(traderAgentVM);

                    await _traderAgentQueries.SaveChangesAsync();
                }
            }
            else if (domainEvent.Balance.GetUserType.Id == UserType.Trader.Id)
            {
                var traderVm = await _traderQueries.GetTrader(domainEvent.Balance.UserId);

                if (traderVm != null)
                {
                    traderVm.Balance.WithdrawalCommissionRateInThousandth = (int)(domainEvent.WithdrawalCommissionRate * 1000);
                    _traderQueries.Update(traderVm);

                    await _traderQueries.SaveChangesAsync();
                }
            }
            else if (domainEvent.Balance.GetUserType.Id == UserType.ShopAgent.Id)
            {
                var shopAgentVm = await _shopAgentQueries.GetShopAgent(domainEvent.Balance.UserId);

                if (shopAgentVm != null)
                {
                    shopAgentVm.Balance.WithdrawalCommissionRateInThousandth = (int)(domainEvent.WithdrawalCommissionRate * 1000);
                    _shopAgentQueries.Update(shopAgentVm);

                    await _shopAgentQueries.SaveChangesAsync();
                }
            }
            else if (domainEvent.Balance.GetUserType.Id == UserType.Shop.Id)
            {
                var shopVm = await _shopQueries.GetShop(domainEvent.Balance.UserId);

                if (shopVm != null)
                {
                    shopVm.Balance.WithdrawalCommissionRateInThousandth = (int)(domainEvent.WithdrawalCommissionRate * 1000);
                    _shopQueries.Update(shopVm);

                    await _shopQueries.SaveChangesAsync();
                }
            }
        }
    }
}
