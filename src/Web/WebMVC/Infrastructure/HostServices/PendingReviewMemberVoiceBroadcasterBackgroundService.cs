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
    public class PendingReviewMemberVoiceBroadcasterBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IHubContext<PendingReviewMemberVoiceHub> _pendingReviewMemberVoiceHubContext;
        private DateTime _dateLastChecked;

        public PendingReviewMemberVoiceBroadcasterBackgroundService(IServiceScopeFactory scopeFactory, IHubContext<PendingReviewMemberVoiceHub> pendingReviewMemberVoiceHubContext)
        {
            this.scopeFactory = scopeFactory;
            _pendingReviewMemberVoiceHubContext = pendingReviewMemberVoiceHubContext;
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


                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var confService = scope.ServiceProvider.GetRequiredService<ISystemConfigurationService>();

                    var conf = confService.GetSystemNotificationSoundAsync();

                    //If the notification is set to false, then don't execute boradcast process. 
                    if (!conf.Member)
                    {
                        return;
                    }

                    var newMembers = await context.Users
                         .AsNoTracking()
                         .Select(u => new
                         {
                             u.BaseRoleType,
                             u.DateCreated,
                             u.Id,
                             u.UserName,
                             u.FullName,
                             u.IsReviewed
                         })
                         .Where(u => u.DateCreated >= dateToCheck
                         && !u.IsReviewed
                         && u.BaseRoleType != Models.Roles.BaseRoleType.Manager)
                         .ToListAsync();

                    if (newMembers.Any())
                    {
                        await _pendingReviewMemberVoiceHubContext.Clients.All.SendAsync("BroadcastVoice", new object[] { newMembers });
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
