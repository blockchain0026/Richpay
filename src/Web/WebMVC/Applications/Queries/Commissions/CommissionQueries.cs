using Distributing.Domain.Model.Commissions;
using Distributing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.Commission
{
    public class CommissionQueries : ICommissionQueries
    {
        private readonly DistributingContext _context;

        public CommissionQueries(DistributingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<RebateCommission> GetCommissionFromShopUserAsync(string userId)
        {
            /*if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("The user Id must be provided.");
            }*/

            var result = await _context.Commissions.Where(b => b.UserId == userId).SingleOrDefaultAsync();

            if (result != null)
            {
                await LoadNavigationObject(result);

                if (result.UserType.Id != UserType.Shop.Id
                    && result.UserType.Id != UserType.ShopAgent.Id)
                {
                    throw new ArgumentOutOfRangeException("The commission found is not a shop user. try use GetCommissionFromTradeUserAsync() instead.");
                }
                return MapRebateCommissionFromEntity(result);
            }

            return null;
        }

        public async Task<TradingCommission> GetCommissionFromTradeUserAsync(string userId)
        {
            /*if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("The user Id must be provided.");
            }*/

            var result = await _context
                .Commissions
                .Where(b => b.UserId == userId)
                .FirstOrDefaultAsync();

            if (result != null)
            {
                await LoadNavigationObject(result);

                if (result.UserType.Id != UserType.Trader.Id
                    && result.UserType.Id != UserType.TraderAgent.Id)
                {
                    throw new ArgumentOutOfRangeException("The commission found is not a trade user. try use GetCommissionFromShopUserAsync() instead.");
                }
                return MapTradingCommissionFromEntity(result);

            }

            return null;
        }
        public async Task<TradingCommission> GetToppestCommissionRateFromTradeUserAsync(string userId)
        {
            /*if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("The user Id must be provided.");
            }*/

            var result = await _context
                .Commissions
                .Where(b => b.UserId == userId)
                .FirstOrDefaultAsync();

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
                    .Where(b => b.Id == currentCommission.UplineCommissionId)
                    .FirstOrDefaultAsync();
            }
            if (result != null)
            {
                await LoadNavigationObject(result);

                if (result.UserType.Id != UserType.Trader.Id
                    && result.UserType.Id != UserType.TraderAgent.Id)
                {
                    throw new ArgumentOutOfRangeException("The commission found is not a trade user. try use GetCommissionFromShopUserAsync() instead.");
                }

                return MapTradingCommissionFromEntity(result);
            }

            return null;
        }
            
        public async Task<RebateCommission> GetToppestCommissionRateFromShopUserAsync(string userId)
        {
            /*if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("The user Id must be provided.");
            }*/

            var result = await _context
                .Commissions
                .Where(b => b.UserId == userId)
                .FirstOrDefaultAsync();

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
                    .Where(b => b.Id == currentCommission.UplineCommissionId)
                    .FirstOrDefaultAsync();
            }
            if (result != null)
            {
                await LoadNavigationObject(result);

                if (result.UserType.Id != UserType.Shop.Id
                    && result.UserType.Id != UserType.ShopAgent.Id)
                {
                    throw new ArgumentOutOfRangeException("The commission found is not a shop user. try use GetToppestCommissionRateFromTradeUserAsync() instead.");
                }

                return MapRebateCommissionFromEntity(result);
            }

            return null;
        }

        private RebateCommission MapRebateCommissionFromEntity(Distributing.Domain.Model.Commissions.Commission entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity must be provided for mapping.");
            }

            var rebateCommission = new RebateCommission
            {
                RateRebateAlipayInThousandth = (int)(entity.RateRebateAlipay * 1000),
                RateRebateWechatInThousandth = (int)(entity.RateRebateWechat * 1000)
            };

            return rebateCommission;
        }

        private TradingCommission MapTradingCommissionFromEntity(Distributing.Domain.Model.Commissions.Commission entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity must be provided for mapping.");
            }

            var tradingCommission = new TradingCommission
            {
                RateAlipayInThousandth = (int)(entity.RateAlipay * 1000),
                RateWechatInThousandth = (int)(entity.RateWechat * 1000)
            };

            return tradingCommission;
        }


        private async Task LoadNavigationObject(Distributing.Domain.Model.Commissions.Commission commission)
        {
            if (commission == null)
            {
                throw new ArgumentNullException("The commission must be provided.");
            }
            await _context.Entry(commission)
                .Reference(c => c.UserType).LoadAsync();
        }

        public async Task<string> GetUplineUserIdFromUserAsync(string userId)
        {
            var commission = await _context
                .Commissions
                .Where(b => b.UserId == userId)
                .FirstOrDefaultAsync();

            if (commission != null)
            {
                var upline = await _context.Commissions.Where(c => c.Id == commission.UplineCommissionId).FirstOrDefaultAsync();
                if (upline != null)
                {
                    return upline.UserId;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new KeyNotFoundException("No commission found by given user Id.");
            }
        }
    }
}
