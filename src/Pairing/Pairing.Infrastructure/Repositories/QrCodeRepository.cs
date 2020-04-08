using Microsoft.EntityFrameworkCore;
using Pairing.Domain.Exceptions;
using Pairing.Domain.Model.QrCodes;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Pairing.Infrastructure.Repositories
{
    public class QrCodeRepository
      : IQrCodeRepository
    {
        private readonly PairingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public QrCodeRepository(PairingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public QrCode Add(QrCode qrCode)
        {
            return _context.QrCodes.Add(qrCode).Entity;

        }

        public void Update(QrCode qrCode)
        {
            _context.Entry(qrCode).State = EntityState.Modified;
        }

        public async Task<QrCode> GetByQrCodeIdAsync(int qrCodeId)
        {
            var qrCode = await _context
                                .QrCodes
                                .FirstOrDefaultAsync(b => b.Id == qrCodeId);
            if (qrCode == null)
            {
                qrCode = _context
                            .QrCodes
                            .Local
                            .FirstOrDefault(b => b.Id == qrCodeId);
            }
            if (qrCode != null)
            {
                await _context.Entry(qrCode)
                    .Reference(b => b.QrCodeSettings).LoadAsync();

                await _context.Entry(qrCode)
                    .Reference(b => b.PairingData).LoadAsync();

                await _context.Entry(qrCode)
                    .Reference(b => b.QrCodeType).LoadAsync();

                await _context.Entry(qrCode)
                    .Reference(b => b.PaymentChannel).LoadAsync();

                await _context.Entry(qrCode)
                    .Reference(b => b.PaymentScheme).LoadAsync();

                await _context.Entry(qrCode)
                    .Reference(b => b.QrCodeStatus).LoadAsync();

                await _context.Entry(qrCode)
                    .Reference(b => b.PairingStatus).LoadAsync();

                await _context.Entry(qrCode)
                    .Reference(b => b.ApprovedBy).LoadAsync();

                //Load by qr code's payment scheme
                if (qrCode.GetPaymentScheme.Id == PaymentScheme.Barcode.Id)
                {
                    //Load by qr code type.
                    if (qrCode.GetQrCodeType.Id == QrCodeType.Manual.Id)
                    {
                        await _context.Entry(qrCode)
                            .Reference(b => b.BarcodeDataForManual).LoadAsync();
                    }
                    else if (qrCode.GetQrCodeType.Id == QrCodeType.Auto.Id)
                    {
                        await _context.Entry(qrCode)
                            .Reference(b => b.BarcodeDataForAuto).LoadAsync();
                    }
                    else
                    {
                        throw new PairingDomainException("Can not recogonize qr code type.");
                    }
                }
                else if (qrCode.GetPaymentScheme.Id == PaymentScheme.Merchant.Id)
                {
                    await _context.Entry(qrCode)
                        .Reference(b => b.MerchantData).LoadAsync();
                }
                else if (qrCode.GetPaymentScheme.Id == PaymentScheme.Transaction.Id)
                {
                    await _context.Entry(qrCode)
                        .Reference(b => b.TransactionData).LoadAsync();
                }
                else if (qrCode.GetPaymentScheme.Id == PaymentScheme.Envelop.Id)
                {
                    await _context.Entry(qrCode)
                        .Reference(b => b.TransactionData).LoadAsync();
                }
                else if (qrCode.GetPaymentScheme.Id == PaymentScheme.EnvelopPassword.Id)
                {
                    await _context.Entry(qrCode)
                        .Reference(b => b.TransactionData).LoadAsync();
                }
                else if (qrCode.GetPaymentScheme.Id == PaymentScheme.Bank.Id)
                {
                    await _context.Entry(qrCode)
                        .Reference(b => b.BankData).LoadAsync();
                }
                else
                {
                    throw new PairingDomainException("Can not recogonize payment scheme.");
                }
            }

            return qrCode;
        }

        public async Task<QrCode> GetByQrCodeIdForFinishingOrderAsync(int qrCodeId)
        {
            var qrCode = await _context
                                .QrCodes
                                //.IncludeOptimized(q => q.PairingData)
                                .FirstOrDefaultAsync(b => b.Id == qrCodeId);
            if (qrCode == null)
            {
                qrCode = _context
                            .QrCodes
                            .Local
                            .FirstOrDefault(b => b.Id == qrCodeId);
            }
            if (qrCode != null)
            {
                await _context.Entry(qrCode)
                    .Reference(b => b.PairingData).LoadAsync();

                //await _context.Entry(qrCode)
                //    .Reference(b => b.QrCodeSettings).LoadAsync();

                /*await _context.Entry(qrCode)
                    .Reference(b => b.QrCodeType).LoadAsync();

                await _context.Entry(qrCode)
                    .Reference(b => b.PaymentChannel).LoadAsync();

                await _context.Entry(qrCode)
                    .Reference(b => b.PaymentScheme).LoadAsync();

                await _context.Entry(qrCode)
                    .Reference(b => b.PairingStatus).LoadAsync();*/
            }

            return qrCode;
        }

        public async Task<QrCode> GetByQrCodeIdForPairingAsync(int qrCodeId)
        {
            var qrCode = await _context
                                .QrCodes
                                .FirstOrDefaultAsync(b => b.Id == qrCodeId);
            if (qrCode == null)
            {
                qrCode = _context
                            .QrCodes
                            .Local
                            .FirstOrDefault(b => b.Id == qrCodeId);
            }
            if (qrCode != null)
            {
                await _context.Entry(qrCode)
                    .Reference(b => b.PairingData).LoadAsync();

                await _context.Entry(qrCode)
                    .Reference(b => b.QrCodeSettings).LoadAsync();

                /*await _context.Entry(qrCode)
                    .Reference(b => b.QrCodeType).LoadAsync();

                await _context.Entry(qrCode)
                    .Reference(b => b.PaymentChannel).LoadAsync();

                await _context.Entry(qrCode)
                    .Reference(b => b.PaymentScheme).LoadAsync();

                await _context.Entry(qrCode)
                    .Reference(b => b.PairingStatus).LoadAsync();*/
            }

            return qrCode;
        }

        public async Task<IEnumerable<QrCode>> GetByUserIdAsync(string userId)
        {
            var qrCodes = _context
                    .QrCodes
                    .Include(q => q.QrCodeStatus)
                    .Include(q => q.PairingStatus)
                    .Where(b => b.UserId == userId);

            foreach (var qrCode in qrCodes)
            {
                /*await _context.Entry(qrCode)
                    .Collection(b => b.QrCodeOrders).LoadAsync();*/

                await _context.Entry(qrCode)
                    .Reference(b => b.QrCodeSettings).LoadAsync();

                await _context.Entry(qrCode)
                    .Reference(b => b.PairingData).LoadAsync();

                //Load by qr code's payment scheme
                if (qrCode.GetPaymentScheme.Id == PaymentScheme.Barcode.Id)
                {
                    //Load by qr code type.
                    if (qrCode.GetQrCodeType.Id == QrCodeType.Manual.Id)
                    {
                        await _context.Entry(qrCode)
                            .Reference(b => b.BarcodeDataForManual).LoadAsync();
                    }
                    else if (qrCode.GetQrCodeType.Id == QrCodeType.Auto.Id)
                    {
                        await _context.Entry(qrCode)
                            .Reference(b => b.BarcodeDataForAuto).LoadAsync();
                    }
                    else
                    {
                        throw new PairingDomainException("Can not recogonize qr code type.");
                    }
                }
                else if (qrCode.GetPaymentScheme.Id == PaymentScheme.Merchant.Id)
                {
                    await _context.Entry(qrCode)
                        .Reference(b => b.MerchantData).LoadAsync();
                }
                else if (qrCode.GetPaymentScheme.Id == PaymentScheme.Transaction.Id)
                {
                    await _context.Entry(qrCode)
                        .Reference(b => b.TransactionData).LoadAsync();
                }
                else if (qrCode.GetPaymentScheme.Id == PaymentScheme.Envelop.Id)
                {
                    await _context.Entry(qrCode)
                        .Reference(b => b.TransactionData).LoadAsync();
                }
                else if (qrCode.GetPaymentScheme.Id == PaymentScheme.EnvelopPassword.Id)
                {
                    await _context.Entry(qrCode)
                        .Reference(b => b.TransactionData).LoadAsync();
                }
                else if (qrCode.GetPaymentScheme.Id == PaymentScheme.Bank.Id)
                {
                    await _context.Entry(qrCode)
                        .Reference(b => b.BankData).LoadAsync();
                }
                else
                {
                    throw new PairingDomainException("Can not recogonize payment scheme.");
                }
            }

            return qrCodes;
        }

        public async Task<IEnumerable<QrCode>> GetByShopIdAsync(string shopId)
        {
            var qrCodes = _context
                    .QrCodes
                    .Include(q => q.PairingData)
                    .Where(b => b.SpecifiedShopId == shopId);

            foreach (var qrCode in qrCodes)
            {
                await _context.Entry(qrCode)
                    .Reference(b => b.QrCodeSettings).LoadAsync();

                await _context.Entry(qrCode)
                    .Reference(b => b.PairingData).LoadAsync();

                //Load by qr code's payment scheme
                if (qrCode.GetPaymentScheme.Id == PaymentScheme.Barcode.Id)
                {
                    //Load by qr code type.
                    if (qrCode.GetQrCodeType.Id == QrCodeType.Manual.Id)
                    {
                        await _context.Entry(qrCode)
                            .Reference(b => b.BarcodeDataForManual).LoadAsync();
                    }
                    else if (qrCode.GetQrCodeType.Id == QrCodeType.Auto.Id)
                    {
                        await _context.Entry(qrCode)
                            .Reference(b => b.BarcodeDataForAuto).LoadAsync();
                    }
                    else
                    {
                        throw new PairingDomainException("Can not recogonize qr code type.");
                    }
                }
                else if (qrCode.GetPaymentScheme.Id == PaymentScheme.Merchant.Id)
                {
                    await _context.Entry(qrCode)
                        .Reference(b => b.MerchantData).LoadAsync();
                }
                else if (qrCode.GetPaymentScheme.Id == PaymentScheme.Transaction.Id)
                {
                    await _context.Entry(qrCode)
                        .Reference(b => b.TransactionData).LoadAsync();
                }
                else if (qrCode.GetPaymentScheme.Id == PaymentScheme.Envelop.Id)
                {
                    await _context.Entry(qrCode)
                        .Reference(b => b.TransactionData).LoadAsync();
                }
                else if (qrCode.GetPaymentScheme.Id == PaymentScheme.EnvelopPassword.Id)
                {
                    await _context.Entry(qrCode)
                        .Reference(b => b.TransactionData).LoadAsync();
                }
                else if (qrCode.GetPaymentScheme.Id == PaymentScheme.Bank.Id)
                {
                    await _context.Entry(qrCode)
                        .Reference(b => b.BankData).LoadAsync();
                }
                else
                {
                    throw new PairingDomainException("Can not recogonize payment scheme.");
                }
            }

            return qrCodes;
        }

        public async Task<QrCode> SearchByOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public void Delete(QrCode qrCode)
        {
            if (qrCode != null)
            {
                _context.QrCodes.Remove(qrCode);
            }
        }




        public async Task UpdateQrCodeBalanceWhenPaired(int qrCodeId, decimal availableBalance)
        {
            var originalQrCode = await _context.QrCodes
                .Include(q => q.PairingStatus)
                .Include(t => t.QrCodeSettings)
                .Where(u => u.Id == qrCodeId)
                .Select(u => new
                {
                    u.Id,
                    u.PairingStatus,
                    u.QrCodeSettings
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var toUpdate = new QrCode(
                originalQrCode.Id,
                originalQrCode.PairingStatus.Id,
                originalQrCode.QrCodeSettings
                );

            _context.QrCodes.Attach(toUpdate);
            _context.Entry(toUpdate).Property("_pairingStatusId").IsModified = true;
            _context.Entry(toUpdate).Property(b => b.AvailableBalance).IsModified = true;

            //Update available balance.
            toUpdate.BalanceUpdated(availableBalance);
        }

        public async Task UpdateUserQrCodesBalanceWhenPaired(string userId, decimal availableBalance, int skipId)
        {
            var userQrCodeIds = _context.QrCodes
                .AsNoTracking()
                .Where(q => q.UserId == userId)
                .Select(q => q.Id);

            foreach (var qrCodeId in userQrCodeIds)
            {
                if (qrCodeId != skipId)
                {
                    var originalQrCode = await _context.QrCodes
                        .Include(q => q.PairingStatus)
                        .Include(t => t.QrCodeSettings)
                        .Where(u => u.Id == qrCodeId)
                        .Select(u => new
                        {
                            u.Id,
                            u.PairingStatus,
                            u.QrCodeSettings
                        })
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                    var toUpdate = new QrCode(
                        originalQrCode.Id,
                        originalQrCode.PairingStatus.Id,
                        originalQrCode.QrCodeSettings
                        );

                    _context.QrCodes.Attach(toUpdate);
                    _context.Entry(toUpdate).Property("_pairingStatusId").IsModified = true;
                    _context.Entry(toUpdate).Property(b => b.AvailableBalance).IsModified = true;

                    //Update available balance.
                    toUpdate.BalanceUpdated(availableBalance);
                }
            }

        }

    }
}
