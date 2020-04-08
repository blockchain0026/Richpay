using Distributing.Domain.Model.Distributions;
using Distributing.Domain.Model.Shared;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Domain.Model.Balances
{
    public interface IBalanceRepository : IRepository<Balance>
    {
        Balance Add(Balance balance);
        void Update(Balance balance);

        Task<Balance> GetByBalanceIdAsync(int balanceId);
        Task<Balance> GetByUserIdAsync(string userId);

        void Delete(Balance balance);
        void DeleteRange(List<Balance> balances);




        Task<int?> GetBalanceIdByUserIdAsync(string userId);

        /// <summary>
        /// This method is use to increase distribution speed. Use carefully.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="increaseAmount"></param>
        /// <returns></returns>
        Task IncreaseBalanceOnlyByUserId(string userId, decimal increaseAmount);

        /// <summary>
        /// Only fetch amount available data of balance entity for performance purpose.
        /// 
        /// Maybe return "decimal?" would be a good choice.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<decimal> GetAmountAvailableByUserIdAsync(string userId);
        Task<decimal> UpdateBalanceForNewOrderByUserId(string userId, Order order, IDateTimeService dateTimeService);
        Task<decimal> UpdateBalanceForDistributeByUserId(string userId, decimal distributedAmount);
    }
}
