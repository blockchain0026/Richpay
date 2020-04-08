using Distributing.Domain.Model.Deposits;
using Distributing.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Hubs;
using WebMVC.Infrastructure.Services;

namespace WebMVC.Infrastructure.HostServices
{
    public class DepositVoiceBroadcasterBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IHubContext<DepositVoiceHub> _depositVoiceHubContext;
        private DateTime _dateLastChecked;

        public DepositVoiceBroadcasterBackgroundService(IServiceScopeFactory scopeFactory, IHubContext<DepositVoiceHub> depositVoiceHubContext)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _depositVoiceHubContext = depositVoiceHubContext ?? throw new ArgumentNullException(nameof(depositVoiceHubContext));
            _dateLastChecked = DateTime.UtcNow;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await BroadcastVoice();

                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task BroadcastVoice()
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    //Set date to check after the value.
                    var dateToCheck = this._dateLastChecked;

                    //Update checked date.
                    this._dateLastChecked = DateTime.UtcNow;


                    var context = scope.ServiceProvider.GetRequiredService<DistributingContext>();
                    var confService = scope.ServiceProvider.GetRequiredService<ISystemConfigurationService>();

                    var conf = confService.GetSystemNotificationSoundAsync();

                    //If the notification is set to false, then don't execute boradcast process. 
                    if (!conf.Deposit)
                    {
                        return;
                    }

                    var newDeposits = await context.Deposits
                         .AsNoTracking()
                         .Select(w => new
                         {
                             w.DepositType,
                             w.DateCreated,
                             w.Id,
                             w.TotalAmount,
                             w.BankAccount,
                             w.Description
                         })
                         .Where(w => w.DateCreated >= dateToCheck && w.DepositType.Id != DepositType.ByAdmin.Id)
                         .ToListAsync();

                    if (newDeposits.Any())
                    {
                        await _depositVoiceHubContext.Clients.All.SendAsync("BroadcastVoice", new object[] { newDeposits });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Broadcast Order Errors: " + ex.Message);
            }
        }

    }
}
