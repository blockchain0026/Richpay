using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Ordering.Domain.Model.Orders;
using Ordering.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.CacheServices;
using WebMVC.Data;
using WebMVC.Extensions;
using WebMVC.Models;
using WebMVC.Models.Queries;
using Z.EntityFramework.Plus;

namespace WebMVC.Applications.Queries.Orders
{
    public class OrderQueries : IOrderQueries
    {
        //private readonly OrderingContext _orderingContext;
        private readonly ApplicationDbContext _applicationContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserCacheService _userCacheService;
        private readonly IOrderDailyCacheService _orderDailyCacheService;
        private readonly IApplicationDateTimeService _applicationDateTimeService;

        public OrderQueries(ApplicationDbContext applicationContext, UserManager<ApplicationUser> userManager, IUserCacheService userCacheService, IOrderDailyCacheService orderDailyCacheService, IApplicationDateTimeService applicationDateTimeService)
        {
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userCacheService = userCacheService ?? throw new ArgumentNullException(nameof(userCacheService));
            _orderDailyCacheService = orderDailyCacheService ?? throw new ArgumentNullException(nameof(orderDailyCacheService));
            _applicationDateTimeService = applicationDateTimeService ?? throw new ArgumentNullException(nameof(applicationDateTimeService));
        }

        public async Task<OrderEntry> GetOrderEntryAsync(int orderId)
        {
            return await _applicationContext.OrderEntrys.Where(w => w.OrderId == orderId).FirstOrDefaultAsync();
        }


