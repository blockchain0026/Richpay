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

namespace WebMVC.Applications.DomainEventHandlers.DistributingDomain
{
    /// <summary>
    /// Check downline's commission when commission rate updated.
    /// Update view model: update shop user's rate.
    /// </summary>
    public class CommissionRebateRateUpdatedDomainEventHandler
                : INotificationHandler<CommissionRebateRateUpdatedDomainEvent>
    {
        private readonly ICommissionRepository _commissionRepository;
        private readonly IShopAgentQueries _shopAgentQueries;
        private readonly IShopQueries _shopQueries;

        public CommissionRebateRateUpdatedDomainEventHandler(ICommissionRepository commissionRepository, IShopAgentQueries shopAgentQueries, IShopQueries shopQueries)
        {
            _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
            _shopAgentQueries = shopAgentQueries ?? throw new ArgumentNullException(nameof(shopAgentQueries));
            _shopQueries = shopQueries ?? throw new ArgumentNullException(nameof(shopQueries));
        }

        public async Task Handle(CommissionRebateRateUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            //Check the downline's fee.
            var downlines = await _commissionRepository.GetDownlinesAsnyc(domainEvent.Commission.Id);

            if (domainEvent.Commission.GetUserType.Id == UserType.Trader.Id 
                || domainEvent.Commission.GetUserType.Id == UserType.TraderAgent.Id)
            {
                foreach (var downline in downlines)
                {
                    bool forceChanged = false;
                    decimal newRateRebateAlipay = downline.RateRebateAlipay;
                    decimal newRateRebateWechat = downline.RateRebateWechat;

                    //If downline's rate is larger than or equal to this commission's rate, then lower it.
                    if (downline.RateAlipay >= domainEvent.RateRebateAlipay)
                    {
                        //Unless the downline's rate is 0, lower the rate. 
                        if (downline.RateRebateAlipay != 0)
                        {
                            if (domainEvent.RateRebateAlipay == 0)
                            {
                                newRateRebateAlipay = 0;
                                forceChanged = true;
                            }
                            else
                            {
                                newRateRebateAlipay = domainEvent.RateRebateAlipay - 0.001M;
                                forceChanged = true;
                            }
                        }
                    }
                    if (downline.RateWechat >= domainEvent.RateRebateWechat)
                    {
                        if (downline.RateRebateWechat != 0)
                        {
                            if (domainEvent.RateRebateWechat == 0)
                            {
                                newRateRebateWechat = 0;
                                forceChanged = true;
                            }
                            else
                            {
                                newRateRebateWechat = domainEvent.RateRebateWechat - 0.001M;
                                forceChanged = true;
                            }
                        }
                    }

                    if (forceChanged)
                    {
                        downline.UpdateRebateRate(newRateRebateAlipay, newRateRebateWechat, domainEvent.Commission);
                        _commissionRepository.Update(downline);
                        await _commissionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                    }
                }
            }
            else if (domainEvent.Commission.GetUserType.Id == UserType.Shop.Id 
                || domainEvent.Commission.GetUserType.Id == UserType.ShopAgent.Id)
            {
                foreach (var downline in downlines)
                {
                    bool forceChanged = false;
                    decimal newRateRebateAlipay = downline.RateRebateAlipay;
                    decimal newRateRebateWechat = downline.RateRebateWechat;

                    //If downline's rate is less than or equal to this commission's rate, then upper it.
                    if (downline.RateAlipay <= domainEvent.RateRebateAlipay)
                    {
                        //Unless the downline's rate is 0, upper the rate. 
                        if (downline.RateRebateAlipay != 0)
                        {
                            if (domainEvent.RateRebateAlipay == 0)
                            {
                                newRateRebateAlipay = 0;
                                forceChanged = true;
                            }
                            else
                            {
                                newRateRebateAlipay = domainEvent.RateRebateAlipay + 0.001M;
                                forceChanged = true;
                            }
                        }
                    }
                    if (downline.RateWechat <= domainEvent.RateRebateWechat)
                    {
                        if (downline.RateRebateWechat != 0)
                        {
                            if (domainEvent.RateRebateWechat == 0)
                            {
                                newRateRebateWechat = 0;
                                forceChanged = true;
                            }
                            else
                            {
                                newRateRebateWechat = domainEvent.RateRebateWechat + 0.001M;
                                forceChanged = true;
                            }
                        }
                    }

                    if (forceChanged)
                    {
                        downline.UpdateRebateRate(newRateRebateAlipay, newRateRebateWechat, domainEvent.Commission);
                        _commissionRepository.Update(downline);
                        await _commissionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                    }
                }
            }



            //Update shop user.
            if (domainEvent.Commission.GetUserType.Id == UserType.ShopAgent.Id)
            {
                var shopAgentVm = await _shopAgentQueries.GetShopAgent(domainEvent.Commission.UserId);
                if (shopAgentVm != null)
                {
                    shopAgentVm.RebateCommission.RateRebateAlipayInThousandth = (int)(domainEvent.Commission.RateRebateAlipay * 1000);
                    shopAgentVm.RebateCommission.RateRebateWechatInThousandth = (int)(domainEvent.Commission.RateRebateWechat * 1000);
                    _shopAgentQueries.Update(shopAgentVm);
                    await _shopAgentQueries.SaveChangesAsync();
                }
            }
            else if (domainEvent.Commission.GetUserType.Id == UserType.Shop.Id)
            {
                var shopVm = await _shopQueries.GetShop(domainEvent.Commission.UserId);
                if (shopVm != null)
                {
                    shopVm.RebateCommission.RateRebateAlipayInThousandth = (int)(domainEvent.Commission.RateRebateAlipay * 1000);
                    shopVm.RebateCommission.RateRebateWechatInThousandth = (int)(domainEvent.Commission.RateRebateWechat * 1000);
                    _shopQueries.Update(shopVm);
                    await _shopQueries.SaveChangesAsync();
                }
            }
        }
    }
}
