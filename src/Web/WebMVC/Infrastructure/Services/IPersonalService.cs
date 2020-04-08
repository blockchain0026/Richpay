using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Infrastructure.Services
{
    public interface IPersonalService
    {
        Task<decimal> GetAvailableBalance(string userId, string searchByUserId);
        Task<Shop> GetShop(string shopId);
        Task<ShopAgent> GetShopAgent(string shopAgentId);
        Task<Trader> GetTrader(string traderId);
        Task<TraderAgent> GetTraderAgent(string traderAgentId);
        Task UpdatePersonalInfo(string userId, string fullName, string nickname, string siteAddress, string phoneNumber, string email, string wechat, string qq);
    }
}
