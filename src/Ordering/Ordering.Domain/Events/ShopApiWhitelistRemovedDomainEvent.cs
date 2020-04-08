using MediatR;
using Ordering.Domain.Model.ShopApis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    public class ShopApiWhitelistRemovedDomainEvent : INotification
    {
        public ShopApiWhitelistRemovedDomainEvent(ShopApi shopApi, int ipWhitelistId)
        {
            ShopApi = shopApi;
            IpWhitelistId = ipWhitelistId;
        }

        public ShopApi ShopApi { get; }
        public int IpWhitelistId { get; }
    }
}
