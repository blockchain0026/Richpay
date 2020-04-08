using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Applications.Queries.IpWhitelists
{
    public interface IIpWhitelistQueries
    {
        List<string> GetIpWhitelistsByShopId(string shopId);
    }
}
