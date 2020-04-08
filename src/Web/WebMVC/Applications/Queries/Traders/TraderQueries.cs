using Distributing.Domain.Model.Balances;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.TraderAgents;
using WebMVC.Data;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.Traders
{
    public class TraderQueries : ITraderQueries
    {
        private readonly ApplicationDbContext _context;
        private readonly IBalanceDomainService _balanceDomainService;

        public TraderQueries(ApplicationDbContext context, IBalanceDomainService balanceDomainService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _balanceDomainService = balanceDomainService ?? throw new ArgumentNullException(nameof(balanceDomainService));
        }

        public async Task<Trader> GetTrader(string traderId)
        {
            var result = await _context.Traders
                .Where(t => t.TraderId == traderId)
                .FirstOrDefaultAsync();
            if (result != null)
            {
                await LoadNavigationObject(result);
            }

            return await this.MapAvailableBalance(result);
        }

        public async Task<List<Trader>> GetTraders(int pageIndex, int take, string searchString = "", string sortField = "", string direction = "desc")
        {
            var result = new List<Trader>();

            var traders = _context.Traders
                .AsNoTracking()
                .Include(t => t.TradingCommission)
                .Include(t => t.Balance)
                .Where(t => t.IsReviewed)
                .Select(t => new Trader
                {
                    TraderId = t.TraderId,
                    Email = t.Email,
                    Username = t.Username,
                    FullName = t.FullName,
                    Nickname = t.Nickname,
                    PhoneNumber = t.PhoneNumber,
                    UplineUserId = t.UplineUserId,
                    UplineUserName = t.UplineUserName,
                    UplineFullName = t.UplineFullName,
                    Balance = t.Balance,
                    TradingCommission = t.TradingCommission,
                    IsEnabled = t.IsEnabled,
                    IsReviewed = t.IsReviewed,
                    LastLoginIP = t.LastLoginIP,
                    DateLastLoggedIn = t.DateLastLoggedIn,
                    DateCreated = t.DateCreated
                });

            IQueryable<Trader> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = traders.Where(s =>
                EF.Functions.Like(s.Email, "%" + searchString + "%")
                || EF.Functions.Like(s.Username, "%" + searchString + "%")
                || EF.Functions.Like(s.FullName, "%" + searchString + "%")
                || EF.Functions.Like(s.Nickname, "%" + searchString + "%")
                || s.PhoneNumber == searchString
                );
            }
            else
            {
                searchResult = traders;
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

        public async Task<int> GetTradersTotalCount(string searchString = null)
        {
            var traders = _context.Traders
                .Where(t => t.IsReviewed);

            var count = 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                count = await traders.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.Nickname.Contains(searchString)
                             || m.PhoneNumber == searchString
                             ).CountAsync();
            }
            else
            {
                count = await traders.CountAsync();
            }

            return count;
        }


        public async Task<List<Trader>> GetDownlines(int pageIndex, int take, string traderAgentId, string searchString = "", string sortField = "", string direction = "desc")
        {
            var result = new List<Trader>();

            IQueryable<Trader> traders = null;

            traders = _context.Traders
               .AsNoTracking()
               .Include(t => t.TradingCommission)
               .Include(t => t.Balance)
               .Where(t => t.UplineUserId == traderAgentId && t.IsReviewed)
               .Select(t => new Trader
               {
                   TraderId = t.TraderId,
                   Email = t.Email,
                   Username = t.Username,
                   FullName = t.FullName,
                   Nickname = t.Nickname,
                   PhoneNumber = t.PhoneNumber,
                   UplineUserId = t.UplineUserId,
                   UplineUserName = t.UplineUserName,
                   UplineFullName = t.UplineFullName,
                   Balance = t.Balance,
                   TradingCommission = t.TradingCommission,
                   IsEnabled = t.IsEnabled,
                   IsReviewed = t.IsReviewed,
                   LastLoginIP = t.LastLoginIP,
                   DateLastLoggedIn = t.DateLastLoggedIn,
                   DateCreated = t.DateCreated
               });

            IQueryable<Trader> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = traders.Where(s =>
                EF.Functions.Like(s.Email, "%" + searchString + "%")
                || EF.Functions.Like(s.Username, "%" + searchString + "%")
                || EF.Functions.Like(s.FullName, "%" + searchString + "%")
                || EF.Functions.Like(s.Nickname, "%" + searchString + "%")
                || s.PhoneNumber == searchString
                );
            }
            else
            {
                searchResult = traders;
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

        public async Task<int> GetDownlinesTotalCount(string traderAgentId, string searchString = null)
        {
            IQueryable<Trader> traders = null;

            if (string.IsNullOrEmpty(traderAgentId))
            {
                traders = _context.Traders
                   .Include(t => t.TradingCommission)
                   .Include(t => t.Balance)
                   .ThenInclude(b => b.WithdrawalLimit)
                   .Where(t => t.UplineUserId == null && t.IsReviewed);
            }
            else
            {
                traders = _context.Traders
                   .Include(t => t.TradingCommission)
                   .Include(t => t.Balance)
                   .ThenInclude(b => b.WithdrawalLimit)
                   .Where(t => t.UplineUserId == traderAgentId && t.IsReviewed);
            }

            var count = 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                count = await traders.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.Nickname.Contains(searchString)
                             || m.PhoneNumber == searchString
                             ).CountAsync();
            }
            else
            {
                count = await traders.CountAsync();
            }

            return count;
        }


        public List<Trader> GetPendingReviews(int pageIndex, int take, string uplineId = null, string searchString = "", string sortField = "", string direction = "desc")
        {
            var result = new List<Trader>();

            IQueryable<Trader> traders = null;

            if (!string.IsNullOrEmpty(uplineId))
            {
                traders = _context.Traders
                    .Include(t => t.TradingCommission)
                    .Include(t => t.Balance)
                    .ThenInclude(b => b.WithdrawalLimit)
                    .Where(t => !t.IsReviewed && t.UplineUserId == uplineId);
            }
            else
            {
                traders = _context.Traders
                    .Include(t => t.TradingCommission)
                    .Include(t => t.Balance)
                    .ThenInclude(b => b.WithdrawalLimit)
                    .Where(t => !t.IsReviewed);
            }

            IQueryable<Trader> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = traders.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.Nickname.Contains(searchString)
                             || m.PhoneNumber == searchString
                             );
            }
            else
            {
                searchResult = traders;
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
            IQueryable<Trader> traders = null;

            if (!string.IsNullOrEmpty(uplineId))
            {
                traders = _context.Traders
                    .Include(t => t.TradingCommission)
                    .Include(t => t.Balance)
                    .ThenInclude(b => b.WithdrawalLimit)
                    .Where(t => !t.IsReviewed && t.UplineUserId == uplineId);

            }
            else
            {
                traders = _context.Traders
                    .Include(t => t.TradingCommission)
                    .Include(t => t.Balance)
                    .ThenInclude(b => b.WithdrawalLimit)
                    .Where(t => !t.IsReviewed);
            }

            var count = 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                count = await traders.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.Nickname.Contains(searchString)
                             || m.PhoneNumber == searchString
                             ).CountAsync();
            }
            else
            {
                count = await traders.CountAsync();
            }

            return count;
        }




        private List<Trader> GetSortedRecords(
            IQueryable<Trader> traders,
            int? pageIndex, int? take, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<Trader>();

            if (pageIndex != null && take != null)
            {
                IEnumerable<Trader> sortedResult = null;
                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "DateCreated")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traders
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traders
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "Username")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traders
                               .OrderBy(f => f.Username)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traders
                               .OrderByDescending(f => f.Username)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "Nickname")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traders
                               .OrderBy(f => f.Nickname)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traders
                               .OrderByDescending(f => f.Nickname)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "LastLogin")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traders
                               .OrderBy(f => f.DateLastLoggedIn)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traders
                               .OrderByDescending(f => f.DateLastLoggedIn)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "Status")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traders
                               .OrderBy(f => f.IsEnabled)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traders
                               .OrderByDescending(f => f.IsEnabled)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "UplineUserName")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traders
                               .OrderBy(f => f.UplineUserName)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traders
                               .OrderByDescending(f => f.UplineUserName)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "AmountAvailable")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traders
                                .OrderBy(f => f.Balance.AmountAvailable)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traders
                               .OrderByDescending(f => f.Balance.AmountAvailable)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "AmountFrozen")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traders
                                .OrderBy(f => f.Balance.AmountFrozen)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traders
                               .OrderByDescending(f => f.Balance.AmountFrozen)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "RateAlipayInThousandth")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traders
                                .OrderBy(f => f.TradingCommission.RateAlipayInThousandth)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traders
                               .OrderByDescending(f => f.TradingCommission.RateAlipayInThousandth)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "RateWechatInThousandth")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traders
                                .OrderBy(f => f.TradingCommission.RateWechatInThousandth)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traders
                               .OrderByDescending(f => f.TradingCommission.RateWechatInThousandth)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traders
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traders
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                }
                else
                {
                    sortedResult = traders
                       .OrderByDescending(f => f.DateCreated)
                       .Skip((int)take * (int)pageIndex)
                       .Take((int)take);
                }

                foreach (var trader in sortedResult)
                {
                    result.Add(trader);
                }
            }
            else
            {
                foreach (var trader in traders)
                {
                    result.Add(trader);
                }
            }

            return result;
        }






        public Trader Add(Trader trader)
        {
            return _context.Traders.Add(trader).Entity;
        }

        public void Update(Trader trader)
        {
            _context.Entry(trader).State = EntityState.Modified;
        }

        public void Delete(Trader trader)
        {
            if (trader != null)
            {
                _context.Traders.Remove(trader);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        private async Task LoadNavigationObject(Trader trader)
        {
            if (trader == null)
            {
                throw new ArgumentNullException("The trader must be provided.");
            }
            await _context.Entry(trader)
                .Reference(c => c.Balance).LoadAsync();
            await _context.Entry(trader.Balance)
                .Reference(b => b.WithdrawalLimit).LoadAsync();
            await _context.Entry(trader)
                .Reference(c => c.TradingCommission).LoadAsync();
        }

        private async Task<Trader> MapAvailableBalance(Trader trader)
        {
            var availableBalance = await _balanceDomainService.GetAvailableBalanceAsync(trader.TraderId);

            trader.Balance.AmountAvailable = availableBalance;

            return trader;
        }


        #region Custom Operations

        public async Task UpdateBalanceAsync(string userId, decimal balance)
        {
            var user = await _context.Traders
                .Include(t => t.Balance)
                .Where(u => u.TraderId == userId)
                .Select(u => new
                {
                    u.TraderId,
                    u.Balance
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();


            var toUpdate = new Trader
            {
                TraderId = userId,
                Balance = user.Balance
            };

            _context.Traders.Attach(toUpdate);
            _context.Entry(toUpdate).Reference(b => b.Balance).IsModified = true;

            toUpdate.Balance.AmountAvailable = balance;
        }

        public void UpdateBalance(string userId, decimal balance)
        {
            var user = _context.Traders
                .Include(t => t.Balance)
                .Where(u => u.TraderId == userId)
                .Select(u => new
                {
                    u.TraderId,
                    u.Balance
                })
                .AsNoTracking()
                .FirstOrDefault();


            var toUpdate = new Trader
            {
                TraderId = userId,
                Balance = user.Balance
            };

            _context.Traders.Attach(toUpdate);
            _context.Entry(toUpdate).Reference(b => b.Balance).IsModified = true;

            toUpdate.Balance.AmountAvailable = balance;
        }

        public void UpdateBaseInfo(string userId, string fullName, string nickname,
            string phoneNumber, string email, string wechat, string qq)
        {
            var toUpdate = new Trader
            {
                TraderId = userId
            };

            _context.Traders.Attach(toUpdate);
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
