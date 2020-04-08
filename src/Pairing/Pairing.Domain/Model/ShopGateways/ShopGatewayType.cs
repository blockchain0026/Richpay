using Pairing.Domain.Exceptions;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pairing.Domain.Model.ShopGateways
{
    public class ShopGatewayType : Enumeration
    {
        public static ShopGatewayType ToPlatform = new ShopGatewayType(1, "ToPlatform");
        public static ShopGatewayType ToFourthParty = new ShopGatewayType(2, "ToFourthParty");

        public ShopGatewayType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<ShopGatewayType> List() =>
            new[] { ToPlatform, ToFourthParty };

        public static ShopGatewayType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new PairingDomainException($"Possible values for ShopGatewayType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static ShopGatewayType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new PairingDomainException($"Possible values for ShopGatewayType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
