using Distributing.Domain.Model.Balances;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Balances;
using WebMVC.Data;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.ViewModels;

namespace WebMVC.Applications.Queries.TraderAgents
{
    public class TraderAgentQueries : ITraderAgentQueries
    {
        private readonly ApplicationDbContext _context;
        private readonly IBalanceDomainService _balanceDomainService;

        public TraderAgentQueries(ApplicationDbContext context, IBalanceDomainService balanceDomainService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _balanceDomainService = balanceDomainService ?? throw new ArgumentNullException(nameof(balanceDomainService));
        }

        public async Task<TraderAgent> GetTraderAgent(string traderAgentId)
        {
            var result = await _context.TraderAgents
                .Where(t => t.TraderAgentId == traderAgentId)
                .FirstOrDefaultAsync();
            if (result != null)
            {
                await LoadNavigationObject(result);
            }

            return await this.MapAvailableBalance(result);
        }

        public async Task<List<TraderAgent>> GetTraderAgents(int pageIndex, int take, string searchString = "", string sortField = "", string direction = "desc")
        {
            var result = new List<TraderAgent>();

            var traderAgents = _context.TraderAgents
                .AsNoTracking()
                .Include(t => t.TradingCommission)
                .Include(t => t.Balance)
                .Where(t => t.IsReviewed)
                .Select(t => new TraderAgent
                {
                    TraderAgentId = t.TraderAgentId,
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
                    HasGrantRight = t.HasGrantRight,
                    LastLoginIP = t.LastLoginIP,
                    DateLastLoggedIn = t.DateLastLoggedIn,
                    DateCreated = t.DateCreated
                });

            IQueryable<TraderAgent> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = traderAgents.Where(s =>
                EF.Functions.Like(s.Email, "%" + searchString + "%")
                || EF.Functions.Like(s.Username, "%" + searchString + "%")
                || EF.Functions.Like(s.FullName, "%" + searchString + "%")
                || EF.Functions.Like(s.Nickname, "%" + searchString + "%")
                || s.PhoneNumber == searchString
                );
            }
            else
            {
                searchResult = traderAgents;
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

        public async Task<int> GetTraderAgentsTotalCount(string searchString = null)
        {
            var traderAgents = _context.TraderAgents
                .Where(t => t.IsReviewed);

            var count = 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                count = await traderAgents.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.Nickname.Contains(searchString)
                             || m.PhoneNumber == searchString
                             ).CountAsync();
            }
            else
            {
                count = await traderAgents.CountAsync();
            }

            return count;
        }


        public async Task<List<TraderAgent>> GetDownlines(int pageIndex, int take, string traderAgentId = null, string searchString = "", string sortField = "", string direction = "desc")
        {
            var result = new List<TraderAgent>();

            IQueryable<TraderAgent> traderAgents = null;

            if (string.IsNullOrEmpty(traderAgentId))
            {
                traderAgents = _context.TraderAgents
                   .AsNoTracking()
                   .Include(t => t.TradingCommission)
                   .Include(t => t.Balance)
                   .Where(t => t.UplineUserId == null && t.IsReviewed)
                   .Select(t => new TraderAgent
                   {
                       TraderAgentId = t.TraderAgentId,
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
                       HasGrantRight = t.HasGrantRight,
                       LastLoginIP = t.LastLoginIP,
                       DateLastLoggedIn = t.DateLastLoggedIn,
                       DateCreated = t.DateCreated
                   });
            }
            else
            {
                traderAgents = _context.TraderAgents
                   .AsNoTracking()
                   .Include(t => t.TradingCommission)
                   .Include(t => t.Balance)
                   .Where(t => t.UplineUserId == traderAgentId && t.IsReviewed)
                   .Select(t => new TraderAgent
                   {
                       TraderAgentId = t.TraderAgentId,
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
                       HasGrantRight = t.HasGrantRight,
                       LastLoginIP = t.LastLoginIP,
                       DateLastLoggedIn = t.DateLastLoggedIn,
                       DateCreated = t.DateCreated
                   });
            }

            IQueryable<TraderAgent> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = traderAgents.Where(s =>
                EF.Functions.Like(s.Email, "%" + searchString + "%")
                || EF.Functions.Like(s.Username, "%" + searchString + "%")
                || EF.Functions.Like(s.FullName, "%" + searchString + "%")
                || EF.Functions.Like(s.Nickname, "%" + searchString + "%")
                || s.PhoneNumber == searchString
                );
            }
            else
            {
                searchResult = traderAgents;
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

        public async Task<int> GetDownlinesTotalCount(string traderAgentId = null, string searchString = null)
        {
            IQueryable<TraderAgent> traderAgents = null;

            if (string.IsNullOrEmpty(traderAgentId))
            {
                traderAgents = _context.TraderAgents
                   .Include(t => t.TradingCommission)
                   .Include(t => t.Balance)
                   .ThenInclude(b => b.WithdrawalLimit)
                   .Where(t => t.UplineUserId == null && t.IsReviewed);
            }
            else
            {
                traderAgents = _context.TraderAgents
                   .Include(t => t.TradingCommission)
                   .Include(t => t.Balance)
                   .ThenInclude(b => b.WithdrawalLimit)
                   .Where(t => t.UplineUserId == traderAgentId && t.IsReviewed);
            }

            var count = 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                count = await traderAgents.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.Nickname.Contains(searchString)
                             || m.PhoneNumber == searchString
                             ).CountAsync();
            }
            else
            {
                count = await traderAgents.CountAsync();
            }

