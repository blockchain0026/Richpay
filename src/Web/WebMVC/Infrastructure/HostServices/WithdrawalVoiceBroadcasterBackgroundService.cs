using Distributing.Domain.Model.Withdrawals;
using Distributing.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Data;
using WebMVC.Hubs;
using WebMVC.Infrastructure.Services;

namespace WebMVC.Infrastructure.HostServices
{
    public class WithdrawalVoiceBroadcasterBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IHubContext<WithdrawalVoiceHub> _withdrawalVoiceHubContext;
        private DateTime _dateLastChecked;

        public WithdrawalVoiceBroadcasterBackgroundService(IServiceScopeFactory scopeFactory, IHubContext<WithdrawalVoiceHub> withdrawalVoiceHubContext)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _withdrawalVoiceHubContext = withdrawalVoiceHubContext ?? throw new ArgumentNullException(nameof(withdrawalVoiceHubContext));
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
                    if (!conf.Withdraw)
                    {
                        return;
                    }

                    var newWithdrawals = await context.Withdrawals
                         .AsNoTracking()
                         .Select(w => new
                         {
                             w.WithdrawalType,
                             w.DateCreated,
                             w.Id,
                             w.TotalAmount,
                             w.BankAccount,
                             w.Description
                         })
                         .Where(w => w.DateCreated >= dateToCheck && w.WithdrawalType.Id != WithdrawalType.ByAdmin.Id)
                         .ToListAsync();

                    if (newWithdrawals.Any())
                    {
                        await _withdrawalVoiceHubContext.Clients.All.SendAsync("BroadcastVoice", new object[] { newWithdrawals });
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
