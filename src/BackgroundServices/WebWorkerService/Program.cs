using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Distributing.Domain.Model.Balances;
using Distributing.Infrastructure;
using Distributing.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ordering.Domain.Model.ShopApis;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Repositories;
using Pairing.Domain.Model.QrCodes;
using Pairing.Infrastructure;
using Pairing.Infrastructure.Repositories;
using WebMVC.Data;
using WebWorkerService.ApiClients;
using WebWorkerService.BackgroundServices;
using WebWorkerService.CacheServices;

namespace WebWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((host, builder) =>
                {
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    builder.AddJsonFile("appsettings.json", optional: true);
                    //builder.AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: true);
                    builder.AddEnvironmentVariables();
                    builder.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    services.AddEntityFrameworkSqlServer()
                           .AddDbContext<DistributingContext>(options =>
                           {
                               options.UseSqlServer(configuration.GetConnectionString("DistributingConnection"),
                                   sqlServerOptionsAction: sqlOptions =>
                                   {
                                       sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                       //sqlOptions.CommandTimeout(1000 * 60);
                                   });
                           },
                               ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                           );

                    services.AddEntityFrameworkSqlServer()
                           .AddDbContext<PairingContext>(options =>
                           {
                               options.UseSqlServer(configuration.GetConnectionString("PairingConnection"),
                                   sqlServerOptionsAction: sqlOptions =>
                                   {
                                       sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                       //sqlOptions.CommandTimeout(1000 * 60);
                                   });
                               //options.EnableSensitiveDataLogging();
                           },
                               ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                           );

                    services.AddEntityFrameworkSqlServer()
                           .AddDbContext<OrderingContext>(options =>
                           {
                               options.UseSqlServer(configuration.GetConnectionString("OrderingConnection"),
                                   sqlServerOptionsAction: sqlOptions =>
                                   {
                                       sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                       //sqlOptions.CommandTimeout(1000 * 60);
                                   });
                           },
                               ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                           );

                    services.AddEntityFrameworkSqlServer()
                                  .AddDbContext<ApplicationDbContext>(options =>
                                  {
                                      options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                                          sqlServerOptionsAction: sqlOptions =>
                                          {
                                              sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                              sqlOptions.CommandTimeout(60);
                                          });
                                  },
                                  ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                                  );

                    services.AddScoped<IBalanceRepository, BalanceRepository>();
                    services.AddScoped<IQrCodeRepository, QrCodeRepository>();

                    services.AddTransient<IReturnOrderApiClient, ReturnOrderApiClient>();
                    services.AddTransient<IShopApiRepository, ShopApiRepository>();

                    services.AddSingleton<ICommissionCacheService, CommissionCacheService>();
                    services.AddSingleton<IUserCacheService, UserCacheService>();

                    services.AddHostedService<ResolveCompletedDistributionBackgroundService>();
                    services.AddHostedService<ResolveCompletedOrderBackgroundService>();
                    services.AddHostedService<MarkOrderAsExpiredBackgroundService>();
                    services.AddHostedService<UpdateCommissionCacheBackgroundService>();
                    services.AddHostedService<UpdateUserCacheBackgroundService>();
                });
    }
}
