using Distributing.Domain.Exceptions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributing.Domain.Model.Withdrawals
{
    public class WithdrawalStatus : Enumeration
    {
        public static WithdrawalStatus Submitted = new WithdrawalStatus(1, "Submitted");
        public static WithdrawalStatus Approved = new WithdrawalStatus(2, "Approved");
        public static WithdrawalStatus Success = new WithdrawalStatus(3, "Success");
        public static WithdrawalStatus AwaitingCancellation = new WithdrawalStatus(4, "AwaitingCancellation");
        public static WithdrawalStatus Canceled = new WithdrawalStatus(5, "Canceled");

        public WithdrawalStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<WithdrawalStatus> List() =>
            new[] { Submitted, Approved, Success, AwaitingCancellation, Canceled };

        public static WithdrawalStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for WithdrawalStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static WithdrawalStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for WithdrawalStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
