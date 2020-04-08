using MediatR;
using Ordering.Domain.Model.ShopApis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    public class ShopApiWhitelistAddedDomainEvent : INotification
    {
        public ShopApiWhitelistAddedDomainEvent(ShopApi shopApi, string apiKey)
        {
            ShopApi = shopApi;
            ApiKey = apiKey;
        }

        public ShopApi ShopApi { get; }
        public string ApiKey { get; }
    }
}
