using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Applications.SideEffectServices
{
    public interface IQrCodeSideEffectService
    {
        Task UpdateQrCodeWhenTraderBalanceUpdated(string traderId, decimal availableBalance);
    }
}
