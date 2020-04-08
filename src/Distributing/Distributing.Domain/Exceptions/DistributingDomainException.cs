using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Exceptions
{
    /// <summary>
    /// Exception type for domain exceptions
    /// </summary>
    public class DistributingDomainException : Exception
    {
        public DistributingDomainException()
        { }

        public DistributingDomainException(string message)
            : base(message)
        { }

        public DistributingDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
