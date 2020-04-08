using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Balances;
using WebMVC.Applications.Queries.ShopAgents;
using WebMVC.Applications.Queries.Shops;
using WebMVC.Applications.Queries.TraderAgents;
using WebMVC.Applications.Queries.Traders;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;

namespace WebMVC.Infrastructure.Services.Implementations
{
    public class PersonalService : IPersonalService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IBalanceQueries _balanceQueries;
        private readonly ITraderQueries _traderQueries;
        private readonly ITraderAgentQueries _traderAgentQueries;
        private readonly IShopQueries _shopQueries;
        private readonly IShopAgentQueries _shopAgentQueries;

        public PersonalService(UserManager<ApplicationUser> userManager, IBalanceQueries balanceQueries, ITraderQueries traderQueries, ITraderAgentQueries traderAgentQueries, IShopQueries shopQueries, IShopAgentQueries shopAgentQueries)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _balanceQueries = balanceQueries ?? throw new ArgumentNullException(nameof(balanceQueries));
            _traderQueries = traderQueries ?? throw new ArgumentNullException(nameof(traderQueries));
            _traderAgentQueries = traderAgentQueries ?? throw new ArgumentNullException(nameof(traderAgentQueries));
            _shopQueries = shopQueries ?? throw new ArgumentNullException(nameof(shopQueries));
            _shopAgentQueries = shopAgentQueries ?? throw new ArgumentNullException(nameof(shopAgentQueries));
        }



        #region Queries

        public async Task<decimal> GetAvailableBalance(string userId, string searchByUserId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new InvalidOperationException("Must provide user Id to search balance.");
            }

            var searchByUser = await _userManager.FindByIdAsync(searchByUserId);
            if (searchByUser == null)
            {
                throw new KeyNotFoundException("No user found by given id.");
            }

            if (searchByUser.BaseRoleType != BaseRoleType.Manager)
            {
                if (searchByUser.Id != userId)
                {
                    //Trader Agent can search his trader's balance.
                    if (searchByUser.BaseRoleType == BaseRoleType.TraderAgent)
                    {
                        var userToSearch = await _userManager.Users
                            .Where(u => u.Id == userId)
                            .Select(u => new
                            {
                                u.Id,
                                u.BaseRoleType,
                                u.UplineId
                            })
                            .FirstOrDefaultAsync();

                        if (string.IsNullOrEmpty(userToSearch.UplineId) || userToSearch.UplineId != searchByUser.Id || userToSearch.BaseRoleType != BaseRoleType.Trader)
                        {
                            throw new InvalidOperationException("代理只能查询自身或直属交易员的余额。");
                        }
                    }
                }
                else if (userId != searchByUserId)
                {
                    throw new InvalidOperationException("User can only search his balance.");
                }
            }

            var availableBalance = await _balanceQueries.GetAvailableBalanceByUserId(userId);

            if (availableBalance == null)
            {
                throw new KeyNotFoundException("No balance found by given user Id.");
            }

            return (decimal)availableBalance;
        }

        public async Task<TraderAgent> GetTraderAgent(string traderAgentId)
        {
            var traderAgent = await _traderAgentQueries.GetTraderAgent(traderAgentId);

            if (traderAgent == null)
            {
                throw new Exception("No trader agent found.");
            }

            return traderAgent;
        }

        public async Task<Trader> GetTrader(string traderId)
        {
            var trader = await _traderQueries.GetTrader(traderId);

            if (trader == null)
            {
                throw new Exception("No trader found.");
            }

            return trader;
        }

        public async Task<ShopAgent> GetShopAgent(string shopAgentId)
        {
            var shopAgent = await _shopAgentQueries.GetShopAgent(shopAgentId);

            if (shopAgent == null)
            {
                throw new Exception("No shop agent found.");
            }

            return shopAgent;
        }

        public async Task<Shop> GetShop(string shopId)
        {
            var shop = await _shopQueries.GetShop(shopId);

            if (shop == null)
            {
                throw new Exception("No shop found.");
            }

            return shop;
        }
        #endregion

        #region Command

        public async Task UpdatePersonalInfo(string userId, string fullName, string nickname, string siteAddress,
            string phoneNumber, string email, string wechat, string qq)
        {
            //Properties can not changed:
            //    ShopId, Username, IsReviewed, LastLoginIP, DateLastLoggedIn, DateCreated.
            var user = await _userManager.FindByIdAsync(userId);


            #region Update User Base Info
            //Update shop's base info.
            user.FullName = fullName;
            if (user.BaseRoleType == BaseRoleType.Shop)
            {
                user.Nickname = fullName;
            }
            else
            {
                user.Nickname = nickname;
            }

            user.PhoneNumber = phoneNumber;
            user.Email = email;
            user.Wechat = wechat ?? string.Empty;
            user.QQ = qq ?? string.Empty;

            //Save user's info.
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new Exception("Failed to update user.");
            }
            #endregion


            #region Update View Model
            if (user.BaseRoleType == BaseRoleType.Shop)
            {
                _shopQueries.UpdateBaseInfo(
                    user.Id,
                    user.FullName,
                    siteAddress,
                    user.PhoneNumber,
                    user.Email,
                    user.Wechat,
                    user.QQ
                    );
            }
            else if (user.BaseRoleType == BaseRoleType.ShopAgent)
            {
                _shopAgentQueries.UpdateBaseInfo(
                    user.Id,
                    user.FullName,
                    user.Nickname,
                    user.PhoneNumber,
                    user.Email,
                    user.Wechat,
                    user.QQ
                    );
            }
            else if (user.BaseRoleType == BaseRoleType.Trader)
            {
                _traderQueries.UpdateBaseInfo(
                    user.Id,
                    user.FullName,
                    user.Nickname,
                    user.PhoneNumber,
                    user.Email,
                    user.Wechat,
                    user.QQ
                    );
            }
            else if (user.BaseRoleType == BaseRoleType.TraderAgent)
            {
                _traderAgentQueries.UpdateBaseInfo(
                    user.Id,
                    user.FullName,
                    user.Nickname,
                    user.PhoneNumber,
                    user.Email,
                    user.Wechat,
                    user.QQ
                    );
            }
            else
            {
                throw new Exception("无法辨识用户身份");
            }

            //Use any queries to save changes, 
            //this'll call ApplicationDbContext's SaveChangesAsync() 
            //and save all change made above.
            await _shopQueries.SaveChangesAsync();
            #endregion
        }

        #endregion




    }
}
