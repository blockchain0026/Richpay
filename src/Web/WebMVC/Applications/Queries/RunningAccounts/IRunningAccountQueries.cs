using Distributing.Domain.Model.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.RunningAccounts
{
    public interface IRunningAccountQueries
    {
        Task<RunningAccountRecord> GetRunningAccountRecordAsync(int runningAccountRecordId);

        Task<RunningAccountRecord> GetRunningAccountRecordByUserIdAndTrackingNumberAsync(string userId, string trackingNumber);

        Task<List<RunningAccountRecord>> GetRunningAccountRecordsAsync(int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null, string status = null,
            string direction = SortDirections.Descending);

        Task<RunningAccountRecordSumData> GetRunningAccountRecordsTotalSumDataAsync(string searchString = null,
            DateTime? from = null, DateTime? to = null, string status = null);

        Task<List<RunningAccountRecord>> GetRunningAccountRecordsByUserIdAsync(
            string userId, int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null, string status = null,
            string direction = SortDirections.Descending);

        Task<RunningAccountRecordSumData> GetRunningAccountRecordsTotalSumDataByUserIdAsync(string userId, string searchString = null,
            DateTime? from = null, DateTime? to = null, string status = null);


        Task<RunningAccountRecord> MapFromEntity(
            Distributing.Domain.Model.Commissions.Commission entity,
            Distributing.Domain.Model.Distributions.Order order,
            string downlineUserId);

        Task<RunningAccountRecord> MapFromEntity(
            Distribution entity,
            Distributing.Domain.Model.Distributions.Order order,
            string downlineUserId);

        Task CreateRunningRecordsFrom(Ordering.Domain.Model.Orders.Order orderEntity);
        void UpdateRunningRecordsToCompleted(int runningAccountRecordId, string status, decimal? distributedAmount = null);

        RunningAccountRecord Add(RunningAccountRecord runningAccountRecord);
        void Update(RunningAccountRecord runningAccountRecord);
        void Delete(RunningAccountRecord runningAccountRecord);
        Task SaveChangesAsync();


        Task<List<TempRunningAccountRecord>> GetTempByUserIdAsync(string userId);
        Task<TempRunningAccountRecord> GetTempByUserIdAndOrderTrackingNumberAsync(string userId, string orderTrackingNumber);

        TempRunningAccountRecord AddTemp(TempRunningAccountRecord  tempRunningAccountRecord);
        void DeleteTemp(TempRunningAccountRecord tempRunningAccountRecord);
        void DeleteTempRange(List<TempRunningAccountRecord> tempRunningAccountRecords);
        Task<RunningAccountRecord> MapFromOrderInfo(Order orderInfo, string userId);
        void AddRange(List<RunningAccountRecord> runningAccountRecords);
        Task<RunningAccountRecord> MapFromDataAsync(string orderTrackingNumber, string shopId, string shopOrderId, string traderId, decimal amount, DateTime dateCreated, decimal distributedAmount, string userId);
        RunningAccountRecord MapFromData(string orderTrackingNumber, string shopId, string shopOrderId, string traderId, decimal amount, DateTime dateCreated, decimal distributedAmount, string userId);
        Task<RunningAccountRecordsStatistic> GetRunningAccountRecordsStatisticAsync(string userId, string userRole, string searchString = null, DateTime? from = null, DateTime? to = null);
    }
}
