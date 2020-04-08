using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Domain.Model.Distributions
{
    public interface IDistributionRepository : IRepository<Distribution>
    {
        Distribution Add(Distribution distribution);
        void Update(Distribution distribution);
        void Delete(Distribution distribution);
        void DeleteRange(List<Distribution> distributions);
        Task<Distribution> GetByDistributionIdAsync(int distributionId);
        Task<IEnumerable<Distribution>> GetByOrderTrackingNumberAsync(string orderTrackingNumber);
        Task<IEnumerable<Distribution>> GetAllAsync();


        Task<decimal> GetSumOfDistributedAmountByUserIdAsync(string userId);
        Task<List<Distribution>> GetByUserIdAsync(string userId, bool isNoTracking = true);
    }
}
