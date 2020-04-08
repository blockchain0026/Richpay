using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public interface IQuotaService
    {
        Quota GetDefaultQuota();
    }
}
