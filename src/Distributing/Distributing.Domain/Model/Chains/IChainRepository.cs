using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Domain.Model.Chains
{
    public interface IChainRepository : IRepository<Chain>
    {
        Chain Add(Chain chain);
        void Update(Chain chain);
        void Delete(Chain chain);
        Task<Chain> GetByChainIdAsync(int chainId);
    }
}
