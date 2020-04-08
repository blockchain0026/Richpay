using Distributing.Domain.Exceptions;
using Distributing.Domain.Model.Commissions;
using Distributing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebMVC.Applications.CacheServices
{
    public class CommissionCacheService : ICommissionCacheService
    {
        private readonly IServiceScopeFactory scopeFactory;

        //Key: CommissionId
        private ConcurrentDictionary<int, CommissionInfo> _alipayCommissions;
        private ConcurrentDictionary<int, CommissionInfo> _wechatCommissions;

        public CommissionCacheService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        public async Task UpdateCommissions()
        {
            try
            {

                using (var scope = scopeFactory.CreateScope())
                {
                    var distributingContext = scope.ServiceProvider.GetRequiredService<DistributingContext>();

                    var commissions = await distributingContext.Commissions
                        .AsNoTracking()
                        .Select(c => new
                        {
                            c.Id,
                            c.BalanceId,

                            c.UserId,
                            c.UplineCommissionId,
                            c.UserType,
                            c.RateAlipay,
                            c.RateWechat,
                            c.RateRebateAlipay,
                            c.RateRebateWechat,

                            c.IsEnabled
                        })
                        .ToListAsync();
                    var alipayCommissions = new ConcurrentDictionary<int, CommissionInfo>();
                    var wechatCommissions = new ConcurrentDictionary<int, CommissionInfo>();
                    Parallel.ForEach(commissions, commission =>
                    {
                        if (commission.UserType.Id == UserType.Trader.Id
                            || commission.UserType.Id == UserType.TraderAgent.Id)
                        {
                            var success = alipayCommissions.TryAdd(commission.Id, new CommissionInfo
                            {
                                Id = commission.Id,
                                BalanceId = commission.BalanceId,
                                Rate = commission.RateAlipay,
                                UplineCommissionId = commission.UplineCommissionId,
                                UserId = commission.UserId,
                                IsEnabled = commission.IsEnabled,
                                ChainNumber = default
                            });
                            if (!success)
                            {
                                Console.WriteLine("Failed to add commission to dictionary.");
                            }

                            success = wechatCommissions.TryAdd(commission.Id, new CommissionInfo
                            {
                                Id = commission.Id,
                                BalanceId = commission.BalanceId,
                                Rate = commission.RateWechat,
                                UplineCommissionId = commission.UplineCommissionId,
                                UserId = commission.UserId,
                                IsEnabled = commission.IsEnabled,
                                ChainNumber = default
                            });
                            if (!success)
                            {
                                Console.WriteLine("Failed to add commission to dictionary.");
                            }
                        }
                        else if (commission.UserType.Id == UserType.Shop.Id
                            || commission.UserType.Id == UserType.ShopAgent.Id)
                        {
                            var success = alipayCommissions.TryAdd(commission.Id, new CommissionInfo
                            {
                                Id = commission.Id,
                                BalanceId = commission.BalanceId,
                                Rate = commission.RateRebateAlipay,
                                UplineCommissionId = commission.UplineCommissionId,
                                UserId = commission.UserId,
                                IsEnabled = commission.IsEnabled,
                                ChainNumber = default
                            });
                            if (!success)
                            {
                                Console.WriteLine("Failed to add commission to dictionary.");
                            }

                            success = wechatCommissions.TryAdd(commission.Id, new CommissionInfo
                            {
                                Id = commission.Id,
                                BalanceId = commission.BalanceId,
                                Rate = commission.RateRebateWechat,
                                UplineCommissionId = commission.UplineCommissionId,
                                UserId = commission.UserId,
                                IsEnabled = commission.IsEnabled,
                                ChainNumber = default
                            });
                            if (!success)
                            {
                                Console.WriteLine("Failed to add commission to dictionary.");
                            }
                        }
                        else
                        {
                            throw new Exception("Unknown user type found at commission" + ". At DistributionService.GetRate()");
                        }
                    });

                    //Update and prevent any conflict on the list.
                    Volatile.Write(ref _alipayCommissions, alipayCommissions);
                    Volatile.Write(ref _wechatCommissions, wechatCommissions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Catch Exceptions While Update Commission Cache.");
                Console.WriteLine(ex.Message);
            }
        }

        public CommissionInfo GetCommissionInfoByUserId(string userId, RateType rateType)
        {
            if (rateType == RateType.Alipay)
            {
                return _alipayCommissions.Where(c => c.Value.UserId == userId).FirstOrDefault().Value;
            }
            else if (rateType == RateType.Wechat)
            {
                return _wechatCommissions.Where(c => c.Value.UserId == userId).FirstOrDefault().Value;
            }
            else
            {
                throw new Exception("Unknown rate type" + ". At CommissionMemoryService.GetCommissionInfoByUserId()");
            }
        }
        public CommissionInfo GetCommissionInfoByCommissionId(int commissionId, RateType rateType)
        {
            if (rateType == RateType.Alipay)
            {
                return _alipayCommissions.Where(c => c.Key == commissionId).FirstOrDefault().Value;
            }
            else if (rateType == RateType.Wechat)
            {
                return _wechatCommissions.Where(c => c.Key == commissionId).FirstOrDefault().Value;
            }
            else
            {
                throw new Exception("Unknown rate type" + ". At CommissionMemoryService.GetCommissionInfoByUserId()");
            }
        }
        public CommissionInfo GetToppestCommissionInfoByCommission(CommissionInfo commission, RateType rateType)
        {
            var result = commission;

            var uplineCommission = result;
            while (true)
            {
                if (uplineCommission == null)
                {
                    break;
                }


                var currentCommission = uplineCommission;

                result = currentCommission;

                uplineCommission = GetCommissionInfoByCommissionId(
                    currentCommission.UplineCommissionId ?? default, rateType);
            }

            return result;
        }
    }
}
