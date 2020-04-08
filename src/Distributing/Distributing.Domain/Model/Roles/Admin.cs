using Distributing.Domain.Exceptions;
using Distributing.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Model.Roles
{
    public class Admin : ValueObject
    {
        public Admin(string adminId, string name)
        {
            AdminId = !string.IsNullOrEmpty(adminId) ? adminId :
                throw new DistributingDomainException("Invalid Param: " + nameof(adminId) + ". At Admin()");
            Name = !string.IsNullOrEmpty(name) ? name :
            throw new DistributingDomainException("Invalid Param: " + nameof(name) + ". At Admin()");
        }

        public string AdminId { get; private set; }
        public string Name { get; private set; }


        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return AdminId;
            yield return Name;
        }
    }

}
