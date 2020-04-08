using Ordering.Domain.Exceptions;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ordering.Domain.Model.Orders
{
    public class OrderPaymentChannel : Enumeration
    {
        public static OrderPaymentChannel Alipay = new OrderPaymentChannel(1, "Alipay");
        public static OrderPaymentChannel Wechat = new OrderPaymentChannel(2, "Wechat");

        public OrderPaymentChannel(int id, string name)
            : base(id, name)
        {
        }


        public static IEnumerable<OrderPaymentChannel> List() =>
            new[] { Alipay, Wechat };

        public static OrderPaymentChannel FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new OrderingDomainException($"Possible values for OrderPaymentChannel: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static OrderPaymentChannel From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new OrderingDomainException($"Possible values for OrderPaymentChannel: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
