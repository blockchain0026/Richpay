using Distributing.Domain.Exceptions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributing.Domain.Model.Banks
{
    public class BankAccount : ValueObject
    {
        public string BankName { get; private set; }
        public string AccountName { get; private set; }
        public string AccountNumber { get; private set; }


        public BankAccount(string bankName, string accountName, string accountNumber)
        {
            //Checking the bank's name isn't null or empty.
            if (string.IsNullOrEmpty(bankName))
            {
                throw new DistributingDomainException("无效的银行名称" + ". At BankAccount()");
            }

            //Check the length of bank's name. (<=10 chinese characters)
            if (bankName.Length > 20)
            {
                throw new DistributingDomainException("银行名称长度必须小于10个中文字" + ". At BankAccount()");
            }

            //Checking the account's name isn't null or empty.
            if (string.IsNullOrEmpty(accountName))
            {
                throw new DistributingDomainException("无效的户名" + ". At BankAccount()");
            }

            //Checking the length of account's name. (<=10 chinese characters)
            if (accountName.Length > 20)
            {
                throw new DistributingDomainException("户名长度必须小于10个中文字" + ". At BankAccount()");
            }

            //Checking the length of account's number (<=50 letters)
            if (accountNumber.Length > 50)
            {
                throw new DistributingDomainException("账号长度必须小于50字" + ". At BankAccount()");
            }

            //Checking the account's number are valid digits.
            if (!accountNumber.All(char.IsDigit))
            {
                throw new DistributingDomainException("账号必须为有效数字" + ". At BankAccount()");
            }


            this.BankName = bankName;
            this.AccountName = accountName;
            this.AccountNumber = accountNumber;
        }


        public static bool IsBankNameValid(string bankName)
        {
            //Checking the bank's name isn't null or empty.
            if (string.IsNullOrEmpty(bankName))
            {
                return false;
            }

            //Check the length of bank's name. (<=10 chinese characters)
            if (bankName.Length > 20)
            {
                return false;
            }

            return true;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return BankName;
            yield return AccountName;
            yield return AccountNumber;
        }
    }
}
