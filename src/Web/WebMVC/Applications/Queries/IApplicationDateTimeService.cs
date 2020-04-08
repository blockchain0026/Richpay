using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Applications.Queries
{
    public interface IApplicationDateTimeService
    {
        public DateTime GetCurrentDateTime();
        public DateTime GetDayStartUTCTime();
    }
}
