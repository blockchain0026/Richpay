using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pairing.Domain.Model.QrCodes
{
    public interface IQrCodeRepository : IRepository<QrCode>
    {
        QrCode Add(QrCode qrCode);
        void Update(QrCode qrCode);
        void Delete(QrCode qrCode);

        Task<QrCode> GetByQrCodeIdAsync(int qrCodeId);
        Task<QrCode> GetByQrCodeIdForFinishingOrderAsync(int qrCodeId);
        Task<QrCode> GetByQrCodeIdForPairingAsync(int qrCodeId);
        Task<IEnumerable<QrCode>> GetByUserIdAsync(string userId);
        Task<IEnumerable<QrCode>> GetByShopIdAsync(string shopId);

        Task<QrCode> SearchByOrder(Order order);


        Task UpdateQrCodeBalanceWhenPaired(int qrCodeId, decimal availableBalance);
        Task UpdateUserQrCodesBalanceWhenPaired(string userId, decimal availableBalance,int skipId);
    }
}
