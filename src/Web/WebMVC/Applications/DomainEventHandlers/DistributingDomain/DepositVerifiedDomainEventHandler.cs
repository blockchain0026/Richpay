using Distributing.Domain.Events;
using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Deposits;
using Distributing.Domain.Model.Withdrawals;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Bankbook;
using WebMVC.Applications.Queries.Deposits;
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
    /// Update balance entity when deposit verified.
    /// If this is trader balance, update his Qr codes.
    /// Update view model: add new BankbookRecord.
    /// Update view model: update balance.
    /// Update view model: update deposit entry.
    /// </summary>
    public class DepositVerifiedDomainEventHandler
                : INotificationHandler<DepositVerifiedDomainEvent>
    {
        private readonly IBalanceRepository _balanceRepository;
        private readonly IBankbookQueries _bankbookQueries;
        private readonly ITraderAgentQueries _traderAgentQueries;
        private readonly ITraderQueries _traderQueries;
        private readonly IShopAgentQueries _shopAgentQueries;
        private readonly IShopQueries _shopQueries;
        private readonly IDepositQueries _depositQueries;
        private readonly IQrCodeSideEffectService _qrCodeSideEffectService;

        public DepositVerifiedDomainEventHandler(IBalanceRepository balanceRepository, IBankbookQueries bankbookQueries, ITraderAgentQueries traderAgentQueries, ITraderQueries traderQueries, IShopAgentQueries shopAgentQueries, IShopQueries shopQueries, IDepositQueries depositQueries, IQrCodeSideEffectService qrCodeSideEffectService)
        {
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _bankbookQueries = bankbookQueries ?? throw new ArgumentNullException(nameof(bankbookQueries));
            _traderAgentQueries = traderAgentQueries ?? throw new ArgumentNullException(nameof(traderAgentQueries));
            _traderQueries = traderQueries ?? throw new ArgumentNullException(nameof(traderQueries));
            _shopAgentQueries = shopAgentQueries ?? throw new ArgumentNullException(nameof(shopAgentQueries));
            _shopQueries = shopQueries ?? throw new ArgumentNullException(nameof(shopQueries));
            _depositQueries = depositQueries ?? throw new ArgumentNullException(nameof(depositQueries));
            _qrCodeSideEffectService = qrCodeSideEffectService ?? throw new ArgumentNullException(nameof(qrCodeSideEffectService));
        }

        public async Task Handle(DepositVerifiedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var balance = domainEvent.Balance;

            //Udpate balance.
            balance.Deposit(domainEvent.Deposit);

            _balanceRepository.Update(balance);

            await _balanceRepository.UnitOfWork
                .SaveEntitiesAsync(cancellationToken);

            //Update trader qrcodes.
            if (balance.GetUserType.Id == UserType.Trader.Id)
            {
                await _qrCodeSideEffectService.UpdateQrCodeWhenTraderBalanceUpdated(
                    balance.UserId,
                    balance.AmountAvailable);
            }

            //Update bankbook record.
            decimal balanceBefore = domainEvent.Deposit.BalanceRecord.BalanceBefore;
            decimal balanceAfter = (decimal)domainEvent.Deposit.BalanceRecord.BalanceAfter;
            decimal amountChange = balanceAfter - balanceBefore;

            var bankBookRecord = new BankbookRecord
            {
                UserId = domainEvent.Deposit.UserId,
                BalanceId = domainEvent.Deposit.BalanceId,
                DateOccurred = domainEvent.DateFinished.ToFullString(),
                Type = domainEvent.Deposit.GetDepositType.Id == DepositType.ByUser.Id ?
                BankbookRecordType.Deposit : BankbookRecordType.DepositByAdmin,
                BalanceBefore = balanceBefore,
                AmountChanged = amountChange,
                BalanceAfter = balanceAfter,
                TrackingId = domainEvent.Deposit.Id.ToString(),
                Description = domainEvent.Deposit.Description
            };

            _bankbookQueries.Add(bankBookRecord);
            await _bankbookQueries.SaveChangesAsync();

            //Update balance record.
            if (balance.GetUserType.Id == UserType.TraderAgent.Id)
            {
                //Update trader agent.
                var traderAgentVM = await _traderAgentQueries.GetTraderAgent(balance.UserId);
                if (traderAgentVM != null)
                {
                    traderAgentVM.Balance.AmountAvailable = balance.AmountAvailable;
                    _traderAgentQueries.Update(traderAgentVM);

                    await _traderAgentQueries.SaveChangesAsync();
                }
            }
            else if (domainEvent.Balance.GetUserType.Id == UserType.Trader.Id)
            {
                var traderVm = await _traderQueries.GetTrader(balance.UserId);

                if (traderVm != null)
                {
                    traderVm.Balance.AmountAvailable = balance.AmountAvailable;
                    _traderQueries.Update(traderVm);
                    await _traderQueries.SaveChangesAsync();
                }
            }
            else if (domainEvent.Balance.GetUserType.Id == UserType.ShopAgent.Id)
            {
                //When admin add amount to shop agnet's balance, upldate the shop agent's balance view data.
                var shopAgentVm = await _shopAgentQueries.GetShopAgent(balance.UserId);

                if (shopAgentVm != null)
                {
                    shopAgentVm.Balance.AmountAvailable = balance.AmountAvailable;
                    _shopAgentQueries.Update(shopAgentVm);
                    await _shopAgentQueries.SaveChangesAsync();
                }
            }
            else if (domainEvent.Balance.GetUserType.Id == UserType.Shop.Id)
            {
                //When admin add amount to shop 's balance, upldate the shop 's balance view data.
                var shopVm = await _shopQueries.GetShop(balance.UserId);

                if (shopVm != null)
                {
                    shopVm.Balance.AmountAvailable = balance.AmountAvailable;
                    _shopQueries.Update(shopVm);
                    await _shopQueries.SaveChangesAsync();
                }
            }

            //Update deposit entry.
            var deposit = domainEvent.Deposit;
            if (deposit.GetDepositType.Id == DepositType.ByUser.Id)
            {
                var depositVM = await this._depositQueries.GetDepositEntryAsync(deposit.Id);
                if (depositVM != null)
                {
                    depositVM.DepositStatus = deposit.GetDepositStatus.Name;
                    depositVM.VerifiedByAdminId = deposit.VerifiedBy?.AdminId;
                    depositVM.VerifiedByAdminName = deposit.VerifiedBy?.Name;
                    depositVM.DateFinished = deposit.DateFinished;

                    this._depositQueries.Update(depositVM);
                    await this._depositQueries.SaveChangesAsync();
                }
            }

        }
    }
}
