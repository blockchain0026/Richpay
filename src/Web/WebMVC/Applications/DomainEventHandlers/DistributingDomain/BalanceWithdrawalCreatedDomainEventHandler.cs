using Distributing.Domain.Events;
using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Frozens;
using Distributing.Domain.Model.Shared;
using Distributing.Domain.Model.Withdrawals;
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
    /// Create frozen and withdrawal entitys when balance withdrawal created.
    /// If this is trader balance, update his Qr codes.
    /// Update view model: add new Bankbook Record.
    /// Update view model: update balance.
    /// </summary>
    public class BalanceWithdrawalCreatedDomainEventHandler
                    : INotificationHandler<BalanceWithdrawalCreatedDomainEvent>
    {
        private readonly IWithdrawalRepository _withdrawalRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IFrozenRepository _frozenRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly IBankbookQueries _bankbookQueries;
        private readonly ITraderAgentQueries _traderAgentQueries;
        private readonly IFrozenQueries _frozenQueries;
        private readonly ITraderQueries _traderQueries;
        private readonly IShopAgentQueries _shopAgentQueries;
        private readonly IShopQueries _shopQueries;
        private readonly IQrCodeSideEffectService _qrCodeSideEffectService;

        public BalanceWithdrawalCreatedDomainEventHandler(IWithdrawalRepository withdrawalRepository, IBalanceRepository balanceRepository, IFrozenRepository frozenRepository, IDateTimeService dateTimeService, IBankbookQueries bankbookQueries, ITraderAgentQueries traderAgentQueries, IFrozenQueries frozenQueries, ITraderQueries traderQueries, IShopAgentQueries shopAgentQueries, IShopQueries shopQueries, IQrCodeSideEffectService qrCodeSideEffectService)
        {
            _withdrawalRepository = withdrawalRepository ?? throw new ArgumentNullException(nameof(withdrawalRepository));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _frozenRepository = frozenRepository ?? throw new ArgumentNullException(nameof(frozenRepository));
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            _bankbookQueries = bankbookQueries ?? throw new ArgumentNullException(nameof(bankbookQueries));
            _traderAgentQueries = traderAgentQueries ?? throw new ArgumentNullException(nameof(traderAgentQueries));
            _frozenQueries = frozenQueries ?? throw new ArgumentNullException(nameof(frozenQueries));
            _traderQueries = traderQueries ?? throw new ArgumentNullException(nameof(traderQueries));
            _shopAgentQueries = shopAgentQueries ?? throw new ArgumentNullException(nameof(shopAgentQueries));
            _shopQueries = shopQueries ?? throw new ArgumentNullException(nameof(shopQueries));
            _qrCodeSideEffectService = qrCodeSideEffectService ?? throw new ArgumentNullException(nameof(qrCodeSideEffectService));
        }

        public async Task Handle(BalanceWithdrawalCreatedDomainEvent balanceWithdrawalCreatedDomainEvent, CancellationToken cancellationToken)
        {
            var withdrawal = balanceWithdrawalCreatedDomainEvent.Withdrawal;

            //Create withdrawal.
            var withdrawalCreated = _withdrawalRepository.Add(withdrawal);

            //Update BalanceWithdrawal entity.
            var balance = balanceWithdrawalCreatedDomainEvent.Balance;
            if (withdrawal.GetWithdrawalType.Id == WithdrawalType.ByUser.Id)
            {
                balance.WithdrawalCreated(withdrawalCreated);
                _balanceRepository.Update(balance);
            }


            //Update trader qrcodes.
            if (balance.GetUserType.Id == UserType.Trader.Id)
            {
                await _qrCodeSideEffectService.UpdateQrCodeWhenTraderBalanceUpdated(
                    balance.UserId,
                    balance.AmountAvailable);
            }

            //Create frozen.
            var frozen = Frozen.FromWithdrawal(
                withdrawalCreated,
                balanceWithdrawalCreatedDomainEvent.BalanceBefore,
                balanceWithdrawalCreatedDomainEvent.BalanceAfter,
                _dateTimeService);

            var frozenCreated = _frozenRepository.Add(frozen);

            await _withdrawalRepository.UnitOfWork
                .SaveEntitiesAsync(cancellationToken);


            //Update bankbook record.
            decimal balanceBefore = frozen.BalanceFrozenRecord.BalanceBefore;
            decimal balanceAfter = (decimal)frozen.BalanceFrozenRecord.BalanceAfter;
            decimal amountChange = balanceAfter - balanceBefore;

            var bankBookRecord = new BankbookRecord
            {
                UserId = frozenCreated.UserId,
                BalanceId = frozenCreated.BalanceId,
                DateOccurred = frozenCreated.DateFroze.ToFullString(),
                Type = withdrawal.GetWithdrawalType.Id == WithdrawalType.ByAdmin.Id ?
                BankbookRecordType.WithdrawalByAdmin : BankbookRecordType.Withdrawal,
                BalanceBefore = balanceBefore,
                AmountChanged = amountChange,
                BalanceAfter = balanceAfter,
                TrackingId = frozenCreated.WithdrawalId.ToString(),
                Description = frozen.Description
            };

            _bankbookQueries.Add(bankBookRecord);

            await _bankbookQueries.SaveChangesAsync();



            //Update Balance
            if (balanceWithdrawalCreatedDomainEvent.Balance.GetUserType.Id == UserType.TraderAgent.Id)
            {

                var traderAgentVM = await _traderAgentQueries.GetTraderAgent(frozen.UserId);
                if (traderAgentVM != null)
                {
                    var amountFrozen =  _frozenQueries.GetUserCurrentFrozenAmountAsync(traderAgentVM.TraderAgentId);
                    traderAgentVM.Balance.AmountFrozen = amountFrozen;
                    traderAgentVM.Balance.AmountAvailable = balanceWithdrawalCreatedDomainEvent.Balance.AmountAvailable;
                    _traderAgentQueries.Update(traderAgentVM);

                    await _traderAgentQueries.SaveChangesAsync();
                }
            }
            else if (balanceWithdrawalCreatedDomainEvent.Balance.GetUserType.Id == UserType.Trader.Id)
            {
                var traderVm = await _traderQueries.GetTrader(frozen.UserId);

                if (traderVm != null)
                {
                    var amountFrozen =  _frozenQueries.GetUserCurrentFrozenAmountAsync(traderVm.TraderId);
                    traderVm.Balance.AmountAvailable = balanceWithdrawalCreatedDomainEvent.Balance.AmountAvailable;
                    traderVm.Balance.AmountFrozen = amountFrozen;
                }

                _traderQueries.Update(traderVm);

                await _traderQueries.SaveChangesAsync();
            }
            else if (balanceWithdrawalCreatedDomainEvent.Balance.GetUserType.Id == UserType.ShopAgent.Id)
            {
                var shopAgentVm = await _shopAgentQueries.GetShopAgent(frozen.UserId);

                if (shopAgentVm != null)
                {
                    var amountFrozen =  _frozenQueries.GetUserCurrentFrozenAmountAsync(shopAgentVm.ShopAgentId);
                    shopAgentVm.Balance.AmountAvailable = balanceWithdrawalCreatedDomainEvent.Balance.AmountAvailable;
                    shopAgentVm.Balance.AmountFrozen = amountFrozen;
                }

                _shopAgentQueries.Update(shopAgentVm);

                await _shopAgentQueries.SaveChangesAsync();
            }
            else if (balanceWithdrawalCreatedDomainEvent.Balance.GetUserType.Id == UserType.Shop.Id)
            {
                var shopVM = await _shopQueries.GetShop(frozen.UserId);

                if (shopVM != null)
                {
                    var amountFrozen =  _frozenQueries.GetUserCurrentFrozenAmountAsync(shopVM.ShopId);
                    shopVM.Balance.AmountAvailable = balanceWithdrawalCreatedDomainEvent.Balance.AmountAvailable;
                    shopVM.Balance.AmountFrozen = amountFrozen;
                }

                _shopQueries.Update(shopVM);

                await _shopQueries.SaveChangesAsync();
            }



        }
    }
}