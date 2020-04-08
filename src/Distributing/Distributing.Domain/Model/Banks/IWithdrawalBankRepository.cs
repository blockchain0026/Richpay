using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Domain.Model.Banks
{
    public interface IWithdrawalBankRepository : IRepository<WithdrawalBank>
    {
        WithdrawalBank Add(WithdrawalBank withdrawalBank);
        void Update(WithdrawalBank withdrawalBank);
        void Delete(WithdrawalBank withdrawalBank);
        void DeleteRange(List<WithdrawalBank> withdrawalBanks);
        Task<WithdrawalBank> GetByWithdrawalBankIdAsync(int withdrawalBankId);
        //Task<IEnumerable<WithdrawalBank>> GetAllAsync();
    }
}
