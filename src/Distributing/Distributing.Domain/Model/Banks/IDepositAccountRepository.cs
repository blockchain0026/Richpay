using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Domain.Model.Banks
{
    public interface IDepositAccountRepository : IRepository<DepositAccount>
    {
        DepositAccount Add(DepositAccount depositAccount);
        void Update(DepositAccount depositAccount);
        void Delete(DepositAccount depositAccount);
        void DeleteRange(List<DepositAccount> depositAccounts);
        Task<DepositAccount> GetByDepositAccountIdAsync(int depositAccountId);
        //Task<IEnumerable<DepositAccount>> GetAllAsync();
    }
}
