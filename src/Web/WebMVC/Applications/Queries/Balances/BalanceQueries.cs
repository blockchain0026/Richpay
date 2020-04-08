using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Commissions;
using Distributing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.Balances
{
    public class BalanceQueries : IBalanceQueries
    {
        private readonly DistributingContext _context;
        private readonly IBalanceDomainService _balanceDomainService;

        public BalanceQueries(DistributingContext context, IBalanceDomainService balanceDomainService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _balanceDomainService = balanceDomainService ?? throw new ArgumentNullException(nameof(balanceDomainService));
        }

        public async Task<decimal?> GetAvailableBalanceByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("The user Id must be provided.");
            }

            /*var balance = await _context.Balances.Where(b => b.UserId == userId).FirstOrDefaultAsync();

            if (balance == null)
            {
                return null;
            }*/

            var availableBalance = await _balanceDomainService.GetAvailableBalanceAsync(userId);

            return availableBalance;
        }

        public async Task<Models.Queries.Balance> GetBalanceFromUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("The user Id must be provided.");
            }

            var result = await _context.Balances.Where(b => b.UserId == userId).SingleOrDefaultAsync();

            if (result != null)
            {
                await LoadNavigationObject(result);

                return MapBalanceFromEntity(result);
            }

            return null;
        }

        public async Task<IEnumerable<Models.Queries.Balance>> GetBalancesFromTraderUsersAsync(IEnumerable<ApplicationUser> users, int? pageIndex, int? take, string sortField = "", string direction = "desc")
        {
            var result = new List<Models.Queries.Balance>();

            var balances = new List<Distributing.Domain.Model.Balances.Balance>();
            foreach (var user in users)
            {
                var balance = await _context.Balances.Where(b => b.UserId == user.Id).SingleOrDefaultAsync();
                if (balance != null)
                {
                    balances.Add(balance);
                }
            }
            /*var balances = _context.Balances
                .Where(b => users.Any(u => u.Id == b.UserId));*/

            if (pageIndex != null && take != null)
            {
                IEnumerable<Distributing.Domain.Model.Balances.Balance> sortedResult = null;

                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "AmountAvailable")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = balances
                               .OrderBy(b => b.AmountAvailable)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = balances
                               .OrderByDescending(b => b.AmountAvailable)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "AmountFrozen")
                    {
                        //Can not sort by this property efficeintly.
                        //Need to use sql directly.

                        //Tempting sorting by amount availble.
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = balances
                               .OrderBy(b => b.AmountAvailable)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = balances
                               .OrderByDescending(b => b.AmountAvailable)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = balances
                               .OrderBy(b => b.AmountAvailable)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = balances
                               .OrderByDescending(b => b.AmountAvailable)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                }
                else
                {
                    sortedResult = balances
                       .OrderByDescending(b => b.AmountAvailable)
                       .Skip((int)take * (int)pageIndex)
                       .Take((int)take);
                }


                foreach (var balance in sortedResult)
                {
                    await LoadNavigationObject(balance);
                    result.Add(MapBalanceFromEntity(balance));
                }
            }
            else
            {
                foreach (var balance in balances)
                {
                    await LoadNavigationObject(balance);
                    result.Add(MapBalanceFromEntity(balance));
                }
            }

            return result;
        }

        private Models.Queries.Balance MapBalanceFromEntity(Distributing.Domain.Model.Balances.Balance entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity must be provided for mapping.");
            }
            var withdrawalLimit = new Models.Queries.WithdrawalLimit
            {
                DailyAmountLimit = (int)entity.WithdrawalLimit.DailyAmountLimit,
                DailyFrequencyLimit = entity.WithdrawalLimit.DailyFrequencyLimit,
                EachAmountUpperLimit = (int)entity.WithdrawalLimit.EachAmountUpperLimit,
                EachAmountLowerLimit = (int)entity.WithdrawalLimit.EachAmountLowerLimit
            };

            var balance = new Models.Queries.Balance
            {
                UserId = entity.UserId,
                AmountAvailable = entity.AmountAvailable,
                WithdrawalLimit = withdrawalLimit,
                WithdrawalCommissionRateInThousandth = (int)(entity.WithdrawalCommissionRate * 1000),
                DepositCommissionRateInThousandth = (int)(entity.DepositCommissionRate * 1000)
            };

            return balance;
        }


        private async Task LoadNavigationObject(Distributing.Domain.Model.Balances.Balance balance)
        {
            if (balance == null)
            {
                throw new ArgumentNullException("The balance must be provided.");
            }
            await _context.Entry(balance)
                .Reference(b => b.UserType).LoadAsync();
            await _context.Entry(balance)
                .Reference(b => b.WithdrawalLimit).LoadAsync();
        }

    }
}
