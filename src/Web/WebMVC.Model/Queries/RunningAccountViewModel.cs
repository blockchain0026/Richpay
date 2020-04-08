using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models.Queries
{
    public class RunningAccountRecord
    {
        public int Id { get; set; }
        public string UserId { get; set; }//Can Map From Order Entity
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public string DownlineUserId { get; set; }
        public string DownlineUserName { get; set; }
        public string DownlineFullName { get; set; }
        public string OrderTrackingNumber { get; set; }//
        public string ShopOrderId { get; set; }//
        public string Status { get; set; }//
        public decimal Amount { get; set; }//
        public decimal CommissionAmount { get; set; }//
        public int? DistributionId { get; set; }
        public decimal? DistributedAmount { get; set; }//
        public string ShopId { get; set; }//
        public string ShopUserName { get; set; }
        public string ShopFullName { get; set; }
        public string TraderId { get; set; }//
        public string TraderUserName { get; set; }
        public string TraderFullName { get; set; }
        public DateTime DateCreated { get; set; }//
    }

    /// <summary>
    /// This is use to improve performance.
    /// When a running account record is created, will also create a temp running account record,
    /// after the order is completed, will find the correspond temp running account record,
    /// and attach corresponding running account record and update it.
    /// 
    /// </summary>
    public class TempRunningAccountRecord
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string OrderTrackingNumber { get; set; }
    }


    public class RunningAccountRecordSumData
    {
        public decimal TotalCount { get; set; }
        public decimal TotalSuccessOrderAmount { get; set; }
        public decimal TotalCommissionAmount { get; set; }
    }



    public class RunningAccountRecordsStatistic
    {
        public decimal TotalProfit { get; set; }

        public decimal TotalProfitCompare { get; set; }

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
