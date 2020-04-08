using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Applications.DomainServices.PairingDomain
{
    public class FakePairingDataService : IPairingDataService
    {
        public FakePairingDataService(decimal toppestCommissionRate, decimal availableBalance, string specifiedShopId)
        {
            ToppestCommissionRate = toppestCommissionRate;
            AvailableBalance = availableBalance;
            SpecifiedShopId = specifiedShopId;
        }

        public decimal ToppestCommissionRate { get; private set; }
        public decimal AvailableBalance { get; private set; }
        public string SpecifiedShopId { get; private set; }





        public PairingData GetDefaultPairingData()
        {
            return PairingData.From(ToppestCommissionRate, AvailableBalance, SpecifiedShopId);
        }

        public void GetDefaultPairingData(out decimal toppestCommissionRate,
            out decimal availableBalance,
            out string specifiedShopId)
        {
            toppestCommissionRate = this.ToppestCommissionRate;
            availableBalance = this.AvailableBalance;
            specifiedShopId = this.SpecifiedShopId;
        }
    }
}
