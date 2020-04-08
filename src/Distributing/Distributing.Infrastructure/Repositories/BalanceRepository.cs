using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Distributions;
using Distributing.Domain.Model.Shared;
using Distributing.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Infrastructure.Repositories
{
    public class BalanceRepository
      : IBalanceRepository
    {
        private readonly DistributingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public BalanceRepository(DistributingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region CURD Functions
        public Balance Add(Balance balance)
        {
            return _context.Balances.Add(balance).Entity;

        }

        public void Update(Balance balance)
        {
            _context.Entry(balance).State = EntityState.Modified;
        }

        public async Task<Balance> GetByBalanceIdAsync(int balanceId)
        {
            var balance = await _context
                                .Balances
                                .Include(b => b.WithdrawalLimit)
                                .FirstOrDefaultAsync(b => b.Id == balanceId);
            if (balance == null)
            {
                balance = _context
                            .Balances
                            .Local
                            .FirstOrDefault(b => b.Id == balanceId);
            }
            if (balance != null)
            {
                await _context.Entry(balance)
                    .Collection(b => b.BalanceWithdrawals).LoadAsync();
                await _context.Entry(balance)
                    .Reference(b => b.UserType).LoadAsync();
            }

            return balance;
        }

        public async Task<Balance> GetByUserIdAsync(string userId)
        {
            var balance = await _context
                    .Balances
                    .Include(b => b.WithdrawalLimit)
                    .FirstOrDefaultAsync(b => b.UserId == userId);
            if (balance == null)
            {
                balance = _context
                            .Balances
                            .Local
                            .FirstOrDefault(b => b.UserId == userId);
            }
            if (balance != null)
            {
                await _context.Entry(balance)
                    .Collection(b => b.BalanceWithdrawals).LoadAsync();
                await _context.Entry(balance)
                    .Reference(b => b.UserType).LoadAsync();
            }

            return balance;
        }


        public void Delete(Balance balance)
        {
            if (balance != null)
            {
                _context.Balances.Remove(balance);
            }
        }

        public void DeleteRange(List<Balance> balances)
        {
            if (balances.Any())
            {
                _context.Balances.RemoveRange(balances);
            }
        }
        #endregion

        #region Custom Functions
        public async Task<int?> GetBalanceIdByUserIdAsync(string userId)
        {
            var balance = await _context
                    .Balances
                    .Select(b => new
                    {
                        b.Id,
                        b.UserId
                    })
                    .FirstOrDefaultAsync(b => b.UserId == userId);

            if (balance != null)
            {
                return balance.Id;
            }

            return null;
        }

        public async Task IncreaseBalanceOnlyByUserId(string userId, decimal increaseAmount)
        {
            var balance = await _context.Balances.Where(b => b.UserId == userId)
                .Select(b => new
                {
                    b.Id,
                    b.UserId,
                    b.AmountAvailable,

                    b.RowVersion
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var toUpdate = new Balance(
                balance.Id,
                balance.UserId,
                balance.AmountAvailable,
                balance.RowVersion);

            _context.Balances.Attach(toUpdate);
            _context.Entry(toUpdate).Property(b => b.AmountAvailable).IsModified = true;
            //_context.Entry(toUpdate).Property(b => b.RowVersion).IsModified = true;

            toUpdate.Distribute(increaseAmount);
        }

        public async Task<decimal> GetAmountAvailableByUserIdAsync(string userId)
        {
            var amountAvailable = await _context.Balances.Where(b => b.UserId == userId)
                .Select(b => b.AmountAvailable)
                .FirstOrDefaultAsync();

            return amountAvailable;
        }

        public async Task<decimal> UpdateBalanceForNewOrderByUserId(string userId, Order order, IDateTimeService dateTimeService)
        {
            var balance = await _context.Balances.Where(b => b.UserId == userId)
                .Select(b => new
                {
                    b.Id,
                    b.UserId,
                    b.AmountAvailable,

                    b.RowVersion
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var toUpdate = new Balance(
                balance.Id,
                balance.UserId,
                balance.AmountAvailable,
                balance.RowVersion);

            _context.Balances.Attach(toUpdate);
            _context.Entry(toUpdate).Property(b => b.AmountAvailable).IsModified = true;
            //_context.Entry(toUpdate).Property(b => b.RowVersion).IsModified = true;

            toUpdate.NewOrder(order, dateTimeService);

            return toUpdate.AmountAvailable;
        }
        public async Task<decimal> UpdateBalanceForDistributeByUserId(string userId, decimal distributedAmount)
        {
            var balance = await _context.Balances.Where(b => b.UserId == userId)
                .Select(b => new
                {
                    b.Id,
                    b.UserId,
                    b.AmountAvailable,

                    b.RowVersion
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var toUpdate = new Balance(
                balance.Id,
                balance.UserId,
                balance.AmountAvailable,
                balance.RowVersion);

            _context.Balances.Attach(toUpdate);
            _context.Entry(toUpdate).Property(b => b.AmountAvailable).IsModified = true;
            //_context.Entry(toUpdate).Property(b => b.RowVersion).IsModified = true;

            toUpdate.Distribute(distributedAmount);

            return toUpdate.AmountAvailable;
        }

        #endregion

    }
}
