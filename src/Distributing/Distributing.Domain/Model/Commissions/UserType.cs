using Distributing.Domain.Exceptions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributing.Domain.Model.Commissions
{
    public class UserType : Enumeration
    {
        public static UserType Trader = new UserType(1, "Trader");
        public static UserType TraderAgent = new UserType(2, "TraderAgent");
        public static UserType Shop = new UserType(3, "Shop");
        public static UserType ShopAgent = new UserType(4, "ShopAgent");

        public UserType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<UserType> List() =>
            new[] { Trader, TraderAgent, Shop, ShopAgent };

        public static UserType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for UserType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static UserType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for UserType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
