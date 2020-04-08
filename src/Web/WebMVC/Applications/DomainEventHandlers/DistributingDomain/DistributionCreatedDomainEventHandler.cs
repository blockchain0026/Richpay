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
    /// Update view model: create running account entry when new distrbution created.
    /// </summary>
    public class DistributionCreatedDomainEventHandler
         : INotificationHandler<DistributionCreatedDomainEvent>
    {
        private readonly IRunningAccountQueries _runningAccountQueries;

        public DistributionCreatedDomainEventHandler(IRunningAccountQueries runningAccountQueries)
        {
            _runningAccountQueries = runningAccountQueries ?? throw new ArgumentNullException(nameof(runningAccountQueries));
        }

        public async Task Handle(DistributionCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            return; //For performance reason.
            //Build view model.
            var distribution = domainEvent.Distribution;
            var runningAccountRecordVM = await _runningAccountQueries.MapFromEntity(
                distribution,
                distribution.Order,
                domainEvent.DownlineUserId
                );

            //Add view model to database.
            this._runningAccountQueries.Add(runningAccountRecordVM);

            //Let the order service save changes.
        }
    }
}
