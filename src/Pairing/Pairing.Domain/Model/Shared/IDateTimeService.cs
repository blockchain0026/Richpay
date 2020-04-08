using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Domain.Model.Shared
{
    public interface IDateTimeService
    {
        public DateTime GetCurrentDateTime();
    }
}
