using Pairing.Domain.Exceptions;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pairing.Domain.Model.CloudDevices
{
    public class CloudDeviceStatus : Enumeration
    {
        public static CloudDeviceStatus Online = new CloudDeviceStatus(1, "Online");
        public static CloudDeviceStatus Offline = new CloudDeviceStatus(2, "Offline");

        public CloudDeviceStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<CloudDeviceStatus> List() =>
            new[] { Online, Offline };

        public static CloudDeviceStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new PairingDomainException($"Possible values for CloudDeviceStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static CloudDeviceStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new PairingDomainException($"Possible values for CloudDeviceStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
