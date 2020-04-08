using Distributing.Domain.Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Applications.DomainServices.DistributingDomain
{
    public class FakeDateTimeService : IDateTimeService
    {
        public FakeDateTimeService(DateTime currentDateTime)
        {
            CurrentDateTime = currentDateTime;
        }

        private DateTime CurrentDateTime { get; set; }


        public DateTime GetCurrentDateTime()
        {
            return CurrentDateTime;
        }
    }
}
