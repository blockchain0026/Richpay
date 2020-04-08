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
    public class CommissionOrderCompletedDomainEventHandler
         : INotificationHandler<CommissionOrderCompletedDomainEvent>
    {
        private readonly IRunningAccountQueries _runningAccountQueries;

        public CommissionOrderCompletedDomainEventHandler(IRunningAccountQueries runningAccountQueries)
        {
            _runningAccountQueries = runningAccountQueries ?? throw new ArgumentNullException(nameof(runningAccountQueries));
        }

        public async Task Handle(CommissionOrderCompletedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            return;
            //Update view model.
            var runningAccountRecordVM = await _runningAccountQueries.GetRunningAccountRecordByUserIdAndTrackingNumberAsync(
                domainEvent.Commission.UserId,
                domainEvent.OrderTrackingNumber);

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

                await this._runningAccountQueries.SaveChangesAsync();
            }
        }
    }
}
