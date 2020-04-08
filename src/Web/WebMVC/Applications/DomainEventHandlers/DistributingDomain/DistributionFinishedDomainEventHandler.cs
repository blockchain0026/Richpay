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
    /// Update view model: update running account records.
    /// </summary>
    public class DistributionFinishedDomainEventHandler
         : INotificationHandler<DistributionFinishedDomainEvent>
    {
        private readonly IRunningAccountQueries _runningAccountQueries;

        public DistributionFinishedDomainEventHandler(IRunningAccountQueries runningAccountQueries)
        {
            _runningAccountQueries = runningAccountQueries ?? throw new ArgumentNullException(nameof(runningAccountQueries));
        }

        public async Task Handle(DistributionFinishedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            return; //For performance reason.

            //Update view model.
            var distribution = domainEvent.Distribution;
            var runningAccountRecordVM = await _runningAccountQueries.GetRunningAccountRecordByUserIdAndTrackingNumberAsync(
                distribution.UserId,
                distribution.Order.TrackingNumber);

            if (runningAccountRecordVM != null)
            {
                //Update status and save changes.
                if (domainEvent.IsSuccess)
                {
                    runningAccountRecordVM.Status = "Success";
                }
                else
                {
                    runningAccountRecordVM.Status = "Failed";
                }

                runningAccountRecordVM.DistributedAmount = domainEvent.DistributedAmount;

                this._runningAccountQueries.Update(runningAccountRecordVM);

                //Let order service do the saving action.
                //await this._runningAccountQueries.SaveChangesAsync();
            }
        }
    }
}
