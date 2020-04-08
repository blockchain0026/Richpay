using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.Commission
{
    public interface ICommissionQueries
    {
        Task<TradingCommission> GetCommissionFromTradeUserAsync(string userId);
        Task<RebateCommission> GetCommissionFromShopUserAsync(string userId);
        Task<TradingCommission> GetToppestCommissionRateFromTradeUserAsync(string userId);
        Task<RebateCommission> GetToppestCommissionRateFromShopUserAsync(string userId);

        Task<string> GetUplineUserIdFromUserAsync(string userId);
    }
}
