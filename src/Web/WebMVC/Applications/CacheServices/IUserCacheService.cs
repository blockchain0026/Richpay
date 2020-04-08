using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Applications.CacheServices
{
    public interface IUserCacheService
    {
        Task UpdateNameInfos();
        UserNameInfo GetNameInfoByUserId(string userId);
    }
}
