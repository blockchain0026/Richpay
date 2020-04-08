using Pairing.Domain.Exceptions;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public class PaymentChannel : Enumeration
    {
        public static PaymentChannel Alipay = new PaymentChannel(1, "Alipay");
        public static PaymentChannel Wechat = new PaymentChannel(2, "Wechat");

        public PaymentChannel(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<PaymentChannel> List() =>
            new[] { Alipay, Wechat };

        public static PaymentChannel FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new PairingDomainException($"Possible values for PaymentChannel: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static PaymentChannel From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new PairingDomainException($"Possible values for PaymentChannel: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
