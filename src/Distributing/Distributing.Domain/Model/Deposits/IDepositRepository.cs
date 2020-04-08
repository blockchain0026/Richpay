using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Domain.Model.Deposits
{
    public interface IDepositRepository : IRepository<Deposit>
    {
        Deposit Add(Deposit deposit);
        void Update(Deposit deposit);
        void Delete(Deposit deposit);
        public void DeleteRange(List<Deposit> deposits);
        Task<Deposit> GetByDepositIdAsync(int depositId);
        //Task<IEnumerable<Deposit>> GetAllAsync();
    }
}
