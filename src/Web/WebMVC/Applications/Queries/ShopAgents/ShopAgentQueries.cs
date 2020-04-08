using Distributing.Domain.Model.Balances;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Data;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.ShopAgents
{
    public class ShopAgentQueries : IShopAgentQueries
    {
        private readonly ApplicationDbContext _context;
        private readonly IBalanceDomainService _balanceDomainService;

        public ShopAgentQueries(ApplicationDbContext context, IBalanceDomainService balanceDomainService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _balanceDomainService = balanceDomainService ?? throw new ArgumentNullException(nameof(balanceDomainService));
        }

        public async Task<ShopAgent> GetShopAgent(string shopAgentId)
        {
            var result = await _context.ShopAgents
                .Where(t => t.ShopAgentId == shopAgentId)
                .FirstOrDefaultAsync();
            if (result != null)
            {
                await LoadNavigationObject(result);
            }

            return await this.MapAvailableBalance(result);
        }

        public async Task<List<ShopAgent>> GetShopAgents(int pageIndex, int take, string searchString = "", string sortField = "", string direction = "desc")
        {
            var result = new List<ShopAgent>();

            var shopAgents = _context.ShopAgents
                .AsNoTracking()
                .Include(t => t.RebateCommission)
                .Include(t => t.Balance)
                .Where(t => t.IsReviewed)
                .Select(t => new ShopAgent
                {
                    ShopAgentId = t.ShopAgentId,
                    Email = t.Email,
                    Username = t.Username,
                    FullName = t.FullName,
                    Nickname = t.Nickname,
                    PhoneNumber = t.PhoneNumber,
                    UplineUserId = t.UplineUserId,
                    UplineUserName = t.UplineUserName,
                    UplineFullName = t.UplineFullName,
                    Balance = t.Balance,
                    RebateCommission = t.RebateCommission,
                    IsEnabled = t.IsEnabled,
                    IsReviewed = t.IsReviewed,
                    HasGrantRight = t.HasGrantRight,
                    LastLoginIP = t.LastLoginIP,
                    DateLastLoggedIn = t.DateLastLoggedIn,
                    DateCreated = t.DateCreated
                });

            IQueryable<ShopAgent> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = shopAgents.Where(s =>
                EF.Functions.Like(s.Email, "%" + searchString + "%")
                || EF.Functions.Like(s.Username, "%" + searchString + "%")
                || EF.Functions.Like(s.FullName, "%" + searchString + "%")
                || EF.Functions.Like(s.Nickname, "%" + searchString + "%")
                || s.PhoneNumber == searchString
                );
            }
            else
            {
                searchResult = shopAgents;
            }

            var sortedRecords = this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            foreach (var record in sortedRecords)
            {
                result.Add(
                    await this.MapAvailableBalance(record));
            }
            return result;
        }

        public async Task<int> GetShopAgentsTotalCount(string searchString = null)
        {
            var shopAgents = _context.ShopAgents
                .Where(t => t.IsReviewed);

            var count = 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                count = await shopAgents.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.Nickname.Contains(searchString)
                             || m.PhoneNumber == searchString
                             ).CountAsync();
            }
            else
            {
                count = await shopAgents.CountAsync();
            }

            return count;
        }


        public async Task<List<ShopAgent>> GetDownlines(int pageIndex, int take, string shopAgentId = null, string searchString = "", string sortField = "", string direction = "desc")
        {
            var result = new List<ShopAgent>();

            IQueryable<ShopAgent> shopAgents = null;

            if (string.IsNullOrEmpty(shopAgentId))
            {
                shopAgents = _context.ShopAgents
                   .AsNoTracking()
                   .Include(t => t.RebateCommission)
                   .Include(t => t.Balance)
                   .Where(t => t.UplineUserId == null && t.IsReviewed)
                   .Select(t => new ShopAgent
                   {
                       ShopAgentId = t.ShopAgentId,
                       Email = t.Email,
                       Username = t.Username,
                       FullName = t.FullName,
                       Nickname = t.Nickname,
                       PhoneNumber = t.PhoneNumber,
                       UplineUserId = t.UplineUserId,
                       UplineUserName = t.UplineUserName,
                       UplineFullName = t.UplineFullName,
                       Balance = t.Balance,
                       RebateCommission = t.RebateCommission,
                       IsEnabled = t.IsEnabled,
                       IsReviewed = t.IsReviewed,
                       HasGrantRight = t.HasGrantRight,
                       LastLoginIP = t.LastLoginIP,
                       DateLastLoggedIn = t.DateLastLoggedIn,
                       DateCreated = t.DateCreated
                   });
            }
            else
            {
                shopAgents = _context.ShopAgents
                   .AsNoTracking()
                   .Include(t => t.RebateCommission)
                   .Include(t => t.Balance)
                   .ThenInclude(b => b.WithdrawalLimit)
                   .Where(t => t.UplineUserId == shopAgentId && t.IsReviewed)
                   .Select(t => new ShopAgent
                   {
                       ShopAgentId = t.ShopAgentId,
                       Email = t.Email,
                       Username = t.Username,
                       FullName = t.FullName,
                       Nickname = t.Nickname,
                       PhoneNumber = t.PhoneNumber,
                       UplineUserId = t.UplineUserId,
                       UplineUserName = t.UplineUserName,
                       UplineFullName = t.UplineFullName,
                       Balance = t.Balance,
                       RebateCommission = t.RebateCommission,
                       IsEnabled = t.IsEnabled,
                       IsReviewed = t.IsReviewed,
                       HasGrantRight = t.HasGrantRight,
                       LastLoginIP = t.LastLoginIP,
                       DateLastLoggedIn = t.DateLastLoggedIn,
                       DateCreated = t.DateCreated
                   });
            }

            IQueryable<ShopAgent> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = shopAgents.Where(s =>
                EF.Functions.Like(s.Email, "%" + searchString + "%")
                || EF.Functions.Like(s.Username, "%" + searchString + "%")
                || EF.Functions.Like(s.FullName, "%" + searchString + "%")
                || EF.Functions.Like(s.Nickname, "%" + searchString + "%")
                || s.PhoneNumber == searchString
                );

            }
            else
            {
                searchResult = shopAgents;
            }

            var sortedRecords = this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            foreach (var record in sortedRecords)
            {
                result.Add(
                    await this.MapAvailableBalance(record));
            }

            return result;
        }

        public async Task<int> GetDownlinesTotalCount(string shopAgentId = null, string searchString = null)
        {
            IQueryable<ShopAgent> shopAgents = null;

            if (string.IsNullOrEmpty(shopAgentId))
            {
                shopAgents = _context.ShopAgents
                   .Include(t => t.RebateCommission)
                   .Include(t => t.Balance)
                   .ThenInclude(b => b.WithdrawalLimit)
                   .Where(t => t.UplineUserId == null && t.IsReviewed);
            }
            else
            {
                shopAgents = _context.ShopAgents
                   .Include(t => t.RebateCommission)
                   .Include(t => t.Balance)
                   .ThenInclude(b => b.WithdrawalLimit)
                   .Where(t => t.UplineUserId == shopAgentId && t.IsReviewed);
            }

            var count = 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                count = await shopAgents.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.Nickname.Contains(searchString)
                             || m.PhoneNumber == searchString
                             ).CountAsync();
            }
            else
            {
                count = await shopAgents.CountAsync();
            }

            return count;
        }


        public List<ShopAgent> GetPendingReviews(int pageIndex, int take, string uplineId = null, string searchString = "", string sortField = "", string direction = "desc")
        {
            var result = new List<ShopAgent>();

            IQueryable<ShopAgent> shopAgents = null;

            if (!string.IsNullOrEmpty(uplineId))
            {
                shopAgents = _context.ShopAgents
                    .Include(t => t.RebateCommission)
                    .Include(t => t.Balance)
                    .ThenInclude(b => b.WithdrawalLimit)
                    .Where(t => !t.IsReviewed && t.UplineUserId == uplineId);
            }
            else
            {
                shopAgents = _context.ShopAgents
                    .Include(t => t.RebateCommission)
                    .Include(t => t.Balance)
                    .ThenInclude(b => b.WithdrawalLimit)
                    .Where(t => !t.IsReviewed);
            }

            IQueryable<ShopAgent> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = shopAgents.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.Nickname.Contains(searchString)
                             || m.PhoneNumber == searchString
                             );
            }
            else
            {
                searchResult = shopAgents;
            }

            result = this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            return result;
        }

        public async Task<int> GetPendingReviewsTotalCount(string uplineId = null, string searchString = null)
        {
            IQueryable<ShopAgent> shopAgents = null;

            if (!string.IsNullOrEmpty(uplineId))
            {
                shopAgents = _context.ShopAgents
                    .Include(t => t.RebateCommission)
                    .Include(t => t.Balance)
                    .ThenInclude(b => b.WithdrawalLimit)
                    .Where(t => !t.IsReviewed && t.UplineUserId == uplineId);

            }
            else
            {
                shopAgents = _context.ShopAgents
                    .Include(t => t.RebateCommission)
                    .Include(t => t.Balance)
                    .ThenInclude(b => b.WithdrawalLimit)
                    .Where(t => !t.IsReviewed);
            }

            var count = 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                count = await shopAgents.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.Nickname.Contains(searchString)
                             || m.PhoneNumber == searchString
                             ).CountAsync();
            }
            else
            {
                count = await shopAgents.CountAsync();
            }

            return count;
        }




        private List<ShopAgent> GetSortedRecords(
            IQueryable<ShopAgent> shopAgents,
            int? pageIndex, int? take, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<ShopAgent>();

            if (pageIndex != null && take != null)
            {
                IEnumerable<ShopAgent> sortedResult = null;
                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "DateCreated")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shopAgents
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shopAgents
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "Username")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shopAgents
                               .OrderBy(f => f.Username)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shopAgents
                               .OrderByDescending(f => f.Username)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "Nickname")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shopAgents
                               .OrderBy(f => f.Nickname)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shopAgents
                               .OrderByDescending(f => f.Nickname)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "LastLogin")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shopAgents
                               .OrderBy(f => f.DateLastLoggedIn)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shopAgents
                               .OrderByDescending(f => f.DateLastLoggedIn)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "Status")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shopAgents
                               .OrderBy(f => f.IsEnabled)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shopAgents
                               .OrderByDescending(f => f.IsEnabled)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "UplineUserName")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shopAgents
                               .OrderBy(f => f.UplineUserName)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shopAgents
                               .OrderByDescending(f => f.UplineUserName)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "AmountAvailable")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shopAgents
                                .OrderBy(f => f.Balance.AmountAvailable)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shopAgents
                               .OrderByDescending(f => f.Balance.AmountAvailable)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "AmountFrozen")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shopAgents
                                .OrderBy(f => f.Balance.AmountFrozen)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shopAgents
                               .OrderByDescending(f => f.Balance.AmountFrozen)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "RateRebateAlipayInThousandth")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shopAgents
                                .OrderBy(f => f.RebateCommission.RateRebateAlipayInThousandth)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shopAgents
                               .OrderByDescending(f => f.RebateCommission.RateRebateAlipayInThousandth)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "RateRebateWechatInThousandth")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shopAgents
                                .OrderBy(f => f.RebateCommission.RateRebateWechatInThousandth)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shopAgents
                               .OrderByDescending(f => f.RebateCommission.RateRebateWechatInThousandth)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "HasGrantRight")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shopAgents
                                .OrderBy(f => f.HasGrantRight)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shopAgents
                               .OrderByDescending(f => f.HasGrantRight)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shopAgents
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shopAgents
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                }
                else
                {
                    sortedResult = shopAgents
                       .OrderByDescending(f => f.DateCreated)
                       .Skip((int)take * (int)pageIndex)
                       .Take((int)take);
                }

                foreach (var shopAgent in sortedResult)
                {
                    result.Add(shopAgent);
                }
            }
            else
            {
                foreach (var shopAgent in shopAgents)
                {
                    result.Add(shopAgent);
                }
            }

            return result;
        }


        public ShopAgent Add(ShopAgent shopAgent)
        {
            return _context.ShopAgents.Add(shopAgent).Entity;
        }

        public void Update(ShopAgent shopAgent)
        {
            _context.Entry(shopAgent).State = EntityState.Modified;
        }

        public void Delete(ShopAgent shopAgent)
        {
            if (shopAgent != null)
            {
                _context.ShopAgents.Remove(shopAgent);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        private async Task LoadNavigationObject(ShopAgent shopAgent)
        {
            if (shopAgent == null)
            {
                throw new ArgumentNullException("The shop agent must be provided.");
            }
            await _context.Entry(shopAgent)
                .Reference(c => c.Balance).LoadAsync();
            await _context.Entry(shopAgent.Balance)
                .Reference(b => b.WithdrawalLimit).LoadAsync();
            await _context.Entry(shopAgent)
                .Reference(c => c.RebateCommission).LoadAsync();
        }

        private async Task<ShopAgent> MapAvailableBalance(ShopAgent shopAgent)
        {
            var availableBalance = await _balanceDomainService.GetAvailableBalanceAsync(shopAgent.ShopAgentId);

            shopAgent.Balance.AmountAvailable = availableBalance;

            return shopAgent;
        }



        #region Custom Operation
        public async Task UpdateBalanceAsync(string userId, decimal balance)
        {
            var user = await _context.ShopAgents
                .Include(t => t.Balance)
                .Where(u => u.ShopAgentId == userId)
                .Select(u => new
                {
                    u.ShopAgentId,
                    u.Balance
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var toUpdate = new ShopAgent
            {
                ShopAgentId = userId,
                Balance = user.Balance
            };

            _context.ShopAgents.Attach(toUpdate);
            _context.Entry(toUpdate).Reference(b => b.Balance).IsModified = true;

            toUpdate.Balance.AmountAvailable = balance;
        }

        public void UpdateBalance(string userId, decimal balance)
        {
            var user = _context.ShopAgents
                .Include(t => t.Balance)
                .Where(u => u.ShopAgentId == userId)
                .Select(u => new
                {
                    u.ShopAgentId,
                    u.Balance
                })
                .AsNoTracking()
                .FirstOrDefault();

            var toUpdate = new ShopAgent
            {
                ShopAgentId = userId,
                Balance = user.Balance
            };

            _context.ShopAgents.Attach(toUpdate);
            _context.Entry(toUpdate).Reference(b => b.Balance).IsModified = true;

            toUpdate.Balance.AmountAvailable = balance;
        }

        public void UpdateBaseInfo(string userId, string fullName, string nickname,
            string phoneNumber, string email, string wechat, string qq)
        {
            var toUpdate = new ShopAgent
            {
                ShopAgentId = userId
            };

            _context.ShopAgents.Attach(toUpdate);
            _context.Entry(toUpdate).Property(b => b.FullName).IsModified = true;
            _context.Entry(toUpdate).Property(b => b.Nickname).IsModified = true;
            _context.Entry(toUpdate).Property(b => b.PhoneNumber).IsModified = true;
            _context.Entry(toUpdate).Property(b => b.Email).IsModified = true;
            _context.Entry(toUpdate).Property(b => b.Wechat).IsModified = true;
            _context.Entry(toUpdate).Property(b => b.QQ).IsModified = true;

            toUpdate.FullName = fullName;
            toUpdate.Nickname = nickname;
            toUpdate.PhoneNumber = phoneNumber;
            toUpdate.Email = email;
            toUpdate.Wechat = wechat;
            toUpdate.QQ = qq;
        }
        #endregion
    }
}
