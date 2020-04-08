using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pairing.Domain.Model.QrCodes
{
    public interface IPairingDomainService
    {
        Task<decimal> GetAvailableBalanceFrom(QrCode qrCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="isAuto"></param>
        /// <returns></returns>
        Task<QrCode> PairFrom(Order order, bool isAuto);
    }
}
