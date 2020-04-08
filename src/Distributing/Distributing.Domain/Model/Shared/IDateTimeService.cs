using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Domain.Model.Shared
{
    public interface IDateTimeService
    {
        public DateTime GetCurrentDateTime();
    }
}
