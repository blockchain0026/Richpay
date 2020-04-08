using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Data;

namespace WebMVC.Applications.CacheServices
{
    public class UserCacheService : IUserCacheService
    {
        private readonly IServiceScopeFactory scopeFactory;

        //Key: UserId
        private ConcurrentDictionary<string, UserNameInfo> _userNameInfos;

        public UserCacheService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        public async Task UpdateNameInfos()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var nameInfos = await applicationDbContext.Users
                    .AsNoTracking()
                    .Select(u => new UserNameInfo
                    {
                        UserId = u.Id,
                        UserName = u.UserName,
                        FullName = u.FullName
                    })
                    .ToListAsync();

                var userNameInfos = new ConcurrentDictionary<string, UserNameInfo>();
                Parallel.ForEach(nameInfos, nameInfo =>
                {
                    var success = userNameInfos.TryAdd(nameInfo.UserId, nameInfo);
                    if (!success)
                    {
                        Console.WriteLine("Failed to add commission to dictionary.");
                    }
                });

                //Update and prevent any conflict on the list.
                Volatile.Write(ref _userNameInfos, userNameInfos);
            }
        }

        public UserNameInfo GetNameInfoByUserId(string userId)
        {
            return _userNameInfos.Where(u => u.Key == userId)
                .FirstOrDefault()
                .Value;
        }

    }

    public class UserNameInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
    }
}
