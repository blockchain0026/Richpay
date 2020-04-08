using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.Model.Shared
{
    public interface IDateTimeService
    {
        public DateTime GetCurrentDateTime();
    }
}
