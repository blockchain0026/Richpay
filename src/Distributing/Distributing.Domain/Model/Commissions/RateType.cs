using Distributing.Domain.Exceptions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributing.Domain.Model.Commissions
{
    public class RateType : Enumeration
    {
        public static RateType Alipay = new RateType(1, "Alipay");
        public static RateType Wechat = new RateType(2, "Wechat");

        public RateType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<RateType> List() =>
            new[] { Alipay, Wechat };

        public static RateType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for RateType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static RateType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DistributingDomainException($"Possible values for RateType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
