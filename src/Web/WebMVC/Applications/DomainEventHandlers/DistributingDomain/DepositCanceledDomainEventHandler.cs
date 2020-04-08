using Distributing.Domain.Events;
using Distributing.Domain.Model.Deposits;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Deposits;
using WebMVC.Extensions;

namespace WebMVC.Applications.DomainEventHandlers.DistributingDomain
{
    /// <summary>
    /// Update view model: update deposit entry.
    /// </summary>
    public class DepositCanceledDomainEventHandler
                : INotificationHandler<DepositCanceledDomainEvent>
    {
        private readonly IDepositQueries _depositQueries;

        public DepositCanceledDomainEventHandler(IDepositQueries depositQueries)
        {
            _depositQueries = depositQueries ?? throw new ArgumentNullException(nameof(depositQueries));
        }

        public async Task Handle(DepositCanceledDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            //Update deposit entry.
            var deposit = domainEvent.Deposit;
            if (deposit.GetDepositType.Id == DepositType.ByUser.Id)
            {
                var depositVM = await this._depositQueries.GetDepositEntryAsync(deposit.Id);
                if (depositVM != null)
                {
                    depositVM.DepositStatus = deposit.GetDepositStatus.Name;
                    depositVM.DateFinished = deposit.DateFinished;

                    this._depositQueries.Update(depositVM);
                    await this._depositQueries.SaveChangesAsync();
                }
            }

        }
    }
}
