using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Domain.Model.Balances
{
    public interface IOpenTimeService
    {
        public bool IsWithdrawalOpenNow(DateTime currentDateTime);
        public bool IsDepositOpenNow(DateTime currentDateTime);
    }
}
