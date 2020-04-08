using Pairing.Domain.Exceptions;
using Pairing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pairing.Domain.Model.QrCodes
{
    public class BankData : ValueObject
    {
        public BankData(string bankName, string bankMark, string accountName, string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(bankName) || bankName.Length > 20)
            {
                throw new PairingDomainException("银行名称需小于10个中文字" + ". At BankData()");
            }
            if (string.IsNullOrWhiteSpace(bankMark) || bankMark.Length > 10)
            {
                throw new PairingDomainException("银行代号需小于10个字" + ". At BankData()");
            }
            if (string.IsNullOrWhiteSpace(accountName) || accountName.Length > 20)
            {
                throw new PairingDomainException("户名须小于10个中文字" + ". At BankData()");
            }
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length > 50 || !cardNumber.All(char.IsDigit))
            {
                throw new PairingDomainException("卡号须为数字且长度小于50个字" + ". At BankData()");
            }

            BankName = bankName ?? throw new ArgumentNullException(nameof(bankName));
            BankMark = bankMark ?? throw new ArgumentNullException(nameof(bankMark));
            AccountName = accountName ?? throw new ArgumentNullException(nameof(accountName));
            CardNumber = cardNumber ?? throw new ArgumentNullException(nameof(cardNumber));
        }

        public string BankName { get; private set; }
        public string BankMark { get; private set; }
        public string AccountName { get; private set; }
        public string CardNumber { get; private set; }



        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return BankName;
            yield return BankMark;
            yield return AccountName;
            yield return CardNumber;
        }
    }
}
