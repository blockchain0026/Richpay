using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Permissions;

namespace WebMVC.Hubs
{
    [Authorize(Policy = Permissions.Hubs.WithdrawalVoice)]
    public class WithdrawalVoiceHub : Hub
    {
    }
}
