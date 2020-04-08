using Distributing.Domain.Events;
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
using WebMVC.Applications.Queries.TraderAgents;
using WebMVC.Applications.Queries.Traders;
using WebMVC.Extensions;

namespace WebMVC.Applications.DomainEventHandlers.DistributingDomain
{
    /// <summary>
    /// Create frozen entity when balance froze from new order.
    /// Update view model: add new BankbookRecord.
    /// Update view model: update trader's balance.
    /// </summary>
    public class BalanceFrozeFromNewOrderDomainEventHandler
                : INotificationHandler<BalanceFrozeFromNewOrderDomainEvent>
    {
        private readonly IWithdrawalRepository _withdrawalRepository;
        private readonly IFrozenRepository _frozenRepository;
        private readonly IFrozenQueries _frozenQueries;
        private readonly IBankbookQueries _bankbookQueries;
        private readonly ITraderAgentQueries _traderAgentQueries;
        private readonly ITraderQueries _traderQueries;

        public BalanceFrozeFromNewOrderDomainEventHandler(IWithdrawalRepository withdrawalRepository, IFrozenRepository frozenRepository, IFrozenQueries frozenQueries, IBankbookQueries bankbookQueries, ITraderAgentQueries traderAgentQueries, ITraderQueries traderQueries)
        {
            _withdrawalRepository = withdrawalRepository ?? throw new ArgumentNullException(nameof(withdrawalRepository));
            _frozenRepository = frozenRepository ?? throw new ArgumentNullException(nameof(frozenRepository));
            _frozenQueries = frozenQueries ?? throw new ArgumentNullException(nameof(frozenQueries));
            _bankbookQueries = bankbookQueries ?? throw new ArgumentNullException(nameof(bankbookQueries));
            _traderAgentQueries = traderAgentQueries ?? throw new ArgumentNullException(nameof(traderAgentQueries));
            _traderQueries = traderQueries ?? throw new ArgumentNullException(nameof(traderQueries));
        }

        public async Task Handle(BalanceFrozeFromNewOrderDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            /*var frozen = domainEvent.Frozen;

            //Create frozen.
            var frozenCreated = _frozenRepository.Add(frozen);

            //await _withdrawalRepository.UnitOfWork
            //    .SaveChangesAsync(cancellationToken);

            //Update bankbook record.
            decimal balanceBefore = frozen.BalanceFrozenRecord.BalanceBefore;
            decimal balanceAfter = (decimal)frozen.BalanceFrozenRecord.BalanceAfter;
            decimal amountChange = balanceAfter - balanceBefore;

            var bankBookRecord = new BankbookRecord
            {
                UserId = frozenCreated.UserId,
                BalanceId = frozenCreated.BalanceId,
                DateOccurred = frozenCreated.DateFroze.ToFullString(),
                Type = BankbookRecordType.ProcessOrder,
                BalanceBefore = balanceBefore,
                AmountChanged = amountChange,
                BalanceAfter = balanceAfter,
                TrackingId = frozenCreated.OrderTrackingNumber,
                Description = frozen.Description
            };

            _bankbookQueries.Add(bankBookRecord);*/

            //Update trader balance.(only trader will be frozen by new order.)
            //_traderQueries.UpdateBalance(domainEvent.Balance.UserId, domainEvent.Balance.AmountAvailable);
        }
    }
}
