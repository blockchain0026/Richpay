using Distributing.Domain.Exceptions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributing.Domain.Model.Frozens
{
    public class FrozenType : Enumeration
    {
        public static FrozenType Order = new FrozenType(1, "Order");
        public static FrozenType Withdrawal = new FrozenType(2, "Withdrawal");
        public static FrozenType ByAdmin = new FrozenType(3, "ByAdmin");

        public FrozenType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<FrozenType> List() =>
            new[] { Order, Withdrawal, ByAdmin };

        public static FrozenType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for FrozenType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static FrozenType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for FrozenType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
