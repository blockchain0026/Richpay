using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Infrastructure.ApiClients
{
    public interface INewPayApiClient
    {
        Task ReturnOrderResult(string url, string orderTrackingNumber, string shopOrderId, string shopId, string dateCreated, string dateConfirmed, int orderAmount, int orderStatus);
    }
}
