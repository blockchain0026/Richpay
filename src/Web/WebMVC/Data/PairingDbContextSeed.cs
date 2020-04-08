using Pairing.Domain.SeedWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pairing.Domain.Model.QrCodes;
using Pairing.Infrastructure;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Pairing.Domain.Model.ShopGateways;
using Pairing.Domain.Model.CloudDevices;

namespace WebMVC.Data
{
    public class PairingDbContextSeed
    {
        public static async Task SeedAsync(IApplicationBuilder applicationBuilder, IWebHostEnvironment env)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<PairingContext>())
                {
                    context.Database.Migrate();

                    if (!context.QrCodeTypes.Any())
                    {
                        context.QrCodeTypes.AddRange(GetPredefinedQrCodeTypes());

                        await context.SaveChangesAsync();
                    }

                    if (!context.PaymentChannels.Any())
                    {
                        context.PaymentChannels.AddRange(GetPredefinedPaymentChannels());
                    }

                    if (!context.PaymentSchemes.Any())
                    {
                        context.PaymentSchemes.AddRange(GetPredefinedPaymentSchemes());
                    }

                    if (!context.QrCodeStatus.Any())
                    {
                        context.QrCodeStatus.AddRange(GetPredefinedQrCodeStatus());
                    }

                    if (!context.PairingStatus.Any())
                    {
                        context.PairingStatus.AddRange(GetPredefinedPairingStatus());
                    }

                    if (!context.ShopGatewayTypes.Any())
                    {
                        context.ShopGatewayTypes.AddRange(GetPredefinedShopGatewayTypes());
                    }

                    if (!context.CloudDeviceStatus.Any())
                    {
                        context.CloudDeviceStatus.AddRange(GetPredefinedCloudDeviceStatus());
                    }


                    await context.SaveChangesAsync();
                }

            }

        }




        #region Predefined

        private static IEnumerable<QrCodeType> GetPredefinedQrCodeTypes()
        {
            return Enumeration.GetAll<QrCodeType>();

        }
        private static IEnumerable<Pairing.Domain.Model.QrCodes.PaymentChannel> GetPredefinedPaymentChannels()
        {
            return Enumeration.GetAll<Pairing.Domain.Model.QrCodes.PaymentChannel>();
        }
        private static IEnumerable<PaymentScheme> GetPredefinedPaymentSchemes()
        {
            return Enumeration.GetAll<PaymentScheme>();
        }
        private static IEnumerable<QrCodeStatus> GetPredefinedQrCodeStatus()
        {
            return Enumeration.GetAll<QrCodeStatus>();
        }

        private static IEnumerable<PairingStatus> GetPredefinedPairingStatus()
        {
            return Enumeration.GetAll<PairingStatus>();
        }
        private static IEnumerable<ShopGatewayType> GetPredefinedShopGatewayTypes()
        {
            return Enumeration.GetAll<ShopGatewayType>();
        }
        private static IEnumerable<CloudDeviceStatus> GetPredefinedCloudDeviceStatus()
        {
            return Enumeration.GetAll<CloudDeviceStatus>();
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
