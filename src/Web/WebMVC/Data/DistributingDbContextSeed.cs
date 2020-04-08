using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Deposits;
using Distributing.Domain.Model.Distributions;
using Distributing.Domain.Model.Frozens;
using Distributing.Domain.Model.Transfers;
using Distributing.Domain.Model.Withdrawals;
using Distributing.Domain.SeedWork;
using Distributing.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebMVC.Data
{
    public class DistributingDbContextSeed
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder, IWebHostEnvironment env)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DistributingContext>())
                {
                    context.Database.Migrate();

                    if (!context.UserTypes.Any())
                    {
                        context.UserTypes.AddRange(GetPredefinedUserTypes());

                        await context.SaveChangesAsync();
                    }

                    if (!context.DistributionTypes.Any())
                    {
                        context.DistributionTypes.AddRange(GetPredefinedDistributionTypes());
                    }

                    if (!context.DepositStatus.Any())
                    {
                        context.DepositStatus.AddRange(GetPredefinedDepositStatus());
                    }

                    if (!context.WithdrawalStatus.Any())
                    {
                        context.WithdrawalStatus.AddRange(GetPredefinedWithdrawalStatus());
                    }
                    
                    if (!context.DepositTypes.Any())
                    {
                        context.DepositTypes.AddRange(GetPredefinedDepositTypes());
                    }
                    
                    if (!context.WithdrawalTypes.Any())
                    {
                        context.WithdrawalTypes.AddRange(GetPredefinedWithdrawalTypes());
                    }
                    
                    if (!context.FrozenTypes.Any())
                    {
                        context.FrozenTypes.AddRange(GetPredefinedFrozenTypes());
                    }
                    
                    if (!context.InitiatedBys.Any())
                    {
                        context.InitiatedBys.AddRange(GetPredefinedInitiatedBys());
                    }
                    
                    if (!context.FrozenStatus.Any())
                    {
                        context.FrozenStatus.AddRange(GetPredefinedFrozenStatus());
                    }
                    
                    await context.SaveChangesAsync();
                }

            }

        }




        #region Predefined

        private static IEnumerable<UserType> GetPredefinedUserTypes()
        {
            return new List<UserType>()
            {
                UserType.Trader,
                UserType.TraderAgent,
                UserType.Shop,
                UserType.ShopAgent
            };
        }
        private static IEnumerable<DistributionType> GetPredefinedDistributionTypes()
        {
            return Enumeration.GetAll<DistributionType>();
        }
        private static IEnumerable<DepositStatus> GetPredefinedDepositStatus()
        {
            return Enumeration.GetAll<DepositStatus>();
        }
        private static IEnumerable<WithdrawalStatus> GetPredefinedWithdrawalStatus()
        {
            return Enumeration.GetAll<WithdrawalStatus>();
        }        
        
        private static IEnumerable<DepositType> GetPredefinedDepositTypes()
        {
            return Enumeration.GetAll<DepositType>();
        }
        private static IEnumerable<WithdrawalType> GetPredefinedWithdrawalTypes()
        {
            return Enumeration.GetAll<WithdrawalType>();
        }
        private static IEnumerable<FrozenType> GetPredefinedFrozenTypes()
        {
            return Enumeration.GetAll<FrozenType>();
        }
        private static IEnumerable<InitiatedBy> GetPredefinedInitiatedBys()
        {
            return Enumeration.GetAll<InitiatedBy>();
        }
        private static IEnumerable<FrozenStatus> GetPredefinedFrozenStatus()
        {
            return Enumeration.GetAll<FrozenStatus>();
        }

        #endregion


        private static string[] GetHeaders(string[] requiredHeaders, string csvfile)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

            if (csvheaders.Count() != requiredHeaders.Count())
            {
                throw new Exception($"requiredHeader count '{ requiredHeaders.Count()}' is different then read header '{csvheaders.Count()}'");
            }

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }

        public static string GetCSV(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();

            return results;
        }



        private AsyncRetryPolicy CreatePolicy(ILogger<ApplicationDbContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogTrace($"[{prefix}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
                    }
                );
        }
    }
}
