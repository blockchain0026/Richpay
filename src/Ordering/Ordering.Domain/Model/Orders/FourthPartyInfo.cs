using Ordering.Domain.Exceptions;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Model.Orders
{
    public class FourthPartyInfo : ValueObject
    {
        public string UserId { get; private set; }
        public string FourthPartyName { get; private set; }

        public string FourthPartyOrderPaymentUrl { get; private set; }
        public string FourthPartyOrderNumber { get; private set; }


        public FourthPartyInfo(string userId, string fourthPartyName)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new OrderingDomainException("无效的四方ID. At FourthPartyInfo()");
            }

            if (string.IsNullOrEmpty(fourthPartyName))
            {
                throw new OrderingDomainException("无效的四方名称. At FourthPartyInfo()");
            }

            this.UserId = userId;
            this.FourthPartyName = fourthPartyName;
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return UserId;
        }
    }
}
