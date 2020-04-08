using Distributing.Domain.Exceptions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributing.Domain.Model.Withdrawals
{

    public class WithdrawalType : Enumeration
    {
        public static WithdrawalType ByUser = new WithdrawalType(1, "ByUser");
        public static WithdrawalType ByAdmin = new WithdrawalType(2, "ByAdmin");

        public WithdrawalType(int id, string name)
            : base(id, name)
        {
        }
        public static IEnumerable<WithdrawalType> List() =>
            new[] { ByUser, ByAdmin };

        public static WithdrawalType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for WithdrawalType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static WithdrawalType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for WithdrawalType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
