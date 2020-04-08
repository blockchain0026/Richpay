using Microsoft.EntityFrameworkCore;
using Pairing.Domain.Model.FourthPartyGateways;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pairing.Infrastructure.Repositories
{
    public class FourthPartyGatewayRepository
     : IFourthPartyGatewayRepository
    {
        private readonly PairingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public FourthPartyGatewayRepository(PairingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public FourthPartyGateway Add(FourthPartyGateway fourthPartyGateway)
        {
            return _context.FourthPartyGateways.Add(fourthPartyGateway).Entity;

        }

        public void Update(FourthPartyGateway fourthPartyGateway)
        {
            _context.Entry(fourthPartyGateway).State = EntityState.Modified;
        }

        public async Task<FourthPartyGateway> GetByFourthPartyGatewayIdAsync(int fourthPartyGatewayId)
        {
            var fourthPartyGateway = await _context
                                .FourthPartyGateways
                                .FirstOrDefaultAsync(b => b.Id == fourthPartyGatewayId);
            if (fourthPartyGateway == null)
            {
                fourthPartyGateway = _context
                            .FourthPartyGateways
                            .Local
                            .FirstOrDefault(b => b.Id == fourthPartyGatewayId);
            }
            if (fourthPartyGateway != null)
            {
                //Nothing to load.
            }

            return fourthPartyGateway;
        }

        public async Task<FourthPartyGateway> GetByUserIdAsync(string userId)
        {
            var fourthPartyGateway = await _context
                    .FourthPartyGateways
                    .FirstOrDefaultAsync(b => b.UserId == userId);

            if (fourthPartyGateway == null)
            {
                fourthPartyGateway = _context
                            .FourthPartyGateways
                            .Local
                            .FirstOrDefault(b => b.UserId == userId);
            }
            if (fourthPartyGateway != null)
            {
                //Nothing to load.
            }

            return fourthPartyGateway;
        }

        public void Delete(FourthPartyGateway fourthPartyGateway)
        {
            if (fourthPartyGateway != null)
            {
                _context.FourthPartyGateways.Remove(fourthPartyGateway);
            }
        }

    }
}
