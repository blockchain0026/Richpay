using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Model.Orders;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository
      : IOrderRepository
    {
        private readonly OrderingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public OrderRepository(OrderingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Order Add(Order order)
        {
            return _context.Orders.Add(order).Entity;

        }

        public void Update(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
        }

        public async Task<Order> GetByOrderIdAsync(int orderId)
        {
            var order = await _context
                                .Orders
                                .FirstOrDefaultAsync(b => b.Id == orderId);
            if (order == null)
            {
                order = _context
                            .Orders
                            .Local
                            .FirstOrDefault(b => b.Id == orderId);
            }
            if (order != null)
            {
                await _context.Entry(order)
                    .Reference(b => b.ShopInfo).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.AlipayPagePreference).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.PayerInfo).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.PayeeInfo).LoadAsync();
            }
            return order;
        }
        public async Task<Order> GetForConfirmPaymentByOrderIdAsync(int orderId)
        {
            var order = await _context
                                .Orders
                                .Where(o => o.Id == orderId)
                                //.IncludeOptimized(o => o.PayeeInfo)
                                //.IncludeOptimized(o => o.ShopInfo)
                                .FirstOrDefaultAsync();
            //Can not confirm on test order.
            if (order.IsTestOrder)
            {
                return null;
            }
            if (order == null)
            {
                order = _context
                            .Orders
                            .Local
                            .FirstOrDefault(b => b.Id == orderId);
            }
            if (order != null)
            {
                await _context.Entry(order)
                    .Reference(b => b.ShopInfo).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.PayeeInfo).LoadAsync();
            }
            return order;
        }
        public async Task<Order> GetByOrderTrackingNumberAsync(string orderTrackingNumber)
        {
            var order = await _context
                                .Orders
                                .FirstOrDefaultAsync(b => b.TrackingNumber == orderTrackingNumber);
            if (order == null)
            {
                order = _context
                            .Orders
                            .Local
                            .FirstOrDefault(b => b.TrackingNumber == orderTrackingNumber);
            }
            if (order != null)
            {
                await _context.Entry(order)
                    .Reference(b => b.ShopInfo).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.AlipayPagePreference).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.PayerInfo).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.PayeeInfo).LoadAsync();
            }

            return order;
        }

        public async Task<IEnumerable<Order>> GetByTraderIdAsync(string traderId)
        {
            var orders = _context
                    .Orders
                    .Include(o => o.PayeeInfo)
                    .Where(b => b.PayeeInfo.TraderId == traderId);

            foreach (var order in orders)
            {
                await _context.Entry(order)
                   .Reference(b => b.ShopInfo).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.AlipayPagePreference).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.PayerInfo).LoadAsync();
            }

            return orders;
        }
        public async Task<IEnumerable<Order>> GetByQrCodeIdAsync(int qrCodeId)
        {
            var orders = _context
                    .Orders
                    .Include(o => o.PayeeInfo)
                    .Where(b => b.PayeeInfo.QrCodeId == qrCodeId);

            foreach (var order in orders)
            {
                await _context.Entry(order)
                   .Reference(b => b.ShopInfo).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.AlipayPagePreference).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.PayerInfo).LoadAsync();
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetByFourthPartyIdAsync(string fourthPartyId)
        {
            var orders = _context
                    .Orders
                    .Include(o => o.PayeeInfo)
                    .Where(b => b.PayeeInfo.FourthPartyId == fourthPartyId);

            foreach (var order in orders)
            {
                await _context.Entry(order)
                   .Reference(b => b.ShopInfo).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.AlipayPagePreference).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.PayerInfo).LoadAsync();
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetByShopIdAsync(string shopId)
        {
            var orders = _context
                    .Orders
                    .Include(o => o.ShopInfo)
                    .Where(b => b.ShopInfo.ShopId == shopId);

            foreach (var order in orders)
            {
                await _context.Entry(order)
                    .Reference(b => b.AlipayPagePreference).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.PayerInfo).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.PayeeInfo).LoadAsync();
            }

            return orders;
        }

        public void Delete(Order order)
        {
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
        }

        public async Task<List<Order>> GetAllOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task DeleteFinishedOrder()
        {
            var orders = _context
                    .Orders
                    .Include(o => o.OrderStatus)
                    .Where(o => o.OrderStatus.Id == OrderStatus.Success.Id || o.IsExpired);

            foreach (var order in orders)
            {
                await _context.Entry(order)
                   .Reference(b => b.ShopInfo).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.AlipayPagePreference).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.PayerInfo).LoadAsync();
                await _context.Entry(order)
                    .Reference(b => b.PayeeInfo).LoadAsync();
            }

            _context.Orders.RemoveRange(orders);
        }
    }
}
