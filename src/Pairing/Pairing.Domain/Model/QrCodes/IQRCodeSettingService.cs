using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pairing.Domain.Model.QrCodes
{
    public interface IQrCodeSettingService
    {
        QrCodeSettings GetDefaultSettings();
    }
}
