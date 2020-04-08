using Ordering.Domain.Exceptions;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ordering.Domain.Model.Orders
{
    public class OrderType : Enumeration
    {
        public static OrderType ShopToPlatform = new OrderType(1, "ShopToPlatform");
        public static OrderType ShopToFourthParty = new OrderType(2, "ShopToFourthParty");
        public static OrderType AdminToPlatform = new OrderType(3, "AdminToPlatform");

        public OrderType(int id, string name)
            : base(id, name)
        {
        }


        public static IEnumerable<OrderType> List() =>
            new[] { ShopToPlatform, ShopToFourthParty, AdminToPlatform };

        public static OrderType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new OrderingDomainException($"Possible values for OrderType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static OrderType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new OrderingDomainException($"Possible values for OrderType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
