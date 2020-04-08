using Pairing.Domain.Exceptions;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public class PairingStatus : Enumeration
    {
        public static PairingStatus DisableBySystem = new PairingStatus(1, "DisableBySystem");
        public static PairingStatus NormalDisable = new PairingStatus(2, "NormalDisable");
        public static PairingStatus Pairing = new PairingStatus(3, "Pairing");
        public static PairingStatus ProcessingOrder = new PairingStatus(4, "ProcessingOrder");

        public PairingStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<PairingStatus> List() =>
            new[] { DisableBySystem, NormalDisable, Pairing, ProcessingOrder };

        public static PairingStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new PairingDomainException($"Possible values for PairingStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static PairingStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new PairingDomainException($"Possible values for PairingStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
