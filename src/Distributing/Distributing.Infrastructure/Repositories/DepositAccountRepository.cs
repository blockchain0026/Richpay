using Distributing.Domain.Model.Banks;
using Distributing.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Infrastructure.Repositories
{
    public class DepositAccountRepository
     : IDepositAccountRepository
    {
        private readonly DistributingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public DepositAccountRepository(DistributingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public DepositAccount Add(DepositAccount depositAccount)
        {
            return _context.DepositAccounts.Add(depositAccount).Entity;

        }

        public void Update(DepositAccount depositAccount)
        {
            _context.Entry(depositAccount).State = EntityState.Modified;
        }

        public async Task<DepositAccount> GetByDepositAccountIdAsync(int depositAccountId)
        {
            var depositAccount = await _context
                                .DepositAccounts
                                .Include(d=>d.BankAccount)
                                .FirstOrDefaultAsync(b => b.Id == depositAccountId);
            if (depositAccount == null)
            {
                depositAccount = _context
                            .DepositAccounts
                            .Local
                            .FirstOrDefault(b => b.Id == depositAccountId);
            }
            /*if (depositAccount != null)
            {
                await _context.Entry(depositAccount)
                    .Reference(b => b.UserType).LoadAsync();
            }*/

            return depositAccount;
        }

        public void Delete(DepositAccount depositAccount)
        {
            if (depositAccount != null)
            {
                _context.DepositAccounts.Remove(depositAccount);
            }
        }
        public void DeleteRange(List<DepositAccount> depositAccounts)
        {
            if (depositAccounts.Any())
            {
                _context.DepositAccounts.RemoveRange(depositAccounts);
            }
        }
    }
}
