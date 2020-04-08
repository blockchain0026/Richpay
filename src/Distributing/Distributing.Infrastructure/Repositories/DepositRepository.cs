using Distributing.Domain.Model.Deposits;
using Distributing.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Infrastructure.Repositories
{
    public class DepositRepository
    : IDepositRepository
    {
        private readonly DistributingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public DepositRepository(DistributingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Deposit Add(Deposit deposit)
        {
            return _context.Deposits.Add(deposit).Entity;

        }

        public void Update(Deposit deposit)
        {
            _context.Entry(deposit).State = EntityState.Modified;
        }

        public async Task<Deposit> GetByDepositIdAsync(int depositId)
        {
            var deposit = await _context
                                .Deposits
                                .Include(d => d.BankAccount)
                                .Include(d => d.VerifiedBy)
                                .FirstOrDefaultAsync(d => d.Id == depositId);
            if (deposit == null)
            {
                deposit = _context
                            .Deposits
                            .Local
                            .FirstOrDefault(d => d.Id == depositId);
            }
            if (deposit != null)
            {
                await _context.Entry(deposit)
                    .Reference(d => d.DepositStatus).LoadAsync();
            }

            return deposit;
        }

        public void Delete(Deposit deposit)
        {
            if (deposit != null)
            {
                _context.Deposits.Remove(deposit);
            }
        }
        public void DeleteRange(List<Deposit> deposits)
        {
            if (deposits.Any())
            {
                _context.Deposits.RemoveRange(deposits);
            }
        }
    }
}
