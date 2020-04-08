using Distributing.Domain.Exceptions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributing.Domain.Model.Distributions
{
    public class DistributionType : Enumeration
    {
        public static DistributionType Commission = new DistributionType(1, "Commission");
        public static DistributionType Liquidation = new DistributionType(2, "Liquidation");

        public DistributionType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<DistributionType> List() =>
            new[] { Commission, Liquidation};

        public static DistributionType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for DistributionType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static DistributionType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for DistributionType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
