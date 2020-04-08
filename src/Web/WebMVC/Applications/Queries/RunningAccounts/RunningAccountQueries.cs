using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Distributions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.CacheServices;
using WebMVC.Data;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;

namespace WebMVC.Applications.Queries.RunningAccounts
{
    public class RunningAccountQueries : IRunningAccountQueries
    {
        private readonly ApplicationDbContext _applicationContext;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserCacheService _userCacheService;

        public RunningAccountQueries(ApplicationDbContext applicationContext, UserManager<ApplicationUser> userManager, IUserCacheService userCacheService)
        {
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userCacheService = userCacheService ?? throw new ArgumentNullException(nameof(userCacheService));
        }

        public async Task<RunningAccountRecord> GetRunningAccountRecordAsync(int runningAccountRecordId)
        {
            return await _applicationContext.RunningAccountRecords.Where(w => w.Id == runningAccountRecordId).FirstOrDefaultAsync();
        }

        public async Task<RunningAccountRecord> GetRunningAccountRecordByUserIdAndTrackingNumberAsync(string userId, string trackingNumber)
        {
            return await _applicationContext.RunningAccountRecords.Where(w => w.UserId == userId && w.OrderTrackingNumber == trackingNumber).FirstOrDefaultAsync();
        }


        public async Task<List<RunningAccountRecord>> GetRunningAccountRecordsAsync(int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null, string status = null,
            string direction = SortDirections.Descending)
        {
            var result = new List<RunningAccountRecord>();

            IQueryable<RunningAccountRecord> runningAccountRecords = null;

            if (from != null && to != null)
            {
                runningAccountRecords = _applicationContext.RunningAccountRecords
                    .AsNoTracking()
                    .Where(q => q.Status.Contains(status ?? string.Empty)
                    && q.DateCreated >= from
                    && q.DateCreated <= to
                    );
            }
            else
            {
                runningAccountRecords = _applicationContext.RunningAccountRecords
                    .AsNoTracking()
                    .Where(q => q.Status.Contains(status ?? string.Empty)
                    );
            }

            IQueryable<RunningAccountRecord> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = runningAccountRecords
                    .Where(w => w.Id.ToString() == searchString
                    || w.UserId == searchString
                    || w.OrderTrackingNumber == searchString
                    || w.ShopUserName.Contains(searchString)
                    || w.ShopFullName.Contains(searchString)
                    || w.DownlineUserName.Contains(searchString)
                    || w.DownlineFullName.Contains(searchString)
                    || w.TraderUserName.Contains(searchString)
                    || w.TraderFullName.Contains(searchString)
                    );
            }
            else
            {
                searchResult = runningAccountRecords;
            }

            result = await this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            return result;
        }

        public async Task<RunningAccountRecordSumData> GetRunningAccountRecordsTotalSumDataAsync(string searchString = null,
            DateTime? from = null, DateTime? to = null, string status = null)
        {
            RunningAccountRecordSumData result = new RunningAccountRecordSumData
            {
                TotalCount = 0,
                TotalSuccessOrderAmount = 0,
                TotalCommissionAmount = 0
            };

            IQueryable<RunningAccountRecord> runningAccountRecords = null;

            if (from != null && to != null)
            {
                runningAccountRecords = _applicationContext.RunningAccountRecords
                    .Where(q => q.Status.Contains(status ?? string.Empty)
                    && q.DateCreated >= from
                    && q.DateCreated <= to
                    );
            }
            else
            {
                runningAccountRecords = _applicationContext.RunningAccountRecords
                    .Where(q => q.Status.Contains(status ?? string.Empty)
                    );
            }


            if (!string.IsNullOrEmpty(searchString))
            {
                runningAccountRecords = runningAccountRecords
                    .Where(w => w.Id.ToString() == searchString
                    || w.UserId == searchString
                    || w.OrderTrackingNumber == searchString
                    || w.ShopUserName.Contains(searchString)
                    || w.ShopFullName.Contains(searchString)
                    || w.TraderUserName.Contains(searchString)
                    || w.TraderFullName.Contains(searchString)
                    );
            }

            //Build sum data.
            var sumDatas = runningAccountRecords
                .GroupBy(p => p.Status)
                .Select(r => new
                {
                    Status = r.Key,
                    TotalCount = r.Count(),
                    TotalSuccessOrderAmount = r.Sum(a => a.Amount),
                    TotalCommissionAmount = r.Sum(a => a.CommissionAmount)
                });

            //Get success data.
            var successData = await sumDatas.Where(s => s.Status == "Success").FirstOrDefaultAsync();

            if (successData != null)
            {
                result.TotalCount = await runningAccountRecords.CountAsync();
                result.TotalSuccessOrderAmount = successData.TotalSuccessOrderAmount;
                result.TotalCommissionAmount = successData.TotalCommissionAmount;
            }

            return result;
        }


