using Distributing.Domain.Events;
using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Frozens;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Bankbook;
using WebMVC.Applications.Queries.ShopAgents;
using WebMVC.Applications.Queries.Shops;
using WebMVC.Applications.Queries.TraderAgents;
using WebMVC.Applications.Queries.Traders;
using WebMVC.Extensions;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.DomainEventHandlers.DistributingDomain
{
    /// <summary>
    /// Update balance when transfer created.
    /// Update view model: add new BalanceRecord.
    /// Update view model: update balance.
    /// </summary>
    public class TransferCreatedDomainEventHandler
            : INotificationHandler<TransferCreatedDomainEvent>
    {
        private readonly IBalanceRepository _balanceRepository;
        private readonly IBankbookQueries _bankbookQueries;
        private readonly ITraderAgentQueries _traderAgentQueries;
        private readonly ITraderQueries _traderQueries;
        private readonly IShopAgentQueries _shopAgentQueries;
        private readonly IShopQueries _shopQueries;

        public TransferCreatedDomainEventHandler(IBalanceRepository balanceRepository, IBankbookQueries bankbookQueries, ITraderAgentQueries traderAgentQueries, ITraderQueries traderQueries, IShopAgentQueries shopAgentQueries, IShopQueries shopQueries)
        {
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _bankbookQueries = bankbookQueries ?? throw new ArgumentNullException(nameof(bankbookQueries));
            _traderAgentQueries = traderAgentQueries ?? throw new ArgumentNullException(nameof(traderAgentQueries));
            _traderQueries = traderQueries ?? throw new ArgumentNullException(nameof(traderQueries));
            _shopAgentQueries = shopAgentQueries ?? throw new ArgumentNullException(nameof(shopAgentQueries));
            _shopQueries = shopQueries ?? throw new ArgumentNullException(nameof(shopQueries));
        }

        public async Task Handle(TransferCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var transfer = domainEvent.Transfer;

            //Update balance.
            var fromBalance = await _balanceRepository.GetByBalanceIdAsync(domainEvent.FromBalanceId);
            var toBalance = await _balanceRepository.GetByBalanceIdAsync(domainEvent.ToBalanceId);

            decimal fromBalanceBefore = fromBalance.AmountAvailable;
            fromBalance.TransferOut(transfer);
            decimal fromBalanceAfter = fromBalance.AmountAvailable;

            decimal toBalanceBefore = toBalance.AmountAvailable;
            toBalance.TransferIn(transfer);
            decimal toBalanceAfter = toBalance.AmountAvailable;

            _balanceRepository.Update(fromBalance);
            _balanceRepository.Update(toBalance);
            await _balanceRepository.UnitOfWork.SaveEntitiesAsync();


            //Update bankbook record.
            var bankBookRecordFrom = new BankbookRecord
            {
                UserId = fromBalance.UserId,
                BalanceId = fromBalance.Id,
                DateOccurred = transfer.DateTransferred.ToFullString(),
                Type = BankbookRecordType.TransferOut,
                BalanceBefore = fromBalanceBefore,
                AmountChanged = fromBalanceAfter - fromBalanceBefore,
                BalanceAfter = fromBalanceAfter,
                TrackingId = transfer.Id.ToString(),
                Description = null
            };

            _bankbookQueries.Add(bankBookRecordFrom);

            var bankBookRecordTo = new BankbookRecord
            {
                UserId = toBalance.UserId,
                BalanceId = toBalance.Id,
                DateOccurred = transfer.DateTransferred.ToFullString(),
                Type = BankbookRecordType.TransferIn,
                BalanceBefore = toBalanceBefore,
                AmountChanged = toBalanceAfter - toBalanceBefore,
                BalanceAfter = toBalanceAfter,
                TrackingId = transfer.Id.ToString(),
                Description = null
            };

            _bankbookQueries.Add(bankBookRecordTo);

            await _bankbookQueries.SaveChangesAsync();


            //Update trader agent.





            if (fromBalance.GetUserType.Id == UserType.TraderAgent.Id)
            {
                var traderAgentVMFrom = await _traderAgentQueries.GetTraderAgent(fromBalance.UserId);
                if (traderAgentVMFrom != null)
                {
                    traderAgentVMFrom.Balance.AmountAvailable = fromBalance.AmountAvailable;
                    _traderAgentQueries.Update(traderAgentVMFrom);
                    await _traderAgentQueries.SaveChangesAsync();
                }
            }
            else if (fromBalance.GetUserType.Id == UserType.Trader.Id)
            {
                var traderVm = await _traderQueries.GetTrader(fromBalance.UserId);

                if (traderVm != null)
                {
                    traderVm.Balance.AmountAvailable = fromBalance.AmountAvailable;

                    _traderQueries.Update(traderVm);

                    await _traderQueries.SaveChangesAsync();
                }
            }
            else if (fromBalance.GetUserType.Id == UserType.ShopAgent.Id)
            {
                var shopAgentVm = await _shopAgentQueries.GetShopAgent(fromBalance.UserId);

                if (shopAgentVm != null)
                {
                    shopAgentVm.Balance.AmountAvailable = fromBalance.AmountAvailable;

                    _shopAgentQueries.Update(shopAgentVm);

                    await _shopAgentQueries.SaveChangesAsync();
                }
            }
            else if (fromBalance.GetUserType.Id == UserType.Shop.Id)
            {
                var shopVm = await _shopQueries.GetShop(fromBalance.UserId);

                if (shopVm != null)
                {
                    shopVm.Balance.AmountAvailable = fromBalance.AmountAvailable;

                    _shopQueries.Update(shopVm);

                    await _shopQueries.SaveChangesAsync();
                }
            }

            if (toBalance.GetUserType.Id == UserType.TraderAgent.Id)
            {
                var traderAgentVMTo = await _traderAgentQueries.GetTraderAgent(toBalance.UserId);
                if (traderAgentVMTo != null)
                {
                    traderAgentVMTo.Balance.AmountAvailable = toBalance.AmountAvailable;
                    _traderAgentQueries.Update(traderAgentVMTo);
                }
            }
            else if (toBalance.GetUserType.Id == UserType.Trader.Id)
            {
                var traderVm = await _traderQueries.GetTrader(toBalance.UserId);

                if (traderVm != null)
                {
                    traderVm.Balance.AmountAvailable = toBalance.AmountAvailable;

                    _traderQueries.Update(traderVm);

                    await _traderQueries.SaveChangesAsync();
                }
            }
            else if (toBalance.GetUserType.Id == UserType.ShopAgent.Id)
            {
                var shopAgentVm = await _shopAgentQueries.GetShopAgent(toBalance.UserId);

                if (shopAgentVm != null)
                {
                    shopAgentVm.Balance.AmountAvailable = toBalance.AmountAvailable;

                    _shopAgentQueries.Update(shopAgentVm);

                    await _shopAgentQueries.SaveChangesAsync();
                }
            }
            else if (toBalance.GetUserType.Id == UserType.Shop.Id)
            {
                var shopVm = await _shopQueries.GetShop(toBalance.UserId);

                if (shopVm != null)
                {
                    shopVm.Balance.AmountAvailable = toBalance.AmountAvailable;

                    _shopQueries.Update(shopVm);

                    await _shopQueries.SaveChangesAsync();
                }
            }
        }
    }
}
