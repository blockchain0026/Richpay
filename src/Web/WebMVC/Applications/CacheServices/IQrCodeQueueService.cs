using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Applications.CacheServices
{
    public interface IQrCodeQueueService
    {
        int GetNextQrCodeId(string paymentChannel, string paymentScheme, decimal orderAmount, string specifiedShopId);
        Task UpdateQrCodeIds();
    }
}
