using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.CacheServices
{
    public interface IQrCodeCacheService
    {
        PairingInfo GetPairingInfoByQrCodeId(int qrCodeId);
        Task UpdatePairingInfo();
    }
}