        public async Task<List<RunningAccountRecord>> GetRunningAccountRecordsByUserIdAsync(string userId, int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null, string status = null,
            string direction = SortDirections.Descending)
        {
            var result = new List<RunningAccountRecord>();

            IQueryable<RunningAccountRecord> runningAccountRecords = null;

            if (from != null && to != null)
            {
                runningAccountRecords = _applicationContext.RunningAccountRecords
                    .AsNoTracking()
                    .Where(q => q.UserId == userId
                    && q.Status.Contains(status ?? string.Empty)
                    && q.DateCreated >= from
                    && q.DateCreated <= to
                    );
            }
            else
            {
                runningAccountRecords = _applicationContext.RunningAccountRecords
                    .AsNoTracking()
                    .Where(q => q.UserId == userId
                    && q.Status.Contains(status ?? string.Empty)
                    );
            }
            IQueryable<RunningAccountRecord> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = runningAccountRecords
                    .Where(w => w.Id.ToString() == searchString
                    || w.UserId == searchString
                    || w.OrderTrackingNumber == searchString
                    || w.ShopUserName.Contains(searchString)
                    || w.ShopFullName.Contains(searchString)
                    || w.TraderUserName.Contains(searchString)
                    || w.TraderFullName.Contains(searchString)
                    );
            }
            else
            {
                searchResult = runningAccountRecords;
            }

