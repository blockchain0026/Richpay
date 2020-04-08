using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Applications.Queries
{
    public class ApplicationDateTimeService : IApplicationDateTimeService
    {
        public DateTime GetCurrentDateTime()
        {
            return DateTime.UtcNow.AddHours(-8);
        }
        public DateTime GetDayStartUTCTime()
        {
            var utcDateTime = DateTime.UtcNow;
            var date = new DateTime(
                utcDateTime.Year,
                utcDateTime.Month
                , utcDateTime.Day);
            
            return date.AddHours(-8);
        }
    }
}
