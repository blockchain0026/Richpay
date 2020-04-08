using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebWorkerService.ApiClients
{
    public interface IReturnOrderApiClient
    {
        Task ReturnOrderResult(string url, string orderTrackingNumber, string shopOrderId, string shopId, string dateCreated, string dateConfirmed, int orderAmount, int orderStatus);

    }
}
