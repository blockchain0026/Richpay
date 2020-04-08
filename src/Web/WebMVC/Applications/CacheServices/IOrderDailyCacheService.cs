using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.CacheServices
{
    public interface IOrderDailyCacheService
    {
        void AddOrUpdateOrder(OrderEntry orderEntry);
        Task<OrderEntriesStatistic> GetOrderEntriesStatisticAsync(string searchString = null, DateTime? from = null, DateTime? to = null);
        Task<List<OrderEntry>> GetOrderEntrysAsync(int? pageIndex, int? take, string searchString = null, string sortField = null, DateTime? from = null, DateTime? to = null, string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null, string direction = "desc");
        Task<OrderSumData> GetOrderEntrysTotalSumDataAsync(string searchString = null, DateTime? from = null, DateTime? to = null, string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null);
        Task Initialize();
        Task UpdateRecentOrderEntries();
    }
}
