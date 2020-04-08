using MediatR;
using Ordering.Domain.Model.ShopApis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Events
{
    public class ShopApiCreatedDomainEvent : INotification
    {
        public ShopApiCreatedDomainEvent(ShopApi shopApi, string shopId, string apiKey)
        {
            ShopApi = shopApi;
            ShopId = shopId;
            ApiKey = apiKey;
        }

        public ShopApi ShopApi { get; }
        public string ShopId { get; }
        public string ApiKey { get; }
    }
}
