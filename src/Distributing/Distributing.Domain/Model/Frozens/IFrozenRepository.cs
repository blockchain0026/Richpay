using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Domain.Model.Frozens
{

    public interface IFrozenRepository : IRepository<Frozen>
    {
        Frozen Add(Frozen frozen);
        void Update(Frozen frozen);
        Task<Frozen> GetByFrozenIdAsync(int frozenId);
        Task<Frozen> GetByOrderTrackingNumberAsync(string trackingNumber);
        Task<Frozen> GetByWithdrawalIdAsync(int withdrawalId);
        Task<Frozen> GetByAdminIdAsync(string adminId);

        Task<IEnumerable<Frozen>> GetByBalanceIdAsync(int balanceId);
        Task<IEnumerable<Frozen>> GetByUserIdAsync(string userId);
        void Delete(Frozen frozen);
    }
}
