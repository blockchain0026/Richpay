using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Domain.Model.Transfers
{
    public interface ITransferRepository : IRepository<Transfer>
    {
        Transfer Add(Transfer transfer);
        void Update(Transfer transfer);
        void Delete(Transfer transfer);
        void DeleteRange(List<Transfer> transfers);
        Task<Transfer> GetByTransferIdAsync(int transferId);
    }
}
