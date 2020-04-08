using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public interface IPairingDataService
    {
        PairingData GetDefaultPairingData();
        void GetDefaultPairingData(out decimal toppestCommissionRate, out decimal availableBalance, out string specifiedShopId);
    }
}
