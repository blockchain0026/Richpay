using Distributing.Domain.Exceptions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributing.Domain.Model.Deposits
{
    public class DepositType : Enumeration
    {
        public static DepositType ByUser = new DepositType(1, "ByUser");
        public static DepositType ByAdmin = new DepositType(2, "ByAdmin");

        public DepositType(int id, string name)
            : base(id, name)
        {
        }


        public static IEnumerable<DepositType> List() =>
            new[] { ByUser, ByAdmin};

        public static DepositType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for DepositType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static DepositType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for DepositType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
