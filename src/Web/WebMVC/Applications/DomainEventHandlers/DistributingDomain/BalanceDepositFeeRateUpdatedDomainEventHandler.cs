using Distributing.Domain.Events;
using Distributing.Domain.Model.Commissions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.ShopAgents;
using WebMVC.Applications.Queries.TraderAgents;
using WebMVC.Applications.Queries.Traders;

namespace WebMVC.Applications.DomainEventHandlers.DistributingDomain
{
    /// <summary>
    /// Update view model: update trader agent's rate.
    /// </summary>
    public class BalanceDepositFeeRateUpdatedDomainEventHandler
            : INotificationHandler<BalanceDepositFeeRateUpdatedDomainEvent>
    {
        private readonly ITraderAgentQueries _traderAgentQueries;
        private readonly ITraderQueries _traderQueries;

        //private readonly IShopAgentQueries _shopAgentQueries;

        public BalanceDepositFeeRateUpdatedDomainEventHandler(ITraderAgentQueries traderAgentQueries, ITraderQueries traderQueries)
        {
            _traderAgentQueries = traderAgentQueries ?? throw new ArgumentNullException(nameof(traderAgentQueries));
            _traderQueries = traderQueries ?? throw new ArgumentNullException(nameof(traderQueries));
        }


        public async Task Handle(BalanceDepositFeeRateUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            //Update trader user.
            if (domainEvent.Balance.GetUserType.Id == UserType.TraderAgent.Id)
            {
                var traderAgentVM = await _traderAgentQueries.GetTraderAgent(domainEvent.Balance.UserId);
                if (traderAgentVM != null)
                {
                    traderAgentVM.Balance.DepositCommissionRateInThousandth = (int)(domainEvent.DepositCommissionRate * 1000);
                    _traderAgentQueries.Update(traderAgentVM);

                    await _traderAgentQueries.SaveChangesAsync();
                }
            }
            else if (domainEvent.Balance.GetUserType.Id == UserType.Trader.Id)
            {
                var traderVm = await _traderQueries.GetTrader(domainEvent.Balance.UserId);

                if (traderVm != null)
                {
                    traderVm.Balance.DepositCommissionRateInThousandth = (int)(domainEvent.Balance.DepositCommissionRate * 1000);
                    _traderQueries.Update(traderVm);

                    await _traderQueries.SaveChangesAsync();
                }
            }

        }
    }
}
