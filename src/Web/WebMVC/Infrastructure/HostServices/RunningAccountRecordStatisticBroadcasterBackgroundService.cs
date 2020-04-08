using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.RunningAccounts;
using WebMVC.Hubs;

namespace WebMVC.Infrastructure.HostServices
{
    public class RunningAccountRecordStatisticBroadcasterBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IHubContext<RunningAccountRecordStatisticHub> _runningAccountRecordStatisticHubContext;

        public RunningAccountRecordStatisticBroadcasterBackgroundService(IServiceScopeFactory scopeFactory, IHubContext<RunningAccountRecordStatisticHub> runningAccountRecordStatisticHubContext)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _runningAccountRecordStatisticHubContext = runningAccountRecordStatisticHubContext ?? throw new ArgumentNullException(nameof(runningAccountRecordStatisticHubContext));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // This eShopOnContainers method is querying a database table
                // and publishing events into the Event Bus (RabbitMQ / ServiceBus)
                await BroadcastStatistic();

                await Task.Delay(10000, stoppingToken);
            }
        }

        private async Task BroadcastStatistic()
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var runningAccountQueries = scope.ServiceProvider.GetRequiredService<IRunningAccountQueries>();

                    //var statistic = await runningAccountQueries.GetRunningAccountRecordsStatisticAsync(null, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

                    //await _orderStatisticHubContext.Clients.All.SendAsync("RefreshData", new object[] { orderStatistic });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Broadcast Order Errors: " + ex.Message);
            }
        }

    }
}
