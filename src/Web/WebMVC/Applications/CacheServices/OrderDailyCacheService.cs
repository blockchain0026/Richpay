using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Domain.Model.Orders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries;
using WebMVC.Data;
using WebMVC.Extensions;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.CacheServices
{
    public class OrderDailyCacheService : IOrderDailyCacheService
    {
        private readonly IServiceScopeFactory scopeFactory;

        //Key: QrCodeId
        private ConcurrentDictionary<int, OrderEntry> _orderEntries;

        public OrderDailyCacheService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        public async Task Initialize()
        {
            Console.WriteLine("Intializing Order Daily Cache Service..." + DateTime.UtcNow.ToFullString());
            using (var scope = scopeFactory.CreateScope())
            {
                var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var applicationDateTimeService = scope.ServiceProvider.GetRequiredService<IApplicationDateTimeService>();

                //Get all order entries today.
                var dateStartTime = applicationDateTimeService.GetDayStartUTCTime();
                var orderEntries = await appContext.OrderEntrys
                    .AsNoTracking()
                    .Where(o => o.DateCreated >= dateStartTime)
                    .ToListAsync();

                var tempOrderEntries = new ConcurrentDictionary<int, OrderEntry>();

                Parallel.ForEach(orderEntries, orderEntry =>
                {
                    tempOrderEntries.TryAdd(orderEntry.OrderId, orderEntry);
                });

                //Update and prevent any conflict on the list.
                Volatile.Write(ref _orderEntries, tempOrderEntries);
            }
        }


        public async Task UpdateRecentOrderEntries()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var utcNow = DateTime.UtcNow;
                var searchDateFrom = utcNow.AddMinutes(-6);
                var orderEntries = await appContext.OrderEntrys
                    .AsNoTracking()
                    .Where(o => o.DateCreated > searchDateFrom)
                    .ToListAsync();

                Parallel.ForEach(orderEntries, orderEntry =>
                {
                    AddOrUpdateOrder(orderEntry);
                });

                //Update and prevent any conflict on the list.
                //Volatile.Write(ref _pairingInfos, pairingInfos);
            }
        }

        public void AddOrUpdateOrder(OrderEntry orderEntry)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    _orderEntries.AddOrUpdate(orderEntry.OrderId, orderEntry,
                        (key, existingOrderEntry) =>
                        {
                            return orderEntry;
                        });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Catch excpetion while add or update order to cache..." + ex.Message);
            }
        }


        public async Task<List<OrderEntry>> GetOrderEntrysAsync(int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null,
            string type = null, string status = null, string paymentChannel = null, string paymentScheme = null, bool? isExpired = null,
            string direction = SortDirections.Descending)
        {
            var result = new List<OrderEntry>();

            IEnumerable<OrderEntry> orderEntrys = null;

            if (isExpired is null)
            {
                if (from != null && to != null)
                {
                    orderEntrys = _orderEntries.Values
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
                    orderEntrys = _orderEntries.Values
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
                    orderEntrys = _orderEntries.Values
                        .Where(o => /*o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        &&*/ o.IsExpired == isExpired
                        && o.DateCreated >= from
                        && o.DateCreated <= to
                        );
                }
                else
                {
                    orderEntrys = _orderEntries.Values
                        .Where(o => o.OrderType.Contains(type ?? string.Empty)
                        && o.OrderStatus.Contains(status ?? string.Empty)
                        && o.OrderPaymentChannel.Contains(paymentChannel ?? string.Empty)
                        && o.OrderPaymentScheme.Contains(paymentScheme ?? string.Empty)
                        && o.IsExpired == isExpired
                        );
                }
            }

            IEnumerable<OrderEntry> searchResult;

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

            IEnumerable<OrderEntry> orderEntrys = null;

            if (isExpired is null)
            {
                if (from != null && to != null)
                {
                    orderEntrys = _orderEntries.Values
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
                    orderEntrys = _orderEntries.Values
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
                    orderEntrys = _orderEntries.Values
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
                    orderEntrys = _orderEntries.Values
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


            var nonTestSumData = sumDatas
                .Where(s => !s.IsTestOrder).FirstOrDefault();

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

            IEnumerable<OrderEntry> orderEntrys = null;

            if (from != null && to != null)
            {
                orderEntrys = _orderEntries.Values
                    .Where(o => o.DateCreated >= from && o.DateCreated <= to);
            }
            else
            {
                var utcNow = DateTime.UtcNow;
                from = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day);
                to = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day + 1);
                orderEntrys = _orderEntries.Values
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

            var nonTestStatistic = statistics
                .Where(s => !s.IsTestOrder)
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
                });

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
                .Take(5); //Using cache to improve performance.


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
                .Take(5); //Using cache to improve performance.

            foreach (var rankData in traderRankData)
            {
                if (!string.IsNullOrEmpty(rankData.Key))
                {
                    result.TraderRankData.Add(rankData.Key.ToString(), rankData.OrderTotalSuccessAmount ?? 0M);
                }
            }

            return result;
        }

        private async Task<List<OrderEntry>> GetSortedRecords(
            IEnumerable<OrderEntry> orderEntrys,
            int? pageIndex, int? take, string sortField = null, string direction = SortDirections.Descending)
        {
            //var result = new List<OrderEntry>();
            IEnumerable<OrderEntry> result;

            if (pageIndex != null && take != null)
            {
                var skip = (int)take * (int)pageIndex;
                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "DateCreated")
                    {
                        var test = orderEntrys.ToList();
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

            return result.ToList();
        }
    }
}
