using Distributing.Domain.Model.Transfers;
using Distributing.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Infrastructure.Repositories
{
    public class TransferRepository
   : ITransferRepository
    {
        private readonly DistributingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public TransferRepository(DistributingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Transfer Add(Transfer transfer)
        {
            return _context.Transfers.Add(transfer).Entity;

        }

        public void Update(Transfer transfer)
        {
            _context.Entry(transfer).State = EntityState.Modified;
        }

        public async Task<Transfer> GetByTransferIdAsync(int transferId)
        {
            var transfer = await _context
                                .Transfers
                                .FirstOrDefaultAsync(b => b.Id == transferId);
            if (transfer == null)
            {
                transfer = _context
                            .Transfers
                            .Local
                            .FirstOrDefault(b => b.Id == transferId);
            }
            if (transfer != null)
            {
                await _context.Entry(transfer)
                    .Reference(b => b.InitiatedBy).LoadAsync();
            }

            return transfer;
        }

        public async Task<Transfer> GetByUserIdAsync(string userId)
        {
            var transfer = await _context
                    .Transfers
                    .FirstOrDefaultAsync(b => b.UserId == userId);
            if (transfer == null)
            {
                transfer = _context
                            .Transfers
                            .Local
                            .FirstOrDefault(b => b.UserId == userId);
            }
            if (transfer != null)
            {
                await _context.Entry(transfer)
                    .Reference(b => b.InitiatedBy).LoadAsync();
            }

            return transfer;
        }

        public void Delete(Transfer transfer)
        {
            if (transfer != null)
            {
                _context.Transfers.Remove(transfer);
            }
        }

        public void DeleteRange(List<Transfer> transfers)
        {
            if (transfers.Any())
            {
                _context.Transfers.RemoveRange(transfers);
            }
        }
    }
}
