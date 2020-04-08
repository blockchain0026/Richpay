using Pairing.Domain.Exceptions;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public class BarcodeDataForManual : ValueObject
    {
        public BarcodeDataForManual(string qrCodeUrl, decimal? amount)
        {
            QrCodeUrl = qrCodeUrl ?? throw new ArgumentNullException(nameof(qrCodeUrl));
            Amount = amount;
        }

        public string QrCodeUrl { get; private set; }
        public decimal? Amount { get; private set; }

        public static BarcodeDataForManual From(string qrCodeUrl, decimal? amount, ShopSettings shopSettings = null)
        {
            //Checking the qrcode url is a valid http/https url.
            var isUrlValid = Uri.TryCreate(qrCodeUrl, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (!isUrlValid)
            {
                throw new PairingDomainException("无效的二维码链结" + ". At BarcodeDataForManual.From()");
            }

            //Checking the amount is larger than 0.
            if (amount <= 0)
            {
                throw new PairingDomainException("无效的金额" + ". At BarcodeDataForManual.From()");
            }

            //Checking the amount match shop amount options
            if (shopSettings != null)
            {
                //If shop has amount options then check the amount is provided and matched.
                if (shopSettings.OrderAmountOptions.Any())
                {
                    if (amount == null)
                    {
                        throw new PairingDomainException("无效的金额，须符合商户提供的金额选项" + ". At BarcodeDataForManual.From()");
                    }
                    if (!shopSettings.OrderAmountOptions.Any(o => o.GetAmount() == (decimal)amount))
                    {
                        throw new PairingDomainException("找不到与个码金额相符合的商户订单金额选项" + ". At BarcodeDataForManual.From()");
                    }
                }
            }

            var barcodeDataForManual = new BarcodeDataForManual(qrCodeUrl, amount);

            return barcodeDataForManual;
        }



        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return QrCodeUrl;
            yield return Amount;
        }
    }
}
