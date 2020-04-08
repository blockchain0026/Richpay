using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Orders;
using WebMVC.Applications.Queries.RunningAccounts;
using WebMVC.Extensions;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.DomainEventHandlers.Ordering
{
    /// <summary>
    /// Update view model: Update Order Entry.
    /// </summary>
    public class OrderPairedByPlatformDomainEventHandler
            : INotificationHandler<OrderPairedByPlatformDomainEvent>
    {
        private readonly IOrderQueries _orderQueries;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRunningAccountQueries _runningAccountQueries;

        public OrderPairedByPlatformDomainEventHandler(IOrderQueries orderQueries, UserManager<ApplicationUser> userManager, IRunningAccountQueries runningAccountQueries)
        {
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _runningAccountQueries = runningAccountQueries ?? throw new ArgumentNullException(nameof(runningAccountQueries));
        }

        public async Task Handle(OrderPairedByPlatformDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            /*var orderEntity = domainEvent.Order;

            //Create Running Account Records
            //await _runningAccountQueries.CreateRunningRecordsFrom(orderEntity);

            //Use imporved update algorism to get better performance.
            var payeeInfo = orderEntity.PayeeInfo;

            await _orderQueries.UpdateOrderEntryToPaired(
                orderEntity.Id,
                orderEntity.GetOrderStatus.Name,
                orderEntity.DatePaired?.ToFullString(),
                payeeInfo?.TraderId,
                payeeInfo?.QrCodeId,
                payeeInfo?.FourthPartyId,
                payeeInfo?.FourthPartyName,
                payeeInfo?.ToppestTradingRate
                );*/
                
        }
    }
}
