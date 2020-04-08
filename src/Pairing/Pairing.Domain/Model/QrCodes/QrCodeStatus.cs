using Pairing.Domain.Exceptions;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public class QrCodeStatus : Enumeration
    {
        public static QrCodeStatus Disabled = new QrCodeStatus(1, "Disabled");
        public static QrCodeStatus Enabled = new QrCodeStatus(2, "Enabled");
        public static QrCodeStatus AutoDisabled = new QrCodeStatus(3, "AutoDisabled");

        public QrCodeStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<QrCodeStatus> List() =>
            new[] { Disabled, Enabled, AutoDisabled };

        public static QrCodeStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new PairingDomainException($"Possible values for QrCodeStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static QrCodeStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new PairingDomainException($"Possible values for QrCodeStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
