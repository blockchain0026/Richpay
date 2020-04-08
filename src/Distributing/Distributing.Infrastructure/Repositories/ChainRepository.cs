using Distributing.Domain.Model.Chains;
using Distributing.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Infrastructure.Repositories
{
    public class ChainRepository
    : IChainRepository
    {
        private readonly DistributingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public ChainRepository(DistributingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Chain Add(Chain chain)
        {
            return _context.Chains.Add(chain).Entity;
        }

        public void Update(Chain chain)
        {
            _context.Entry(chain).State = EntityState.Modified;
        }

        public async Task<Chain> GetByChainIdAsync(int chainId)
        {
            var chain = await _context
                                .Chains
                                .FirstOrDefaultAsync(d => d.Id == chainId);
            if (chain == null)
            {
                chain = _context
                            .Chains
                            .Local
                            .FirstOrDefault(d => d.Id == chainId);
            }

            return chain;
        }

        public void Delete(Chain chain)
        {
            if (chain != null)
            {
                _context.Chains.Remove(chain);
            }
        }

    }
}
