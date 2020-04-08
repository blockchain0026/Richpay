using Ordering.Domain.Exceptions;
using Ordering.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Ordering.Domain.Model.ShopApis
{
    public class IpWhitelist
    : Entity
    {
        // DDD Patterns comment
        // Using private fields, allowed since EF Core 1.1, is a much better encapsulation
        // aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)
        private string _address;

        public IpWhitelist(string address)
        {
            this._address = IsValidIP(address) ? address : throw new OrderingDomainException("无效的IP");
        }

        protected IpWhitelist() { }



        public string GetAddress() => _address;

        private bool IsValidIP(string addr)
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
    }
}
