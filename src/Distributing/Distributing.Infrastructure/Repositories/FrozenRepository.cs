using Distributing.Domain.Model.Frozens;
using Distributing.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Infrastructure.Repositories
{
    public class FrozenRepository
       : IFrozenRepository
    {
        private readonly DistributingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public FrozenRepository(DistributingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Frozen Add(Frozen frozen)
        {
            return _context.Frozens.Add(frozen).Entity;

        }

        public void Update(Frozen frozen)
        {
            _context.Entry(frozen).State = EntityState.Modified;
        }

        public async Task<Frozen> GetByFrozenIdAsync(int frozenId)
        {
            var frozen = await _context
                                .Frozens
                                .Include(b => b.ByAdmin)
                                .FirstOrDefaultAsync(b => b.Id == frozenId);
            if (frozen == null)
            {
                frozen = _context
                            .Frozens
                            .Local
                            .FirstOrDefault(b => b.Id == frozenId);
            }
            if (frozen != null)
            {
                await _context.Entry(frozen)
                    .Reference(b => b.FrozenStatus).LoadAsync();
                await _context.Entry(frozen)
                    .Reference(b => b.FrozenType).LoadAsync();
            }

            return frozen;
        }

        public async Task<Frozen> GetByOrderTrackingNumberAsync(string trackingNumber)
        {
            var frozen = await _context
                     .Frozens
                     .Include(b => b.ByAdmin)
                     .FirstOrDefaultAsync(b => b.OrderTrackingNumber == trackingNumber);
            if (frozen == null)
            {
                frozen = _context
                            .Frozens
                            .Local
                            .FirstOrDefault(b => b.OrderTrackingNumber == trackingNumber);
            }
            if (frozen != null)
            {
                await _context.Entry(frozen)
                    .Reference(b => b.FrozenStatus).LoadAsync();
                await _context.Entry(frozen)
                    .Reference(b => b.FrozenType).LoadAsync();
            }



            return frozen;
        }

        public async Task<Frozen> GetByWithdrawalIdAsync(int withdrawalId)
        {
            var frozen = await _context
                    .Frozens
                    .Include(b => b.ByAdmin)
                    .FirstOrDefaultAsync(b => b.WithdrawalId == withdrawalId);
            if (frozen == null)
            {
                frozen = _context
                            .Frozens
                            .Local
                            .FirstOrDefault(b => b.WithdrawalId == withdrawalId);
            }
            if (frozen != null)
            {
                await _context.Entry(frozen)
                    .Reference(b => b.FrozenStatus).LoadAsync();
                await _context.Entry(frozen)
                    .Reference(b => b.FrozenType).LoadAsync();
            }

            return frozen;

        }

        public async Task<Frozen> GetByAdminIdAsync(string adminId)
        {
            var frozen = await _context
                    .Frozens
                    .Include(b => b.ByAdmin)
                    .FirstOrDefaultAsync(b => b.ByAdmin.AdminId == adminId);

            if (frozen == null)
            {
                frozen = _context
                            .Frozens
                            .Local
                            .FirstOrDefault(b => b.ByAdmin.AdminId == adminId);
            }
            if (frozen != null)
            {
                await _context.Entry(frozen)
                    .Reference(b => b.FrozenStatus).LoadAsync();
                await _context.Entry(frozen)
                    .Reference(b => b.FrozenType).LoadAsync();
            }


            return frozen;
        }

        public async Task<IEnumerable<Frozen>> GetByBalanceIdAsync(int balanceId)
        {
            var frozens =  _context
                    .Frozens
                    .Include(b => b.ByAdmin)
                    .Where(b => b.BalanceId == balanceId);


            if (frozens.Any())
            {
                foreach (var frozen in frozens)
                {
                    await _context.Entry(frozen)
                            .Reference(b => b.FrozenStatus).LoadAsync();
                    await _context.Entry(frozen)
                        .Reference(b => b.FrozenType).LoadAsync();
                }
            }

            return frozens;
        }

        public async Task<IEnumerable<Frozen>> GetByUserIdAsync(string userId)
        {
            var frozens = _context
                    .Frozens
                    .Include(b => b.ByAdmin)
                    .Where(b => b.UserId == userId);


            if (frozens.Any())
            {
                foreach (var frozen in frozens)
                {
                    await _context.Entry(frozen)
                            .Reference(b => b.FrozenStatus).LoadAsync();
                    await _context.Entry(frozen)
                        .Reference(b => b.FrozenType).LoadAsync();
                }
            }

            return frozens;
        }

        public void Delete(Frozen frozen)
        {
            if (frozen != null)
            {
                _context.Frozens.Remove(frozen);
            }
        }

        public void DeleteRange(List<Frozen> frozens)
        {
            if (frozens.Any())
            {
                _context.Frozens.RemoveRange(frozens);
            }
        }


    }
}

