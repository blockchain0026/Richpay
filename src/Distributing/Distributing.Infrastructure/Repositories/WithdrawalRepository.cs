using Distributing.Domain.Model.Withdrawals;
using Distributing.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Infrastructure.Repositories
{
    public class WithdrawalRepository
        : IWithdrawalRepository
    {
        private readonly DistributingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public WithdrawalRepository(DistributingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Withdrawal Add(Withdrawal withdrawal)
        {
            return _context.Withdrawals.Add(withdrawal).Entity;

        }

        public void Update(Withdrawal withdrawal)
        {
            _context.Entry(withdrawal).State = EntityState.Modified;
        }

        public async Task<Withdrawal> GetByWithdrawalIdAsync(int withdrawalId)
        {
            var withdrawal = await _context
                                .Withdrawals
                                .Include(w => w.BankAccount)
                                .Include(w => w.ApprovedBy)
                                .Include(w => w.CancellationApprovedBy)
                                .FirstOrDefaultAsync(d => d.Id == withdrawalId);
            if (withdrawal == null)
            {
                withdrawal = _context
                            .Withdrawals
                            .Local
                            .FirstOrDefault(d => d.Id == withdrawalId);
            }
            if (withdrawal != null)
            {
                await _context.Entry(withdrawal)
                    .Reference(d => d.WithdrawalStatus).LoadAsync();
            }

            return withdrawal;
        }

        public void Delete(Withdrawal withdrawal)
        {
            if (withdrawal != null)
            {
                _context.Withdrawals.Remove(withdrawal);
            }
        }

        public void DeleteRange(List<Withdrawal> withdrawals)
        {
            if (withdrawals.Any())
            {
                _context.Withdrawals.RemoveRange(withdrawals);
            }
        }
    }
}
