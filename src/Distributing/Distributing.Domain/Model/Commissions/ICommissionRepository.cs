using Distributing.Domain.Model.Distributions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Domain.Model.Commissions
{
    public interface ICommissionRepository : IRepository<Commission>
    {
        Commission Add(Commission commission);
        void Update(Commission commission);
        void Delete(Commission commission);
        void DeleteRange(List<Commission> commissions);

        Task<Commission> GetByCommissionIdAsync(int commissionId);
        Task<IEnumerable<Commission>> GetDownlinesAsnyc(int uplineCommissionId);
        Task<Commission> GetByUserIdAsync(string userId);
        Task<IEnumerable<Commission>> GetByStatusAsync(bool isEnabled);

        decimal? GetCommissionRateByUserIdAsync(string userId, RateType rateType, out int? commissionId);
       
        Task<CommissionInfo> GetCommissionInfoByUserIdAsync(string userId, RateType rateType);
        
        Task<CommissionInfo> GetCommissionInfoByCommissionIdAsync(int commissionId, RateType rateType);
        Task<CommissionInfo> GetToppestCommissionInfoByUserIdAsync(string userId, RateType rateType);
        Task<List<CommissionInfo>> GetCommissionInfosByChainNumberAsync(int chainNumber, RateType rateType);
    }
}
