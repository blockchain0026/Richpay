﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.CacheServices;

namespace WebMVC.Infrastructure.HostServices
{
    public class UpdateUserCacheBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public UpdateUserCacheBackgroundService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // This eShopOnContainers method is querying a database table
                // and publishing events into the Event Bus (RabbitMQ / ServiceBus)
                await UpdateUserNameInfosCache();

                await Task.Delay(60000, stoppingToken);
            }
        }

        private async Task UpdateUserNameInfosCache()
        {
            Console.WriteLine("Updating User Name Infos Cache...");
            using (var scope = scopeFactory.CreateScope())
            {
                var userCacheService = scope.ServiceProvider.GetRequiredService<IUserCacheService>();
                await userCacheService.UpdateNameInfos();
            }
        }
    }
}