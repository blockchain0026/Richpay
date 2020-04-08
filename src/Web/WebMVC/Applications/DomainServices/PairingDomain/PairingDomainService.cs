using Microsoft.EntityFrameworkCore;
using Pairing.Domain.Exceptions;
using Pairing.Domain.Model.QrCodes;
using Pairing.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.CacheServices;
using WebMVC.Applications.Queries;

namespace WebMVC.Applications.DomainServices.PairingDomain
{
    public class PairingDomainService : IPairingDomainService
    {
        private readonly PairingContext _context;
        private readonly IQrCodeRepository _qrCodeRepository;
        private readonly IQrCodeQueueService _qrCodeQueueService;

        public PairingDomainService(PairingContext context, IQrCodeRepository qrCodeRepository, IQrCodeQueueService qrCodeQueueService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _qrCodeRepository = qrCodeRepository ?? throw new ArgumentNullException(nameof(qrCodeRepository));
            _qrCodeQueueService = qrCodeQueueService ?? throw new ArgumentNullException(nameof(qrCodeQueueService));
        }

        public async Task<decimal> GetAvailableBalanceFrom(QrCode qrCode)
        {
            //Checking the qr code is not null.
            if (qrCode is null)
            {
                throw new PairingDomainException("The qr code must be provided.");
            }

            //Get qr code's available balance.
            var availableBalance = qrCode.AvailableBalance;

            return availableBalance;
        }

        public async Task<QrCode> PairFrom(Order order, bool isAuto)
        {
            while (true)
            {
                //Check the order is provided.
                if (order is null)
                {
                    throw new PairingDomainException("The order must be provided.");
                }

                if (isAuto)
                {
                    throw new NotImplementedException("尚未实作自动配对");
                }
                else
                {
                    //Search available qr code for manual pairing.
                    var qrCode = await this.GetNextQrCode(
                        order.PaymentChannel,
                        order.PaymentScheme,
                        order.Amount,
                        order.ShopId
                        );

                    if (qrCode is null)
                    {
                        Console.WriteLine("No Qr Code Available...");
                        return null;
                    }

                    //Prevent any change on the qr code between the time gap above.
                    if (qrCode.GetPairingStatus.Id != PairingStatus.Pairing.Id)
                    {
                        //Clear the tracking on qr code.
                        _context.ClearTrackedChangeOnAllEntityEntries();
                        continue;
                    }

                    //Pair.
                    qrCode.PairWithOrder(order);

                    //Update.
                    _context.Entry(qrCode).State = EntityState.Modified;

                    //return null;

                    return qrCode;
                }
            }
        }


        private async Task<QrCode> GetNextQrCode(string paymentChannel, string paymentScheme, decimal orderAmount, string specifiedShopId)
        {
            var nextId = _qrCodeQueueService.GetNextQrCodeId(
                paymentChannel,
                paymentScheme,
                orderAmount,
                specifiedShopId
                );
            if (nextId == default(int))
            {
                return null;
            }

            var nextQrCode = await _qrCodeRepository.GetByQrCodeIdForPairingAsync(nextId);
            return nextQrCode;
        }
    }
}
