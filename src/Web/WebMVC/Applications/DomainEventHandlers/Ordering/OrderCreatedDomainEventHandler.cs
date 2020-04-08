using MediatR;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Events;
using Ordering.Domain.Model.Orders;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Orders;

namespace WebMVC.Applications.DomainEventHandlers.Ordering
{
    /// <summary>
    /// To Platform order: Find qr code to pairing.
    /// To Fourth Party order: Send request to fourth party.
    /// 
    /// Update view model: Create new Order Entry.
    /// </summary>
    public class OrderCreatedDomainEventHandler
            : INotificationHandler<OrderCreatedDomainEvent>
    {
        private readonly IPairingDomainService _pairingDomainService;

        private readonly IOrderQueries _orderQueries;

        private readonly IQrCodeRepository _qrCodeRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderCreatedDomainEventHandler(IPairingDomainService pairingDomainService, IOrderQueries orderQueries, IQrCodeRepository qrCodeRepository, IOrderRepository orderRepository)
        {
            _pairingDomainService = pairingDomainService ?? throw new ArgumentNullException(nameof(pairingDomainService));
            _orderQueries = orderQueries ?? throw new ArgumentNullException(nameof(orderQueries));
            _qrCodeRepository = qrCodeRepository ?? throw new ArgumentNullException(nameof(qrCodeRepository));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task Handle(OrderCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            return; //The logics moved to ordering service.
        }
    }
}
