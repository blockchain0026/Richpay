using Distributing.Domain.Exceptions;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Distributions;
using Distributing.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Distributing.Infrastructure.Repositories
{
    public class CommissionRepository
        : ICommissionRepository
    {
        private readonly DistributingContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public CommissionRepository(DistributingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Commission Add(Commission commission)
        {
            return _context.Commissions.Add(commission).Entity;

        }

        public void Update(Commission commission)
        {
            _context.Entry(commission).State = EntityState.Modified;
        }

        public async Task<Commission> GetByCommissionIdAsync(int commissionId)
        {
            var commission = await _context
                                .Commissions
                                .FirstOrDefaultAsync(b => b.Id == commissionId);
            if (commission == null)
            {
                commission = _context
                            .Commissions
                            .Local
                            .FirstOrDefault(b => b.Id == commissionId);
            }
            if (commission != null)
            {
                await _context.Entry(commission)
                    .Reference(b => b.UserType).LoadAsync();
            }

            return commission;
        }

        public async Task<IEnumerable<Commission>> GetDownlinesAsnyc(int uplineCommissionId)
        {
            var commissions = await _context
                        .Commissions
                        .Where(c => c.UplineCommissionId == uplineCommissionId)
                        .ToListAsync();

            if (commissions.Any())
            {
                foreach (var commission in commissions)
                {
                    await _context.Entry(commission)
                        .Reference(b => b.UserType).LoadAsync();
                }
            }

            return commissions;
        }

        public async Task<Commission> GetByUserIdAsync(string userId)
        {
            var commission = await _context
                    .Commissions
                    .FirstOrDefaultAsync(b => b.UserId == userId);
            if (commission == null)
            {
                commission = _context
                            .Commissions
                            .Local
                            .FirstOrDefault(b => b.UserId == userId);
            }
            if (commission != null)
            {
                await _context.Entry(commission)
                    .Reference(b => b.UserType).LoadAsync();
            }

            return commission;
        }

        public async Task<IEnumerable<Commission>> GetByStatusAsync(bool isEnabled)
        {
            var commissions = await _context.Commissions.Where(c => c.IsEnabled == isEnabled).ToListAsync();

            if (commissions.Any())
            {
                foreach (var commission in commissions)
                {
                    await _context.Entry(commission)
                        .Reference(b => b.UserType).LoadAsync();
                }
            }

            return commissions;
        }


        public decimal? GetCommissionRateByUserIdAsync(string userId, RateType rateType, out int? commissionId)
        {
            var commission = _context
                .Commissions
                .Include(c => c.UserType)
                .Where(c => c.UserId == userId)
                .Select(c => new
                {
                    c.Id,
                    c.UserType,
                    c.RateAlipay,
                    c.RateWechat,
                    c.RateRebateAlipay,
                    c.RateRebateWechat
                }).FirstOrDefault();

            if (commission is null)
            {
                commissionId = null;
                return null;
            }
            else
            {
                commissionId = commission.Id;
            }

            if (commission.UserType.Id == UserType.Trader.Id
                || commission.UserType.Id == UserType.TraderAgent.Id)
            {
                if (rateType.Id == RateType.Alipay.Id)
                {
                    return commission.RateAlipay;
                }
                else if (rateType.Id == RateType.Wechat.Id)
                {
                    return commission.RateWechat;
                }
                else
                {
                    throw new DistributingDomainException("Unknown rate type" + ". At DistributionService.GetRate()");
                }
            }
            else if (commission.UserType.Id == UserType.Shop.Id
                || commission.UserType.Id == UserType.ShopAgent.Id)
            {
                if (rateType.Id == RateType.Alipay.Id)
                {
                    return commission.RateRebateAlipay;
                }
                else if (rateType.Id == RateType.Wechat.Id)
                {
                    return commission.RateRebateWechat;
                }
                else
                {
                    throw new DistributingDomainException("Unknown rate type" + ". At DistributionService.GetRate()");
                }
            }
            else
            {
                throw new DistributingDomainException("Unknown user type found at commission" + ". At DistributionService.GetRate()");
            }
        }


        public async Task<CommissionInfo> GetCommissionInfoByUserIdAsync(string userId, RateType rateType)
        {
            var commissions = await _context
                .Commissions
                .Include(c => c.UserType)
                .Where(c => c.UserId == userId)
                .Select(c => new
                {
                    c.Id,
                    c.UserId,
                    c.ChainNumber,
                    c.UplineCommissionId,
                    c.UserType,
                    c.RateAlipay,
                    c.RateWechat,
                    c.RateRebateAlipay,
                    c.RateRebateWechat,
                    c.BalanceId
                }).FromCacheAsync();

            var commission = commissions.FirstOrDefault();

            if (commissions is null)
            {
                return null;
            }

            if (commission.UserType.Id == UserType.Trader.Id
                || commission.UserType.Id == UserType.TraderAgent.Id)
            {
                if (rateType.Id == RateType.Alipay.Id)
                {
                    return new CommissionInfo
                    {
                        Id = commission.Id,
                        Rate = commission.RateAlipay,
                        UplineCommissionId = commission.UplineCommissionId,
                        UserId = commission.UserId,
                        BalanceId = commission.BalanceId,
                        ChainNumber = commission.ChainNumber
                    };
                }
                else if (rateType.Id == RateType.Wechat.Id)
                {
                    return new CommissionInfo
                    {
                        Id = commission.Id,
                        Rate = commission.RateWechat,
                        UplineCommissionId = commission.UplineCommissionId,
                        UserId = commission.UserId,
                        BalanceId = commission.BalanceId,
                        ChainNumber = commission.ChainNumber
                    };
                }
                else
                {
                    throw new DistributingDomainException("Unknown rate type" + ". At DistributionService.GetRate()");
                }
            }
            else if (commission.UserType.Id == UserType.Shop.Id
                || commission.UserType.Id == UserType.ShopAgent.Id)
            {
                if (rateType.Id == RateType.Alipay.Id)
                {
                    return new CommissionInfo
                    {
                        Id = commission.Id,
                        Rate = commission.RateRebateAlipay,
                        UplineCommissionId = commission.UplineCommissionId,
                        UserId = commission.UserId,
                        BalanceId = commission.BalanceId,
                        ChainNumber = commission.ChainNumber
                    };
                }
                else if (rateType.Id == RateType.Wechat.Id)
                {
                    return new CommissionInfo
                    {
                        Id = commission.Id,
                        Rate = commission.RateRebateWechat,
                        UplineCommissionId = commission.UplineCommissionId,
                        UserId = commission.UserId,
                        BalanceId = commission.BalanceId,
                        ChainNumber = commission.ChainNumber
                    };
                }
                else
                {
                    throw new DistributingDomainException("Unknown rate type" + ". At DistributionService.GetRate()");
                }
            }
            else
            {
                throw new DistributingDomainException("Unknown user type found at commission" + ". At DistributionService.GetRate()");
            }
        }


        public async Task<CommissionInfo> GetCommissionInfoByCommissionIdAsync(int commissionId, RateType rateType)
        {
            var commissions = await _context
                .Commissions
                .Include(c => c.UserType)
                .Where(c => c.Id == commissionId)
                .Select(c => new
                {
                    c.Id,
                    c.UserId,
                    c.ChainNumber,
                    c.UplineCommissionId,
                    c.UserType,
                    c.RateAlipay,
                    c.RateWechat,
                    c.RateRebateAlipay,
                    c.RateRebateWechat,
                    c.BalanceId
                }).FromCacheAsync();

            var commission = commissions.FirstOrDefault();

            if (commission is null)
            {
                return null;
            }

            if (commission.UserType.Id == UserType.Trader.Id
                || commission.UserType.Id == UserType.TraderAgent.Id)
            {
                if (rateType.Id == RateType.Alipay.Id)
                {
                    return new CommissionInfo
                    {
                        Id = commission.Id,
                        Rate = commission.RateAlipay,
                        UplineCommissionId = commission.UplineCommissionId,
                        UserId = commission.UserId,
                        BalanceId = commission.BalanceId,
                        ChainNumber = commission.ChainNumber
                    };
                }
                else if (rateType.Id == RateType.Wechat.Id)
                {
                    return new CommissionInfo
                    {
                        Id = commission.Id,
                        Rate = commission.RateWechat,
                        UplineCommissionId = commission.UplineCommissionId,
                        UserId = commission.UserId,
                        BalanceId = commission.BalanceId,
                        ChainNumber = commission.ChainNumber
                    };
                }
                else
                {
                    throw new DistributingDomainException("Unknown rate type" + ". At DistributionService.GetRate()");
                }
            }
            else if (commission.UserType.Id == UserType.Shop.Id
                || commission.UserType.Id == UserType.ShopAgent.Id)
            {
                if (rateType.Id == RateType.Alipay.Id)
                {
                    return new CommissionInfo
                    {
                        Id = commission.Id,
                        Rate = commission.RateRebateAlipay,
                        UplineCommissionId = commission.UplineCommissionId,
                        UserId = commission.UserId,
                        BalanceId = commission.BalanceId,
                        ChainNumber = commission.ChainNumber
                    };
                }
                else if (rateType.Id == RateType.Wechat.Id)
                {
                    return new CommissionInfo
                    {
                        Id = commission.Id,
                        Rate = commission.RateRebateWechat,
                        UplineCommissionId = commission.UplineCommissionId,
                        UserId = commission.UserId,
                        BalanceId = commission.BalanceId,
                        ChainNumber = commission.ChainNumber
                    };
                }
                else
                {
                    throw new DistributingDomainException("Unknown rate type" + ". At DistributionService.GetRate()");
                }
            }
            else
            {
                throw new DistributingDomainException("Unknown user type found at commission" + ". At DistributionService.GetRate()");
            }
        }


        public async Task<CommissionInfo> GetToppestCommissionInfoByUserIdAsync(string userId, RateType rateType)
        {
            /*if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("The user Id must be provided.");
            }*/

            var result = await _context
                .Commissions
                .Where(b => b.UserId == userId)
                .Select(c => new
                {
                    c.Id,
                    c.UplineCommissionId
                })
                .FirstOrDefaultAsync();

            if (result is null)
            {
                throw new DistributingDomainException("No commission found by given user id.");
            }

            var uplineCommission = result;
            while (true)
            {
                if (uplineCommission == null)
                {
                    break;
                }

                var currentCommission = uplineCommission;

                result = currentCommission;

                uplineCommission = await _context
                    .Commissions
                    .Where(b => b.Id == result.UplineCommissionId)
                    .Select(c => new
                    {
                        c.Id,
                        c.UplineCommissionId
                    })
                    .FirstOrDefaultAsync();
            }

            var commissionInfo = await this.GetCommissionInfoByCommissionIdAsync(result.Id, rateType);
            return commissionInfo;
        }

        public async Task<List<CommissionInfo>> GetCommissionInfosByChainNumberAsync(int chainNumber, RateType rateType)
        {
            var result = new List<CommissionInfo>();

            var commissions = await _context
                .Commissions
                .Include(c => c.UserType)
                .Where(c => c.ChainNumber == chainNumber)
                .Select(c => new
                {
                    c.Id,
                    c.UserId,
                    c.ChainNumber,
                    c.UplineCommissionId,
                    c.UserType,
                    c.RateAlipay,
                    c.RateWechat,
                    c.RateRebateAlipay,
                    c.RateRebateWechat
                })
                .ToListAsync();

            Parallel.ForEach(commissions, commission =>
            {
                if (commission.UserType.Id == UserType.Trader.Id
                    || commission.UserType.Id == UserType.TraderAgent.Id)
                {
                    if (rateType.Id == RateType.Alipay.Id)
                    {
                        result.Add(new CommissionInfo
                        {
                            Id = commission.Id,
                            Rate = commission.RateAlipay,
                            UplineCommissionId = commission.UplineCommissionId,
                            UserId = commission.UserId,
                            ChainNumber = commission.ChainNumber
                        });
                    }
                    else if (rateType.Id == RateType.Wechat.Id)
                    {
                        result.Add(new CommissionInfo
                        {
                            Id = commission.Id,
                            Rate = commission.RateWechat,
                            UplineCommissionId = commission.UplineCommissionId,
                            UserId = commission.UserId,
                            ChainNumber = commission.ChainNumber
                        });
                    }
                    else
                    {
                        throw new DistributingDomainException("Unknown rate type" + ". At DistributionService.GetRate()");
                    }
                }
                else if (commission.UserType.Id == UserType.Shop.Id
                    || commission.UserType.Id == UserType.ShopAgent.Id)
                {
                    if (rateType.Id == RateType.Alipay.Id)
                    {
                        result.Add(new CommissionInfo
                        {
                            Id = commission.Id,
                            Rate = commission.RateRebateAlipay,
                            UplineCommissionId = commission.UplineCommissionId,
                            UserId = commission.UserId,
                            ChainNumber = commission.ChainNumber
                        });
                    }
                    else if (rateType.Id == RateType.Wechat.Id)
                    {
                        result.Add(new CommissionInfo
                        {
                            Id = commission.Id,
                            Rate = commission.RateRebateWechat,
                            UplineCommissionId = commission.UplineCommissionId,
                            UserId = commission.UserId,
                            ChainNumber = commission.ChainNumber
                        });
                    }
                    else
                    {
                        throw new DistributingDomainException("Unknown rate type" + ". At DistributionService.GetRate()");
                    }
                }
                else
                {
                    throw new DistributingDomainException("Unknown user type found at commission" + ". At DistributionService.GetRate()");
                }

            });

            return result;
        }



        public void Delete(Commission commission)
        {
            if (commission != null)
            {
                _context.Commissions.Remove(commission);
            }
        }
        public void DeleteRange(List<Commission> commissions)
        {
            if (commissions.Any())
            {
                _context.Commissions.RemoveRange(commissions);
            }
        }
    }
}
