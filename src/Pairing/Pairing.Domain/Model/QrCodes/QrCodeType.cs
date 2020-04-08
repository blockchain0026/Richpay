using Pairing.Domain.Exceptions;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public class QrCodeType : Enumeration
    {
        public static QrCodeType Auto = new QrCodeType(1, "Auto");
        public static QrCodeType Manual = new QrCodeType(2, "Manual");

        public QrCodeType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<QrCodeType> List() =>
            new[] { Auto, Manual };

        public static QrCodeType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new PairingDomainException($"Possible values for QrCodeType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static QrCodeType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new PairingDomainException($"Possible values for QrCodeType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
