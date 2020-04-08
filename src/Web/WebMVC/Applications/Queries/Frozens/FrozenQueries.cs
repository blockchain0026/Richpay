using Distributing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Distributing.Domain.Model.Frozens;
using WebMVC.Models;
using WebMVC.Extensions;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.Frozen
{
    public class FrozenQueries : IFrozenQueries
    {
        private readonly DistributingContext _context;

        public FrozenQueries(DistributingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<List<FrozenRecord>> GetFrozenRecordsByBalanceIdAsync(int balanceId, int? pageIndex, int? take, string searchString = null, string sortField = null, int? typeFilter = null, int? statusFilter = null, string direction = SortDirections.Descending)
        {
            var result = new List<FrozenRecord>();

            IQueryable<Distributing.Domain.Model.Frozens.Frozen> frozens = null;


            if (statusFilter != null && typeFilter != null)
            {
                frozens = _context.Frozens
                    .Include(f => f.FrozenStatus)
                    .Include(f => f.FrozenType)
                    .Where(f => f.BalanceId == balanceId && f.FrozenType.Id == typeFilter && f.FrozenStatus.Id == statusFilter);
            }
            else if (statusFilter != null)
            {
                frozens = _context.Frozens
                    .Include(f => f.FrozenStatus)
                    .Include(f => f.FrozenType)
                    .Where(f => f.BalanceId == balanceId && f.FrozenStatus.Id == statusFilter);
            }
            else if (typeFilter != null)
            {
                frozens = _context.Frozens
                    .Include(f => f.FrozenStatus)
                    .Include(f => f.FrozenType)
                    .Where(f => f.BalanceId == balanceId && f.FrozenType.Id == typeFilter);
            }



            IQueryable<Distributing.Domain.Model.Frozens.Frozen> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = frozens.Where(m =>
                             m.OrderTrackingNumber == searchString
                             || m.WithdrawalId.ToString() == searchString
                             //|| m.UserId.ToString() == searchString
                             || m.Description.Contains(searchString)
                             || m.BalanceId.ToString() == searchString);
            }
            else
            {
                searchResult = frozens;
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

        public decimal GetUserCurrentFrozenAmountAsync(string userId)
        {
            /*decimal result = 0;

            var frozens = _context.Frozens
                .Include(f => f.FrozenStatus)
                .Include(f => f.FrozenType)
                .Where(f => f.UserId == userId && f.FrozenStatus.Id == FrozenStatus.Frozen.Id && f.FrozenType.Id == FrozenType.ByAdmin.Id)
                .Select(f => new
                {
                    f.Amount
                });

            foreach (var frozen in frozens)
            {
                result += frozen.Amount;
            }

            return result;*/

            var result = _context.Frozens
                          .Include(f => f.FrozenStatus)
                          .Include(f => f.FrozenType)
                          .Where(f => f.UserId == userId && f.FrozenStatus.Id == FrozenStatus.Frozen.Id && f.FrozenType.Id == FrozenType.ByAdmin.Id)
                          .Sum(f => f.Amount);

            return result;
        }


        public async Task<List<FrozenRecord>> GetFrozenRecordsByUserIdAsync(string userId, int? pageIndex, int? take, string searchString = null, string sortField = null, int? typeFilter = null, int? statusFilter = null, string direction = SortDirections.Descending)
        {
            var result = new List<FrozenRecord>();

            //var frozens = new List<Distributing.Domain.Model.Frozens.Frozen>();
            IQueryable<Distributing.Domain.Model.Frozens.Frozen> frozens = null;


            if (statusFilter != null && typeFilter != null)
            {
                frozens = _context.Frozens
                    .Include(f => f.FrozenStatus)
                    .Include(f => f.FrozenType)
                    .Where(f => f.UserId == userId && f.FrozenType.Id == typeFilter && f.FrozenStatus.Id == statusFilter);
            }
            else if (statusFilter != null)
            {
                frozens = _context.Frozens
                    .Include(f => f.FrozenStatus)
                    .Include(f => f.FrozenType)
                    .Where(f => f.UserId == userId && f.FrozenStatus.Id == statusFilter);
            }
            else if (typeFilter != null)
            {
                frozens = _context.Frozens
                    .Include(f => f.FrozenStatus)
                    .Include(f => f.FrozenType)
                    .Where(f => f.UserId == userId && f.FrozenType.Id == typeFilter);
            }

            IQueryable<Distributing.Domain.Model.Frozens.Frozen> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = frozens.Where(m =>
                             m.OrderTrackingNumber == searchString
                             || m.WithdrawalId.ToString() == searchString
                             || m.Description.Contains(searchString)
                             || m.BalanceId.ToString() == searchString);
            }
            else
            {
                searchResult = frozens;
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

        public async Task<int> GetFrozenRecordsTotalCountByUserId(string userId, string searchString = null, int? typeFilter = null, int? statusFilter = null)
        {
            IQueryable<Distributing.Domain.Model.Frozens.Frozen> frozens = null;


            if (statusFilter != null && typeFilter != null)
            {
                frozens = _context.Frozens
                    .Include(f => f.FrozenStatus)
                    .Include(f => f.FrozenType)
                    .Where(f => f.UserId == userId && f.FrozenType.Id == typeFilter && f.FrozenStatus.Id == statusFilter);
            }
            else if (statusFilter != null)
            {
                frozens = _context.Frozens
                    .Include(f => f.FrozenStatus)
                    .Include(f => f.FrozenType)
                    .Where(f => f.UserId == userId && f.FrozenStatus.Id == statusFilter);
            }
            else if (typeFilter != null)
            {
                frozens = _context.Frozens
                    .Include(f => f.FrozenStatus)
                    .Include(f => f.FrozenType)
                    .Where(f => f.UserId == userId && f.FrozenType.Id == typeFilter);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                return await frozens.Where(m =>
                             m.OrderTrackingNumber == searchString
                             || m.WithdrawalId.ToString() == searchString
                             || m.Description.Contains(searchString)
                             || m.BalanceId.ToString() == searchString).CountAsync();
            }
            else
            {
                return await frozens.CountAsync();
            }
        }


        private FrozenRecord MapFrozenFromEntity(Distributing.Domain.Model.Frozens.Frozen entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity must be provided for mapping.");
            }

            var frozen = new FrozenRecord
            {
                FrozenId = entity.Id,
                FrozenStatus = entity.FrozenStatus.Name,
                FrozenType = entity.FrozenType.Name,
                Amount = entity.Amount,
                OrderTrackingNumber = entity.OrderTrackingNumber,
                WithdrawalId = entity.WithdrawalId,
                ByAdminId = entity.ByAdmin?.AdminId,
                ByAdminName = entity.ByAdmin?.Name,
                Description = entity.Description,
                DateFroze = entity.DateFroze.ToFullString(),
                DateUnfroze = entity.DateUnfroze?.ToFullString()
            };

            return frozen;
        }

        private async Task LoadNavigationObject(Distributing.Domain.Model.Frozens.Frozen frozen)
        {
            if (frozen == null)
            {
                throw new ArgumentNullException("The frozen must be provided.");
            }
            await _context.Entry(frozen)
                .Reference(f => f.FrozenStatus).LoadAsync();
            await _context.Entry(frozen)
                .Reference(f => f.FrozenType).LoadAsync();
            await _context.Entry(frozen)
                .Reference(f => f.ByAdmin).LoadAsync();
        }

        private async Task<List<FrozenRecord>> GetSortedRecords(
            IQueryable<Distributing.Domain.Model.Frozens.Frozen> frozens,
            int? pageIndex, int? take, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<FrozenRecord>();

            if (pageIndex != null && take != null)
            {
                IEnumerable<Distributing.Domain.Model.Frozens.Frozen> sortedResult = null;
                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "DateFroze")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = frozens
                               .OrderBy(f => f.DateFroze)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = frozens
                               .OrderByDescending(f => f.DateFroze)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "DateUnfroze")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = frozens
                               .OrderBy(f => f.DateUnfroze)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = frozens
                               .OrderByDescending(f => f.DateUnfroze)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "Description")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = frozens
                               .OrderBy(f => f.Description)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = frozens
                               .OrderByDescending(f => f.Description)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "WithdrawalId")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = frozens
                               .OrderBy(f => f.WithdrawalId)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = frozens
                               .OrderByDescending(f => f.WithdrawalId)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "OrderTrackingNumber")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = frozens
                               .OrderBy(f => f.OrderTrackingNumber)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = frozens
                               .OrderByDescending(f => f.OrderTrackingNumber)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "Amount")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = frozens
                               .OrderBy(f => f.Amount)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = frozens
                               .OrderByDescending(f => f.Amount)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "ByAdminName")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = frozens
                                .Include(f => f.ByAdmin)
                                .OrderBy(f => f.ByAdmin.Name)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = frozens
                               .OrderByDescending(f => f.DateUnfroze)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = frozens
                               .OrderBy(f => f.DateFroze)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = frozens
                               .OrderByDescending(f => f.DateFroze)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                }
                else
                {
                    sortedResult = frozens
                       .OrderByDescending(f => f.DateFroze)
                       .Skip((int)take * (int)pageIndex)
                       .Take((int)take);
                }

                foreach (var frozen in sortedResult)
                {
                    await LoadNavigationObject(frozen);
                    result.Add(MapFrozenFromEntity(frozen));
                }
            }
            else
            {
                foreach (var frozen in frozens)
                {
                    await LoadNavigationObject(frozen);
                    result.Add(MapFrozenFromEntity(frozen));
                }
            }

            return result;
        }

    }
}
