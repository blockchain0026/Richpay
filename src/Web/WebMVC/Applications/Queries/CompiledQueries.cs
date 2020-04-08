using Distributing.Domain.Model.Commissions;
using Distributing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Pairing.Domain.Model.QrCodes;
using Pairing.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Applications.Queries
{
    public static class CompiledQueries
    {
        public static Func<PairingContext, decimal, string, int, Task<QrCode>> _availableQrCodes =
            EF.CompileAsyncQuery((PairingContext db, decimal orderAmount, string shopId, int take) =>
                db.QrCodes
                    .Where(q =>
                        //q.PairingStatus.Id == PairingStatus.Pairing.Id
                        q._pairingStatusId == PairingStatus.Pairing.Id
                        //&& q.PaymentChannel.Name == order.PaymentChannel
                        //&& q.PaymentScheme.Name == order.PaymentScheme
                        && q.AvailableBalance >= orderAmount
                        && q.QuotaLeftToday >= orderAmount
                        //&& order.Amount >= q.Quota.OrderAmountLowerLimit
                        //&& order.Amount <= q.Quota.OrderAmountUpperLimit
                        && (q.SpecifiedShopId == null || q.SpecifiedShopId == shopId)
                        //&& q.MinCommissionRate <= order.RateRebate
                        //&& (q.BarcodeDataForManual == null || q.BarcodeDataForManual.Amount == order.Amount)
                        )
                        .OrderBy(q => q.DateLastTraded)
                        .Skip(take)
                        .Take(1)
                        //.Select(q => q.Id)
                        .FirstOrDefault()
                        );

        public static Func<DistributingContext, string, Task<CommissionInfo>> _commissionsInfoByUserId =
            EF.CompileAsyncQuery((DistributingContext db, string userId) =>
            db.Commissions
            .Where(c => c.UserId == userId)
            .Select(c => new CommissionInfo
            {
                Id = c.Id,

                Rate = c.RateRebateAlipay,

                IsEnabled = c.IsEnabled
            })
            .FirstOrDefault()
            );
    }
}
