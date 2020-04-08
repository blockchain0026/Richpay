using Distributing.Domain.Events;
using Distributing.Domain.Exceptions;
using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Frozens;
using Distributing.Domain.Model.Shared;
using Distributing.Domain.Model.Withdrawals;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Withdrawals;
using WebMVC.Extensions;

namespace WebMVC.Applications.DomainEventHandlers.DistributingDomain
{
    /// <summary>
    /// Update frozen entity when withdrawal canceled.
    /// Update view model: Update withdrawal entry.
    /// </summary>
    public class WithdrawalCanceledDomainEventHandler
            : INotificationHandler<WithdrawalCanceledDomainEvent>
    {
        private readonly IFrozenRepository _frozenRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly IWithdrawalQueries _withdrawalQueries;

        public WithdrawalCanceledDomainEventHandler(IFrozenRepository frozenRepository, IBalanceRepository balanceRepository, IDateTimeService dateTimeService, IWithdrawalQueries withdrawalQueries)
        {
            _frozenRepository = frozenRepository ?? throw new ArgumentNullException(nameof(frozenRepository));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            _withdrawalQueries = withdrawalQueries ?? throw new ArgumentNullException(nameof(withdrawalQueries));
        }

        public async Task Handle(WithdrawalCanceledDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            //Update frozen entity.
            var existingFrozen = await _frozenRepository.GetByWithdrawalIdAsync(domainEvent.Withdrawal.Id);

            if (existingFrozen == null)
            {
                throw new DistributingDomainException("No frozen found by event's withdrawal Id" + ". At WithdrawalCanceledDomainEventHandler");
            }
            var existingBalance = await _balanceRepository.GetByBalanceIdAsync(domainEvent.Withdrawal.BalanceId);

            if (existingBalance == null)
            {
                throw new DistributingDomainException("No frozen found by event's withdrawal Id" + ". At WithdrawalCanceledDomainEventHandler");
            }
            existingFrozen.WithdrawalCanceled(existingBalance, domainEvent.Withdrawal, _dateTimeService);

            _frozenRepository.Update(existingFrozen);

            await _frozenRepository.UnitOfWork
                .SaveEntitiesAsync(cancellationToken);


            //Update withdrawal entry.
            var withdrawal = domainEvent.Withdrawal;
            if (withdrawal.GetWithdrawalType.Id == WithdrawalType.ByUser.Id)
            {
                var withdrawalEntryVM = await _withdrawalQueries.GetWithdrawalEntryAsync(withdrawal.Id);

                if (withdrawalEntryVM != null)
                {
                    withdrawalEntryVM.WithdrawalStatus = withdrawal.GetWithdrawalStatus.Name;
                    withdrawalEntryVM.CancellationApprovedByAdminId = withdrawal.CancellationApprovedBy?.AdminId;
                    withdrawalEntryVM.CancellationApprovedByAdminName = withdrawal.CancellationApprovedBy?.Name;
                    withdrawalEntryVM.DateFinished = withdrawal.DateFinished;

                    _withdrawalQueries.Update(withdrawalEntryVM);
                    await _withdrawalQueries.SaveChangesAsync();
                }
            }
        }
    }
}
