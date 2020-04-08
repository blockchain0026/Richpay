﻿using Distributing.Domain.Events;
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
    /// Update view model: update withdrawal entry.
    /// </summary>
    public class WithdrawalAwaitingCancellationDomainEventHandler : INotificationHandler<WithdrawalAwaitingCancellationDomainEvent>
    {
        private readonly IWithdrawalQueries _withdrawalQueries;

        public WithdrawalAwaitingCancellationDomainEventHandler(IWithdrawalQueries withdrawalQueries)
        {
            _withdrawalQueries = withdrawalQueries ?? throw new ArgumentNullException(nameof(withdrawalQueries));
        }

        public async Task Handle(WithdrawalAwaitingCancellationDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            //Update withdrawal entry.
            var withdrawal = domainEvent.Withdrawal;
            if (withdrawal.GetWithdrawalType.Id == WithdrawalType.ByUser.Id)
            {
                var withdrawalEntryVM = await _withdrawalQueries.GetWithdrawalEntryAsync(withdrawal.Id);

                if (withdrawalEntryVM != null)
                {
                    withdrawalEntryVM.WithdrawalStatus = withdrawal.GetWithdrawalStatus.Name;

                    _withdrawalQueries.Update(withdrawalEntryVM);
                    await _withdrawalQueries.SaveChangesAsync();
                }
            }
        }
    }
}