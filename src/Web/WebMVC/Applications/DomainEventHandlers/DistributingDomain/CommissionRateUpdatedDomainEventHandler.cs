using Distributing.Domain.Events;
using Distributing.Domain.Exceptions;
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
    /// Check downline's commission when commission rate updated.
    /// Update view model: update trader user's rate.
    /// </summary>
    public class CommissionRateUpdatedDomainEventHandler
                    : INotificationHandler<CommissionRateUpdatedDomainEvent>
    {
        private readonly ICommissionRepository _commissionRepository;
        private readonly ITraderAgentQueries _traderAgentQueries;
        private readonly ITraderQueries _traderQueries;
        private readonly IShopAgentQueries _shopAgentQueries;

        public CommissionRateUpdatedDomainEventHandler(ICommissionRepository commissionRepository, ITraderAgentQueries traderAgentQueries, ITraderQueries traderQueries, IShopAgentQueries shopAgentQueries)
        {
            _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
            _traderAgentQueries = traderAgentQueries ?? throw new ArgumentNullException(nameof(traderAgentQueries));
            _traderQueries = traderQueries ?? throw new ArgumentNullException(nameof(traderQueries));
            _shopAgentQueries = shopAgentQueries ?? throw new ArgumentNullException(nameof(shopAgentQueries));
        }

        public async Task Handle(CommissionRateUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            //Check the downline's fee is less than upline.
            var downlines = await _commissionRepository.GetDownlinesAsnyc(domainEvent.Commission.Id);

            foreach (var downline in downlines)
            {
                bool forceChanged = false;
                decimal newRateAlipay = downline.RateAlipay;
                decimal newRateWechat = downline.RateWechat;

                //If downline's rate is larger than or equal to this commission's rate, then lower it.
                if (downline.RateAlipay >= domainEvent.RateAlipay)
                {
                    throw new DistributingDomainException("用户费率必须大于下级用户的费率。");
                    //Unless the downline's rate is 0, lower the rate. 
                    if (downline.RateAlipay != 0)
                    {
                        if (domainEvent.RateAlipay == 0)
                        {
                            newRateAlipay = 0;
                            forceChanged = true;
                        }
                        else
                        {
                            newRateAlipay = domainEvent.RateAlipay - 0.001M;
                            forceChanged = true;
                        }
                    }
                }
                if (downline.RateWechat >= domainEvent.RateWechat)
                {
                    throw new DistributingDomainException("用户费率必须大于下级用户的费率。");

                    if (downline.RateWechat != 0)
                    {
                        if (domainEvent.RateWechat == 0)
                        {
                            newRateWechat = 0;
                            forceChanged = true;
                        }
                        else
                        {
                            newRateWechat = domainEvent.RateWechat - 0.001M;
                            forceChanged = true;
                        }
                    }
                }

                if (forceChanged)
                {
                    downline.UpdateRate(newRateAlipay, newRateWechat, domainEvent.Commission);
                    _commissionRepository.Update(downline);
                    await _commissionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                }
            }


            //Update trader agent.
            if (domainEvent.Commission.GetUserType.Id == UserType.TraderAgent.Id)
            {
                var traderAgentVM = await _traderAgentQueries.GetTraderAgent(domainEvent.Commission.UserId);
                if (traderAgentVM != null)
                {
                    traderAgentVM.TradingCommission.RateAlipayInThousandth = (int)(domainEvent.Commission.RateAlipay * 1000);
                    traderAgentVM.TradingCommission.RateWechatInThousandth = (int)(domainEvent.Commission.RateWechat * 1000);
                   
                    _traderAgentQueries.Update(traderAgentVM);

                    await _traderAgentQueries.SaveChangesAsync();
                }
            }
            else if (domainEvent.Commission.GetUserType.Id == UserType.Trader.Id)
            {
                var traderVm = await _traderQueries.GetTrader(domainEvent.Commission.UserId);

                if (traderVm != null)
                {
                    traderVm.TradingCommission.RateAlipayInThousandth = (int)(domainEvent.Commission.RateAlipay * 1000);
                    traderVm.TradingCommission.RateWechatInThousandth = (int)(domainEvent.Commission.RateWechat * 1000);
                    _traderQueries.Update(traderVm);
                    await _traderQueries.SaveChangesAsync();
                }
            }
        }
    }
}
