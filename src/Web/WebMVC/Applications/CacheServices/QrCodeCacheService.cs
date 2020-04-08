using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pairing.Domain.Model.QrCodes;
using Pairing.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Data;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.CacheServices
{
    public class QrCodeCacheService : IQrCodeCacheService
    {
        private readonly IServiceScopeFactory scopeFactory;

        //Key: QrCodeId
        private ConcurrentDictionary<int, PairingInfo> _pairingInfos;

        public QrCodeCacheService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        public async Task UpdatePairingInfo()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var qrCodeEntries = await appContext.QrCodeEntrys
                    .AsNoTracking()
                    .Select(c => new
                    {
                        c.QrCodeId,
                        c.PairingInfo
                    })
                    .ToListAsync();

                var pairingInfos = new ConcurrentDictionary<int, PairingInfo>();

                Parallel.ForEach(qrCodeEntries, qrCodeEntry =>
                {
                    pairingInfos.TryAdd(qrCodeEntry.QrCodeId, qrCodeEntry.PairingInfo);
                });

                //Update and prevent any conflict on the list.
                Volatile.Write(ref _pairingInfos, pairingInfos);
            }
        }

        public PairingInfo GetPairingInfoByQrCodeId(int qrCodeId)
        {
            return _pairingInfos.Where(q => q.Key == qrCodeId).FirstOrDefault().Value;
        }
    }
}
