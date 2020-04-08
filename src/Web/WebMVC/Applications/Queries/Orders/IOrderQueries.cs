using Ordering.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.Orders
{
    public interface IOrderQueries
    {
        Task<OrderEntry> GetOrderEntryAsync(int orderId);

        Task<List<OrderEntry>> GetOrderEntrysAsync(int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null,
            string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null,
            string direction = SortDirections.Descending);
        Task<OrderSumData> GetOrderEntrysTotalSumDataAsync(string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null);

        Task<OrderEntry> MapFromEntity(Order entity);

        /* Task<List<OrderEntry>> GetOrderEntrysByTraderIdAsync(string traderId, int? pageIndex, int? take, string searchString = null, string sortField = null,
             DateTime? from = null, DateTime? to = null,
             string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null,
             string direction = SortDirections.Descending);
         Task<int> GetOrderEntrysTotalCountByTraderIdAsync(string traderId, string searchString = null,
             DateTime? from = null, DateTime? to = null,
             string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null);


         Task<List<OrderEntry>> GetOrderEntrysByQrCodeIdAsync(int qrCodeId, int? pageIndex, int? take, string searchString = null, string sortField = null, 
             DateTime? from = null, DateTime? to = null,
             string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null,
             string direction = SortDirections.Descending);
         Task<int> GetOrderEntrysTotalCountByQrCodeIdAsync(int qrCodeId, string searchString = null, 
             DateTime? from = null, DateTime? to = null,
             string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null);


         Task<List<OrderEntry>> GetOrderEntrysByShopIdAsync(string shopId, int? pageIndex, int? take, string searchString = null, string sortField = null, 
             DateTime? from = null, DateTime? to = null,
             string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null,
             string direction = SortDirections.Descending);
         Task<int> GetOrderEntrysTotalCountByShopIdAsync(string shopId, string searchString = null, 
             DateTime? from = null, DateTime? to = null,
             string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null);


         Task<List<OrderEntry>> GetOrderEntrysByFourthPartyIdAsync(string fourthPartyId, int? pageIndex, int? take, string searchString = null, 
             DateTime? from = null, DateTime? to = null,
             string sortField = null, string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null, 
             string direction = SortDirections.Descending);
         Task<int> GetOrderEntrysTotalCountByFourthPartyIdAsync(string fourthPartyId, string searchString = null,
             DateTime? from = null, DateTime? to = null,
             string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null);

     */
        Task DeleteFinishedOrderEntry();
        Task<List<OrderEntry>> GetAllOrderEntries();

        void UpdateOrderEntryToSuccess(int orderId, string orderStatus,
            decimal amountPaid, decimal shopUserCommissionRealized, decimal tradingUserCommissionRealized, decimal platformCommissionRealized,
            decimal traderCommissionRealized, decimal shopCommissionRealized,
            string datePaymentRecieved);
        Task UpdateOrderEntryToPaired(int orderId, string orderStatus, string datePaired, string traderId,
            int? qrCodeId, string fourthPartyId, string fourthPartyName, decimal? toppestTradingRate);

        OrderEntry Add(OrderEntry orderEntry);
        void Update(OrderEntry orderEntry);
        void Delete(OrderEntry orderEntry);
        Task SaveChangesAsync();
        Task<OrderEntriesStatistic> GetOrderEntriesStatisticAsync(string searchString = null, DateTime? from = null, DateTime? to = null);
        Task<List<OrderEntry>> GetOrderEntrysByTraderIdAsync(string traderId, int? pageIndex, int? take, string searchString = null, string sortField = null, DateTime? from = null, DateTime? to = null, string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null, string direction = "desc");
        Task<OrderSumData> GetOrderEntrysTotalSumDataByTraderIdAsync(string traderId, string searchString = null, DateTime? from = null, DateTime? to = null, string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null);
        Task<OrderEntriesStatistic> GetOrderEntriesStatisticByTraderIdAsync(string traderId, string searchString = null, DateTime? from = null, DateTime? to = null);
        Task<List<OrderEntry>> GetOrderEntrysByShopIdAsync(string shopId, int? pageIndex, int? take, string searchString = null, string sortField = null, DateTime? from = null, DateTime? to = null, string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null, string direction = "desc");
        Task<OrderSumData> GetOrderEntrysTotalSumDataByShopIdAsync(string shopId, string searchString = null, DateTime? from = null, DateTime? to = null, string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null);
        Task<OrderEntriesStatistic> GetOrderEntriesStatisticByShopIdAsync(string shopId, string searchString = null, DateTime? from = null, DateTime? to = null);
        void UpdateOrderEntryToExpired(int orderId, string orderStatus, string orderStatusDescription);
    }
}
