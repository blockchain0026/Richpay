using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebWorkerService.CacheServices
{
    public interface IUserCacheService
    {
        Task UpdateNameInfos();
        UserNameInfo GetNameInfoByUserId(string userId);
    }
}
