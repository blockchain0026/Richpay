using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Infrastructure.Services
{
    public interface IOrderService
    {
        Task<OrderEntry> GetOrderById(string searchByUserId, int orderId);
        //Task<OrderEntry> GetOrderByTrackingNumber(string searchByUserId, string orderTrackingNumber);


        Task<List<OrderEntry>> GetPlatformOrderEntrys(int pageIndex, int take, string searchByUserId, string searchString = "", string sortField = "",
            DateTime? from = null, DateTime? to = null,
            string orderStatus = null, string orderPaymentChannel = null, string orderPaymentScheme = null,
            string direction = SortDirections.Descending);

        Task<OrderSumData> GetPlatformOrderEntrysTotalSumData(string searchByUserId, string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string orderStatus = null, string orderPaymentChannel = null, string orderPaymentScheme = null);


        Task<List<RunningAccountRecord>> GetRunningAccountRecordsByUserIdAsync(string searchByUserId, string userId, int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null, string status = null,
            string direction = SortDirections.Descending);

        Task<RunningAccountRecordSumData> GetRunningAccountRecordsTotalSumDataByUserIdAsync(string searchByUserId, string userId, string searchString = null,
            DateTime? from = null, DateTime? to = null, string status = null);


        Task<int?> CreateOrderToPlatform(string createByUserId, string shopId, string shopOrderId, decimal shopOrderAmount, string shopReturnUrl, string shopOkReturnUrl,
            OrderGatewayType orderGatewayType);
        Task<string> CreateTestOrderToPlatform(string createByUserId, decimal orderAmount, int qrCodeId);



        //Task CreateOrderToFourthParty()

        Task ConfirmOrderById(int orderId, string confirmedByTraderId);
        Task ConfirmOrderByTrackingNumber(string orderTrackingNumber, string confirmedByTraderId);
        Task ConfirmOrderByAdmin(int orderId);

        Task ForceConfirmOrder(int orderId, string forcedByAdminId);
        Task ForceConfirmOrders(List<int> orderIds, string forcedByAdminId);
        Task DeleteAllTestOrders(string deleteByUserId);
    }
}
