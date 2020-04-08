using Distributing.Domain.Exceptions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributing.Domain.Model.Frozens
{
    public class FrozenStatus : Enumeration
    {
        public static FrozenStatus Frozen = new FrozenStatus(1, "Frozen");
        public static FrozenStatus Unfrozen = new FrozenStatus(2, "Unfrozen");

        public FrozenStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<FrozenStatus> List() =>
            new[] { Frozen, Unfrozen };

        public static FrozenStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for FrozenStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static FrozenStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for FrozenStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
