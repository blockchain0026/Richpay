using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pairing.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Hubs;
using WebMVC.Infrastructure.Services;

namespace WebMVC.Infrastructure.HostServices
{
    public class PendingReviewQrCodeVoiceBroadcasterBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IHubContext<PendingReviewQrCodeVoiceHub> _pendingReviewQrCodeVoiceHubContext;
        private DateTime _dateLastChecked;

        public PendingReviewQrCodeVoiceBroadcasterBackgroundService(IServiceScopeFactory scopeFactory, IHubContext<PendingReviewQrCodeVoiceHub> pendingReviewQrCodeVoiceHubContext)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _pendingReviewQrCodeVoiceHubContext = pendingReviewQrCodeVoiceHubContext ?? throw new ArgumentNullException(nameof(pendingReviewQrCodeVoiceHubContext));
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


                    var context = scope.ServiceProvider.GetRequiredService<PairingContext>();
                    var confService = scope.ServiceProvider.GetRequiredService<ISystemConfigurationService>();

                    var conf = confService.GetSystemNotificationSoundAsync();

                    //If the notification is set to false, then don't execute boradcast process. 
                    if (!conf.QrCode)
                    {
                        return;
                    }

                    var newQrCodes = await context.QrCodes
                         .AsNoTracking()
                         .Select(u => new
                         {
                             u.DateCreated,
                             u.Id,
                             u.FullName,
                             u.PaymentChannel,
                             u.PaymentScheme,
                             u.IsApproved
                         })
                         .Where(u => u.DateCreated >= dateToCheck && !u.IsApproved)
                         .ToListAsync();

                    if (newQrCodes.Any())
                    {
                        await _pendingReviewQrCodeVoiceHubContext.Clients.All.SendAsync("BroadcastVoice", new object[] { newQrCodes });
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
