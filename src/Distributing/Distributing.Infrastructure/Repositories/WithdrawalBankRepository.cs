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
    public class WithdrawalBankRepository
    : IWithdrawalBankRepository
    {
        private readonly DistributingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public WithdrawalBankRepository(DistributingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public WithdrawalBank Add(WithdrawalBank withdrawalBank)
        {
            return _context.WithdrawalBanks.Add(withdrawalBank).Entity;

        }

        public void Update(WithdrawalBank withdrawalBank)
        {
            _context.Entry(withdrawalBank).State = EntityState.Modified;
        }

        public async Task<WithdrawalBank> GetByWithdrawalBankIdAsync(int withdrawalBankId)
        {
            var withdrawalBank = await _context
                                .WithdrawalBanks
                                .FirstOrDefaultAsync(w => w.Id == withdrawalBankId);
            if (withdrawalBank == null)
            {
                withdrawalBank = _context
                            .WithdrawalBanks
                            .Local
                            .FirstOrDefault(w => w.Id == withdrawalBankId);
            }
           

            return withdrawalBank;
        }

        public void Delete(WithdrawalBank withdrawalBank)
        {
            if (withdrawalBank != null)
            {
                _context.WithdrawalBanks.Remove(withdrawalBank);
            }
        }

        public void DeleteRange(List<WithdrawalBank> withdrawalBanks)
        {
            if (withdrawalBanks.Any())
            {
                _context.WithdrawalBanks.RemoveRange(withdrawalBanks);
            }
        }
    }
}
