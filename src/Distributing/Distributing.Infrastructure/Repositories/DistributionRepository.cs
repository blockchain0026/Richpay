using Distributing.Domain.Model.Distributions;
using Distributing.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Infrastructure.Repositories
{
    public class DistributionRepository
        : IDistributionRepository
    {
        private readonly DistributingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public DistributionRepository(DistributingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Distribution Add(Distribution distribution)
        {
            return _context.Distributions.Add(distribution).Entity;

        }

        public void Update(Distribution distribution)
        {
            _context.Entry(distribution).State = EntityState.Modified;
        }

        public async Task<Distribution> GetByDistributionIdAsync(int distributionId)
        {
            var distribution = await _context
                                .Distributions
                                .Include(d => d.Order)
                                .FirstOrDefaultAsync(d => d.Id == distributionId);
            if (distribution == null)
            {
                distribution = _context
                            .Distributions
                            .Local
                            .FirstOrDefault(d => d.Id == distributionId);
            }
            if (distribution != null)
            {
                await _context.Entry(distribution)
                    .Reference(b => b.DistributionType).LoadAsync();
            }

            return distribution;
        }

        public async Task<IEnumerable<Distribution>> GetByOrderTrackingNumberAsync(string orderTrackingNumber)
        {
            var distributions = _context
                                .Distributions
                                .Include(d => d.Order)
                                .Where(d => d.Order.TrackingNumber == orderTrackingNumber);

            foreach (var distribution in distributions)
            {
                await _context.Entry(distribution)
                    .Reference(b => b.DistributionType).LoadAsync();

            }

            return distributions;
        }

        public async Task<List<Distribution>> GetByUserIdAsync(string userId, bool isNoTracking = true)
        {
            List<Distribution> distributions;

            if (isNoTracking)
            {
                distributions = await _context
                    .Distributions
                    .AsNoTracking()
                    .Where(d => d.UserId == userId)
                    .ToListAsync();
            }
            else
            {
                distributions = await _context
                    .Distributions
                    .Where(d => d.UserId == userId)
                    .ToListAsync();
            }

            /*foreach (var distribution in distributions)
            {
                await _context.Entry(distribution)
                    .Reference(b => b.DistributionType).LoadAsync();
                await _context.Entry(distribution)
                    .Reference(b => b.Order).LoadAsync();
            }*/

            return distributions;
        }

        public async Task<IEnumerable<Distribution>> GetAllAsync()
        {
            var distributions = await _context
                                .Distributions
                                .ToListAsync();

            return distributions;
        }


        public void Delete(Distribution distribution)
        {
            if (distribution != null)
            {
                _context.Distributions.Remove(distribution);
            }
        }
        public void DeleteRange(List<Distribution> distributions)
        {
            if (distributions.Any())
            {
                _context.Distributions.RemoveRange(distributions);
            }
        }


        public async Task<decimal> GetSumOfDistributedAmountByUserIdAsync(string userId)
        {
            var totalDistributedAmount = await _context.Distributions
                .Where(d => d.UserId == userId)
                .SumAsync(d => d.DistributedAmount);

            return totalDistributedAmount;
        }
    }
}
