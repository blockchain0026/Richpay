using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.QrCodes;

namespace WebMVC.Applications.SideEffectServices
{
    public class QrCodeSideEffectService : IQrCodeSideEffectService
    {
        private readonly IQrCodeQueries _qrCodeQueries;

        private readonly IQrCodeRepository _qrCodeRepository;

        public QrCodeSideEffectService(IQrCodeQueries qrCodeQueries, IQrCodeRepository qrCodeRepository)
        {
            _qrCodeQueries = qrCodeQueries ?? throw new ArgumentNullException(nameof(qrCodeQueries));
            _qrCodeRepository = qrCodeRepository ?? throw new ArgumentNullException(nameof(qrCodeRepository));
        }

        public async Task UpdateQrCodeWhenTraderBalanceUpdated(string traderId, decimal availableBalance)
        {
            //Find all qr codes and update balance.
            var userQrCodes = await _qrCodeRepository.GetByUserIdAsync(traderId);
            foreach (var userQrCode in userQrCodes)
            {
                userQrCode.BalanceUpdated(availableBalance);

                _qrCodeRepository.Update(userQrCode);

                //Update Qr code view model.
                var userQrCodeVM = await _qrCodeQueries.GetQrCodeEntryAsync(userQrCode.Id);

                if (userQrCodeVM != null)
                {
                    userQrCodeVM.AvailableBalance = userQrCode.AvailableBalance;

                    userQrCodeVM.PairingStatus = userQrCode.GetPairingStatus.Name;
                    userQrCodeVM.PairingStatusDescription = userQrCode.PairingStatusDescription;
                    _qrCodeQueries.Update(userQrCodeVM);
                }
            }

            await _qrCodeQueries.SaveChangesAsync();
            await _qrCodeRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
