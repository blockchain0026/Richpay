using Pairing.Domain.Exceptions;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Model.Shared
{
    public class PaymentSchemeValidator
    {
        public static bool IsPaymentSchemeSupportedBy(PaymentChannel paymentChannel, PaymentScheme paymentScheme)
        {
            if (paymentChannel.Id == PaymentChannel.Alipay.Id)
            {
                if (paymentScheme.Id == PaymentScheme.Barcode.Id
                    || paymentScheme.Id == PaymentScheme.Merchant.Id
                    || paymentScheme.Id == PaymentScheme.Transaction.Id
                    || paymentScheme.Id == PaymentScheme.Bank.Id
                    || paymentScheme.Id == PaymentScheme.Envelop.Id
                    || paymentScheme.Id == PaymentScheme.EnvelopPassword.Id
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (paymentChannel.Id == PaymentChannel.Wechat.Id)
            {
                if (paymentScheme.Id == PaymentScheme.Barcode.Id
                    || paymentScheme.Id == PaymentScheme.Merchant.Id
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            throw new PairingDomainException($"The payment channel provided is not recognize.");
        }
    }
}
