using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pairing.Domain.Model.FourthPartyGateways
{
    public interface IFourthPartyGatewayRepository : IRepository<FourthPartyGateway>
    {
        FourthPartyGateway Add(FourthPartyGateway fourthPartyGateway);
        void Update(FourthPartyGateway fourthPartyGateway);
        void Delete(FourthPartyGateway fourthPartyGateway);

        Task<FourthPartyGateway> GetByFourthPartyGatewayIdAsync(int fourthPartyGatewayId);
        Task<FourthPartyGateway> GetByUserIdAsync(string userId);
    }
}