            return count;
        }


        public List<TraderAgent> GetPendingReviews(int pageIndex, int take, string uplineId = null, string searchString = "", string sortField = "", string direction = "desc")
        {
            var result = new List<TraderAgent>();

            IQueryable<TraderAgent> traderAgents = null;

            if (!string.IsNullOrEmpty(uplineId))
            {
                traderAgents = _context.TraderAgents
                    .Include(t => t.TradingCommission)
                    .Include(t => t.Balance)
                    .ThenInclude(b => b.WithdrawalLimit)
                    .Where(t => !t.IsReviewed && t.UplineUserId == uplineId);

            }
            else
            {
                traderAgents = _context.TraderAgents
                    .Include(t => t.TradingCommission)
                    .Include(t => t.Balance)
                    .ThenInclude(b => b.WithdrawalLimit)
                    .Where(t => !t.IsReviewed);
            }

            IQueryable<TraderAgent> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = traderAgents.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.Nickname.Contains(searchString)
                             || m.PhoneNumber == searchString
                             );
            }
            else
            {
                searchResult = traderAgents;
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
            IQueryable<TraderAgent> traderAgents = null;

            if (!string.IsNullOrEmpty(uplineId))
            {
                traderAgents = _context.TraderAgents
                    .Include(t => t.TradingCommission)
                    .Include(t => t.Balance)
                    .ThenInclude(b => b.WithdrawalLimit)
                    .Where(t => !t.IsReviewed && t.UplineUserId == uplineId);

            }
            else
            {
                traderAgents = _context.TraderAgents
                    .Include(t => t.TradingCommission)
                    .Include(t => t.Balance)
                    .ThenInclude(b => b.WithdrawalLimit)
                    .Where(t => !t.IsReviewed);
            }

            var count = 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                count = await traderAgents.Where(m =>
                             m.Email.Contains(searchString)
                             || m.Username.Contains(searchString)
                             || m.FullName.Contains(searchString)
                             || m.Nickname.Contains(searchString)
                             || m.PhoneNumber == searchString
                             ).CountAsync();
            }
            else
            {
                count = await traderAgents.CountAsync();
            }

            return count;
        }




        private List<TraderAgent> GetSortedRecords(
            IQueryable<TraderAgent> traderAgents,
            int? pageIndex, int? take, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<TraderAgent>();

            if (pageIndex != null && take != null)
            {
                IEnumerable<TraderAgent> sortedResult = null;
                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "DateCreated")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traderAgents
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traderAgents
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "Username")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traderAgents
                               .OrderBy(f => f.Username)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traderAgents
                               .OrderByDescending(f => f.Username)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "Nickname")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traderAgents
                               .OrderBy(f => f.Nickname)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traderAgents
                               .OrderByDescending(f => f.Nickname)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "LastLogin")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traderAgents
                               .OrderBy(f => f.DateLastLoggedIn)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traderAgents
                               .OrderByDescending(f => f.DateLastLoggedIn)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "Status")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traderAgents
                               .OrderBy(f => f.IsEnabled)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traderAgents
                               .OrderByDescending(f => f.IsEnabled)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "UplineUserName")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traderAgents
                               .OrderBy(f => f.UplineUserName)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traderAgents
                               .OrderByDescending(f => f.UplineUserName)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "AmountAvailable")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traderAgents
                                .OrderBy(f => f.Balance.AmountAvailable)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traderAgents
                               .OrderByDescending(f => f.Balance.AmountAvailable)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "AmountFrozen")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traderAgents
                                .OrderBy(f => f.Balance.AmountFrozen)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traderAgents
                               .OrderByDescending(f => f.Balance.AmountFrozen)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "RateAlipayInThousandth")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traderAgents
                                .OrderBy(f => f.TradingCommission.RateAlipayInThousandth)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traderAgents
                               .OrderByDescending(f => f.TradingCommission.RateAlipayInThousandth)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "RateWechatInThousandth")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traderAgents
                                .OrderBy(f => f.TradingCommission.RateWechatInThousandth)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traderAgents
                               .OrderByDescending(f => f.TradingCommission.RateWechatInThousandth)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "HasGrantRight")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traderAgents
                                .OrderBy(f => f.HasGrantRight)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traderAgents
                               .OrderByDescending(f => f.HasGrantRight)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = traderAgents
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = traderAgents
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                }
                else
                {
                    sortedResult = traderAgents
                       .OrderByDescending(f => f.DateCreated)
                       .Skip((int)take * (int)pageIndex)
                       .Take((int)take);
                }

                foreach (var traderAgent in sortedResult)
                {
                    result.Add(traderAgent);
                }
            }
            else
            {
                foreach (var traderAgent in traderAgents)
                {
                    result.Add(traderAgent);
                }
            }

            return result;
        }


        public TraderAgent Add(TraderAgent traderAgent)
        {
            return _context.TraderAgents.Add(traderAgent).Entity;
        }

        public void Update(TraderAgent traderAgent)
        {
            _context.Entry(traderAgent).State = EntityState.Modified;
        }

        public void Delete(TraderAgent traderAgent)
        {
            if (traderAgent != null)
            {
                _context.TraderAgents.Remove(traderAgent);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


        private async Task LoadNavigationObject(TraderAgent traderAgent)
        {
            if (traderAgent == null)
            {
                throw new ArgumentNullException("The trader agent must be provided.");
            }
            await _context.Entry(traderAgent)
                .Reference(c => c.Balance).LoadAsync();
            await _context.Entry(traderAgent.Balance)
                .Reference(b => b.WithdrawalLimit).LoadAsync();
            await _context.Entry(traderAgent)
                .Reference(c => c.TradingCommission).LoadAsync();
        }

        private async Task<TraderAgent> MapAvailableBalance(TraderAgent traderAgent)
        {
            var availableBalance = await _balanceDomainService.GetAvailableBalanceAsync(traderAgent.TraderAgentId);

            traderAgent.Balance.AmountAvailable = availableBalance;

            return traderAgent;
        }


        #region Custom Operation
        public async Task UpdateBalanceAsync(string userId, decimal balance)
        {
            var user = await _context.TraderAgents
                .Include(t => t.Balance)
                .Where(u => u.TraderAgentId == userId)
                .Select(u => new
                {
                    u.TraderAgentId,
                    u.Balance
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var toUpdate = new TraderAgent
            {
                TraderAgentId = userId,
                Balance = user.Balance
            };

            _context.TraderAgents.Attach(toUpdate);
            _context.Entry(toUpdate).Reference(b => b.Balance).IsModified = true;

            toUpdate.Balance.AmountAvailable = balance;
        }

        public void UpdateBalance(string userId, decimal balance)
        {
            var user = _context.TraderAgents
                .Include(t => t.Balance)
                .Where(u => u.TraderAgentId == userId)
                .Select(u => new
                {
                    u.TraderAgentId,
                    u.Balance
                })
                .AsNoTracking()
                .FirstOrDefault();

            var toUpdate = new TraderAgent
            {
                TraderAgentId = userId,
                Balance = user.Balance
            };

            _context.TraderAgents.Attach(toUpdate);
            _context.Entry(toUpdate).Reference(b => b.Balance).IsModified = true;

            toUpdate.Balance.AmountAvailable = balance;
        }

        public void UpdateBaseInfo(string userId, string fullName, string nickname,
            string phoneNumber, string email, string wechat, string qq)
        {
            var toUpdate = new TraderAgent
            {
                TraderAgentId = userId
            };

            _context.TraderAgents.Attach(toUpdate);
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
