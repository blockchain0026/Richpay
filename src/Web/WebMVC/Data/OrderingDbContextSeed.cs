using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Model.Orders;
using Ordering.Domain.SeedWork;
using Ordering.Infrastructure;
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
    public class OrderingDbContextSeed
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder, IWebHostEnvironment env)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<OrderingContext>())
                {
                    context.Database.Migrate();

                    if (!context.OrderTypes.Any())
                    {
                        context.OrderTypes.AddRange(GetPredefinedOrderTypes());

                        await context.SaveChangesAsync();
                    }

                    if (!context.OrderStatus.Any())
                    {
                        context.OrderStatus.AddRange(GetPredefinedOrderStatus());
                    }

                    if (!context.OrderPaymentChannels.Any())
                    {
                        context.OrderPaymentChannels.AddRange(GetPredefinedOrderPaymentChannels());
                    }

                    if (!context.OrderPaymentSchemes.Any())
                    {
                        context.OrderPaymentSchemes.AddRange(GetPredefinedOrderPaymentSchemes());
                    }

                    await context.SaveChangesAsync();
                }

            }

        }




        #region Predefined

        private static IEnumerable<OrderType> GetPredefinedOrderTypes()
        {
            return Enumeration.GetAll<OrderType>();
        }
        private static IEnumerable<OrderStatus> GetPredefinedOrderStatus()
        {
            return Enumeration.GetAll<OrderStatus>();
        }
        private static IEnumerable<OrderPaymentChannel> GetPredefinedOrderPaymentChannels()
        {
            return Enumeration.GetAll<OrderPaymentChannel>();
        }
        private static IEnumerable<OrderPaymentScheme> GetPredefinedOrderPaymentSchemes()
        {
            return Enumeration.GetAll<OrderPaymentScheme>();
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
