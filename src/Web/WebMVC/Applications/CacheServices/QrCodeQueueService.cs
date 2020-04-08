using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pairing.Domain.Model.QrCodes;
using Pairing.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.CacheServices
{
    public class QrCodeQueueService : IQrCodeQueueService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private List<QrCodeCache> _availableQrCodeIds = new List<QrCodeCache>();
        private List<Balance> _traderBalances = new List<Balance>();

        private object listLock = new object();

        public QrCodeQueueService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        public async Task UpdateQrCodeIds()
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var pairingContext = scope.ServiceProvider.GetRequiredService<PairingContext>();

                var availableQrCodeIds = await pairingContext.QrCodes
                    .AsNoTracking()
                    .Where(q => q._pairingStatusId == PairingStatus.Pairing.Id)
                    .OrderBy(q => q.DateLastTraded)
                    .Select(q => new QrCodeCache
                    {
                        QrCodeId = q.Id,
                        PaymentChannel = q.PaymentChannel.Name,
                        PaymentScheme = q.PaymentScheme.Name,
                        AvailableBalance = q.AvailableBalance,
                        QuotaLeftToday = q.QuotaLeftToday,
                        OrderAmountLowerLimit = q.OrderAmountLowerLimit,
                        OrderAmountUpperLimit = q.OrderAmountUpperLimit,
                        SpecifiedShopId = q.SpecifiedShopId
                    })
                    .ToListAsync();
                var test = _availableQrCodeIds
                    .Where(q => q.OrderAmountUpperLimit > 100
                    && _traderBalances.Where(b => b.UserId == q.PaymentChannel).FirstOrDefault().AmountAvailable > 100)
                    .FirstOrDefault();

                lock (listLock)
                {
                    //Update and prevent any conflict on the list.
                    //Volatile.Write(ref _availableQrCodeIds, availableQrCodeIds);
                    _availableQrCodeIds = availableQrCodeIds;
                }
            }
        }

        public int GetNextQrCodeId(string paymentChannel, string paymentScheme, decimal orderAmount, string specifiedShopId)
        {
            lock (listLock)
            {
                //var tmpList = new List<int>(Volatile.Read(ref _availableQrCodeIds));
                var tmpList = new List<QrCodeCache>(_availableQrCodeIds);
                var nextQrCode = tmpList.Where(q =>
                q.PaymentChannel == paymentChannel
                && q.PaymentScheme == paymentScheme
                && q.AvailableBalance >= orderAmount
                && q.QuotaLeftToday >= orderAmount
                && orderAmount >= q.OrderAmountLowerLimit
                && orderAmount <= q.OrderAmountUpperLimit
                && (q.SpecifiedShopId == null || q.SpecifiedShopId == specifiedShopId)
                ).FirstOrDefault();

                int nextId = default;

                if (nextQrCode != null)
                {
                    tmpList.Remove(nextQrCode);
                    nextId = nextQrCode.QrCodeId;
                }
                //Volatile.Write(ref _availableQrCodeIds, tmpList);
                _availableQrCodeIds = tmpList;
                return nextId;
            }
        }

        class QrCodeCache
        {
            public int QrCodeId { get; set; }
            public string PaymentChannel { get; set; }
            public string PaymentScheme { get; set; }
            public decimal AvailableBalance { get; set; }
            public decimal QuotaLeftToday { get; set; }
            public decimal OrderAmountLowerLimit { get; set; }
            public decimal OrderAmountUpperLimit { get; set; }
            public string SpecifiedShopId { get; set; }
        }
    }
}
