using Distributing.Domain.Events;
using Distributing.Domain.Exceptions;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Frozens;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
    /// Create frozen entity when balance froze by admin.
    /// If this is trader balance, update his Qr codes.
    /// Update view model: add new BalanceRecord.
    /// Update view model: update balance.
    /// </summary>
    public class BalanceFrozeByAdminDomainEventHandler
            : INotificationHandler<BalanceFrozeByAdminDomainEvent>
    {
        private readonly IFrozenRepository _frozenRepository;

        private readonly ITraderAgentQueries _traderAgentQueries;
        private readonly IBankbookQueries _bankbookQueries;
        private readonly IFrozenQueries _frozenQueries;
        private readonly ITraderQueries _traderQueries;
        private readonly IShopAgentQueries _shopAgentQueries;
        private readonly IShopQueries _shopQueries;

        private readonly IQrCodeSideEffectService _qrCodeSideEffectService;

        public BalanceFrozeByAdminDomainEventHandler(IFrozenRepository frozenRepository, ITraderAgentQueries traderAgentQueries, IBankbookQueries bankbookQueries, IFrozenQueries frozenQueries, ITraderQueries traderQueries, IShopAgentQueries shopAgentQueries, IShopQueries shopQueries, IQrCodeSideEffectService qrCodeSideEffectService)
        {
            _frozenRepository = frozenRepository ?? throw new ArgumentNullException(nameof(frozenRepository));
            _traderAgentQueries = traderAgentQueries ?? throw new ArgumentNullException(nameof(traderAgentQueries));
            _bankbookQueries = bankbookQueries ?? throw new ArgumentNullException(nameof(bankbookQueries));
            _frozenQueries = frozenQueries ?? throw new ArgumentNullException(nameof(frozenQueries));
            _traderQueries = traderQueries ?? throw new ArgumentNullException(nameof(traderQueries));
            _shopAgentQueries = shopAgentQueries ?? throw new ArgumentNullException(nameof(shopAgentQueries));
            _shopQueries = shopQueries ?? throw new ArgumentNullException(nameof(shopQueries));
            _qrCodeSideEffectService = qrCodeSideEffectService ?? throw new ArgumentNullException(nameof(qrCodeSideEffectService));
        }

        public async Task Handle(BalanceFrozeByAdminDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            //--- Save Changes First To Ensure No Concurrencies and Conflicts ---//
            /*try
            {
                await _frozenRepository.UnitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //Available balance is different with original value.
                //To Do: Inform the client to retry.
                throw new DistributingDomainException("系统繁忙中，请稍后再试。错误信息:" + ex.Message);
            }*/

            var frozen = domainEvent.Frozen;

            //Create frozen.
            var frozenCreated = _frozenRepository.Add(frozen);

            await _frozenRepository.UnitOfWork
                .SaveEntitiesAsync(cancellationToken);

            //Update trader balance.
            if (domainEvent.Balance.GetUserType.Id == UserType.Trader.Id)
            {
                await _qrCodeSideEffectService.UpdateQrCodeWhenTraderBalanceUpdated(
                    domainEvent.Balance.UserId,
                    domainEvent.Balance.AmountAvailable);
            }


            //Update bankbook record.
            decimal balanceBefore = frozen.BalanceFrozenRecord.BalanceBefore;
            decimal balanceAfter = (decimal)frozen.BalanceFrozenRecord.BalanceAfter;
            decimal amountChange = balanceAfter - balanceBefore;

            var bankBookRecord = new BankbookRecord
            {
                UserId = frozenCreated.UserId,
                BalanceId = frozenCreated.BalanceId,
                DateOccurred = frozenCreated.DateFroze.ToFullString(),
                Type = BankbookRecordType.FreezeByAdmin,
                BalanceBefore = balanceBefore,
                AmountChanged = amountChange,
                BalanceAfter = balanceAfter,
                TrackingId = frozenCreated.Id.ToString(),
                Description = frozen.Description
            };

            _bankbookQueries.Add(bankBookRecord);

            await _bankbookQueries.SaveChangesAsync();


            //Update balance view data.
            if (domainEvent.Balance.GetUserType.Id == UserType.TraderAgent.Id)
            {
                var traderAgentVM = await _traderAgentQueries.GetTraderAgent(frozen.UserId);
                if (traderAgentVM != null)
                {
                    var amountFrozen = _frozenQueries.GetUserCurrentFrozenAmountAsync(traderAgentVM.TraderAgentId);
                    traderAgentVM.Balance.AmountAvailable = domainEvent.Balance.AmountAvailable;
                    traderAgentVM.Balance.AmountFrozen = amountFrozen;
                    _traderAgentQueries.Update(traderAgentVM);

                    await _traderAgentQueries.SaveChangesAsync();
                }
            }
            else if (domainEvent.Balance.GetUserType.Id == UserType.Trader.Id)
            {
                var traderVm = await _traderQueries.GetTrader(frozen.UserId);

                if (traderVm != null)
                {
                    var amountFrozen = _frozenQueries.GetUserCurrentFrozenAmountAsync(traderVm.TraderId);
                    traderVm.Balance.AmountAvailable = domainEvent.Balance.AmountAvailable;
                    traderVm.Balance.AmountFrozen = amountFrozen;
                }

                _traderQueries.Update(traderVm);

                await _traderQueries.SaveChangesAsync();
            }
            else if (domainEvent.Balance.GetUserType.Id == UserType.ShopAgent.Id)
            {
                var shopAgentVm = await _shopAgentQueries.GetShopAgent(frozen.UserId);

                if (shopAgentVm != null)
                {
                    var amountFrozen = _frozenQueries.GetUserCurrentFrozenAmountAsync(shopAgentVm.ShopAgentId);
                    shopAgentVm.Balance.AmountAvailable = domainEvent.Balance.AmountAvailable;
                    shopAgentVm.Balance.AmountFrozen = amountFrozen;
                }

                _shopAgentQueries.Update(shopAgentVm);

                await _shopAgentQueries.SaveChangesAsync();
            }
            else if (domainEvent.Balance.GetUserType.Id == UserType.Shop.Id)
            {
                var shopVm = await _shopQueries.GetShop(frozen.UserId);

                if (shopVm != null)
                {
                    var amountFrozen = _frozenQueries.GetUserCurrentFrozenAmountAsync(shopVm.ShopId);
                    shopVm.Balance.AmountAvailable = domainEvent.Balance.AmountAvailable;
                    shopVm.Balance.AmountFrozen = amountFrozen;
                }

                _shopQueries.Update(shopVm);

                await _shopQueries.SaveChangesAsync();
            }

        }
    }
}
