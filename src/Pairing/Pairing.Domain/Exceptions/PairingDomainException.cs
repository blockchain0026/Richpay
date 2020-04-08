using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Exceptions
{
    /// <summary>
    /// Exception type for domain exceptions
    /// </summary>
    public class PairingDomainException : Exception
    {
        public PairingDomainException()
        { }

        public PairingDomainException(string message)
            : base(message)
        { }

        public PairingDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
