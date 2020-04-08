using Distributing.Domain.Model.Commissions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebWorkerService.CacheServices
{
    public interface ICommissionCacheService
    {
        CommissionInfo GetCommissionInfoByCommissionId(int commissionId, RateType rateType);
        CommissionInfo GetCommissionInfoByUserId(string userId, RateType rateType);
        CommissionInfo GetToppestCommissionInfoByCommission(CommissionInfo commission, RateType rateType);
        Task UpdateCommissions();
    }
}
