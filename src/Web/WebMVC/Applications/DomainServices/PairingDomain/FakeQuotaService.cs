using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Applications.DomainServices.PairingDomain
{
    public class FakeQuotaService : IQuotaService
    {
        public decimal DailyAmountLimit { get; private set; }
        public decimal OrderAmountUpperLimit { get; private set; }
        public decimal OrderAmountLowerLimit { get; private set; }


        public FakeQuotaService(decimal dailyAmountLimit, decimal orderAmountUpperLimit, decimal orderAmountLowerLimit)
        {
            DailyAmountLimit = dailyAmountLimit;
            OrderAmountUpperLimit = orderAmountUpperLimit;
            OrderAmountLowerLimit = orderAmountLowerLimit;
        }



        public Quota GetDefaultQuota()
        {
            return new Quota(
                DailyAmountLimit,
                OrderAmountUpperLimit,
                OrderAmountLowerLimit
                );
        }
    }
}
