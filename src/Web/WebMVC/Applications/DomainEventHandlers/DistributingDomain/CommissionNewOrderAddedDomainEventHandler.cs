using Distributing.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.RunningAccounts;

namespace WebMVC.Applications.DomainEventHandlers.DistributingDomain
{
    /// <summary>
    /// Update view model: create running account entry when commission new order added.
    /// </summary>
    public class CommissionNewOrderAddedDomainEventHandler
         : INotificationHandler<CommissionNewOrderAddedDomainEvent>
    {
        private readonly IRunningAccountQueries _runningAccountQueries;

        public CommissionNewOrderAddedDomainEventHandler(IRunningAccountQueries runningAccountQueries)
        {
            _runningAccountQueries = runningAccountQueries ?? throw new ArgumentNullException(nameof(runningAccountQueries));
        }

        public async Task Handle(CommissionNewOrderAddedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            return;
            //Build view model.
            var runningAccountRecordVM = await _runningAccountQueries.MapFromEntity(
                domainEvent.Commission,
                domainEvent.Order,
                domainEvent.DownlineUserId
                );

            //Add view model to database.
            this._runningAccountQueries.Add(runningAccountRecordVM);

            //Let the order service save changes.
        }
    }
}
