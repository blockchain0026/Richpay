using Pairing.Domain.Exceptions;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public class TransactionData : ValueObject
    {
        public TransactionData(string userId)
        {
            //The alipay user id is 16 digit number.
            if (string.IsNullOrWhiteSpace(userId) || userId.Length != 16 || !userId.All(char.IsDigit))
            {
                throw new PairingDomainException("无效的UserId" + ". At TransactionData()");
            }
            UserId = userId;
        }

        public string UserId { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return UserId;
        }
    }
}
