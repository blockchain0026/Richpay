using Distributing.Domain.Model.Balances;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Data;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.Shops
{
    public class ShopQueries : IShopQueries
    {
        private readonly ApplicationDbContext _context;
        private readonly IBalanceDomainService _balanceDomainService;

        public ShopQueries(ApplicationDbContext context, IBalanceDomainService balanceDomainService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _balanceDomainService = balanceDomainService ?? throw new ArgumentNullException(nameof(balanceDomainService));
        }

        public async Task<Shop> GetShop(string shopId)
        {
            var result = await _context.Shops
                .Where(t => t.ShopId == shopId)
                .FirstOrDefaultAsync();
            if (result != null)
            {
                await LoadNavigationObject(result);
            }

            return await this.MapAvailableBalance(result);
        }

        public async Task<List<Shop>> GetShops(int pageIndex, int take, string searchString = "", string sortField = "", string direction = "desc")
        {
            var result = new List<Shop>();

            var shops = _context.Shops
                .AsNoTracking()
                .Include(t => t.RebateCommission)
                .Include(t => t.Balance)
                .Where(t => t.IsReviewed)
                .Select(t => new Shop
                {
                    ShopId = t.ShopId,
                    Email = t.Email,
                    Username = t.Username,
                    FullName = t.FullName,
                    SiteAddress = t.SiteAddress,
                    PhoneNumber = t.PhoneNumber,
                    UplineUserId = t.UplineUserId,
                    UplineUserName = t.UplineUserName,
                    UplineFullName = t.UplineFullName,
                    Balance = t.Balance,
                    RebateCommission = t.RebateCommission,
                    IsEnabled = t.IsEnabled,
                    IsReviewed = t.IsReviewed,
                    LastLoginIP = t.LastLoginIP,
                    DateLastLoggedIn = t.DateLastLoggedIn,
                    DateCreated = t.DateCreated
                });

            IQueryable<Shop> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                /*searchResult = shops.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.SiteAddress.Contains(searchString)
                             || m.PhoneNumber == searchString
                             );*/
                searchResult = shops.Where(s =>
                EF.Functions.Like(s.Email, "%" + searchString + "%")
                || EF.Functions.Like(s.Username, "%" + searchString + "%")
                || EF.Functions.Like(s.FullName, "%" + searchString + "%")
                || EF.Functions.Like(s.SiteAddress, "%" + searchString + "%")
                || s.PhoneNumber == searchString
                );
            }
            else
            {
                searchResult = shops;
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

        public async Task<int> GetShopsTotalCount(string searchString = null)
        {
            var shops = _context.Shops
                .Where(t => t.IsReviewed);

            var count = 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                count = await shops.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.SiteAddress.Contains(searchString)
                             || m.PhoneNumber == searchString
                             ).CountAsync();
            }
            else
            {
                count = await shops.CountAsync();
            }

            return count;
        }


        public async Task<List<Shop>> GetDownlines(int pageIndex, int take, string shopAgentId, string searchString = "", string sortField = "", string direction = "desc")
        {
            var result = new List<Shop>();

            IQueryable<Shop> shops = null;


            shops = _context.Shops
               .AsNoTracking()
               .Include(t => t.RebateCommission)
               .Include(t => t.Balance)
               .ThenInclude(b => b.WithdrawalLimit)
               .Where(t => t.UplineUserId == shopAgentId && t.IsReviewed)
               .Select(t => new Shop
               {
                   ShopId = t.ShopId,
                   Email = t.Email,
                   Username = t.Username,
                   FullName = t.FullName,
                   SiteAddress = t.SiteAddress,
                   PhoneNumber = t.PhoneNumber,
                   UplineUserId = t.UplineUserId,
                   UplineUserName = t.UplineUserName,
                   UplineFullName = t.UplineFullName,
                   Balance = t.Balance,
                   RebateCommission = t.RebateCommission,
                   IsEnabled = t.IsEnabled,
                   IsReviewed = t.IsReviewed,
                   LastLoginIP = t.LastLoginIP,
                   DateLastLoggedIn = t.DateLastLoggedIn,
                   DateCreated = t.DateCreated
               });

            IQueryable<Shop> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = shops.Where(s =>
                EF.Functions.Like(s.Email, "%" + searchString + "%")
                || EF.Functions.Like(s.Username, "%" + searchString + "%")
                || EF.Functions.Like(s.FullName, "%" + searchString + "%")
                || EF.Functions.Like(s.SiteAddress, "%" + searchString + "%")
                || s.PhoneNumber == searchString
                );
            }
            else
            {
                searchResult = shops;
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

        public async Task<int> GetDownlinesTotalCount(string shopAgentId, string searchString = null)
        {
            IQueryable<Shop> shops = null;

            if (string.IsNullOrEmpty(shopAgentId))
            {
                shops = _context.Shops
                   .Include(t => t.RebateCommission)
                   .Include(t => t.Balance)
                   .ThenInclude(b => b.WithdrawalLimit)
                   .Where(t => t.UplineUserId == null && t.IsReviewed);
            }
            else
            {
                shops = _context.Shops
                   .Include(t => t.RebateCommission)
                   .Include(t => t.Balance)
                   .ThenInclude(b => b.WithdrawalLimit)
                   .Where(t => t.UplineUserId == shopAgentId && t.IsReviewed);
            }

            var count = 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                count = await shops.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.SiteAddress.Contains(searchString)
                             || m.PhoneNumber == searchString
                             ).CountAsync();
            }
            else
            {
                count = await shops.CountAsync();
            }

            return count;
        }


        public List<Shop> GetPendingReviews(int pageIndex, int take, string uplineId = null, string searchString = "", string sortField = "", string direction = "desc")
        {
            var result = new List<Shop>();

            IQueryable<Shop> shops = null;

            if (!string.IsNullOrEmpty(uplineId))
            {
                shops = _context.Shops
                    .Include(t => t.RebateCommission)
                    .Include(t => t.Balance)
                    .ThenInclude(b => b.WithdrawalLimit)
                    .Where(t => !t.IsReviewed && t.UplineUserId == uplineId);
            }
            else
            {
                shops = _context.Shops
                    .Include(t => t.RebateCommission)
                    .Include(t => t.Balance)
                    .ThenInclude(b => b.WithdrawalLimit)
                    .Where(t => !t.IsReviewed);
            }

            IQueryable<Shop> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = shops.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.SiteAddress.Contains(searchString)
                             || m.PhoneNumber == searchString
                             );
            }
            else
            {
                searchResult = shops;
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
            IQueryable<Shop> shops = null;

            if (!string.IsNullOrEmpty(uplineId))
            {
                shops = _context.Shops
                    .Include(t => t.RebateCommission)
                    .Include(t => t.Balance)
                    .ThenInclude(b => b.WithdrawalLimit)
                    .Where(t => !t.IsReviewed && t.UplineUserId == uplineId);

            }
            else
            {
                shops = _context.Shops
                    .Include(t => t.RebateCommission)
                    .Include(t => t.Balance)
                    .ThenInclude(b => b.WithdrawalLimit)
                    .Where(t => !t.IsReviewed);
            }

            var count = 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                count = await shops.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.SiteAddress.Contains(searchString)
                             || m.PhoneNumber == searchString
                             ).CountAsync();
            }
            else
            {
                count = await shops.CountAsync();
            }

            return count;
        }




        private List<Shop> GetSortedRecords(
            IQueryable<Shop> shops,
            int? pageIndex, int? take, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<Shop>();

            if (pageIndex != null && take != null)
            {
                IEnumerable<Shop> sortedResult = null;
                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "DateCreated")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shops
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shops
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "DateLastTrade")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shops
                               .OrderBy(f => f.DateLastTrade)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shops
                               .OrderByDescending(f => f.DateLastTrade)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "Username")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shops
                               .OrderBy(f => f.Username)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shops
                               .OrderByDescending(f => f.Username)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "SiteAddress")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shops
                               .OrderBy(f => f.SiteAddress)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shops
                               .OrderByDescending(f => f.SiteAddress)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "LastLogin")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shops
                               .OrderBy(f => f.DateLastLoggedIn)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shops
                               .OrderByDescending(f => f.DateLastLoggedIn)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "UplineUserName")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shops
                               .OrderBy(f => f.UplineUserName)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shops
                               .OrderByDescending(f => f.UplineUserName)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "AmountAvailable")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shops
                                .OrderBy(f => f.Balance.AmountAvailable)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shops
                               .OrderByDescending(f => f.Balance.AmountAvailable)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "AmountFrozen")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shops
                                .OrderBy(f => f.Balance.AmountFrozen)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shops
                               .OrderByDescending(f => f.Balance.AmountFrozen)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "RateRebateAlipayInThousandth")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shops
                                .OrderBy(f => f.RebateCommission.RateRebateAlipayInThousandth)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shops
                               .OrderByDescending(f => f.RebateCommission.RateRebateAlipayInThousandth)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "RateRebateWechatInThousandth")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shops
                                .OrderBy(f => f.RebateCommission.RateRebateWechatInThousandth)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shops
                               .OrderByDescending(f => f.RebateCommission.RateRebateWechatInThousandth)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "Status")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shops
                               .OrderBy(f => f.IsEnabled)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shops
                               .OrderByDescending(f => f.IsEnabled)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "IsOpen")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shops
                               .OrderBy(f => f.IsOpen)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shops
                               .OrderByDescending(f => f.IsOpen)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "IsReviewed")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shops
                               .OrderBy(f => f.IsReviewed)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shops
                               .OrderByDescending(f => f.IsReviewed)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = shops
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = shops
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                }
                else
                {
                    sortedResult = shops
                       .OrderByDescending(f => f.DateCreated)
                       .Skip((int)take * (int)pageIndex)
                       .Take((int)take);
                }

                foreach (var shop in sortedResult)
                {
                    result.Add(shop);
                }
            }
            else
            {
                foreach (var shop in shops)
                {
                    result.Add(shop);
                }
            }

            return result;
        }

        public async Task UpdateBalanceAsync(string userId, decimal balance)
        {
            var user = await _context.Shops
                .Include(t => t.Balance)
                .Where(u => u.ShopId == userId)
                .Select(u => new
                {
                    u.ShopId,
                    u.Balance
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var toUpdate = new Shop
            {
                ShopId = userId,
                Balance = user.Balance
            };

            _context.Shops.Attach(toUpdate);
            _context.Entry(toUpdate).Reference(b => b.Balance).IsModified = true;

            toUpdate.Balance.AmountAvailable = balance;
        }


        public void UpdateBalance(string userId, decimal balance)
        {
            var user = _context.Shops
                .Include(t => t.Balance)
                .Where(u => u.ShopId == userId)
                .Select(u => new
                {
                    u.ShopId,
                    u.Balance
                })
                .AsNoTracking()
                .FirstOrDefault();

            var toUpdate = new Shop
            {
                ShopId = userId,
                Balance = user.Balance
            };

            _context.Shops.Attach(toUpdate);
            _context.Entry(toUpdate).Reference(b => b.Balance).IsModified = true;

            toUpdate.Balance.AmountAvailable = balance;
        }


        public Shop Add(Shop shop)
        {
            return _context.Shops.Add(shop).Entity;
        }

        public void Update(Shop shop)
        {
            _context.Entry(shop).State = EntityState.Modified;
        }

        public void Delete(Shop shop)
        {
            if (shop != null)
            {
                _context.Shops.Remove(shop);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        private async Task LoadNavigationObject(Shop shop)
        {
            if (shop == null)
            {
                throw new ArgumentNullException("The shop must be provided.");
            }
            await _context.Entry(shop)
                .Reference(c => c.Balance).LoadAsync();
            await _context.Entry(shop.Balance)
                .Reference(b => b.WithdrawalLimit).LoadAsync();
            await _context.Entry(shop)
                .Reference(c => c.RebateCommission).LoadAsync();
        }
        private async Task<Shop> MapAvailableBalance(Shop shop)
        {
            var availableBalance = await _balanceDomainService.GetAvailableBalanceAsync(shop.ShopId);

            shop.Balance.AmountAvailable = availableBalance;

            return shop;
        }


        #region Custom Operation
        public void UpdateBaseInfo(string userId, string fullName, string siteAddress,
            string phoneNumber, string email, string wechat, string qq)
        {
            var toUpdate = new Shop
            {
                ShopId = userId
            };

            _context.Shops.Attach(toUpdate);
            _context.Entry(toUpdate).Property(b => b.FullName).IsModified = true;
            _context.Entry(toUpdate).Property(b => b.SiteAddress).IsModified = true;
            _context.Entry(toUpdate).Property(b => b.PhoneNumber).IsModified = true;
            _context.Entry(toUpdate).Property(b => b.Email).IsModified = true;
            _context.Entry(toUpdate).Property(b => b.Wechat).IsModified = true;
            _context.Entry(toUpdate).Property(b => b.QQ).IsModified = true;

            toUpdate.FullName = fullName;
            toUpdate.SiteAddress = siteAddress;
            toUpdate.PhoneNumber = phoneNumber;
            toUpdate.Email = email;
            toUpdate.Wechat = wechat;
            toUpdate.QQ = qq;
        }
        #endregion
    }
}
