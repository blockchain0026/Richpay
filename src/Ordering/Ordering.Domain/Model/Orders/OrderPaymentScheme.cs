using Ordering.Domain.Exceptions;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ordering.Domain.Model.Orders
{
    public class OrderPaymentScheme : Enumeration
    {
        public static OrderPaymentScheme Barcode = new OrderPaymentScheme(1, "Barcode");
        public static OrderPaymentScheme Merchant = new OrderPaymentScheme(2, "Merchant");
        public static OrderPaymentScheme Transaction = new OrderPaymentScheme(3, "Transaction");
        public static OrderPaymentScheme Bank = new OrderPaymentScheme(4, "Bank");
        public static OrderPaymentScheme Envelop = new OrderPaymentScheme(5, "Envelop");
        public static OrderPaymentScheme EnvelopPassword = new OrderPaymentScheme(6, "EnvelopPassword");

        public OrderPaymentScheme(int id, string name)
            : base(id, name)
        {
        }


        public static IEnumerable<OrderPaymentScheme> List() =>
            new[] { Barcode, Merchant, Transaction, Bank, Envelop, EnvelopPassword };

        public static OrderPaymentScheme FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new OrderingDomainException($"Possible values for OrderPaymentScheme: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static OrderPaymentScheme From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new OrderingDomainException($"Possible values for OrderPaymentScheme: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
