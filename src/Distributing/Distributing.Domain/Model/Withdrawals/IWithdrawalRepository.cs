using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Domain.Model.Withdrawals
{
    public interface IWithdrawalRepository : IRepository<Withdrawal>
    {
        Withdrawal Add(Withdrawal withdrawal);
        void Update(Withdrawal withdrawal);
        void Delete(Withdrawal withdrawal);
        void DeleteRange(List<Withdrawal> withdrawals);
        Task<Withdrawal> GetByWithdrawalIdAsync(int withdrawalId);
        //Task<IEnumerable<Withdrawal>> GetAllAsync();
    }
}
