using Pairing.Domain.Exceptions;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public class PaymentScheme : Enumeration
    {
        public static PaymentScheme Barcode = new PaymentScheme(1, "Barcode");
        public static PaymentScheme Merchant = new PaymentScheme(2, "Merchant");
        public static PaymentScheme Transaction = new PaymentScheme(3, "Transaction");
        public static PaymentScheme Bank = new PaymentScheme(4, "Bank");
        public static PaymentScheme Envelop = new PaymentScheme(5, "Envelop");
        public static PaymentScheme EnvelopPassword = new PaymentScheme(6, "EnvelopPassword");

        public PaymentScheme(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<PaymentScheme> List() =>
            new[] { Barcode, Merchant, Transaction, Bank, Envelop , EnvelopPassword };

        public static PaymentScheme FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new PairingDomainException($"Possible values for PaymentScheme: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static PaymentScheme From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new PairingDomainException($"Possible values for PaymentScheme: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
