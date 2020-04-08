using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Domain.Model.Distributions
{
    public interface IDistributionService
    {
        public Task OrderCreated(Order order, RateType rateType, IDateTimeService dateTimeService);
        public Task<List<decimal>> DistributeFrom(Order order, IDateTimeService dateTimeService, int qrCodeId, RateType rateType);
        Task OrderCanceled(Order order, IDateTimeService dateTimeService,RateType rateType);
    }
}
