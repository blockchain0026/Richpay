using Distributing.Domain.Exceptions;
using Distributing.Domain.Model.Balances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Infrastructure.Services;

namespace WebMVC.Applications.DomainServices.DistributingDomain
{
    public class OpenTimeService : IOpenTimeService
    {
        private readonly ISystemConfigurationService _systemConfigurationService;

        public OpenTimeService(ISystemConfigurationService systemConfigurationService)
        {
            _systemConfigurationService = systemConfigurationService ?? throw new ArgumentNullException(nameof(systemConfigurationService));
        }

        public bool IsDepositOpenNow(DateTime currentDateTime)
        {
            try
            {
                var conf = _systemConfigurationService.GetWithdrawalAndDeposit();
                
                var openFrom = TimeSpan.Parse(conf.DepositOpenFrom);
                var openTo = TimeSpan.Parse(conf.DepositOpenTo);

                //Ex:
                //   OpenFrom: 10:30, OpenTo: 22:30
                // ->From Today to Today.
                if (openFrom < openTo)
                {

                    if (currentDateTime.Hour >= openFrom.Hours && currentDateTime.Hour <= openTo.Hours)
                    {
                        //Ex:
                        //   Current: 10:01, OpenFrom: 10:30
                        // -> 01 < 30 
                        // -> return false
                        if (currentDateTime.Hour == openFrom.Hours)
                        {
                            if (currentDateTime.Minute < openFrom.Minutes)
                            {
                                return false;
                            }
                        }

                        //Ex:
                        //   Current: 22:59, OpenTo: 22:30
                        // -> 59 > 30 
                        // -> return false
                        if (currentDateTime.Hour == openTo.Hours)
                        {
                            if (currentDateTime.Minute > openTo.Minutes)
                            {
                                return false;
                            }
                        }

                        return true;
                    }

                    return false;
                }

                //Ex:
                //   OpenFrom: 10:30, OpenTo: 00:30
                // ->From Today to Next Day.
                if (openFrom > openTo)
                {
                    //Ex:
                    //   Current: 10:01, OpenFrom: 10:30, OpenTo: 00:30
                    // -> 10 <= 10 && 10 >= 0
                    // -> Probably out of the business hours.
                    if (currentDateTime.Hour <= openFrom.Hours && currentDateTime.Hour >= openTo.Hours)
                    {
                        //Ex:
                        //   Current: 10:59, OpenFrom: 10:30
                        // -> 59 > 30 
                        // -> return true
                        if (currentDateTime.Hour == openFrom.Hours)
                        {
                            if (currentDateTime.Minute > openFrom.Minutes)
                            {
                                return true;
                            }
                        }

                        //Ex:
                        //   Current: 00:16, OpenTo: 00:30
                        // -> 16 <= 30
                        // -> return true
                        if (currentDateTime.Hour == openTo.Hours)
                        {
                            if (currentDateTime.Minute <= openTo.Minutes)
                            {
                                return true;
                            }
                        }

                        return false;
                    }

                    return true;
                }

                //Ex:
                //   OpenFrom: 00:00, OpenTo: 00:00
                // ->24 Hours.
                if (openFrom == openTo)
                {
                    return true;
                }

                throw new ArgumentOutOfRangeException("Unclear problems.");
            }
            catch (Exception ex)
            {
                throw new DistributingDomainException("Failed to check deposit open hours.", ex);
            }
        }

        public bool IsWithdrawalOpenNow(DateTime currentDateTime)
        {
            try
            {
                var conf = _systemConfigurationService.GetWithdrawalAndDeposit();

                var openFrom = TimeSpan.Parse(conf.WithdrawalOpenFrom);
                var openTo = TimeSpan.Parse(conf.WithdrawalOpenTo);

                //Ex:
                //   OpenFrom: 10:30, OpenTo: 22:30
                // ->From Today to Today.
                if (openFrom < openTo)
                {

                    if (currentDateTime.Hour >= openFrom.Hours && currentDateTime.Hour <= openTo.Hours)
                    {
                        //Ex:
                        //   Current: 10:01, OpenFrom: 10:30
                        // -> 01 < 30 
                        // -> return false
                        if (currentDateTime.Hour == openFrom.Hours)
                        {
                            if (currentDateTime.Minute < openFrom.Minutes)
                            {
                                return false;
                            }
                        }

                        //Ex:
                        //   Current: 22:59, OpenTo: 22:30
                        // -> 59 > 30 
                        // -> return false
                        if (currentDateTime.Hour == openTo.Hours)
                        {
                            if (currentDateTime.Minute > openTo.Minutes)
                            {
                                return false;
                            }
                        }

                        return true;
                    }

                    return false;
                }

                //Ex:
                //   OpenFrom: 10:30, OpenTo: 00:30
                // ->From Today to Next Day.
                if (openFrom > openTo)
                {
                    //Ex:
                    //   Current: 10:01, OpenFrom: 10:30, OpenTo: 00:30
                    // -> 10 <= 10 && 10 >= 0
                    // -> Probably out of the business hours.
                    if (currentDateTime.Hour <= openFrom.Hours && currentDateTime.Hour >= openTo.Hours)
                    {
                        //Ex:
                        //   Current: 10:59, OpenFrom: 10:30
                        // -> 59 > 30 
                        // -> return true
                        if (currentDateTime.Hour == openFrom.Hours)
                        {
                            if (currentDateTime.Minute > openFrom.Minutes)
                            {
                                return true;
                            }
                        }

                        //Ex:
                        //   Current: 00:16, OpenTo: 00:30
                        // -> 16 <= 30
                        // -> return true
                        if (currentDateTime.Hour == openTo.Hours)
                        {
                            if (currentDateTime.Minute <= openTo.Minutes)
                            {
                                return true;
                            }
                        }

                        return false;
                    }

                    return true;
                }

                //Ex:
                //   OpenFrom: 00:00, OpenTo: 00:00
                // ->24 Hours.
                if (openFrom == openTo)
                {
                    return true;
                }

                throw new ArgumentOutOfRangeException("Unclear problems.");
            }
            catch (Exception ex)
            {
                throw new DistributingDomainException("Failed to check deposit open hours.", ex);
            }
        }
    }
}