            result = await this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            return result;
        }

        public async Task<RunningAccountRecordSumData> GetRunningAccountRecordsTotalSumDataByUserIdAsync(string userId, string searchString = null,
            DateTime? from = null, DateTime? to = null, string status = null)
        {
            RunningAccountRecordSumData result = new RunningAccountRecordSumData
            {
                TotalCount = 0,
                TotalSuccessOrderAmount = 0,
                TotalCommissionAmount = 0
            };

            IQueryable<RunningAccountRecord> runningAccountRecords = null;

            if (from != null && to != null)
            {
                runningAccountRecords = _applicationContext.RunningAccountRecords
                    .AsNoTracking()
                    .Where(q => q.UserId == userId
                    && q.Status.Contains(status ?? string.Empty)
                    && q.DateCreated >= from
                    && q.DateCreated <= to
                    );
            }
            else
            {
                runningAccountRecords = _applicationContext.RunningAccountRecords
                    .AsNoTracking()
                    .Where(q => q.UserId == userId
                    && q.Status.Contains(status ?? string.Empty)
                    );
            }


            if (!string.IsNullOrEmpty(searchString))
            {
                runningAccountRecords = runningAccountRecords
                    .Where(w => w.Id.ToString() == searchString
                    || w.UserId == searchString
                    || w.OrderTrackingNumber == searchString
                    || w.ShopUserName.Contains(searchString)
                    || w.ShopFullName.Contains(searchString)
                    || w.TraderUserName.Contains(searchString)
                    || w.TraderFullName.Contains(searchString)
                    );
            }

            //Build sum data.
            /*var sumDatas = runningAccountRecords
                .GroupBy(p => p.Status)
                .Select(r => new
                {
                    Status = r.Key,
                    TotalCount = r.Count(),
                    TotalSuccessOrderAmount = r.Sum(a => a.Amount),
                    TotalCommissionAmount = r.Sum(a => a.CommissionAmount)
                });

            //Get success data.
            var successData = await sumDatas.Where(s => s.Status == "Success").FirstOrDefaultAsync();

            if (successData != null)
            {
              
            }*/
            result.TotalCount = await runningAccountRecords.CountAsync();
            result.TotalSuccessOrderAmount = await runningAccountRecords.SumAsync(r => r.Status == "Success" ? r.Amount : 0M);
            result.TotalCommissionAmount = (decimal)await runningAccountRecords.SumAsync(r => r.DistributedAmount);
            return result;
        }

        public async Task<RunningAccountRecordsStatistic> GetRunningAccountRecordsStatisticAsync(string userId, string userRole, string searchString = null,
            DateTime? from = null, DateTime? to = null)
        {
            RunningAccountRecordsStatistic result = new RunningAccountRecordsStatistic
            {
                TotalProfit = 0,

                OrderTotalCount = 0,
                OrderTotalSuccessCount = 0,
                OrderTotalAmount = 0,
                OrderTotalSuccessAmount = 0,
                OrderDailyAverageSuccessAmount = 0,
                OrderAverageRevenueRate = 0,
                OrderSuccessRate = 0,

                OrderChartData = new Dictionary<string, decimal>(),
                ShopRankData = new Dictionary<string, decimal>(),
                TraderRankData = new Dictionary<string, decimal>()
            };

            IQueryable<RunningAccountRecord> runningAccountRecords = null;

            if (from != null && to != null)
            {
                runningAccountRecords = _applicationContext.RunningAccountRecords
                    .Where(o => o.UserId == userId && o.DateCreated >= from && o.DateCreated <= to);
            }
            else
            {
                from = DateTime.UtcNow.AddDays(-1);
                to = DateTime.UtcNow;
                runningAccountRecords = _applicationContext.RunningAccountRecords
                    .Where(o => o.UserId == userId && o.DateCreated >= from && o.DateCreated <= to);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                runningAccountRecords = runningAccountRecords
                    .Where(o => o.TraderUserName == searchString
                    || o.TraderFullName == searchString
                    || o.ShopUserName == searchString
                    || o.ShopFullName == searchString
                    );
            }

            //Build sum data.
            //Get each payment scheme's sum data.
            if (userRole == Roles.ShopAgent)
            {
                var statistics = runningAccountRecords
                    .GroupBy(p => p.TraderUserName)
                    .Select(r => new
                    {
                        IsTestOrder = r.Key,
                        TotalProfit = r.Sum(a => a.DistributedAmount),


                        OrderTotalCount = r.Count(),
                        OrderTotalSuccessCount = r.Sum(a => a.Status == "Success" ? 1 : 0),
                        OrderTotalAmount = r.Sum(a => a.Amount),
                        OrderTotalSuccessAmount = r.Sum(a => a.Status == "Success" ? a.Amount : 0M),

                        OrderAverageRevenueRate = r.Average(a => a.DistributedAmount / a.Amount),
                    });

                if (await statistics.AnyAsync())
                {
                    result.TotalProfit = statistics.Sum(s => s.TotalProfit ?? 0M);
                    var orderTotalCount = statistics.Sum(s => s.OrderTotalCount);
                    result.OrderTotalCount = orderTotalCount;
                    result.OrderTotalSuccessCount = statistics.Sum(s => s.OrderTotalSuccessCount);
                    result.OrderTotalAmount = statistics.Sum(s => s.OrderTotalAmount);
                    result.OrderTotalSuccessAmount = statistics.Sum(s => s.OrderTotalSuccessAmount);
                    result.OrderAverageRevenueRate = statistics.Sum(s => s.OrderAverageRevenueRate ?? 0M);

                    var orderTotalSuccessCount = statistics.Sum(s => s.OrderTotalCount);
                    result.OrderSuccessRate = orderTotalSuccessCount == 0 ?
                        0 : (decimal)orderTotalSuccessCount / (decimal)orderTotalCount;
                }


                //Order Chart Data
                var orderChartData = runningAccountRecords
                    .GroupBy(x => new
                    {
                        x.DateCreated.Hour
                    })
                    .Select(r => new
                    {
                        Key = r.Key,
                        OrderTotalAmount = r.Sum(a => a.Amount)
                    });

                foreach (var hourData in orderChartData)
                {
                    result.OrderChartData.Add(hourData.Key.Hour.ToString(), hourData.OrderTotalAmount);
                }

                //Shop Rank Data
                var shopRankData = runningAccountRecords
                    .GroupBy(x => x.ShopFullName)
                    .Select(r => new
                    {
                        Key = r.Key,
                        OrderTotalSuccessAmount = r.Sum(a => a.Status == "Success" ? a.Amount : 0M)
                    })
                    .OrderByDescending(o => o.OrderTotalSuccessAmount)
                    .Take(5);

                foreach (var rankData in shopRankData)
                {
                    result.ShopRankData.Add(rankData.Key.ToString(), rankData.OrderTotalSuccessAmount);
                }

                return result;
            }
            else if (userRole == Roles.TraderAgent)
            {
                var statistics = runningAccountRecords
                    .GroupBy(p => p.TraderUserName)
                    .Select(r => new
                    {
                        IsTestOrder = r.Key,
                        TotalProfit = r.Sum(a => a.DistributedAmount),


                        OrderTotalCount = r.Count(),
                        OrderTotalSuccessCount = r.Sum(a => a.Status == "Success" ? 1 : 0),
                        OrderTotalAmount = r.Sum(a => a.Amount),
                        OrderTotalSuccessAmount = r.Sum(a => a.Status == "Success" ? a.Amount : 0M),

                        OrderAverageRevenueRate = r.Average(a => a.DistributedAmount / a.Amount),
                    });

                if (await statistics.AnyAsync())
                {
                    result.TotalProfit = statistics.Sum(s => s.TotalProfit ?? 0M);
                    var orderTotalCount = statistics.Sum(s => s.OrderTotalCount);
                    result.OrderTotalCount = orderTotalCount;
                    result.OrderTotalSuccessCount = statistics.Sum(s => s.OrderTotalSuccessCount);
                    result.OrderTotalAmount = statistics.Sum(s => s.OrderTotalAmount);
                    result.OrderTotalSuccessAmount = statistics.Sum(s => s.OrderTotalSuccessAmount);
                    result.OrderAverageRevenueRate = statistics.Sum(s => s.OrderAverageRevenueRate ?? 0M);

                    var orderTotalSuccessCount = statistics.Sum(s => s.OrderTotalCount);
                    result.OrderSuccessRate = orderTotalSuccessCount == 0 ?
                        0 : (decimal)orderTotalSuccessCount / (decimal)orderTotalCount;
                }


                //Order Chart Data
                var orderChartData = runningAccountRecords
                    .GroupBy(x => new
                    {
                        x.DateCreated.Hour
                    })
                    .Select(r => new
                    {
                        Key = r.Key,
                        OrderTotalAmount = r.Sum(a => a.Amount)
                    });

                foreach (var hourData in orderChartData)
                {
                    result.OrderChartData.Add(hourData.Key.Hour.ToString(), hourData.OrderTotalAmount);
                }


                //Trader Rank Data
                var traderRankData = runningAccountRecords
                    .GroupBy(x => x.TraderFullName)
                    .Select(r => new
                    {
                        Key = r.Key,
                        OrderTotalSuccessAmount = r.Sum(a => a.Status == "Success" ? a.Amount : 0M)
                    })
                    .OrderByDescending(o => o.OrderTotalSuccessAmount)
                    .Take(5);

                foreach (var rankData in traderRankData)
                {
                    if (!string.IsNullOrEmpty(rankData.Key))
                    {
                        result.TraderRankData.Add(rankData.Key.ToString(), rankData.OrderTotalSuccessAmount);
                    }
                }

                return result;
            }
            else
            {
                var statistics = runningAccountRecords
                  .GroupBy(p => p.Status)
                  .Select(r => new
                  {
                      IsTestOrder = r.Key,
                      TotalProfit = r.Sum(a => a.DistributedAmount),


                      OrderTotalCount = r.Count(),
                      OrderTotalSuccessCount = r.Sum(a => a.Status == "Success" ? 1 : 0),
                      OrderTotalAmount = r.Sum(a => a.Amount),
                      OrderTotalSuccessAmount = r.Sum(a => a.Status == "Success" ? a.Amount : 0M),

                      OrderAverageRevenueRate = r.Average(a => a.DistributedAmount / a.Amount),
                  });

                if (await statistics.AnyAsync())
                {
                    result.TotalProfit = statistics.Sum(s => s.TotalProfit ?? 0M);
                    var orderTotalCount = statistics.Sum(s => s.OrderTotalCount);
                    result.OrderTotalCount = orderTotalCount;
                    result.OrderTotalSuccessCount = statistics.Sum(s => s.OrderTotalSuccessCount);
                    result.OrderTotalAmount = statistics.Sum(s => s.OrderTotalAmount);
                    result.OrderTotalSuccessAmount = statistics.Sum(s => s.OrderTotalSuccessAmount);
                    result.OrderAverageRevenueRate = statistics.Sum(s => s.OrderAverageRevenueRate ?? 0M);

                    var orderTotalSuccessCount = statistics.Sum(s => s.OrderTotalCount);
                    result.OrderSuccessRate = orderTotalSuccessCount == 0 ?
                        0 : (decimal)orderTotalSuccessCount / (decimal)orderTotalCount;
                }


                //Order Chart Data
                var orderChartData = runningAccountRecords
                    .GroupBy(x => new
                    {
                        x.DateCreated.Hour
                    })
                    .Select(r => new
                    {
                        Key = r.Key,
                        OrderTotalAmount = r.Sum(a => a.Amount)
                    });

                foreach (var hourData in orderChartData)
                {
                    result.OrderChartData.Add(hourData.Key.Hour.ToString(), hourData.OrderTotalAmount);
                }

                return result;
            }
        }

        private async Task<List<RunningAccountRecord>> GetSortedRecords(
            IQueryable<RunningAccountRecord> runningAccountRecords,
            int? pageIndex, int? take, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<RunningAccountRecord>();

            if (pageIndex != null && take != null)
            {
                var skip = (int)take * (int)pageIndex;

                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "DateCreated")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await runningAccountRecords
                               .OrderBy(f => f.DateCreated)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await runningAccountRecords
                               .OrderByDescending(f => f.DateCreated)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "RunningAccountRecordId")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await runningAccountRecords
                               .OrderBy(f => f.Id)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await runningAccountRecords
                               .OrderByDescending(f => f.Id)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "OrderTrackingNumber")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await runningAccountRecords
                               .OrderBy(f => f.OrderTrackingNumber)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await runningAccountRecords
                               .OrderByDescending(f => f.OrderTrackingNumber)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "Status")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await runningAccountRecords
                               .OrderBy(f => f.Status)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await runningAccountRecords
                               .OrderByDescending(f => f.Status)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "Amount")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await runningAccountRecords
                               .OrderBy(f => f.Amount)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await runningAccountRecords
                               .OrderByDescending(f => f.Amount)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "CommissionAmount")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await runningAccountRecords
                               .OrderBy(f => f.CommissionAmount)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await runningAccountRecords
                               .OrderByDescending(f => f.CommissionAmount)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "DistributedAmount")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await runningAccountRecords
                                .OrderBy(f => f.DistributedAmount)
                                .Skip(skip)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await runningAccountRecords
                               .OrderByDescending(f => f.DistributedAmount)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "ShopUserName")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await runningAccountRecords
                                .OrderBy(f => f.ShopUserName)
                                .Skip(skip)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await runningAccountRecords
                               .OrderByDescending(f => f.ShopUserName)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "TraderUserName")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await runningAccountRecords
                                .OrderBy(f => f.TraderUserName)
                                .Skip(skip)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await runningAccountRecords
                               .OrderByDescending(f => f.TraderUserName)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await runningAccountRecords
                               .OrderBy(f => f.DateCreated)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await runningAccountRecords
                               .OrderByDescending(f => f.DateCreated)
                               .Skip(skip)
                               .Take((int)take).ToListAsync();
                        }
                    }
                }
                else
                {
                    result = await runningAccountRecords
                       .OrderByDescending(f => f.DateCreated)
                       .Skip(skip)
                       .Take((int)take)
                       .ToListAsync();
                }
            }
            else
            {
                result = await runningAccountRecords.ToListAsync();
            }

            return result;
        }



        public async Task<RunningAccountRecord> MapFromEntity(
            Distributing.Domain.Model.Commissions.Commission entity,
            Distributing.Domain.Model.Distributions.Order order,
            string downlineUserId)
        {
            //Check the entity and user is not null.
            if (entity is null)
            {
                throw new ArgumentNullException("The qr code entity must be provided.");
            }

            //Check the user is exist.
            var user = await _userManager.Users
                .Where(u => u.Id == entity.UserId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName,
                    UplineId = u.UplineId ?? null
                })
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (user == null)
            {
                throw new KeyNotFoundException("找无用户。");
            }

            //get downline info.
            var downline = await _userManager.Users
                .Where(u => u.Id == downlineUserId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName
                })
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);


            //Check the shop is exist.
            var shop = await _userManager.Users
                .Where(u => u.Id == order.ShopId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName
                })
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (shop == null)
            {
                throw new KeyNotFoundException("找无商户。");
            }


            //Check the trader is exist.
            var trader = await _userManager.Users
                .Where(u => u.Id == order.TraderId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName
                })
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (trader == null)
            {
                throw new KeyNotFoundException("找无交易员。");
            }

            //Build view model.
            var runningAccountRecordVM = new RunningAccountRecord
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserFullName = user.FullName,

                //Set downline data if user has downline.
                DownlineUserId = downline?.Id,
                DownlineUserName = downline?.UserName,
                DownlineFullName = downline?.FullName,

                OrderTrackingNumber = order.TrackingNumber,
                ShopOrderId = order.ShopOrderId,

                //The running account record is created by domain event handler 
                //which happend when an order is paired.
                Status = "AwaitingPayment",

                Amount = order.Amount,

                //The commission amount is the expected amount distributed to the user.
                CommissionAmount = order.CommissionAmount,

                ShopId = shop.Id,
                ShopUserName = shop.UserName,
                ShopFullName = shop.FullName,

                TraderId = trader.Id,
                TraderUserName = trader.UserName,
                TraderFullName = trader.FullName,
                DateCreated = order.DateCreated
            };

            return runningAccountRecordVM;
        }


        public async Task<RunningAccountRecord> MapFromEntity(
            Distribution entity,
            Distributing.Domain.Model.Distributions.Order order,
            string downlineUserId)
        {
            //Check the entity and user is not null.
            if (entity is null)
            {
                throw new ArgumentNullException("The distribution entity must be provided.");
            }

            //Check the user is exist.
            var user = await _userManager.Users
                .Where(u => u.Id == entity.UserId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName,
                    UplineId = u.UplineId ?? null
                })
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (user == null)
            {
                throw new KeyNotFoundException("找无用户。");
            }

            //get downline info.
            var downline = await _userManager.Users
                .Where(u => u.Id == downlineUserId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName
                })
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);


            //Check the shop is exist.
            var shop = await _userManager.Users
                .Where(u => u.Id == order.ShopId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName
                })
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (shop == null)
            {
                throw new KeyNotFoundException("找无商户。");
            }


            //Check the trader is exist.
            var trader = await _userManager.Users
                .Where(u => u.Id == order.TraderId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName
                })
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            if (trader == null)
            {
                throw new KeyNotFoundException("找无交易员。");
            }

            //Build view model.
            var runningAccountRecordVM = new RunningAccountRecord
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserFullName = user.FullName,

                //Set downline data if user has downline.
                DownlineUserId = downline?.Id,
                DownlineUserName = downline?.UserName,
                DownlineFullName = downline?.FullName,

                OrderTrackingNumber = order.TrackingNumber,
                ShopOrderId = order.ShopOrderId,

                //The running account record is created by domain event handler 
                //which happend when an order is paired.
                Status = "AwaitingPayment",

                Amount = order.Amount,

                //The commission amount is the expected amount distributed to the user.
                CommissionAmount = order.CommissionAmount,

                ShopId = shop.Id,
                ShopUserName = shop.UserName,
                ShopFullName = shop.FullName,

                TraderId = trader.Id,
                TraderUserName = trader.UserName,
                TraderFullName = trader.FullName,
                DateCreated = order.DateCreated
            };

            return runningAccountRecordVM;
        }

        public async Task CreateRunningRecordsFrom(Ordering.Domain.Model.Orders.Order orderEntity)
        {
            var shopInfo = orderEntity.ShopInfo;
            var payeeInfo = orderEntity.PayeeInfo;

            var shopUser = await _userManager.Users
                .Where(u => u.Id == shopInfo.ShopId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName,
                    u.UplineId
                })
                .FirstOrDefaultAsync();

            var traderUser = await _userManager.Users
                .Where(u => u.Id == payeeInfo.TraderId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName,
                    u.UplineId
                })
                .FirstOrDefaultAsync();

            #region Create running account shop
            if (shopUser == null)
            {
                throw new KeyNotFoundException("No shop user found by given shop id. At DistributionService.OrderCreated");
            }

            //Add new running account record .
            var createdRunningAccountRecordForShop = _applicationContext.RunningAccountRecords.Add(this.MapFromEntity(
                orderEntity,
                shopInfo.ShopOrderId,
                shopUser.Id,
                shopUser.UserName,
                shopUser.FullName,
                shopInfo.ShopId,
                shopUser.UserName,
                shopUser.FullName,
                traderUser.Id,
                traderUser.UserName,
                traderUser.FullName
                )).Entity;

            //Create temp running account record.
            this.AddTemp(
                new TempRunningAccountRecord
                {
                    Id = createdRunningAccountRecordForShop.Id,
                    UserId = createdRunningAccountRecordForShop.UserId,
                    OrderTrackingNumber = createdRunningAccountRecordForShop.OrderTrackingNumber
                });
            #endregion


            #region Create running account shop agents

            var uplineShopAgentUser = await _userManager.Users
                .Where(u => u.Id == shopUser.UplineId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName,
                    u.UplineId
                })
                .FirstOrDefaultAsync();

            //For UI Search Purpose.
            var downlineUser = shopUser;

            while (true)
            {
                if (uplineShopAgentUser == null)
                {
                    break;
                }

                var currentUser = uplineShopAgentUser;

                //Add new running account record .
                var createdRunningAcountRecordForCurrentUser = _applicationContext.RunningAccountRecords.Add(this.MapFromEntity(
                    orderEntity,
                    shopInfo.ShopOrderId,
                    currentUser.Id,
                    currentUser.UserName,
                    currentUser.FullName,
                    shopInfo.ShopId,
                    shopUser.UserName,
                    shopUser.FullName,
                    traderUser.Id,
                    traderUser.UserName,
                    traderUser.FullName,
                    downlineUser.Id,
                    downlineUser.UserName,
                    downlineUser.FullName
                    )).Entity;

                //Create temp running account record.
                this.AddTemp(
                    new TempRunningAccountRecord
                    {
                        Id = createdRunningAcountRecordForCurrentUser.Id,
                        UserId = createdRunningAcountRecordForCurrentUser.UserId,
                        OrderTrackingNumber = createdRunningAcountRecordForCurrentUser.OrderTrackingNumber
                    });

                downlineUser = currentUser;

                uplineShopAgentUser = await _userManager.Users
                    .Where(u => u.Id == currentUser.UplineId)
                    .Select(u => new
                    {
                        u.Id,
                        u.UserName,
                        u.FullName,
                        u.UplineId
                    })
                    .FirstOrDefaultAsync();
            }

            #endregion


            #region Create running account record for trader

            if (shopUser == null)
            {
                throw new KeyNotFoundException("No shop user found by given shop id. At DistributionService.OrderCreated");
            }

            var createdRunningAccountRecordForTrader = _applicationContext.RunningAccountRecords.Add(this.MapFromEntity(
                orderEntity,
                shopInfo.ShopOrderId,
                traderUser.Id,
                traderUser.UserName,
                traderUser.FullName,
                shopInfo.ShopId,
                shopUser.UserName,
                shopUser.FullName,
                traderUser.Id,
                traderUser.UserName,
                traderUser.FullName
                )).Entity;

            //Create temp running account record.
            this.AddTemp(
                new TempRunningAccountRecord
                {
                    Id = createdRunningAccountRecordForTrader.Id,
                    UserId = createdRunningAccountRecordForTrader.UserId,
                    OrderTrackingNumber = createdRunningAccountRecordForTrader.OrderTrackingNumber
                });
            #endregion


            #region Create running account record for trader agents

            var uplineTraderAgentUser = await _userManager.Users
                .Where(u => u.Id == traderUser.UplineId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName,
                    u.UplineId
                })
                .FirstOrDefaultAsync();

            downlineUser = traderUser;

            while (true)
            {
                if (uplineTraderAgentUser == null)
                {
                    break;
                }

                var currentUser = uplineTraderAgentUser;

                //Add new running account record .
                var createdRunningAcountRecordForCurrentUser = _applicationContext.RunningAccountRecords.Add(this.MapFromEntity(
                    orderEntity,
                    shopInfo.ShopOrderId,
                    currentUser.Id,
                    currentUser.UserName,
                    currentUser.FullName,
                    shopInfo.ShopId,
                    shopUser.UserName,
                    shopUser.FullName,
                    traderUser.Id,
                    traderUser.UserName,
                    traderUser.FullName,
                    downlineUser.Id,
                    downlineUser.UserName,
                    downlineUser.FullName
                    )).Entity;

                //Create temp running account record.
                this.AddTemp(
                    new TempRunningAccountRecord
                    {
                        Id = createdRunningAcountRecordForCurrentUser.Id,
                        UserId = createdRunningAcountRecordForCurrentUser.UserId,
                        OrderTrackingNumber = createdRunningAcountRecordForCurrentUser.OrderTrackingNumber
                    });

                uplineTraderAgentUser = await _userManager.Users
                    .Where(u => u.Id == currentUser.UplineId)
                    .Select(u => new
                    {
                        u.Id,
                        u.UserName,
                        u.FullName,
                        u.UplineId
                    })
                    .FirstOrDefaultAsync();
            }
            #endregion
        }

        public void UpdateRunningRecordsToCompleted(int runningAccountRecordId, string status, decimal? distributedAmount = null)
        {
            var toUpdate = new RunningAccountRecord
            {
                Id = runningAccountRecordId
            };

            _applicationContext.RunningAccountRecords.Attach(toUpdate);
            _applicationContext.Entry(toUpdate).Property(b => b.Status).IsModified = true;
            _applicationContext.Entry(toUpdate).Property(b => b.DistributedAmount).IsModified = true;

            toUpdate.Status = status;
            toUpdate.DistributedAmount = distributedAmount;
        }


        public async Task<RunningAccountRecord> MapFromOrderInfo(Distributing.Domain.Model.Distributions.Order orderInfo,
            string userId)
        {
            var user = _userCacheService.GetNameInfoByUserId(userId);

            var shopUser = _userCacheService.GetNameInfoByUserId(orderInfo.ShopId);

            var traderUser = _userCacheService.GetNameInfoByUserId(orderInfo.TraderId);

            //Build view model.
            var runningAccountRecord = new RunningAccountRecord
            {
                UserId = userId,
                UserName = user.UserName,
                UserFullName = user.FullName,
                //UserName = string.Empty,
                //UserFullName = string.Empty,

                //Set downline data if user has downline.
                //DownlineUserId = downlineUserId,
                //DownlineUserName = downlineUserName,
                // = downlineFullName,

                OrderTrackingNumber = orderInfo.TrackingNumber,
                ShopOrderId = orderInfo.ShopOrderId,

                //The running account record is created by domain event handler 
                //if the order is success, then the commission amount will be non-zero.
                Status = orderInfo.CommissionAmount > 0 ? "Success" : "Failed",

                Amount = orderInfo.Amount,

                //The commission amount is the expected amount distributed to the user.
                CommissionAmount = orderInfo.CommissionAmount,

                //The amount distributed to user.
                DistributedAmount = orderInfo.CommissionAmount,

                ShopId = orderInfo.ShopId,
                ShopUserName = shopUser.UserName,
                ShopFullName = shopUser.FullName,
                //ShopUserName = string.Empty,
                //ShopFullName = string.Empty,

                TraderId = orderInfo.TraderId,
                TraderUserName = traderUser?.UserName,
                TraderFullName = traderUser?.FullName,
                //TraderUserName = string.Empty,
                //TraderFullName = string.Empty,


                DateCreated = orderInfo.DateCreated
            };

            return runningAccountRecord;
        }

        public async Task<RunningAccountRecord> MapFromDataAsync(string orderTrackingNumber, string shopId, string shopOrderId, string traderId, decimal amount, DateTime dateCreated,
            decimal distributedAmount, string userId)
        {
            var user = await _userManager.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName,
                    u.UplineId
                })
                .FirstOrDefaultAsync();

            var shopUser = await _userManager.Users
                .Where(u => u.Id == shopId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName,
                    u.UplineId
                })
                .FirstOrDefaultAsync();

            var traderUser = await _userManager.Users
                .Where(u => u.Id == traderId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName,
                    u.UplineId
                })
                .FirstOrDefaultAsync();

            //Build view model.
            var runningAccountRecord = new RunningAccountRecord
            {
                UserId = userId,
                UserName = string.Empty,
                UserFullName = string.Empty,

                //Set downline data if user has downline.
                //DownlineUserId = downlineUserId,
                //DownlineUserName = downlineUserName,
                // = downlineFullName,

                OrderTrackingNumber = orderTrackingNumber,
                ShopOrderId = shopOrderId,

                //The running account record is created by domain event handler 
                //which happend when an order is paired.
                Status = "Success",

                Amount = amount,

                //The commission amount is the expected amount distributed to the user.
                //CommissionAmount = order.CommissionAmount, //Will update in background task.
                DistributedAmount = distributedAmount,

                ShopId = shopId,
                ShopUserName = shopUser.UserName,
                ShopFullName = shopUser.FullName,

                TraderId = traderId,
                TraderUserName = traderUser.UserName,
                TraderFullName = traderUser.FullName,
                DateCreated = dateCreated
            };

            return runningAccountRecord;
        }

        public RunningAccountRecord MapFromData(string orderTrackingNumber, string shopId, string shopOrderId, string traderId, decimal amount, DateTime dateCreated,
            decimal distributedAmount, string userId)
        {
            /*var user = await _userManager.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName,
                    u.UplineId
                })
                .FirstOrDefaultAsync();

            var shopUser = await _userManager.Users
                .Where(u => u.Id == shopId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName,
                    u.UplineId
                })
                .FirstOrDefaultAsync();

            var traderUser = await _userManager.Users
                .Where(u => u.Id == traderId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FullName,
                    u.UplineId
                })
                .FirstOrDefaultAsync();*/

            //Build view model.
            var runningAccountRecord = new RunningAccountRecord
            {
                UserId = userId,
                UserName = string.Empty,
                UserFullName = string.Empty,

                //Set downline data if user has downline.
                //DownlineUserId = downlineUserId,
                //DownlineUserName = downlineUserName,
                // = downlineFullName,

                OrderTrackingNumber = orderTrackingNumber,
                ShopOrderId = shopOrderId,

                //The running account record is created by domain event handler 
                //which happend when an order is paired.
                Status = "Success",

                Amount = amount,

                //The commission amount is the expected amount distributed to the user.
                //CommissionAmount = order.CommissionAmount, //Will update in background task.
                DistributedAmount = distributedAmount,

                ShopId = shopId,
                ShopUserName = string.Empty,
                ShopFullName = string.Empty,

                TraderId = traderId,
                TraderUserName = string.Empty,
                TraderFullName = string.Empty,
                DateCreated = dateCreated
            };

            return runningAccountRecord;
        }

        private RunningAccountRecord MapFromEntity(Ordering.Domain.Model.Orders.Order orderEntity, string shopOrderId,
            string userId, string userName, string userFullName,
            string shopUserId, string shopUserName, string shopFullName,
            string traderUserId, string traderUserName, string traderFullName,
            string downlineUserId = null, string downlineUserName = null, string downlineFullName = null)
        {
            //Build view model.
            var runningAccountRecord = new RunningAccountRecord
            {
                UserId = userId,
                UserName = userName,
                UserFullName = userFullName,

                //Set downline data if user has downline.
                DownlineUserId = downlineUserId,
                DownlineUserName = downlineUserName,
                DownlineFullName = downlineFullName,

                OrderTrackingNumber = orderEntity.TrackingNumber,
                ShopOrderId = shopOrderId,

                //The running account record is created by domain event handler 
                //which happend when an order is paired.
                Status = "AwaitingPayment",

                Amount = orderEntity.Amount,

                //The commission amount is the expected amount distributed to the user.
                //CommissionAmount = order.CommissionAmount, //Will update in background task.

                ShopId = shopUserId,
                ShopUserName = shopUserName,
                ShopFullName = shopFullName,

                TraderId = traderUserId,
                TraderUserName = traderUserName,
                TraderFullName = traderFullName,
                DateCreated = orderEntity.DateCreated
            };

            return runningAccountRecord;
        }

        public RunningAccountRecord Add(RunningAccountRecord runningAccountRecord)
        {
            return _applicationContext.RunningAccountRecords.Add(runningAccountRecord).Entity;
        }



        public void AddRange(List<RunningAccountRecord> runningAccountRecords)
        {
            _applicationContext.RunningAccountRecords.AddRange(runningAccountRecords);
        }
        public void Update(RunningAccountRecord runningAccountRecord)
        {
            _applicationContext.Entry(runningAccountRecord).State = EntityState.Modified;
        }

        public void Delete(RunningAccountRecord runningAccountRecord)
        {
            if (runningAccountRecord != null)
            {
                _applicationContext.RunningAccountRecords.Remove(runningAccountRecord);
            }
        }


        public async Task<List<TempRunningAccountRecord>> GetTempByUserIdAsync(string userId)
        {
            return await _applicationContext
                .TempRunningAccountRecords
                .Where(w => w.UserId == userId)
                .ToListAsync();
        }

        public async Task<TempRunningAccountRecord> GetTempByUserIdAndOrderTrackingNumberAsync(string userId, string orderTrackingNumber)
        {
            return await _applicationContext
                .TempRunningAccountRecords
                .Where(t => t.UserId == userId && t.OrderTrackingNumber == orderTrackingNumber)
                .FirstOrDefaultAsync();
        }


        public TempRunningAccountRecord AddTemp(TempRunningAccountRecord tempRunningAccountRecord)
        {
            return _applicationContext.TempRunningAccountRecords.Add(tempRunningAccountRecord).Entity;
        }

        public void DeleteTemp(TempRunningAccountRecord tempRunningAccountRecord)
        {
            if (tempRunningAccountRecord != null)
            {
                _applicationContext.TempRunningAccountRecords.Remove(tempRunningAccountRecord);
            }
        }

        public void DeleteTempRange(List<TempRunningAccountRecord> tempRunningAccountRecords)
        {
            _applicationContext.TempRunningAccountRecords.RemoveRange(tempRunningAccountRecords);
        }


        public async Task SaveChangesAsync()
        {
            await _applicationContext.SaveChangesAsync();
        }

    }
}
