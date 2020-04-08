using Distributing.Domain.Events;
using Distributing.Domain.Exceptions;
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
using WebMVC.Applications.Queries.Frozen;
using WebMVC.Applications.Queries.ShopAgents;
using WebMVC.Applications.Queries.Shops;
using WebMVC.Applications.Queries.TraderAgents;
using WebMVC.Applications.Queries.Traders;
using WebMVC.Applications.SideEffectServices;
using WebMVC.Extensions;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.DomainEventHandlers.DistributingDomain
{

    /// <summary>
    /// Update balance entity when unfrozen.
    /// If this is trader balance, update his Qr codes.
    /// Update view model: add new BankbookRecord.
    /// Update view model: update trader agent's balance.
    /// </summary>
    public class UnfrozenDomainEventHandler
            : INotificationHandler<UnfrozenDomainEvent>
    {
        private readonly IBalanceRepository _balanceRepository;
        private readonly IBankbookQueries _bankbookQueries;
        private readonly ITraderAgentQueries _traderAgentQueries;
        private readonly IFrozenQueries _frozenQueries;
        private readonly ITraderQueries _traderQueries;
        private readonly IShopAgentQueries _shopAgentQueries;
        private readonly IShopQueries _shopQueries;

        private readonly IQrCodeSideEffectService _qrCodeSideEffectService;

        public UnfrozenDomainEventHandler(IBalanceRepository balanceRepository, IBankbookQueries bankbookQueries, ITraderAgentQueries traderAgentQueries, IFrozenQueries frozenQueries, ITraderQueries traderQueries, IShopAgentQueries shopAgentQueries, IShopQueries shopQueries, IQrCodeSideEffectService qrCodeSideEffectService)
        {
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _bankbookQueries = bankbookQueries ?? throw new ArgumentNullException(nameof(bankbookQueries));
            _traderAgentQueries = traderAgentQueries ?? throw new ArgumentNullException(nameof(traderAgentQueries));
            _frozenQueries = frozenQueries ?? throw new ArgumentNullException(nameof(frozenQueries));
            _traderQueries = traderQueries ?? throw new ArgumentNullException(nameof(traderQueries));
            _shopAgentQueries = shopAgentQueries ?? throw new ArgumentNullException(nameof(shopAgentQueries));
            _shopQueries = shopQueries ?? throw new ArgumentNullException(nameof(shopQueries));
            _qrCodeSideEffectService = qrCodeSideEffectService ?? throw new ArgumentNullException(nameof(qrCodeSideEffectService));
        }

        public async Task Handle(UnfrozenDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var frozen = domainEvent.Frozen;
            var balance = domainEvent.Balance;

            //Update balance.
            balance.Unfrozen(frozen);

            _balanceRepository.Update(balance);

            await _balanceRepository.UnitOfWork
                .SaveEntitiesAsync(cancellationToken);

            //Update trader qrcodes.
            if (domainEvent.Balance.GetUserType.Id == UserType.Trader.Id)
            {
                await _qrCodeSideEffectService.UpdateQrCodeWhenTraderBalanceUpdated(
                    balance.UserId,
                    balance.AmountAvailable);
            }

            //Add bankbook record record.
            decimal balanceBefore = frozen.BalanceUnfrozenRecord.BalanceBefore;
            decimal balanceAfter = (decimal)frozen.BalanceUnfrozenRecord.BalanceAfter;
            decimal amountChange = balanceAfter - balanceBefore;

            string recordType = string.Empty;
            string trackingId = string.Empty;
            string dateOccurred = ((DateTime)frozen.DateUnfroze).ToFullString();


            if (frozen.FrozenType.Id == FrozenType.Withdrawal.Id)
            {
                recordType = BankbookRecordType.CancelWithdrawal;
                trackingId = frozen.WithdrawalId.ToString();
            }
            else if (frozen.FrozenType.Id == FrozenType.Order.Id)
            {
                recordType = BankbookRecordType.CancelOrder;
                trackingId = frozen.OrderTrackingNumber;
            }
            else if (frozen.FrozenType.Id == FrozenType.ByAdmin.Id)
            {
                recordType = BankbookRecordType.UnfreezeByAdmin;
                trackingId = frozen.Id.ToString();
            }

            var bankBookRecord = new BankbookRecord
            {
                UserId = frozen.UserId,
                BalanceId = frozen.BalanceId,
                DateOccurred = dateOccurred,
                Type = recordType,
                BalanceBefore = balanceBefore,
                AmountChanged = amountChange,
                BalanceAfter = balanceAfter,
                TrackingId = trackingId,
                Description = frozen.Description
            };

            _bankbookQueries.Add(bankBookRecord);



            if (domainEvent.Balance.GetUserType.Id == UserType.TraderAgent.Id)
            {
                //Update trader agent.
                var traderAgentVM = await _traderAgentQueries.GetTraderAgent(balance.UserId);
                if (traderAgentVM != null)
                {
                    var amountFrozen =  _frozenQueries.GetUserCurrentFrozenAmountAsync(traderAgentVM.TraderAgentId);
                    traderAgentVM.Balance.AmountFrozen = amountFrozen;
                    traderAgentVM.Balance.AmountAvailable = balance.AmountAvailable;
                    _traderAgentQueries.Update(traderAgentVM);

                    //await _traderAgentQueries.SaveChangesAsync();
                }
            }
            else if (domainEvent.Balance.GetUserType.Id == UserType.Trader.Id)
            {
                //Update trader.
                var traderVM = await _traderQueries.GetTrader(balance.UserId);
                if (traderVM != null)
                {
                    var amountFrozen =  _frozenQueries.GetUserCurrentFrozenAmountAsync(traderVM.TraderId);
                    traderVM.Balance.AmountFrozen = amountFrozen;
                    traderVM.Balance.AmountAvailable = balance.AmountAvailable;
                    _traderQueries.Update(traderVM);

                    //await _traderQueries.SaveChangesAsync();
                }
            }
            else if (domainEvent.Balance.GetUserType.Id == UserType.ShopAgent.Id)
            {
                //Update shop agent.
                var shopAgentVm = await _shopAgentQueries.GetShopAgent(balance.UserId);
                if (shopAgentVm != null)
                {
                    var amountFrozen =  _frozenQueries.GetUserCurrentFrozenAmountAsync(shopAgentVm.ShopAgentId);
                    shopAgentVm.Balance.AmountFrozen = amountFrozen;
                    shopAgentVm.Balance.AmountAvailable = balance.AmountAvailable;
                    _shopAgentQueries.Update(shopAgentVm);

                    //await _shopAgentQueries.SaveChangesAsync();
                }
            }
            else if (domainEvent.Balance.GetUserType.Id == UserType.Shop.Id)
            {
                //Update shop agent.
                var shopVm = await _shopQueries.GetShop(balance.UserId);
                if (shopVm != null)
                {
                    var amountFrozen =  _frozenQueries.GetUserCurrentFrozenAmountAsync(shopVm.ShopId);
                    shopVm.Balance.AmountFrozen = amountFrozen;
                    shopVm.Balance.AmountAvailable = balance.AmountAvailable;
                    _shopQueries.Update(shopVm);

                    //await _shopQueries.SaveChangesAsync();
                }
            }

            //Only need to save once.
            await _bankbookQueries.SaveChangesAsync();

        }
    }
}
