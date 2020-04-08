using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Data;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.Bankbook
{
    public class BankbookQueries : IBankbookQueries
    {
        private readonly ApplicationDbContext _context;

        public BankbookQueries(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public List<BankbookRecord> GetBankbookRecordsByUserIdAsync(string userId, int? pageIndex, int? take, string searchString = null, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<BankbookRecord>();


            var bankbooks = _context.BankbookRecords
                .Where(f => f.UserId == userId);

            IQueryable<BankbookRecord> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = bankbooks.Where(m =>
                             m.UserId == searchString
                             || m.DateOccurred== searchString
                             || m.Type== searchString
                             || m.TrackingId == searchString
                             || m.Description.Contains(searchString)
                             //|| m.BalanceId.ToString() == searchString
                             );
            }
            else
            {
                searchResult = bankbooks;
            }

            result =  this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            return result;
        }

        public List<BankbookRecord> GetBankbookRecordsByBalanceIdAsync(int balanceId, int? pageIndex, int? take, string searchString = null, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<BankbookRecord>();


            var bankbooks = _context.BankbookRecords
                .Where(f => f.BalanceId == balanceId);

            IQueryable<BankbookRecord> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = bankbooks.Where(m =>
                             m.UserId == searchString
                             || m.DateOccurred== searchString
                             || m.Type == searchString
                             || m.TrackingId == searchString
                             || m.Description.Contains(searchString)
                             //|| m.BalanceId.ToString() == searchString
                             );
            }
            else
            {
                searchResult = bankbooks;
            }

            result =  this.GetSortedRecords(
                searchResult,
                pageIndex,
                take,
                sortField,
                direction
                );

            return result;
        }

        public async Task<int> GetBankbookRecordsTotalCountAsync(string userId, string searchString = null)
        {
            var bankbooks = _context.BankbookRecords
                .Where(f => f.UserId == userId);

            if (!string.IsNullOrEmpty(searchString))
            {
                return await bankbooks.Where(m =>
                             m.UserId == searchString
                             || m.DateOccurred == searchString
                             || m.Type== searchString
                             || m.TrackingId== searchString
                             || m.Description.Contains(searchString)
                             //|| m.BalanceId.ToString() == searchString
                             ).CountAsync();
            }
            else
            {
                return await bankbooks.CountAsync();
            }
        }



        private List<BankbookRecord> GetSortedRecords(
            IQueryable<BankbookRecord> bankbooks,
            int? pageIndex, int? take, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<BankbookRecord>();

            if (pageIndex != null && take != null)
            {
                IEnumerable<BankbookRecord> sortedResult = null;
                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "Id")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = bankbooks
                               .OrderBy(f => f.Id)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = bankbooks
                               .OrderByDescending(f => f.Id)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "DateOccurred")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = bankbooks
                               .OrderBy(f => f.DateOccurred)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = bankbooks
                               .OrderByDescending(f => f.DateOccurred)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "Type")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = bankbooks
                               .OrderBy(f => f.Type)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = bankbooks
                               .OrderByDescending(f => f.Type)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "BalanceBefore")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = bankbooks
                               .OrderBy(f => f.BalanceBefore)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = bankbooks
                               .OrderByDescending(f => f.BalanceBefore)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "AmountChanged")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = bankbooks
                               .OrderBy(f => f.AmountChanged)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = bankbooks
                               .OrderByDescending(f => f.AmountChanged)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "BalanceAfter")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = bankbooks
                               .OrderBy(f => f.BalanceAfter)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = bankbooks
                               .OrderByDescending(f => f.BalanceAfter)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "TrackingId")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = bankbooks
                                .OrderBy(f => f.TrackingId)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = bankbooks
                               .OrderByDescending(f => f.TrackingId)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "Description")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = bankbooks
                                .OrderBy(f => f.Description)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take);
                        }
                        else
                        {
                            sortedResult = bankbooks
                               .OrderByDescending(f => f.Description)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = bankbooks
                               .OrderBy(f => f.DateOccurred)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = bankbooks
                               .OrderByDescending(f => f.DateOccurred)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                }
                else
                {
                    sortedResult = bankbooks
                       .OrderByDescending(f => f.DateOccurred)
                       .Skip((int)take * (int)pageIndex)
                       .Take((int)take);
                }

                foreach (var bankbook in sortedResult)
                {
                    result.Add(bankbook);
                }
            }
            else
            {
                foreach (var bankbook in bankbooks)
                {
                    result.Add(bankbook);
                }
            }

            return result;
        }

        public BankbookRecord Add(BankbookRecord bankbookRecord)
        {
            return _context.BankbookRecords.Add(bankbookRecord).Entity;
        }

        public void Update(BankbookRecord bankbookRecord)
        {
            _context.Entry(bankbookRecord).State = EntityState.Modified;
        }

        public void Delete(BankbookRecord bankbookRecord)
        {
            if (bankbookRecord != null)
            {
                _context.BankbookRecords.Remove(bankbookRecord);
            }
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
