using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models.Queries
{
    public class OrderEntry
    {
        public int OrderId { get; set; }
        public string TrackingNumber { get; set; }

        public string OrderType { get; set; }

        public string OrderStatus { get; set; }

        public string OrderStatusDescription { get; set; }

        public bool IsTestOrder { get; set; }
        public bool IsExpired { get; set; }

        public string ShopId { get; set; }
        public string ShopUserName { get; set; }
        public string ShopFullName { get; set; }
        public string ShopOrderId { get; set; }

        public decimal RateRebateShop { get; set; }
        public decimal RateRebateFinal { get; set; }
        public decimal? ToppestTradingRate { get; set; }


        public string OrderPaymentChannel { get; set; }
        public string OrderPaymentScheme { get; set; }

        public string IpAddress { get; set; }
        public string Device { get; set; }
        public string Location { get; set; }

        public string TraderId { get; set; }
        public string TraderUserName { get; set; }
        public string TraderFullName { get; set; }
        public int? QrCodeId { get; set; }
        public string FourthPartyId { get; set; }
        public string FourthPartyName { get; set; }

        public decimal Amount { get; set; }
        public decimal? AmountPaid { get; set; }

        public decimal? ShopUserCommissionRealized { get; set; }
        public decimal? TradingUserCommissionRealized { get; set; }
        public decimal? PlatformCommissionRealized { get; set; }
        public decimal? TraderCommissionRealized { get; set; }
        public decimal? ShopCommissionRealized { get; set; }

        public DateTime DateCreated { get; set; }
        public string DatePaired { get; set; }
        public string DatePaymentRecieved { get; set; }
    }

    public class OrderSumData
    {
        public int TotalCount { get; set; }
        public decimal SuccessRate { get; set; }

        public decimal TotalOrderAmount { get; set; }
        public decimal TotalOrderSuccessAmount { get; set; }

        public decimal TotalShopUserCommissionAmount { get; set; }
        public decimal TotalTradingUserCommissionAmount { get; set; }
        public decimal TotalPlatformCommissionAmount { get; set; }
        public decimal TotalTraderCommissionAmount { get; set; }
        public decimal TotalShopCommissionAmount { get; set; }
    }

    public class OrderEntriesStatistic
    {
        public decimal TotalPlatformProfit { get; set; }
        public decimal TotalShopAgentProfit { get; set; }
        public decimal TotalTradeUserProfit { get; set; }
        public decimal TotalShopReturn { get; set; }

        public decimal TotalTraderProfit { get; set; }
        public decimal TotalShopProfit { get; set; }


        public decimal TotalPlatformProfitCompare { get; set; }
        public decimal TotalShopAgentProfitCompare { get; set; }
        public decimal TotalTradeUserProfitCompare { get; set; }


        public int AlipayBarcodeOrderSuccessCount { get; set; }
        public int AlipayBarcodeOrderCount { get; set; }
        public int AlipayMerchantOrderSuccessCount { get; set; }
        public int AlipayMerchantOrderCount { get; set; }
        public int AlipayTransactionOrderSuccessCount { get; set; }
        public int AlipayTransactionOrderCount { get; set; }
        public int AlipayBankOrderSuccessCount { get; set; }
        public int AlipayBankOrderCount { get; set; }
        public int AlipayEnvelopOrderSuccessCount { get; set; }
        public int AlipayEnvelopOrderCount { get; set; }
        public int WechatBarcodeOrderSuccessCount { get; set; }
        public int WechatBarcodeOrderCount { get; set; }




        public decimal AlipayBarcodeSuccessRate { get; set; }
        public decimal AlipayMerchantSuccessRate { get; set; }
        public decimal AlipayTransactionSuccessRate { get; set; }
        public decimal AlipayBankSuccessRate { get; set; }
        public decimal AlipayEnvelopSuccessRate { get; set; }
        public decimal WechatBarcodeSuccessRate { get; set; }

        public int OrderTotalCount { get; set; }
        public int OrderTotalSuccessCount { get; set; }
        public decimal OrderTotalAmount { get; set; }
        public decimal OrderTotalSuccessAmount { get; set; }
        public decimal OrderDailyAverageSuccessAmount { get; set; }
        public decimal OrderAverageRevenueRate { get; set; }
        public decimal OrderSuccessRate { get; set; }

        public Dictionary<string, decimal> OrderChartData { get; set; }
        public Dictionary<string, decimal> ShopRankData { get; set; }
        public Dictionary<string, decimal> TraderRankData { get; set; }
    }
}
