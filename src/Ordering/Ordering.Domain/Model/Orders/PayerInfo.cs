using Ordering.Domain.Exceptions;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Ordering.Domain.Model.Orders
{
    public class PayerInfo : ValueObject
    {
        public string IpAddress { get; private set; }
        public string Device { get; private set; }
        public string Location { get; private set; }

        public PayerInfo(string ipAddress, string device, string location)
        {
            IpAddress = IsValidIP(ipAddress) ? ipAddress : throw new OrderingDomainException("无效的IP");
            Device = device;
            Location = location;
        }

        public bool IsValidIP(string addr)
        {
            //create our match pattern
            string pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

            //create our Regular Expression object
            Regex check = new Regex(pattern);

            //boolean variable to hold the status
            bool valid;

            //check to make sure an ip address was provided
            if (string.IsNullOrEmpty(addr))
            {
                //no address provided so return false

                valid = false;
            }
            else
            {
                //address provided so use the IsMatch Method
                //of the Regular Expression object
                valid = check.IsMatch(addr, 0);
            }

            //return the results
            return valid;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return IpAddress;
            yield return Device;
            yield return Location;
        }
    }
}
