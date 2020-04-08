using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Model.Orders
{
    public interface IOrderRepository : IRepository<Order>
    {
        Order Add(Order order);
        void Update(Order order);
        void Delete(Order order);

        Task<Order> GetByOrderIdAsync(int orderId);
        Task<Order> GetByOrderTrackingNumberAsync(string orderTrackingNumber);
        Task<IEnumerable<Order>> GetByTraderIdAsync(string traderId);
        Task<IEnumerable<Order>> GetByQrCodeIdAsync(int qrCodeId);
        Task<IEnumerable<Order>> GetByFourthPartyIdAsync(string fourthPartyId);

        Task<IEnumerable<Order>> GetByShopIdAsync(string shopId);
        Task DeleteFinishedOrder();

        Task<List<Order>> GetAllOrders();
        Task<Order> GetForConfirmPaymentByOrderIdAsync(int orderId);
    }
}
