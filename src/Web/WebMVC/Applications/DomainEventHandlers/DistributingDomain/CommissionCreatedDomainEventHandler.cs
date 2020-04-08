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
    /// Update view model when commission created.
    /// </summary>
    public class CommissionCreatedDomainEventHandler
         : INotificationHandler<CommissionCreatedDomainEvent>
    {
        private readonly ITraderAgentQueries _traderAgentQueries;
        private readonly ITraderQueries _traderQueries;
        private readonly IShopAgentQueries _shopAgentQueries;
        private readonly IShopQueries _shopQueries;

        public CommissionCreatedDomainEventHandler(ITraderAgentQueries traderAgentQueries, ITraderQueries traderQueries, IShopAgentQueries shopAgentQueries, IShopQueries shopQueries)
        {
            _traderAgentQueries = traderAgentQueries ?? throw new ArgumentNullException(nameof(traderAgentQueries));
            _traderQueries = traderQueries ?? throw new ArgumentNullException(nameof(traderQueries));
            _shopAgentQueries = shopAgentQueries ?? throw new ArgumentNullException(nameof(shopAgentQueries));
            _shopQueries = shopQueries ?? throw new ArgumentNullException(nameof(shopQueries));
        }

        public async Task Handle(CommissionCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var commission = domainEvent.Commission;
            if (commission.GetUserType.Id == UserType.TraderAgent.Id)
            {
                var traderAgentVm = await _traderAgentQueries.GetTraderAgent(domainEvent.UserId);

                if (traderAgentVm != null)
                {
                    traderAgentVm.TradingCommission.RateAlipayInThousandth = (int)(commission.RateAlipay * 1000);
                    traderAgentVm.TradingCommission.RateWechatInThousandth = (int)(commission.RateWechat * 1000);
                }

                _traderAgentQueries.Update(traderAgentVm);

                await _traderAgentQueries.SaveChangesAsync();
            }
            else if (commission.GetUserType.Id == UserType.Trader.Id)
            {
                var traderVm = await _traderQueries.GetTrader(domainEvent.UserId);

                if (traderVm != null)
                {
                    traderVm.TradingCommission.RateAlipayInThousandth = (int)(commission.RateAlipay * 1000);
                    traderVm.TradingCommission.RateWechatInThousandth = (int)(commission.RateWechat * 1000);
                }

                _traderQueries.Update(traderVm);

                await _traderQueries.SaveChangesAsync();
            }
            else if (commission.GetUserType.Id == UserType.ShopAgent.Id)
            {
                var shopAgentVm = await _shopAgentQueries.GetShopAgent(domainEvent.UserId);

                if (shopAgentVm != null)
                {
                    shopAgentVm.RebateCommission.RateRebateAlipayInThousandth = (int)(commission.RateRebateAlipay * 1000);
                    shopAgentVm.RebateCommission.RateRebateWechatInThousandth = (int)(commission.RateRebateWechat * 1000);
                }

                _shopAgentQueries.Update(shopAgentVm);

                await _shopAgentQueries.SaveChangesAsync();
            }
            else if (commission.GetUserType.Id == UserType.Shop.Id)
            {
                var shopVm = await _shopQueries.GetShop(domainEvent.UserId);

                if (shopVm != null)
                {
                    shopVm.RebateCommission.RateRebateAlipayInThousandth = (int)(commission.RateRebateAlipay * 1000);
                    shopVm.RebateCommission.RateRebateWechatInThousandth = (int)(commission.RateRebateWechat * 1000);
                }

                _shopQueries.Update(shopVm);

                await _shopQueries.SaveChangesAsync();
            }
        }
    }
}
