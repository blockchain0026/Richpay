using Distributing.Domain.Exceptions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributing.Domain.Model.Deposits
{
    public class DepositStatus : Enumeration
    {
        public static DepositStatus Submitted = new DepositStatus(1, "Submitted");
        public static DepositStatus Success = new DepositStatus(2, "Success");
        public static DepositStatus Canceled = new DepositStatus(3, "Canceled");

        public DepositStatus(int id, string name)
            : base(id, name)
        {
        }


        public static IEnumerable<DepositStatus> List() =>
            new[] { Submitted, Success, Canceled };

        public static DepositStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for DepositStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static DepositStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for DepositStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
