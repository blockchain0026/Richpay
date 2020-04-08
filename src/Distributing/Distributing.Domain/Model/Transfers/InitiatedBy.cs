using Distributing.Domain.Exceptions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributing.Domain.Model.Transfers
{
    public class InitiatedBy : Enumeration
    {
        public static InitiatedBy TraderAgent = new InitiatedBy(1, "TraderAgent");
        public static InitiatedBy Admin = new InitiatedBy(2, "Admin");

        public InitiatedBy(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<InitiatedBy> List() =>
            new[] { TraderAgent, Admin };

        public static InitiatedBy FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for InitiatedBy: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static InitiatedBy From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for InitiatedBy: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
