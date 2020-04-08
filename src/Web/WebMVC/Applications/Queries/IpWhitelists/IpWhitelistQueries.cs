using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Model.ShopApis;
using Ordering.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.IpWhitelists
{
    public class IpWhitelistQueries : IIpWhitelistQueries
    {
        private readonly OrderingContext _context;

        public IpWhitelistQueries(OrderingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public List<string> GetIpWhitelistsByShopId(string shopId)
        {
            //var result = new List<IpWhitelistEntry>();
            var result = new List<string>();

            var shopApi = _context.ShopApis
                .AsNoTracking()
                .Include(s => s.IpWhitelists)
                .Where(a => a.ShopId == shopId)
                .FirstOrDefault();

            foreach (var ipWhitelist in shopApi.IpWhitelists)
            {
                /*result.Add(
                    MapIpWhitelistEntryFromEntity(ipWhitelist));*/

                result.Add(ipWhitelist.GetAddress());
            }

            return result;
        }

        private IpWhitelistEntry MapIpWhitelistEntryFromEntity(IpWhitelist entity)
        {
            var ipWhitelistEntry = new IpWhitelistEntry
            {
                IpWhitelistId = entity.Id,
                Address = entity.GetAddress()
            };

            return ipWhitelistEntry;
        }
    }
}