        public async Task<List<OrderEntry>> GetOrderEntrysAsync(int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null,
            string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null,
            string direction = SortDirections.Descending)
        {
            var result = new List<OrderEntry>();

            IQueryable<OrderEntry> orderEntrys = null;

            if (isExpired is null)
            {
                if (from != null && to != null)
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .AsNoTracking()
                        .Where(o => o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.DateCreated >= from
                        && o.DateCreated <= to
                        )
                        .Select(o => new OrderEntry
                        {
                            OrderId = o.OrderId,
                            TrackingNumber = o.TrackingNumber,
                            ShopOrderId = o.ShopOrderId,
                            OrderPaymentChannel = o.OrderPaymentChannel,
                            OrderPaymentScheme = o.OrderPaymentScheme,
                            ShopId = o.ShopId,
                            ShopUserName = o.ShopUserName,
                            ShopFullName = o.ShopFullName,
                            TraderId = o.TraderUserName,
                            TraderUserName = o.TraderUserName,
                            TraderFullName = o.TraderFullName,
                            QrCodeId = o.QrCodeId,
                            Amount = o.Amount,
                            ShopUserCommissionRealized = o.ShopUserCommissionRealized,
                            TradingUserCommissionRealized = o.TradingUserCommissionRealized,
                            IpAddress = o.IpAddress,
                            PlatformCommissionRealized = o.PlatformCommissionRealized,
                            Device = o.Device,
                            Location = o.Location,
                            OrderStatus = o.OrderStatus,
                            OrderStatusDescription = o.OrderStatusDescription,
                            IsExpired = o.IsExpired,
                            DateCreated = o.DateCreated
                        });
                }
                else
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .AsNoTracking()
                        .Where(o => o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        )
                        .Select(o => new OrderEntry
                        {
                            OrderId = o.OrderId,
                            TrackingNumber = o.TrackingNumber,
                            ShopOrderId = o.ShopOrderId,
                            OrderPaymentChannel = o.OrderPaymentChannel,
                            OrderPaymentScheme = o.OrderPaymentScheme,
                            ShopId = o.ShopId,
                            ShopUserName = o.ShopUserName,
                            ShopFullName = o.ShopFullName,
                            TraderId = o.TraderUserName,
                            TraderUserName = o.TraderUserName,
                            TraderFullName = o.TraderFullName,
                            QrCodeId = o.QrCodeId,
                            Amount = o.Amount,
                            ShopUserCommissionRealized = o.ShopUserCommissionRealized,
                            TradingUserCommissionRealized = o.TradingUserCommissionRealized,
                            IpAddress = o.IpAddress,
                            PlatformCommissionRealized = o.PlatformCommissionRealized,
                            Device = o.Device,
                            Location = o.Location,
                            OrderStatus = o.OrderStatus,
                            OrderStatusDescription = o.OrderStatusDescription,
                            IsExpired = o.IsExpired,
                            DateCreated = o.DateCreated
                        });
                }
            }
            else
            {
                if (from != null && to != null)
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .AsNoTracking()
                        .Where(o => o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.IsExpired == isExpired
                        && o.DateCreated >= from
                        && o.DateCreated <= to
                        )
                        .Select(o => new OrderEntry
                        {
                            OrderId = o.OrderId,
                            TrackingNumber = o.TrackingNumber,
                            ShopOrderId = o.ShopOrderId,
                            OrderPaymentChannel = o.OrderPaymentChannel,
                            OrderPaymentScheme = o.OrderPaymentScheme,
                            ShopId = o.ShopId,
                            ShopUserName = o.ShopUserName,
                            ShopFullName = o.ShopFullName,
                            TraderId = o.TraderUserName,
                            TraderUserName = o.TraderUserName,
                            TraderFullName = o.TraderFullName,
                            QrCodeId = o.QrCodeId,
                            Amount = o.Amount,
                            ShopUserCommissionRealized = o.ShopUserCommissionRealized,
                            TradingUserCommissionRealized = o.TradingUserCommissionRealized,
                            IpAddress = o.IpAddress,
                            PlatformCommissionRealized = o.PlatformCommissionRealized,
                            Device = o.Device,
                            Location = o.Location,
                            OrderStatus = o.OrderStatus,
                            OrderStatusDescription = o.OrderStatusDescription,
                            IsExpired = o.IsExpired,
                            DateCreated = o.DateCreated
                        });
                }
                else
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .AsNoTracking()
                        .Where(o => o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.IsExpired == isExpired
                        )
                        .Select(o => new OrderEntry
                        {
                            OrderId = o.OrderId,
                            TrackingNumber = o.TrackingNumber,
                            ShopOrderId = o.ShopOrderId,
                            OrderPaymentChannel = o.OrderPaymentChannel,
                            OrderPaymentScheme = o.OrderPaymentScheme,
                            ShopId = o.ShopId,
                            ShopUserName = o.ShopUserName,
                            ShopFullName = o.ShopFullName,
                            TraderId = o.TraderUserName,
                            TraderUserName = o.TraderUserName,
                            TraderFullName = o.TraderFullName,
                            QrCodeId = o.QrCodeId,
                            Amount = o.Amount,
                            ShopUserCommissionRealized = o.ShopUserCommissionRealized,
                            TradingUserCommissionRealized = o.TradingUserCommissionRealized,
                            IpAddress = o.IpAddress,
                            PlatformCommissionRealized = o.PlatformCommissionRealized,
                            Device = o.Device,
                            Location = o.Location,
                            OrderStatus = o.OrderStatus,
                            OrderStatusDescription = o.OrderStatusDescription,
                            IsExpired = o.IsExpired,
                            DateCreated = o.DateCreated
                        });
                }
            }

            IQueryable<OrderEntry> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = orderEntrys
                    .Where(w => w.TrackingNumber == searchString
                    || w.ShopOrderId == searchString
                    || w.OrderStatusDescription.Contains(searchString)
                    || w.TraderUserName.Contains(searchString)
                    || w.TraderFullName.Contains(searchString)
                    || w.ShopUserName.Contains(searchString)
                    || w.ShopFullName.Contains(searchString)
                    );
            }
            else
            {
                searchResult = orderEntrys;
            }

            result = await this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            return result;
        }

        public async Task<OrderSumData> GetOrderEntrysTotalSumDataAsync(string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null)
        {
            OrderSumData result = new OrderSumData
            {
                TotalCount = 0,
                SuccessRate = 0,
                TotalOrderAmount = 0,
                TotalOrderSuccessAmount = 0,
                TotalShopUserCommissionAmount = 0,
                TotalTradingUserCommissionAmount = 0,
                TotalPlatformCommissionAmount = 0
            };

            IQueryable<OrderEntry> orderEntrys = null;

            if (isExpired is null)
            {
                if (from != null && to != null)
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .Where(o => o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.DateCreated >= from
                        && o.DateCreated <= to
                        );
                }
                else
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .Where(o => o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        );
                }
            }
            else
            {
                if (from != null && to != null)
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .Where(o => o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.IsExpired == isExpired
                        && o.DateCreated >= from
                        && o.DateCreated <= to
                        );
                }
                else
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .Where(o => o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.IsExpired == isExpired
                        );
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                orderEntrys = orderEntrys
                    .Where(w => w.TrackingNumber == searchString
                    || w.ShopOrderId == searchString
                    || w.OrderStatusDescription.Contains(searchString)
                    || w.TraderUserName.Contains(searchString)
                    || w.TraderFullName.Contains(searchString)
                    || w.ShopUserName.Contains(searchString)
                    || w.ShopFullName.Contains(searchString)
                    );
            }

            //Build sum data.
            //Get each payment scheme's sum data.
            var sumDatas = orderEntrys
                .GroupBy(p => p.IsTestOrder)
                .Select(r => new
                {
                    IsTestOrder = r.Key,
                    TotalCount = r.Count(),

                    //TotalSuccessCount = r.Count(a => a.OrderStatus == "Success"),
                    //In ef core 3.0+, we need to use Sum(condition ? 1:0) to make sql translation success.
                    TotalSuccessCount = r.Sum(a => a.OrderStatus == "Success" ? 1 : 0),

                    TotalOrderAmount = r.Sum(a => a.Amount),
                    TotalOrderSuccessAmount = r.Sum(a => a.AmountPaid),
                    TotalShopUserCommissionAmount = r.Sum(a => a.ShopUserCommissionRealized),
                    TotalTradingUserCommissionAmount = r.Sum(a => a.TradingUserCommissionRealized),
                    TotalPlatformCommissionAmount = r.Sum(a => a.PlatformCommissionRealized),
                });

            var options = new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5) };

            var nonTestSumDatas = await sumDatas
                .Where(s => !s.IsTestOrder)
                .FromCacheAsync(options);

            var nonTestSumData = nonTestSumDatas.FirstOrDefault();


            if (nonTestSumData != null)
            {
                result.TotalCount = nonTestSumData.TotalCount;
                result.SuccessRate = (decimal)nonTestSumData.TotalSuccessCount / nonTestSumData.TotalCount;
                result.TotalOrderAmount = nonTestSumData.TotalOrderAmount;
                result.TotalOrderSuccessAmount = nonTestSumData.TotalOrderSuccessAmount ?? 0M;
                result.TotalShopUserCommissionAmount = nonTestSumData.TotalShopUserCommissionAmount ?? 0M;
                result.TotalTradingUserCommissionAmount = nonTestSumData.TotalTradingUserCommissionAmount ?? 0M;
                result.TotalPlatformCommissionAmount = nonTestSumData.TotalPlatformCommissionAmount ?? 0M;
            }

            return result;
        }

        public async Task<OrderEntriesStatistic> GetOrderEntriesStatisticAsync(string searchString = null,
            DateTime? from = null, DateTime? to = null)
        {
            OrderEntriesStatistic result = new OrderEntriesStatistic
            {
                TotalPlatformProfit = 0,
                TotalShopAgentProfit = 0,
                TotalTradeUserProfit = 0,
                TotalShopReturn = 0,

                TotalPlatformProfitCompare = 0,
                TotalShopAgentProfitCompare = 0,
                TotalTradeUserProfitCompare = 0,

                AlipayBarcodeSuccessRate = 0,
                AlipayMerchantSuccessRate = 0,
                AlipayTransactionSuccessRate = 0,
                AlipayBankSuccessRate = 0,
                AlipayEnvelopSuccessRate = 0,
                WechatBarcodeSuccessRate = 0,

                OrderTotalCount = 0,
                OrderTotalSuccessCount = 0,
                OrderTotalAmount = 0,
                OrderTotalSuccessAmount = 0,
                OrderDailyAverageSuccessAmount = 0,
                OrderAverageRevenueRate = 0,
                OrderSuccessRate = 0,

                OrderChartData = new Dictionary<string, decimal>(),
                ShopRankData = new Dictionary<string, decimal>(),
                TraderRankData = new Dictionary<string, decimal>()
            };

            IQueryable<OrderEntry> orderEntrys = null;

            if (from != null && to != null)
            {
                orderEntrys = _applicationContext.OrderEntrys
                    .Where(o => o.DateCreated >= from && o.DateCreated <= to);
            }
            else
            {
                var utcNow = DateTime.UtcNow;
                from = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day);
                to = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day + 1);
                orderEntrys = _applicationContext.OrderEntrys
                    .Where(o => o.DateCreated >= from && o.DateCreated <= to);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                orderEntrys = orderEntrys
                    .Where(o => o.TraderUserName == searchString
                    || o.TraderFullName == searchString
                    || o.ShopUserName == searchString
                    || o.ShopFullName == searchString
                    );
            }

            //Build sum data.
            //Get each payment scheme's sum data.
            var statistics = orderEntrys
                .GroupBy(p => p.IsTestOrder)
                .Select(r => new
                {
                    IsTestOrder = r.Key,
                    TotalPlatformProfit = r.Sum(a => a.PlatformCommissionRealized),
                    TotalShopAgentProfit = r.Sum(a => a.ShopUserCommissionRealized),
                    TotalTradeUserProfit = r.Sum(a => a.TradingUserCommissionRealized),
                    TotalShopReturn = r.Sum(a => a.AmountPaid - (a.PlatformCommissionRealized + a.ShopUserCommissionRealized + a.TradingUserCommissionRealized)),


                    AlipayBarcodeOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Barcode.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    AlipayBarcodeOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Barcode.Name ? 1 : 0),

                    AlipayMerchantOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Merchant.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    AlipayMerchantOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Merchant.Name ? 1 : 0),

                    AlipayTransactionOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Transaction.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    AlipayTransactionOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Transaction.Name ? 1 : 0),

                    AlipayBankOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Bank.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    AlipayBankOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Bank.Name ? 1 : 0),

                    AlipayEnvelopOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Envelop.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    AlipayEnvelopOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Envelop.Name ? 1 : 0),

                    WechatBarcodeOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Wechat.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Barcode.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    WechatBarcodeOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Wechat.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Barcode.Name ? 1 : 0),


                    OrderTotalCount = r.Count(),
                    OrderTotalSuccessCount = r.Sum(a => a.OrderStatus == "Success" ? 1 : 0),
                    OrderTotalAmount = r.Sum(a => a.Amount),
                    OrderTotalSuccessAmount = r.Sum(a => a.AmountPaid),

                    OrderAverageRevenueRate = r.Average(a => a.PlatformCommissionRealized / a.Amount),
                });

            var options = new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5) };

            var nonTestStatistic = statistics
                .Where(s => !s.IsTestOrder)
                .FromCache(options) //Using cache to improve performance.
                .FirstOrDefault();

            if (nonTestStatistic != null)
            {
                result.TotalPlatformProfit = nonTestStatistic.TotalPlatformProfit ?? 0M;
                result.TotalShopAgentProfit = nonTestStatistic.TotalShopAgentProfit ?? 0M;
                result.TotalTradeUserProfit = nonTestStatistic.TotalTradeUserProfit ?? 0M;
                result.TotalShopReturn = nonTestStatistic.TotalShopReturn ?? 0M;


                result.AlipayBarcodeOrderSuccessCount = nonTestStatistic.AlipayBarcodeOrderSuccessCount;
                result.AlipayBarcodeOrderCount = nonTestStatistic.AlipayBarcodeOrderCount;
                result.AlipayMerchantOrderSuccessCount = nonTestStatistic.AlipayMerchantOrderSuccessCount;
                result.AlipayMerchantOrderCount = nonTestStatistic.AlipayMerchantOrderCount;
                result.AlipayTransactionOrderSuccessCount = nonTestStatistic.AlipayTransactionOrderSuccessCount;
                result.AlipayTransactionOrderCount = nonTestStatistic.AlipayTransactionOrderCount;
                result.AlipayBankOrderSuccessCount = nonTestStatistic.AlipayBankOrderSuccessCount;
                result.AlipayBankOrderCount = nonTestStatistic.AlipayBankOrderCount;
                result.AlipayEnvelopOrderSuccessCount = nonTestStatistic.AlipayEnvelopOrderSuccessCount;
                result.AlipayEnvelopOrderCount = nonTestStatistic.AlipayEnvelopOrderCount;
                result.WechatBarcodeOrderSuccessCount = nonTestStatistic.WechatBarcodeOrderSuccessCount;
                result.WechatBarcodeOrderCount = nonTestStatistic.WechatBarcodeOrderCount;


                result.AlipayBarcodeSuccessRate = nonTestStatistic.AlipayBarcodeOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.AlipayBarcodeOrderSuccessCount / (decimal)nonTestStatistic.AlipayBarcodeOrderCount;
                result.AlipayMerchantSuccessRate = nonTestStatistic.AlipayMerchantOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.AlipayMerchantOrderSuccessCount / (decimal)nonTestStatistic.AlipayMerchantOrderCount;
                result.AlipayTransactionSuccessRate = nonTestStatistic.AlipayTransactionOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.AlipayTransactionOrderSuccessCount / (decimal)nonTestStatistic.AlipayTransactionOrderCount;
                result.AlipayBankSuccessRate = nonTestStatistic.AlipayBankOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.AlipayBankOrderSuccessCount / (decimal)nonTestStatistic.AlipayBankOrderCount;
                result.AlipayEnvelopSuccessRate = nonTestStatistic.AlipayEnvelopOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.AlipayEnvelopOrderSuccessCount / (decimal)nonTestStatistic.AlipayEnvelopOrderCount;
                result.WechatBarcodeSuccessRate = nonTestStatistic.WechatBarcodeOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.WechatBarcodeOrderSuccessCount / (decimal)nonTestStatistic.WechatBarcodeOrderCount;

                result.OrderTotalCount = nonTestStatistic.OrderTotalCount;
                result.OrderTotalSuccessCount = nonTestStatistic.OrderTotalSuccessCount;
                result.OrderTotalAmount = nonTestStatistic.OrderTotalAmount;
                result.OrderTotalSuccessAmount = nonTestStatistic.OrderTotalSuccessAmount ?? 0M;
                result.OrderAverageRevenueRate = nonTestStatistic.OrderAverageRevenueRate ?? 0M;
                result.OrderSuccessRate = nonTestStatistic.OrderTotalSuccessCount == 0 ?
                    0 : (decimal)nonTestStatistic.OrderTotalSuccessCount / (decimal)nonTestStatistic.OrderTotalCount;
            }


            //Order Chart Data
            var orderChartData = orderEntrys
                .GroupBy(x => new
                {
                    x.DateCreated.Hour
                })
                .Select(r => new
                {
                    Key = r.Key,
                    OrderTotalSuccessAmount = r.Sum(a => a.AmountPaid)
                })
                .FromCache(options); //Using cache to improve performance.

            foreach (var hourData in orderChartData)
            {
                result.OrderChartData.Add(hourData.Key.Hour.ToString(), hourData.OrderTotalSuccessAmount ?? 0M);
            }

            //Shop Rank Data
            var shopRankData = orderEntrys
                .GroupBy(x => x.ShopFullName)
                .Select(r => new
                {
                    Key = r.Key,
                    OrderTotalSuccessAmount = r.Sum(a => a.AmountPaid)
                })
                .OrderByDescending(o => o.OrderTotalSuccessAmount)
                .Take(5)
                .FromCache(options); //Using cache to improve performance.


            foreach (var rankData in shopRankData)
            {
                result.ShopRankData.Add(rankData.Key.ToString(), rankData.OrderTotalSuccessAmount ?? 0M);
            }

            //Trader Rank Data
            var traderRankData = orderEntrys
                .GroupBy(x => x.TraderFullName)
                .Select(r => new
                {
                    Key = r.Key,
                    OrderTotalSuccessAmount = r.Sum(a => a.AmountPaid)
                })
                .OrderByDescending(o => o.OrderTotalSuccessAmount)
                .Take(5)
                .FromCache(options); //Using cache to improve performance.

            foreach (var rankData in traderRankData)
            {
                if (!string.IsNullOrEmpty(rankData.Key))
                {
                    result.TraderRankData.Add(rankData.Key.ToString(), rankData.OrderTotalSuccessAmount ?? 0M);
                }
            }

            return result;
        }


        public async Task<List<OrderEntry>> GetOrderEntrysByTraderIdAsync(string traderId,
            int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null,
            string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null,
            string direction = SortDirections.Descending)
        {
            var result = new List<OrderEntry>();

            IQueryable<OrderEntry> orderEntrys = null;
            var orderEntries = _applicationContext.OrderEntrys.ToList();
            if (isExpired is null)
            {
                if (from != null && to != null)
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .AsNoTracking()
                        .Where(o => o.TraderId == traderId
                        && o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.DateCreated >= from
                        && o.DateCreated <= to
                        )
                        .Select(o => new OrderEntry
                        {
                            OrderId = o.OrderId,
                            TrackingNumber = o.TrackingNumber,
                            ShopOrderId = o.ShopOrderId,
                            OrderPaymentChannel = o.OrderPaymentChannel,
                            OrderPaymentScheme = o.OrderPaymentScheme,
                            ShopId = o.ShopId,
                            ShopUserName = o.ShopUserName,
                            ShopFullName = o.ShopFullName,
                            TraderId = o.TraderUserName,
                            TraderUserName = o.TraderUserName,
                            TraderFullName = o.TraderFullName,
                            QrCodeId = o.QrCodeId,
                            Amount = o.Amount,
                            ShopUserCommissionRealized = o.ShopUserCommissionRealized,
                            TradingUserCommissionRealized = o.TradingUserCommissionRealized,
                            IpAddress = o.IpAddress,
                            PlatformCommissionRealized = o.PlatformCommissionRealized,
                            Device = o.Device,
                            Location = o.Location,
                            OrderStatus = o.OrderStatus,
                            OrderStatusDescription = o.OrderStatusDescription,
                            IsExpired = o.IsExpired,
                            DateCreated = o.DateCreated
                        });
                }
                else
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .AsNoTracking()
                        .Where(o => o.TraderId == traderId
                        && o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        )
                        .Select(o => new OrderEntry
                        {
                            OrderId = o.OrderId,
                            TrackingNumber = o.TrackingNumber,
                            ShopOrderId = o.ShopOrderId,
                            OrderPaymentChannel = o.OrderPaymentChannel,
                            OrderPaymentScheme = o.OrderPaymentScheme,
                            ShopId = o.ShopId,
                            ShopUserName = o.ShopUserName,
                            ShopFullName = o.ShopFullName,
                            TraderId = o.TraderUserName,
                            TraderUserName = o.TraderUserName,
                            TraderFullName = o.TraderFullName,
                            QrCodeId = o.QrCodeId,
                            Amount = o.Amount,
                            ShopUserCommissionRealized = o.ShopUserCommissionRealized,
                            TradingUserCommissionRealized = o.TradingUserCommissionRealized,
                            IpAddress = o.IpAddress,
                            PlatformCommissionRealized = o.PlatformCommissionRealized,
                            Device = o.Device,
                            Location = o.Location,
                            OrderStatus = o.OrderStatus,
                            OrderStatusDescription = o.OrderStatusDescription,
                            IsExpired = o.IsExpired,
                            DateCreated = o.DateCreated
                        });
                }
            }
            else
            {
                if (from != null && to != null)
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .AsNoTracking()
                        .Where(o => o.TraderId == traderId
                        && o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.IsExpired == isExpired
                        && o.DateCreated >= from
                        && o.DateCreated <= to
                        )
                        .Select(o => new OrderEntry
                        {
                            OrderId = o.OrderId,
                            TrackingNumber = o.TrackingNumber,
                            ShopOrderId = o.ShopOrderId,
                            OrderPaymentChannel = o.OrderPaymentChannel,
                            OrderPaymentScheme = o.OrderPaymentScheme,
                            ShopId = o.ShopId,
                            ShopUserName = o.ShopUserName,
                            ShopFullName = o.ShopFullName,
                            TraderId = o.TraderUserName,
                            TraderUserName = o.TraderUserName,
                            TraderFullName = o.TraderFullName,
                            QrCodeId = o.QrCodeId,
                            Amount = o.Amount,
                            ShopUserCommissionRealized = o.ShopUserCommissionRealized,
                            TradingUserCommissionRealized = o.TradingUserCommissionRealized,
                            IpAddress = o.IpAddress,
                            PlatformCommissionRealized = o.PlatformCommissionRealized,
                            Device = o.Device,
                            Location = o.Location,
                            OrderStatus = o.OrderStatus,
                            OrderStatusDescription = o.OrderStatusDescription,
                            IsExpired = o.IsExpired,
                            DateCreated = o.DateCreated
                        });
                }
                else
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .AsNoTracking()
                        .Where(o => o.TraderId == traderId
                        && o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.IsExpired == isExpired
                        )
                        .Select(o => new OrderEntry
                        {
                            OrderId = o.OrderId,
                            TrackingNumber = o.TrackingNumber,
                            ShopOrderId = o.ShopOrderId,
                            OrderPaymentChannel = o.OrderPaymentChannel,
                            OrderPaymentScheme = o.OrderPaymentScheme,
                            ShopId = o.ShopId,
                            ShopUserName = o.ShopUserName,
                            ShopFullName = o.ShopFullName,
                            TraderId = o.TraderUserName,
                            TraderUserName = o.TraderUserName,
                            TraderFullName = o.TraderFullName,
                            QrCodeId = o.QrCodeId,
                            Amount = o.Amount,
                            ShopUserCommissionRealized = o.ShopUserCommissionRealized,
                            TradingUserCommissionRealized = o.TradingUserCommissionRealized,
                            IpAddress = o.IpAddress,
                            PlatformCommissionRealized = o.PlatformCommissionRealized,
                            Device = o.Device,
                            Location = o.Location,
                            OrderStatus = o.OrderStatus,
                            OrderStatusDescription = o.OrderStatusDescription,
                            IsExpired = o.IsExpired,
                            DateCreated = o.DateCreated
                        });
                }
            }

            IQueryable<OrderEntry> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = orderEntrys
                    .Where(w => w.TrackingNumber == searchString
                    || w.ShopOrderId == searchString
                    || w.OrderStatusDescription.Contains(searchString)
                    || w.TraderUserName.Contains(searchString)
                    || w.TraderFullName.Contains(searchString)
                    || w.ShopUserName.Contains(searchString)
                    || w.ShopFullName.Contains(searchString)
                    );
            }
            else
            {
                searchResult = orderEntrys;
            }

            result = await this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            return result;
        }

        public async Task<OrderSumData> GetOrderEntrysTotalSumDataByTraderIdAsync(string traderId,
            string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null)
        {

            OrderSumData result = new OrderSumData
            {
                TotalCount = 0,
                SuccessRate = 0,
                TotalOrderAmount = 0,
                TotalOrderSuccessAmount = 0,
                //TotalShopUserCommissionAmount = 0,
                //TotalTradingUserCommissionAmount = 0,
                //TotalPlatformCommissionAmount = 0
                TotalTraderCommissionAmount = 0
            };

            IQueryable<OrderEntry> orderEntrys = null;

            if (isExpired is null)
            {
                if (from != null && to != null)
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .Where(o => o.TraderId == traderId
                        && o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.DateCreated >= from
                        && o.DateCreated <= to
                        );
                }
                else
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .Where(o => o.TraderId == traderId
                        && o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        );
                }
            }
            else
            {
                if (from != null && to != null)
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .Where(o => o.TraderId == traderId
                        && o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.IsExpired == isExpired
                        && o.DateCreated >= from
                        && o.DateCreated <= to
                        );
                }
                else
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .Where(o => o.TraderId == traderId
                        && o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.IsExpired == isExpired
                        );
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                orderEntrys = orderEntrys
                    .Where(w => w.TrackingNumber == searchString
                    || w.ShopOrderId == searchString
                    || w.OrderStatusDescription.Contains(searchString)
                    || w.TraderUserName.Contains(searchString)
                    || w.TraderFullName.Contains(searchString)
                    || w.ShopUserName.Contains(searchString)
                    || w.ShopFullName.Contains(searchString)
                    );
            }

            //Build sum data.
            //Get each payment scheme's sum data.
            var sumDatas = orderEntrys
                .GroupBy(p => p.IsTestOrder)
                .Select(r => new
                {
                    IsTestOrder = r.Key,
                    TotalCount = r.Count(),

                    //TotalSuccessCount = r.Count(a => a.OrderStatus == "Success"),
                    //In ef core 3.0+, we need to use Sum(condition ? 1:0) to make sql translation success.
                    TotalSuccessCount = r.Sum(a => a.OrderStatus == "Success" ? 1 : 0),

                    TotalOrderAmount = r.Sum(a => a.Amount),
                    TotalOrderSuccessAmount = r.Sum(a => a.AmountPaid),
                    TotalTraderCommissionAmount = r.Sum(a => a.TraderCommissionRealized),
                });

            var nonTestSumData = await sumDatas
                .Where(s => !s.IsTestOrder)
                .FirstOrDefaultAsync();


            if (nonTestSumData != null)
            {
                result.TotalCount = nonTestSumData.TotalCount;
                result.SuccessRate = (decimal)nonTestSumData.TotalSuccessCount / nonTestSumData.TotalCount;
                result.TotalOrderAmount = nonTestSumData.TotalOrderAmount;
                result.TotalOrderSuccessAmount = nonTestSumData.TotalOrderSuccessAmount ?? 0M;
                result.TotalTraderCommissionAmount = nonTestSumData.TotalTraderCommissionAmount ?? 0M;
            }

            return result;
        }

        public async Task<OrderEntriesStatistic> GetOrderEntriesStatisticByTraderIdAsync(string traderId,
            string searchString = null,
            DateTime? from = null, DateTime? to = null)
        {
            OrderEntriesStatistic result = new OrderEntriesStatistic
            {
                //TotalPlatformProfit = 0,
                //TotalShopAgentProfit = 0,
                TotalTraderProfit = 0,
                //TotalShopReturn = 0,

                //TotalPlatformProfitCompare = 0,
                //TotalShopAgentProfitCompare = 0,
                //TotalTradeUserProfitCompare = 0,

                AlipayBarcodeSuccessRate = 0,
                AlipayMerchantSuccessRate = 0,
                AlipayTransactionSuccessRate = 0,
                AlipayBankSuccessRate = 0,
                AlipayEnvelopSuccessRate = 0,
                WechatBarcodeSuccessRate = 0,

                OrderTotalCount = 0,
                OrderTotalSuccessCount = 0,
                OrderTotalAmount = 0,
                OrderTotalSuccessAmount = 0,
                OrderDailyAverageSuccessAmount = 0,
                OrderAverageRevenueRate = 0,
                OrderSuccessRate = 0,

                OrderChartData = new Dictionary<string, decimal>(),
                //ShopRankData = new Dictionary<string, decimal>(),
                //TraderRankData = new Dictionary<string, decimal>()
            };

            IQueryable<OrderEntry> orderEntrys = null;

            if (from != null && to != null)
            {
                orderEntrys = _applicationContext.OrderEntrys
                    .Where(o => o.TraderId == traderId && o.DateCreated >= from && o.DateCreated <= to);
            }
            else
            {
                from = DateTime.UtcNow.AddDays(-1);
                to = DateTime.UtcNow;
                orderEntrys = _applicationContext.OrderEntrys
                    .Where(o => o.TraderId == traderId && o.DateCreated >= from && o.DateCreated <= to);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                orderEntrys = orderEntrys
                    .Where(o => o.TraderUserName == searchString
                    || o.TraderFullName == searchString
                    || o.ShopUserName == searchString
                    || o.ShopFullName == searchString
                    );
            }

            //Build sum data.
            //Get each payment scheme's sum data.
            var statistics = orderEntrys
                .GroupBy(p => p.IsTestOrder)
                .Select(r => new
                {
                    IsTestOrder = r.Key,
                    TotalTraderProfit = r.Sum(a => a.TraderCommissionRealized),

                    AlipayBarcodeOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Barcode.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    AlipayBarcodeOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Barcode.Name ? 1 : 0),

                    AlipayMerchantOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Merchant.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    AlipayMerchantOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Merchant.Name ? 1 : 0),

                    AlipayTransactionOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Transaction.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    AlipayTransactionOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Transaction.Name ? 1 : 0),

                    AlipayBankOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Bank.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    AlipayBankOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Bank.Name ? 1 : 0),

                    AlipayEnvelopOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Envelop.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    AlipayEnvelopOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Envelop.Name ? 1 : 0),

                    WechatBarcodeOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Wechat.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Barcode.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    WechatBarcodeOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Wechat.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Barcode.Name ? 1 : 0),


                    OrderTotalCount = r.Count(),
                    OrderTotalSuccessCount = r.Sum(a => a.OrderStatus == "Success" ? 1 : 0),
                    OrderTotalAmount = r.Sum(a => a.Amount),
                    OrderTotalSuccessAmount = r.Sum(a => a.AmountPaid),

                    OrderAverageRevenueRate = r.Average(a => a.PlatformCommissionRealized / a.Amount),
                });

            var nonTestStatistic = await statistics
                .Where(s => !s.IsTestOrder)
                .FirstOrDefaultAsync();

            if (nonTestStatistic != null)
            {
                result.TotalTraderProfit = nonTestStatistic.TotalTraderProfit ?? 0M;


                result.AlipayBarcodeOrderSuccessCount = nonTestStatistic.AlipayBarcodeOrderSuccessCount;
                result.AlipayBarcodeOrderCount = nonTestStatistic.AlipayBarcodeOrderCount;
                result.AlipayMerchantOrderSuccessCount = nonTestStatistic.AlipayMerchantOrderSuccessCount;
                result.AlipayMerchantOrderCount = nonTestStatistic.AlipayMerchantOrderCount;
                result.AlipayTransactionOrderSuccessCount = nonTestStatistic.AlipayTransactionOrderSuccessCount;
                result.AlipayTransactionOrderCount = nonTestStatistic.AlipayTransactionOrderCount;
                result.AlipayBankOrderSuccessCount = nonTestStatistic.AlipayBankOrderSuccessCount;
                result.AlipayBankOrderCount = nonTestStatistic.AlipayBankOrderCount;
                result.AlipayEnvelopOrderSuccessCount = nonTestStatistic.AlipayEnvelopOrderSuccessCount;
                result.AlipayEnvelopOrderCount = nonTestStatistic.AlipayEnvelopOrderCount;
                result.WechatBarcodeOrderSuccessCount = nonTestStatistic.WechatBarcodeOrderSuccessCount;
                result.WechatBarcodeOrderCount = nonTestStatistic.WechatBarcodeOrderCount;


                result.AlipayBarcodeSuccessRate = nonTestStatistic.AlipayBarcodeOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.AlipayBarcodeOrderSuccessCount / (decimal)nonTestStatistic.AlipayBarcodeOrderCount;
                result.AlipayMerchantSuccessRate = nonTestStatistic.AlipayMerchantOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.AlipayMerchantOrderSuccessCount / (decimal)nonTestStatistic.AlipayMerchantOrderCount;
                result.AlipayTransactionSuccessRate = nonTestStatistic.AlipayTransactionOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.AlipayTransactionOrderSuccessCount / (decimal)nonTestStatistic.AlipayTransactionOrderCount;
                result.AlipayBankSuccessRate = nonTestStatistic.AlipayBankOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.AlipayBankOrderSuccessCount / (decimal)nonTestStatistic.AlipayBankOrderCount;
                result.AlipayEnvelopSuccessRate = nonTestStatistic.AlipayEnvelopOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.AlipayEnvelopOrderSuccessCount / (decimal)nonTestStatistic.AlipayEnvelopOrderCount;
                result.WechatBarcodeSuccessRate = nonTestStatistic.WechatBarcodeOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.WechatBarcodeOrderSuccessCount / (decimal)nonTestStatistic.WechatBarcodeOrderCount;

                result.OrderTotalCount = nonTestStatistic.OrderTotalCount;
                result.OrderTotalSuccessCount = nonTestStatistic.OrderTotalSuccessCount;
                result.OrderTotalAmount = nonTestStatistic.OrderTotalAmount;
                result.OrderTotalSuccessAmount = nonTestStatistic.OrderTotalSuccessAmount ?? 0M;
                result.OrderAverageRevenueRate = nonTestStatistic.OrderAverageRevenueRate ?? 0M;
                result.OrderSuccessRate = nonTestStatistic.OrderTotalSuccessCount == 0 ?
                    0 : (decimal)nonTestStatistic.OrderTotalSuccessCount / (decimal)nonTestStatistic.OrderTotalCount;
            }


            //Order Chart Data
            var orderChartData = orderEntrys
                .GroupBy(x => new
                {
                    x.DateCreated.Hour
                })
                .Select(r => new
                {
                    Key = r.Key,
                    OrderTotalSuccessAmount = r.Sum(a => a.AmountPaid)
                });

            foreach (var hourData in orderChartData)
            {
                result.OrderChartData.Add(hourData.Key.Hour.ToString(), hourData.OrderTotalSuccessAmount ?? 0M);
            }

            return result;
        }



        public async Task<List<OrderEntry>> GetOrderEntrysByShopIdAsync(string shopId,
            int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null,
            string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null,
            string direction = SortDirections.Descending)
        {
            var result = new List<OrderEntry>();

            IQueryable<OrderEntry> orderEntrys = null;

            if (isExpired is null)
            {
                if (from != null && to != null)
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .AsNoTracking()
                        .Where(o => o.ShopId == shopId
                        && o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.DateCreated >= from
                        && o.DateCreated <= to
                        )
                        .Select(o => new OrderEntry
                        {
                            OrderId = o.OrderId,
                            TrackingNumber = o.TrackingNumber,
                            ShopOrderId = o.ShopOrderId,
                            OrderPaymentChannel = o.OrderPaymentChannel,
                            OrderPaymentScheme = o.OrderPaymentScheme,
                            ShopId = o.ShopId,
                            ShopUserName = o.ShopUserName,
                            ShopFullName = o.ShopFullName,
                            TraderId = o.TraderUserName,
                            TraderUserName = o.TraderUserName,
                            TraderFullName = o.TraderFullName,
                            QrCodeId = o.QrCodeId,
                            Amount = o.Amount,
                            ShopUserCommissionRealized = o.ShopUserCommissionRealized,
                            TradingUserCommissionRealized = o.TradingUserCommissionRealized,
                            IpAddress = o.IpAddress,
                            PlatformCommissionRealized = o.PlatformCommissionRealized,
                            Device = o.Device,
                            Location = o.Location,
                            OrderStatus = o.OrderStatus,
                            OrderStatusDescription = o.OrderStatusDescription,
                            IsExpired = o.IsExpired,
                            DateCreated = o.DateCreated
                        });
                }
                else
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .AsNoTracking()
                        .Where(o => o.ShopId == shopId
                        && o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        )
                        .Select(o => new OrderEntry
                        {
                            OrderId = o.OrderId,
                            TrackingNumber = o.TrackingNumber,
                            ShopOrderId = o.ShopOrderId,
                            OrderPaymentChannel = o.OrderPaymentChannel,
                            OrderPaymentScheme = o.OrderPaymentScheme,
                            ShopId = o.ShopId,
                            ShopUserName = o.ShopUserName,
                            ShopFullName = o.ShopFullName,
                            TraderId = o.TraderUserName,
                            TraderUserName = o.TraderUserName,
                            TraderFullName = o.TraderFullName,
                            QrCodeId = o.QrCodeId,
                            Amount = o.Amount,
                            ShopUserCommissionRealized = o.ShopUserCommissionRealized,
                            TradingUserCommissionRealized = o.TradingUserCommissionRealized,
                            IpAddress = o.IpAddress,
                            PlatformCommissionRealized = o.PlatformCommissionRealized,
                            Device = o.Device,
                            Location = o.Location,
                            OrderStatus = o.OrderStatus,
                            OrderStatusDescription = o.OrderStatusDescription,
                            IsExpired = o.IsExpired,
                            DateCreated = o.DateCreated
                        });
                }
            }
            else
            {
                if (from != null && to != null)
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .AsNoTracking()
                        .Where(o => o.ShopId == shopId
                        && o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.IsExpired == isExpired
                        && o.DateCreated >= from
                        && o.DateCreated <= to
                        )
                        .Select(o => new OrderEntry
                        {
                            OrderId = o.OrderId,
                            TrackingNumber = o.TrackingNumber,
                            ShopOrderId = o.ShopOrderId,
                            OrderPaymentChannel = o.OrderPaymentChannel,
                            OrderPaymentScheme = o.OrderPaymentScheme,
                            ShopId = o.ShopId,
                            ShopUserName = o.ShopUserName,
                            ShopFullName = o.ShopFullName,
                            TraderId = o.TraderUserName,
                            TraderUserName = o.TraderUserName,
                            TraderFullName = o.TraderFullName,
                            QrCodeId = o.QrCodeId,
                            Amount = o.Amount,
                            ShopUserCommissionRealized = o.ShopUserCommissionRealized,
                            TradingUserCommissionRealized = o.TradingUserCommissionRealized,
                            IpAddress = o.IpAddress,
                            PlatformCommissionRealized = o.PlatformCommissionRealized,
                            Device = o.Device,
                            Location = o.Location,
                            OrderStatus = o.OrderStatus,
                            OrderStatusDescription = o.OrderStatusDescription,
                            IsExpired = o.IsExpired,
                            DateCreated = o.DateCreated
                        });
                }
                else
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .AsNoTracking()
                        .Where(o => o.ShopId == shopId
                        && o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.IsExpired == isExpired
                        )
                        .Select(o => new OrderEntry
                        {
                            OrderId = o.OrderId,
                            TrackingNumber = o.TrackingNumber,
                            ShopOrderId = o.ShopOrderId,
                            OrderPaymentChannel = o.OrderPaymentChannel,
                            OrderPaymentScheme = o.OrderPaymentScheme,
                            ShopId = o.ShopId,
                            ShopUserName = o.ShopUserName,
                            ShopFullName = o.ShopFullName,
                            TraderId = o.TraderUserName,
                            TraderUserName = o.TraderUserName,
                            TraderFullName = o.TraderFullName,
                            QrCodeId = o.QrCodeId,
                            Amount = o.Amount,
                            ShopUserCommissionRealized = o.ShopUserCommissionRealized,
                            TradingUserCommissionRealized = o.TradingUserCommissionRealized,
                            IpAddress = o.IpAddress,
                            PlatformCommissionRealized = o.PlatformCommissionRealized,
                            Device = o.Device,
                            Location = o.Location,
                            OrderStatus = o.OrderStatus,
                            OrderStatusDescription = o.OrderStatusDescription,
                            IsExpired = o.IsExpired,
                            DateCreated = o.DateCreated
                        });
                }
            }

            IQueryable<OrderEntry> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = orderEntrys
                    .Where(w => w.TrackingNumber == searchString
                    || w.ShopOrderId == searchString
                    || w.OrderStatusDescription.Contains(searchString)
                    || w.TraderUserName.Contains(searchString)
                    || w.TraderFullName.Contains(searchString)
                    || w.ShopUserName.Contains(searchString)
                    || w.ShopFullName.Contains(searchString)
                    );
            }
            else
            {
                searchResult = orderEntrys;
            }

            result = await this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            return result;
        }

        public async Task<OrderSumData> GetOrderEntrysTotalSumDataByShopIdAsync(string shopId,
            string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null)
        {
            OrderSumData result = new OrderSumData
            {
                TotalCount = 0,
                SuccessRate = 0,
                TotalOrderAmount = 0,
                TotalOrderSuccessAmount = 0,
                TotalShopCommissionAmount = 0
            };

            IQueryable<OrderEntry> orderEntrys = null;

            if (isExpired is null)
            {
                if (from != null && to != null)
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .Where(o => o.ShopId == shopId
                        && o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.DateCreated >= from
                        && o.DateCreated <= to
                        );
                }
                else
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .Where(o => o.ShopId == shopId
                        && o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        );
                }
            }
            else
            {
                if (from != null && to != null)
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .Where(o => o.ShopId == shopId
                        && o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.IsExpired == isExpired
                        && o.DateCreated >= from
                        && o.DateCreated <= to
                        );
                }
                else
                {
                    orderEntrys = _applicationContext.OrderEntrys
                        .Where(o => o.ShopId == shopId
                        && o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.IsExpired == isExpired
                        );
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                orderEntrys = orderEntrys
                    .Where(w => w.TrackingNumber == searchString
                    || w.ShopOrderId == searchString
                    || w.OrderStatusDescription.Contains(searchString)
                    || w.TraderUserName.Contains(searchString)
                    || w.TraderFullName.Contains(searchString)
                    || w.ShopUserName.Contains(searchString)
                    || w.ShopFullName.Contains(searchString)
                    );
            }

            //Build sum data.
            //Get each payment scheme's sum data.
            var sumDatas = orderEntrys
                .GroupBy(p => p.IsTestOrder)
                .Select(r => new
                {
                    IsTestOrder = r.Key,
                    TotalCount = r.Count(),

                    //TotalSuccessCount = r.Count(a => a.OrderStatus == "Success"),
                    //In ef core 3.0+, we need to use Sum(condition ? 1:0) to make sql translation success.
                    TotalSuccessCount = r.Sum(a => a.OrderStatus == "Success" ? 1 : 0),

                    TotalOrderAmount = r.Sum(a => a.Amount),
                    TotalOrderSuccessAmount = r.Sum(a => a.AmountPaid),
                    TotalShopCommissionAmount = r.Sum(a => a.ShopCommissionRealized),
                });

            var nonTestSumData = await sumDatas
                .Where(s => !s.IsTestOrder)
                .FirstOrDefaultAsync();


            if (nonTestSumData != null)
            {
                result.TotalCount = nonTestSumData.TotalCount;
                result.SuccessRate = (decimal)nonTestSumData.TotalSuccessCount / nonTestSumData.TotalCount;
                result.TotalOrderAmount = nonTestSumData.TotalOrderAmount;
                result.TotalOrderSuccessAmount = nonTestSumData.TotalOrderSuccessAmount ?? 0M;
                result.TotalShopCommissionAmount = nonTestSumData.TotalShopCommissionAmount ?? 0M;
            }

            return result;
        }

        public async Task<OrderEntriesStatistic> GetOrderEntriesStatisticByShopIdAsync(string shopId, string searchString = null,
            DateTime? from = null, DateTime? to = null)
        {
            OrderEntriesStatistic result = new OrderEntriesStatistic
            {
                TotalShopReturn = 0,

                AlipayBarcodeSuccessRate = 0,
                AlipayMerchantSuccessRate = 0,
                AlipayTransactionSuccessRate = 0,
                AlipayBankSuccessRate = 0,
                AlipayEnvelopSuccessRate = 0,
                WechatBarcodeSuccessRate = 0,

                OrderTotalCount = 0,
                OrderTotalSuccessCount = 0,
                OrderTotalAmount = 0,
                OrderTotalSuccessAmount = 0,
                OrderDailyAverageSuccessAmount = 0,
                OrderAverageRevenueRate = 0,
                OrderSuccessRate = 0,

                OrderChartData = new Dictionary<string, decimal>(),
                ShopRankData = new Dictionary<string, decimal>(),
                TraderRankData = new Dictionary<string, decimal>()
            };

            IQueryable<OrderEntry> orderEntrys = null;

            if (from != null && to != null)
            {
                orderEntrys = _applicationContext.OrderEntrys
                    .Where(o => o.ShopId == shopId && o.DateCreated >= from && o.DateCreated <= to);
            }
            else
            {
                from = DateTime.UtcNow.AddDays(-1);
                to = DateTime.UtcNow;
                orderEntrys = _applicationContext.OrderEntrys
                    .Where(o => o.ShopId == shopId && o.DateCreated >= from && o.DateCreated <= to);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                orderEntrys = orderEntrys
                    .Where(o => o.TraderUserName == searchString
                    || o.TraderFullName == searchString
                    || o.ShopUserName == searchString
                    || o.ShopFullName == searchString
                    );
            }

            //Build sum data.
            //Get each payment scheme's sum data.
            var statistics = orderEntrys
                .GroupBy(p => p.IsTestOrder)
                .Select(r => new
                {
                    IsTestOrder = r.Key,
                    TotalShopReturn = r.Sum(a => a.AmountPaid - (a.PlatformCommissionRealized + a.ShopUserCommissionRealized + a.TradingUserCommissionRealized)),


                    AlipayBarcodeOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Barcode.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    AlipayBarcodeOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Barcode.Name ? 1 : 0),

                    AlipayMerchantOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Merchant.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    AlipayMerchantOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Merchant.Name ? 1 : 0),

                    AlipayTransactionOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Transaction.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    AlipayTransactionOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Transaction.Name ? 1 : 0),

                    AlipayBankOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Bank.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    AlipayBankOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Bank.Name ? 1 : 0),

                    AlipayEnvelopOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Envelop.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    AlipayEnvelopOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Alipay.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Envelop.Name ? 1 : 0),

                    WechatBarcodeOrderSuccessCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Wechat.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Barcode.Name
                    && a.OrderStatus == "Success" ? 1 : 0),
                    WechatBarcodeOrderCount = r.Sum(a =>
                    a.OrderPaymentChannel == OrderPaymentChannel.Wechat.Name
                    && a.OrderPaymentScheme == OrderPaymentScheme.Barcode.Name ? 1 : 0),


                    OrderTotalCount = r.Count(),
                    OrderTotalSuccessCount = r.Sum(a => a.OrderStatus == "Success" ? 1 : 0),
                    OrderTotalAmount = r.Sum(a => a.Amount),
                    OrderTotalSuccessAmount = r.Sum(a => a.AmountPaid),

                    OrderAverageRevenueRate = r.Average(a => a.PlatformCommissionRealized / a.Amount),
                });

            var nonTestStatistic = await statistics
                .Where(s => !s.IsTestOrder)
                .FirstOrDefaultAsync();

            if (nonTestStatistic != null)
            {
                result.TotalShopReturn = nonTestStatistic.TotalShopReturn ?? 0M;


                result.AlipayBarcodeOrderSuccessCount = nonTestStatistic.AlipayBarcodeOrderSuccessCount;
                result.AlipayBarcodeOrderCount = nonTestStatistic.AlipayBarcodeOrderCount;
                result.AlipayMerchantOrderSuccessCount = nonTestStatistic.AlipayMerchantOrderSuccessCount;
                result.AlipayMerchantOrderCount = nonTestStatistic.AlipayMerchantOrderCount;
                result.AlipayTransactionOrderSuccessCount = nonTestStatistic.AlipayTransactionOrderSuccessCount;
                result.AlipayTransactionOrderCount = nonTestStatistic.AlipayTransactionOrderCount;
                result.AlipayBankOrderSuccessCount = nonTestStatistic.AlipayBankOrderSuccessCount;
                result.AlipayBankOrderCount = nonTestStatistic.AlipayBankOrderCount;
                result.AlipayEnvelopOrderSuccessCount = nonTestStatistic.AlipayEnvelopOrderSuccessCount;
                result.AlipayEnvelopOrderCount = nonTestStatistic.AlipayEnvelopOrderCount;
                result.WechatBarcodeOrderSuccessCount = nonTestStatistic.WechatBarcodeOrderSuccessCount;
                result.WechatBarcodeOrderCount = nonTestStatistic.WechatBarcodeOrderCount;


                result.AlipayBarcodeSuccessRate = nonTestStatistic.AlipayBarcodeOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.AlipayBarcodeOrderSuccessCount / (decimal)nonTestStatistic.AlipayBarcodeOrderCount;
                result.AlipayMerchantSuccessRate = nonTestStatistic.AlipayMerchantOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.AlipayMerchantOrderSuccessCount / (decimal)nonTestStatistic.AlipayMerchantOrderCount;
                result.AlipayTransactionSuccessRate = nonTestStatistic.AlipayTransactionOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.AlipayTransactionOrderSuccessCount / (decimal)nonTestStatistic.AlipayTransactionOrderCount;
                result.AlipayBankSuccessRate = nonTestStatistic.AlipayBankOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.AlipayBankOrderSuccessCount / (decimal)nonTestStatistic.AlipayBankOrderCount;
                result.AlipayEnvelopSuccessRate = nonTestStatistic.AlipayEnvelopOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.AlipayEnvelopOrderSuccessCount / (decimal)nonTestStatistic.AlipayEnvelopOrderCount;
                result.WechatBarcodeSuccessRate = nonTestStatistic.WechatBarcodeOrderCount == 0 ?
                    0 : (decimal)nonTestStatistic.WechatBarcodeOrderSuccessCount / (decimal)nonTestStatistic.WechatBarcodeOrderCount;

                result.OrderTotalCount = nonTestStatistic.OrderTotalCount;
                result.OrderTotalSuccessCount = nonTestStatistic.OrderTotalSuccessCount;
                result.OrderTotalAmount = nonTestStatistic.OrderTotalAmount;
                result.OrderTotalSuccessAmount = nonTestStatistic.OrderTotalSuccessAmount ?? 0M;
                result.OrderAverageRevenueRate = nonTestStatistic.OrderAverageRevenueRate ?? 0M;
                result.OrderSuccessRate = nonTestStatistic.OrderTotalSuccessCount == 0 ?
                    0 : (decimal)nonTestStatistic.OrderTotalSuccessCount / (decimal)nonTestStatistic.OrderTotalCount;
            }


            //Order Chart Data
            var orderChartData = orderEntrys
                .GroupBy(x => new
                {
                    x.DateCreated.Hour
                })
                .Select(r => new
                {
                    Key = r.Key,
                    OrderTotalSuccessAmount = r.Sum(a => a.AmountPaid)
                });

            foreach (var hourData in orderChartData)
            {
                result.OrderChartData.Add(hourData.Key.Hour.ToString(), hourData.OrderTotalSuccessAmount ?? 0M);
            }

            return result;
        }



        public async Task<List<OrderEntry>> GetOrderEntrysByQrCodeIdAsync(int qrCodeId,
            int? pageIndex, int? take, string searchString = null, string sortField = null,
            string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null,
            string direction = SortDirections.Descending)
        {
            var result = new List<OrderEntry>();

            IQueryable<OrderEntry> orderEntrys = null;

            if (isExpired is null)
            {
                orderEntrys = _applicationContext.OrderEntrys
                    .Where(o => o.QrCodeId == qrCodeId
                    && o.OrderType.Contains(type ?? string.Empty)
                    && o.OrderStatus.Contains(status ?? string.Empty)
                    && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                    && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                    );
            }
            else
            {
                orderEntrys = _applicationContext.OrderEntrys
                    .Where(o => o.QrCodeId == qrCodeId
                    && o.OrderType.Contains(type ?? string.Empty)
                    && o.OrderStatus.Contains(status ?? string.Empty)
                    && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                    && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                    && o.IsExpired == isExpired
                    );
            }

            IQueryable<OrderEntry> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = orderEntrys
                    .Where(w => w.TrackingNumber == searchString
                    || w.OrderStatusDescription.Contains(searchString)
                    || w.TraderUserName.Contains(searchString)
                    || w.TraderFullName.Contains(searchString)
                    );
            }
            else
            {
                searchResult = orderEntrys;
            }

            result = await this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            return result;
        }

        public async Task<int> GetOrderEntrysTotalCountByQrCodeIdAsync(int qrCodeId, string searchString = null, string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null)
        {
            IQueryable<OrderEntry> orderEntrys = null;

            if (isExpired is null)
            {
                orderEntrys = _applicationContext.OrderEntrys
                    .Where(o => o.QrCodeId == qrCodeId
                    && o.OrderType.Contains(type ?? string.Empty)
                    && o.OrderStatus.Contains(status ?? string.Empty)
                    && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                    && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                    );
            }
            else
            {
                orderEntrys = _applicationContext.OrderEntrys
                    .Where(o => o.QrCodeId == qrCodeId
                    && o.OrderType.Contains(type ?? string.Empty)
                    && o.OrderStatus.Contains(status ?? string.Empty)
                    && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                    && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                    && o.IsExpired == isExpired
                    );
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                return await orderEntrys
                    .Where(w => w.TrackingNumber == searchString
                    || w.OrderStatusDescription.Contains(searchString)
                    || w.TraderUserName.Contains(searchString)
                    || w.TraderFullName.Contains(searchString)
                    ).CountAsync();
            }
            else
            {
                return await orderEntrys.CountAsync();
            }
        }



        public async Task<List<OrderEntry>> GetOrderEntrysByFourthPartyIdAsync(string fourthPartyId,
            int? pageIndex, int? take, string searchString = null, string sortField = null,
            string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null,
            string direction = SortDirections.Descending)
        {
            var result = new List<OrderEntry>();

            IQueryable<OrderEntry> orderEntrys = null;

            if (isExpired is null)
            {
                orderEntrys = _applicationContext.OrderEntrys
                    .Where(o => o.FourthPartyId == fourthPartyId
                    && o.OrderType.Contains(type ?? string.Empty)
                    && o.OrderStatus.Contains(status ?? string.Empty)
                    && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                    && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                    );
            }
            else
            {
                orderEntrys = _applicationContext.OrderEntrys
                    .Where(o => o.FourthPartyId == fourthPartyId
                    && o.OrderType.Contains(type ?? string.Empty)
                    && o.OrderStatus.Contains(status ?? string.Empty)
                    && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                    && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                    && o.IsExpired == isExpired
                    );
            }

            IQueryable<OrderEntry> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = orderEntrys
                    .Where(w => w.TrackingNumber == searchString
                    || w.OrderStatusDescription.Contains(searchString)
                    || w.TraderUserName.Contains(searchString)
                    || w.TraderFullName.Contains(searchString)
                    );
            }
            else
            {
                searchResult = orderEntrys;
            }

            result = await this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            return result;
        }

        public async Task<int> GetOrderEntrysTotalCountByFourthPartyIdAsync(string fourthPartyId, string searchString = null, string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null)
        {
            IQueryable<OrderEntry> orderEntrys = null;

            if (isExpired is null)
            {
                orderEntrys = _applicationContext.OrderEntrys
                    .Where(o => o.FourthPartyId == fourthPartyId
                    && o.OrderType.Contains(type ?? string.Empty)
                    && o.OrderStatus.Contains(status ?? string.Empty)
                    && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                    && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                    );
            }
            else
            {
                orderEntrys = _applicationContext.OrderEntrys
                    .Where(o => o.FourthPartyId == fourthPartyId
                    && o.OrderType.Contains(type ?? string.Empty)
                    && o.OrderStatus.Contains(status ?? string.Empty)
                    && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                    && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                    && o.IsExpired == isExpired
                    );
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                return await orderEntrys
                    .Where(w => w.TrackingNumber == searchString
                    || w.OrderStatusDescription.Contains(searchString)
                    || w.TraderUserName.Contains(searchString)
                    || w.TraderFullName.Contains(searchString)
                    ).CountAsync();
            }
            else
            {
                return await orderEntrys.CountAsync();
            }
        }



        private async Task<List<OrderEntry>> GetSortedRecords(
            IQueryable<OrderEntry> orderEntrys,
            int? pageIndex, int? take, string sortField = null, string direction = SortDirections.Descending)
        {
            //var result = new List<OrderEntry>();
            IQueryable<OrderEntry> result;

            if (pageIndex != null && take != null)
            {
                var skip = (int)take * (int)pageIndex;
                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "DateCreated")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.DateCreated)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.DateCreated)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "OrderId")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.OrderId)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.OrderId)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "TrackingNumber")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.TrackingNumber)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.TrackingNumber)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "OrderType")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.OrderType)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.OrderType)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "OrderStatus")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.OrderStatus)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.OrderStatus)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "ShopUserName")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.ShopUserName)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.ShopUserName)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "ShopOrderId")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.ShopOrderId)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.ShopOrderId)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "RateRebateShop")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.RateRebateShop)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.RateRebateShop)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "RateRebateFinal")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.RateRebateFinal)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.RateRebateFinal)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "OrderPaymentChannel")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.OrderPaymentChannel)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.OrderPaymentChannel)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "OrderPaymentScheme")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.OrderPaymentScheme)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.OrderPaymentScheme)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "PayerInfo")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.IpAddress)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.IpAddress)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "TraderUserName")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.TraderUserName)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.TraderUserName)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "QrCodeId")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.QrCodeId)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.QrCodeId)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "FourthPartyName")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.FourthPartyName)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.FourthPartyName)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "Amount")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.Amount)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.Amount)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "ShopUserCommissionRealized")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.ShopUserCommissionRealized)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.ShopUserCommissionRealized)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "TradingUserCommissionRealized")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.TradingUserCommissionRealized)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.TradingUserCommissionRealized)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "PlatformCommissionRealized")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.PlatformCommissionRealized)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.PlatformCommissionRealized)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                    else
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = orderEntrys
                               .OrderBy(f => f.DateCreated)
                               .Skip(skip)
                               .Take((int)take);
                        }
                        else
                        {
                            result = orderEntrys
                               .OrderByDescending(f => f.DateCreated)
                               .Skip(skip)
                               .Take((int)take);
                        }
                    }
                }
                else
                {
                    result = orderEntrys
                       .OrderByDescending(f => f.DateCreated)
                       .Skip(skip)
                       .Take((int)take);
                }
            }
            else
            {
                result = orderEntrys;
            }

            var options = new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(25) };
            var cachingResult = await result.FromCacheAsync(options);
            return cachingResult.ToList();
        }



        public async Task<OrderEntry> MapFromEntity(Order entity)
        {
            //Check the entity and user is not null.
            if (entity is null)
            {
                throw new ArgumentNullException("The qr code entity must be provided.");
            }


            //Check the shop is exist.
            string shopId = entity.ShopInfo != null ? entity.ShopInfo.ShopId : string.Empty;
            string shopUserName = string.Empty;
            string shopFullName = string.Empty;
            decimal rateRebateShop = 0M;
            decimal rateRebateFinal = 0M;
            string shopOrderId = string.Empty;
            if (!string.IsNullOrEmpty(shopId))
            {
                var shop = _userCacheService.GetNameInfoByUserId(entity.ShopInfo.ShopId);

                if (shop == null)
                {
                    throw new KeyNotFoundException("找无商户。");
                }
                shopUserName = shop?.UserName;
                shopFullName = shop?.FullName;
                rateRebateShop = entity.ShopInfo.RateRebateShop;
                rateRebateFinal = entity.ShopInfo.RateRebateFinal;
                shopOrderId = entity.ShopInfo.ShopOrderId;
            }

            string traderId = entity.PayeeInfo?.TraderId;
            string traderUserName = null;
            string traderFullName = null;
            //Get trader.
            if (traderId != null)
            {
                var trader = _userCacheService.GetNameInfoByUserId(entity.PayeeInfo?.TraderId);

                if (trader == null)
                {
                    throw new KeyNotFoundException("找无交易员。");
                }
                traderUserName = trader.UserName;
                traderFullName = trader.FullName;
            }


            //Build qr code view model.
            var orderVM = new OrderEntry
            {
                OrderId = entity.Id,
                TrackingNumber = entity.TrackingNumber,
                OrderType = entity.GetOrderType.Name,
                OrderStatus = entity.GetOrderStatus.Name,
                OrderStatusDescription = entity.OrderStatusDescription,
                IsTestOrder = entity.IsTestOrder,
                IsExpired = entity.IsExpired,
                ShopId = shopId,
                ShopUserName = shopUserName,
                ShopFullName = shopFullName,
                //ShopUserName = string.Empty,
                //ShopFullName = string.Empty,
                ShopOrderId = shopOrderId,
                RateRebateShop = rateRebateShop,
                RateRebateFinal = rateRebateFinal,
                ToppestTradingRate = entity.PayeeInfo?.ToppestTradingRate,
                OrderPaymentChannel = entity.GetOrderPaymentChannel.Name,
                OrderPaymentScheme = entity.GetOrderPaymentScheme.Name,
                IpAddress = entity.PayerInfo?.IpAddress,
                Device = entity.PayerInfo?.Device,
                Location = entity.PayerInfo?.Location,
                TraderId = traderId,
                TraderUserName = traderUserName,
                TraderFullName = traderFullName,
                QrCodeId = entity.PayeeInfo?.QrCodeId,
                FourthPartyId = entity.PayeeInfo?.FourthPartyId,
                FourthPartyName = entity.PayeeInfo?.FourthPartyName,
                Amount = entity.Amount,
                AmountPaid = entity.AmountPaid,
                //ShopUserCommissionRealized = entity.Amount * (entity.ShopInfo.RateRebateShop - entity.ShopInfo.RateRebateFinal),
                //TradingUserCommissionRealized = entity.Amount * (decimal)entity.PayeeInfo.ToppestTradingRate,
                DateCreated = entity.DateCreated,
                DatePaired = entity.DatePaired?.ToFullString(),
                DatePaymentRecieved = entity.DatePaymentRecieved?.ToFullString(),
            };

            return orderVM;
        }

        public void UpdateOrderEntryToSuccess(int orderId, string orderStatus,
            decimal amountPaid, decimal shopUserCommissionRealized, decimal tradingUserCommissionRealized, decimal platformCommissionRealized,
            decimal traderCommissionRealized, decimal shopCommissionRealized,
            string datePaymentRecieved)
        {
            /*var user = await _applicationContext.OrderEntrys
                .Include(t => t.Balance)
                .Where(u => u.TraderId == userId)
                .Select(u => new
                {
                    u.TraderId,
                    u.Balance
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();*/


            var toUpdate = new OrderEntry
            {
                OrderId = orderId
            };

            _applicationContext.OrderEntrys.Attach(toUpdate);
            _applicationContext.Entry(toUpdate).Property(b => b.OrderStatus).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.AmountPaid).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.ShopUserCommissionRealized).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.TradingUserCommissionRealized).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.PlatformCommissionRealized).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.TraderCommissionRealized).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.ShopCommissionRealized).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.DatePaymentRecieved).IsModified = true;

            toUpdate.OrderStatus = orderStatus;
            toUpdate.AmountPaid = amountPaid;
            toUpdate.ShopUserCommissionRealized = shopUserCommissionRealized;
            toUpdate.TradingUserCommissionRealized = tradingUserCommissionRealized;
            toUpdate.PlatformCommissionRealized = platformCommissionRealized;
            toUpdate.TraderCommissionRealized = traderCommissionRealized;
            toUpdate.ShopCommissionRealized = shopCommissionRealized;
            toUpdate.DatePaymentRecieved = datePaymentRecieved;
        }

        public async Task UpdateOrderEntryToPaired(int orderId, string orderStatus, string datePaired, string traderId,
            int? qrCodeId, string fourthPartyId, string fourthPartyName, decimal? toppestTradingRate)
        {
            /*var user = await _applicationContext.OrderEntrys
                .Include(t => t.Balance)
                .Where(u => u.TraderId == userId)
                .Select(u => new
                {
                    u.TraderId,
                    u.Balance
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();*/

            var trader = await _userManager.Users
            .Where(u => u.Id == traderId)
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.FullName
            })
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

            if (trader == null)
            {
                throw new KeyNotFoundException("找无交易员。");
            }

            var toUpdate = new OrderEntry
            {
                OrderId = orderId
            };

            _applicationContext.OrderEntrys.Attach(toUpdate);
            _applicationContext.Entry(toUpdate).Property(b => b.OrderStatus).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.DatePaired).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.TraderId).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.TraderUserName).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.TraderFullName).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.QrCodeId).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.FourthPartyId).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.FourthPartyName).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.ToppestTradingRate).IsModified = true;

            toUpdate.OrderStatus = orderStatus;
            toUpdate.DatePaired = datePaired;
            toUpdate.TraderId = traderId;
            toUpdate.TraderUserName = trader.UserName;
            toUpdate.TraderFullName = trader.FullName;
            toUpdate.QrCodeId = qrCodeId;
            toUpdate.FourthPartyId = fourthPartyId;
            toUpdate.FourthPartyName = fourthPartyName;
            toUpdate.ToppestTradingRate = toppestTradingRate;
        }


        public void UpdateOrderEntryToExpired(int orderId, string orderStatus, string orderStatusDescription)
        {
            var toUpdate = new OrderEntry
            {
                OrderId = orderId
            };

            _applicationContext.OrderEntrys.Attach(toUpdate);
            _applicationContext.Entry(toUpdate).Property(b => b.IsExpired).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.OrderStatus).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.OrderStatusDescription).IsModified = true;

            toUpdate.IsExpired = true;
            toUpdate.OrderStatus = orderStatus;
            toUpdate.OrderStatusDescription = orderStatusDescription;
        }



        public async Task DeleteFinishedOrderEntry()
        {
            var orders = await _applicationContext
                    .OrderEntrys
                    .Where(o => o.OrderStatus == OrderStatus.Success.Name || o.OrderStatus == OrderStatus.Submitted.Name)
                    .ToListAsync();


            _applicationContext.OrderEntrys.RemoveRange(orders);
        }

        public async Task<List<OrderEntry>> GetAllOrderEntries()
        {
            return await _applicationContext.OrderEntrys.ToListAsync();
        }

        public OrderEntry Add(OrderEntry orderEntry)
        {
            return _applicationContext.OrderEntrys.Add(orderEntry).Entity;
        }

        public void Update(OrderEntry orderEntry)
        {
            _applicationContext.Entry(orderEntry).State = EntityState.Modified;
        }

        public void Delete(OrderEntry orderEntry)
        {
            if (orderEntry != null)
            {
                _applicationContext.OrderEntrys.Remove(orderEntry);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _applicationContext.SaveChangesAsync();
        }
    }
}
