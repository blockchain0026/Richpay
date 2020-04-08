using Distributing.Domain.Model.Banks;
using Distributing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Data;
using WebMVC.Extensions;
using WebMVC.Models;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.Queries.Deposits
{
    public class DepositQueries : IDepositQueries
    {
        private readonly DistributingContext _distributingContext;
        private readonly ApplicationDbContext _applicationContext;

        public DepositQueries(DistributingContext distributingContext, ApplicationDbContext appicationContext)
        {
            _distributingContext = distributingContext ?? throw new ArgumentNullException(nameof(distributingContext));
            _applicationContext = appicationContext ?? throw new ArgumentNullException(nameof(appicationContext));
        }

        public async Task<DepositEntry> GetDepositEntryAsync(int depositId)
        {
            return await this._applicationContext.DepositEntrys.Where(d => d.DepositId == depositId).FirstOrDefaultAsync();
        }

        public async Task<List<DepositEntry>> GetDepositEntrysAsync(int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false, string direction = SortDirections.Descending)
        {
            var result = new List<DepositEntry>();

            IQueryable<DepositEntry> depositEntrys = null;

            if (isInProcess)
            {
                if (from != null && to != null)
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => EF.Functions.Like(f.UserType, "%" + userType)
                        && f.DepositStatus == "Submitted"
                        && f.DateCreated >= from
                        && f.DateCreated <= to
                        );
                }
                else
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => EF.Functions.Like(f.UserType, "%" + userType)
                        && f.DepositStatus == "Submitted"
                        );
                }
            }
            else
            {
                if (from != null && to != null)
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => EF.Functions.Like(f.UserType, "%" + userType)
                        && f.DepositStatus != "Submitted"
                        && f.DateCreated >= from
                        && f.DateCreated <= to
                        );
                }
                else
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => EF.Functions.Like(f.UserType, "%" + userType)
                        && f.DepositStatus != "Submitted"
                        );
                }
            }

            /*if (!string.IsNullOrWhiteSpace(status) && !string.IsNullOrWhiteSpace(userType))
            {
                depositEntrys = _applicationContext.DepositEntrys
                    .Where(f => f.UserType == userType && f.DepositStatus == status);
            }
            else if (!string.IsNullOrWhiteSpace(status))
            {
                depositEntrys = _applicationContext.DepositEntrys
                    .Where(f => f.DepositStatus == status);
            }
            else if (!string.IsNullOrWhiteSpace(userType))
            {
                if (isInProcess)
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => f.UserType == userType && f.DepositStatus == "Submitted");
                }
                else
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => f.UserType == userType && f.DepositStatus != "Submitted");
                }

            }
            else
            {
                if (isInProcess)
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => f.DepositStatus == "Submitted");

                }
                else
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => f.DepositStatus != "Submitted");
                }
            }
            */

            IQueryable<DepositEntry> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = depositEntrys
                    .AsNoTracking()
                    .Include(w => w.DepositBankAccount)
                    .Where(w => w.DepositId.ToString() == searchString
                    || w.UserId == searchString
                    || EF.Functions.Like(w.Username, "%" + searchString + "%")
                    || EF.Functions.Like(w.FullName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.BankName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.AccountName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.AccountNumber, "%" + searchString + "%")
                    || EF.Functions.Like(w.Description, "%" + searchString + "%")
                    );
            }
            else
            {
                searchResult = depositEntrys.AsNoTracking();
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

        public async Task<int> GetDepositEntrysTotalCount(string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false)
        {
            IQueryable<DepositEntry> depositEntrys = null;

            if (isInProcess)
            {
                if (from != null && to != null)
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => EF.Functions.Like(f.UserType, "%" + userType)
                        && f.DepositStatus == "Submitted"
                        && f.DateCreated >= from
                        && f.DateCreated <= to
                        );
                }
                else
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => EF.Functions.Like(f.UserType, "%" + userType)
                        && f.DepositStatus == "Submitted"
                        );
                }
            }
            else
            {
                if (from != null && to != null)
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => EF.Functions.Like(f.UserType, "%" + userType)
                        && f.DepositStatus != "Submitted"
                        && f.DateCreated >= from
                        && f.DateCreated <= to
                        );
                }
                else
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => EF.Functions.Like(f.UserType, "%" + userType)
                        && f.DepositStatus != "Submitted"
                        );
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                return await depositEntrys
                    .Include(w => w.DepositBankAccount)
                    .Where(w => w.DepositId.ToString() == searchString
                    || w.UserId == searchString
                    || EF.Functions.Like(w.Username, "%" + searchString + "%")
                    || EF.Functions.Like(w.FullName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.BankName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.AccountName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.AccountNumber, "%" + searchString + "%")
                    || EF.Functions.Like(w.Description, "%" + searchString + "%")
                    ).CountAsync();
            }
            else
            {
                return await depositEntrys.CountAsync();

            }
        }


        public async Task<List<DepositEntry>> GetDepositEntrysByUserIdAsync(string userId, int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false, bool onlySelf = true, string direction = SortDirections.Descending)
        {
            var result = new List<DepositEntry>();

            IQueryable<DepositEntry> depositEntrys = null;


            if (isInProcess)
            {
                if (from != null && to != null)
                {
                    if (onlySelf)
                    {
                        depositEntrys = _applicationContext.DepositEntrys
                            .Where(f => f.UserId == userId
                            && f.DepositStatus == "Submitted"
                            && f.DateCreated >= from
                            && f.DateCreated <= to
                            );
                    }
                    else
                    {
                        depositEntrys = _applicationContext.DepositEntrys
                            .Where(f => (f.UserId == userId || f.CreateByUplineId == userId)
                            && f.DepositStatus == "Submitted"
                            && f.DateCreated >= from
                            && f.DateCreated <= to
                            );
                    }
                }
                else
                {
                    if (onlySelf)
                    {
                        depositEntrys = _applicationContext.DepositEntrys
                            .Where(f => f.UserId == userId
                            && f.DepositStatus == "Submitted"
                            );
                    }
                    else
                    {
                        depositEntrys = _applicationContext.DepositEntrys
                            .Where(f => (f.UserId == userId || f.CreateByUplineId == userId)
                            && f.DepositStatus == "Submitted"
                            );
                    }
                }
            }
            else
            {
                if (from != null && to != null)
                {
                    if (onlySelf)
                    {
                        depositEntrys = _applicationContext.DepositEntrys
                            .Where(f => f.UserId == userId
                            && f.DepositStatus != "Submitted"
                            && f.DateCreated >= from
                            && f.DateCreated <= to
                            );
                    }
                    else
                    {
                        depositEntrys = _applicationContext.DepositEntrys
                            .Where(f => (f.UserId == userId || f.CreateByUplineId == userId)
                            && f.DepositStatus != "Submitted"
                            && f.DateCreated >= from
                            && f.DateCreated <= to
                            );
                    }
                }
                else
                {
                    if (onlySelf)
                    {
                        depositEntrys = _applicationContext.DepositEntrys
                            .Where(f => f.UserId == userId
                            && f.DepositStatus != "Submitted"
                            );
                    }
                    else
                    {
                        depositEntrys = _applicationContext.DepositEntrys
                            .Where(f => (f.UserId == userId || f.CreateByUplineId == userId)
                            && f.DepositStatus != "Submitted"
                            );
                    }
                }
            }

            IQueryable<DepositEntry> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = depositEntrys
                    .AsNoTracking()
                    .Include(w => w.DepositBankAccount)
                    .Where(w => w.DepositId.ToString() == searchString
                    || EF.Functions.Like(w.Username, "%" + searchString + "%")
                    || EF.Functions.Like(w.FullName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.BankName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.AccountName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.AccountNumber, "%" + searchString + "%")
                    || EF.Functions.Like(w.Description, "%" + searchString + "%")
                    );
            }
            else
            {
                searchResult = depositEntrys.AsNoTracking();
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

        public async Task<int> GetDepositEntrysTotalCountByUserId(string userId, string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false, bool onlySelf = true)
        {
            IQueryable<DepositEntry> depositEntrys = null;


            if (isInProcess)
            {
                if (from != null && to != null)
                {
                    if (onlySelf)
                    {
                        depositEntrys = _applicationContext.DepositEntrys
                            .Where(f => f.UserId == userId
                            && f.DepositStatus == "Submitted"
                            && f.DateCreated >= from
                            && f.DateCreated <= to
                            );
                    }
                    else
                    {
                        depositEntrys = _applicationContext.DepositEntrys
                            .Where(f => (f.UserId == userId || f.CreateByUplineId == userId)
                            && f.DepositStatus == "Submitted"
                            && f.DateCreated >= from
                            && f.DateCreated <= to
                            );
                    }
                }
                else
                {
                    if (onlySelf)
                    {
                        depositEntrys = _applicationContext.DepositEntrys
                            .Where(f => f.UserId == userId
                            && f.DepositStatus == "Submitted"
                            );
                    }
                    else
                    {
                        depositEntrys = _applicationContext.DepositEntrys
                            .Where(f => (f.UserId == userId || f.CreateByUplineId == userId)
                            && f.DepositStatus == "Submitted"
                            );
                    }
                }
            }
            else
            {
                if (from != null && to != null)
                {
                    if (onlySelf)
                    {
                        depositEntrys = _applicationContext.DepositEntrys
                            .Where(f => f.UserId == userId
                            && f.DepositStatus != "Submitted"
                            && f.DateCreated >= from
                            && f.DateCreated <= to
                            );
                    }
                    else
                    {
                        depositEntrys = _applicationContext.DepositEntrys
                            .Where(f => (f.UserId == userId || f.CreateByUplineId == userId)
                            && f.DepositStatus != "Submitted"
                            && f.DateCreated >= from
                            && f.DateCreated <= to
                            );
                    }
                }
                else
                {
                    if (onlySelf)
                    {
                        depositEntrys = _applicationContext.DepositEntrys
                            .Where(f => f.UserId == userId
                            && f.DepositStatus != "Submitted"
                            );
                    }
                    else
                    {
                        depositEntrys = _applicationContext.DepositEntrys
                            .Where(f => (f.UserId == userId || f.CreateByUplineId == userId)
                            && f.DepositStatus != "Submitted"
                            );
                    }
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                return await depositEntrys
                    .Include(w => w.DepositBankAccount)
                    .Where(w => w.DepositId.ToString() == searchString
                    || w.UserId == searchString
                    || EF.Functions.Like(w.Username, "%" + searchString + "%")
                    || EF.Functions.Like(w.FullName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.BankName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.AccountName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.AccountNumber, "%" + searchString + "%")
                    || EF.Functions.Like(w.Description, "%" + searchString + "%")
                    ).CountAsync();
            }
            else
            {
                return await depositEntrys.CountAsync();
            }
        }


        public async Task<List<DepositEntry>> GetDepositEntrysByUplineIdAsync(string uplineId, int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false, string direction = SortDirections.Descending)
        {
            var result = new List<DepositEntry>();

            IQueryable<DepositEntry> depositEntrys = null;


            if (isInProcess)
            {
                if (from != null && to != null)
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => f.CreateByUplineId == uplineId
                        && f.DepositStatus == "Submitted"
                        && f.DateCreated >= from
                        && f.DateCreated <= to
                        );
                }
                else
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => f.CreateByUplineId == uplineId
                        && f.DepositStatus == "Submitted"
                        );
                }
            }
            else
            {
                if (from != null && to != null)
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => f.CreateByUplineId == uplineId
                        && f.DepositStatus != "Submitted"
                        && f.DateCreated >= from
                        && f.DateCreated <= to
                        );
                }
                else
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => f.CreateByUplineId == uplineId
                        && f.DepositStatus != "Submitted"
                        );
                }
            }

            IQueryable<DepositEntry> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = depositEntrys
                    .AsNoTracking()
                    .Include(w => w.DepositBankAccount)
                    .Where(w => w.DepositId.ToString() == searchString
                    || EF.Functions.Like(w.Username, "%" + searchString + "%")
                    || EF.Functions.Like(w.FullName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.BankName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.AccountName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.AccountNumber, "%" + searchString + "%")
                    || EF.Functions.Like(w.Description, "%" + searchString + "%")
                    );
            }
            else
            {
                searchResult = depositEntrys.AsNoTracking();
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

        public async Task<int> GetDepositEntrysTotalCountByUplineId(string uplineId, string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false)
        {
            IQueryable<DepositEntry> depositEntrys = null;

            if (isInProcess)
            {
                if (from != null && to != null)
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => f.CreateByUplineId == uplineId
                        && f.DepositStatus == "Submitted"
                        && f.DateCreated >= from
                        && f.DateCreated <= to
                        );
                }
                else
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => f.CreateByUplineId == uplineId
                        && f.DepositStatus == "Submitted"
                        );
                }
            }
            else
            {
                if (from != null && to != null)
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => f.CreateByUplineId == uplineId
                        && f.DepositStatus != "Submitted"
                        && f.DateCreated >= from
                        && f.DateCreated <= to
                        );
                }
                else
                {
                    depositEntrys = _applicationContext.DepositEntrys
                        .Where(f => f.CreateByUplineId == uplineId
                        && f.DepositStatus != "Submitted"
                        );
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                return await depositEntrys
                    .Include(w => w.DepositBankAccount)
                    .Where(w => w.DepositId.ToString() == searchString
                    || w.UserId == searchString
                    || EF.Functions.Like(w.Username, "%" + searchString + "%")
                    || EF.Functions.Like(w.FullName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.BankName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.AccountName, "%" + searchString + "%")
                    || EF.Functions.Like(w.DepositBankAccount.AccountNumber, "%" + searchString + "%")
                    || EF.Functions.Like(w.Description, "%" + searchString + "%")
                    ).CountAsync();
            }
            else
            {
                return await depositEntrys.CountAsync();
            }
        }



        public async Task<DepositBankAccount> GetDepositBankAccount(int depositBankAccountId)
        {
            var depositAccount = await this._distributingContext.DepositAccounts
                .Where(d => d.Id == depositBankAccountId)
                .FirstOrDefaultAsync();

            if (depositAccount != null)
            {
                return MapDepositBankAccountFromEntity(depositAccount);
            }
            return null;

        }

        public List<DepositBankAccount> GetDepositBankAccounts(int? pageIndex, int? take, string searchString = null, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<DepositBankAccount>();

            IQueryable<DepositAccount> depositAccounts = null;

            depositAccounts = _distributingContext.DepositAccounts;

            IQueryable<DepositAccount> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = depositAccounts
                    .Where(w => w.Id.ToString() == searchString
                    || w.BankAccount.BankName.Contains(searchString)
                    || w.BankAccount.AccountName.Contains(searchString)
                    || w.BankAccount.AccountNumber == searchString
                    );
            }
            else
            {
                searchResult = depositAccounts;
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

        public async Task<int> GetDepositBankAccountsTotalCount(string searchString = null)
        {
            var result = 0;

            if (!string.IsNullOrEmpty(searchString))
            {
                result = await _distributingContext.DepositAccounts
                    .Where(w => w.Id.ToString() == searchString
                    || w.BankAccount.BankName.Contains(searchString)
                    || w.BankAccount.AccountName.Contains(searchString)
                    || w.BankAccount.AccountNumber == searchString
                    ).CountAsync();
            }
            else
            {
                result = await _distributingContext.DepositAccounts.CountAsync();
            }

            return result;
        }










        private DepositBankAccount MapDepositBankAccountFromEntity(DepositAccount entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity must be provided for mapping.");
            }

            var depositBankAccount = new DepositBankAccount
            {
                BankAccountId = entity.Id,
                Name = entity.Name,
                BankName = entity.BankAccount.BankName,
                AccountName = entity.BankAccount.AccountName,
                AccountNumber = entity.BankAccount.AccountNumber,
                DateCreated = entity.DateCreated.ToFullString()
            };

            return depositBankAccount;
        }

        private async Task<List<DepositEntry>> GetSortedRecords(
            IQueryable<DepositEntry> depositEntrys,
            int? pageIndex, int? take, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<DepositEntry>();

            if (pageIndex != null && take != null)
            {
                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "DateCreated")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await depositEntrys
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await depositEntrys
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "DepositId")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await depositEntrys
                               .OrderBy(f => f.DepositId)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await depositEntrys
                               .OrderByDescending(f => f.DepositId)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "DepositStatus")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await depositEntrys
                               .OrderBy(f => f.DepositStatus)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await depositEntrys
                               .OrderByDescending(f => f.DepositStatus)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "Username")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await depositEntrys
                               .OrderBy(f => f.Username)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await depositEntrys
                               .OrderByDescending(f => f.Username)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "UserType")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await depositEntrys
                               .OrderBy(f => f.UserType)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await depositEntrys
                               .OrderByDescending(f => f.UserType)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "TotalAmount")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await depositEntrys
                               .OrderBy(f => f.TotalAmount)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await depositEntrys
                               .OrderByDescending(f => f.TotalAmount)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "CommissionRateInThousandth")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await depositEntrys
                                .OrderBy(f => f.CommissionRateInThousandth)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await depositEntrys
                               .OrderByDescending(f => f.CommissionRateInThousandth)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "CommissionAmount")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await depositEntrys
                                .OrderBy(f => f.CommissionAmount)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await depositEntrys
                               .OrderByDescending(f => f.CommissionAmount)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "ActualAmount")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await depositEntrys
                                .OrderBy(f => f.ActualAmount)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await depositEntrys
                               .OrderByDescending(f => f.ActualAmount)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "VerifiedBy")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await depositEntrys
                                .OrderBy(f => f.VerifiedByAdminName)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await depositEntrys
                               .OrderByDescending(f => f.VerifiedByAdminName)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await depositEntrys
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await depositEntrys
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                }
                else
                {
                    result = await depositEntrys
                       .OrderByDescending(f => f.DateCreated)
                       .Skip((int)take * (int)pageIndex)
                       .Take((int)take)
                       .ToListAsync();
                }
            }
            else
            {
                result = await depositEntrys.ToListAsync();
            }

            return result;
        }

        private List<DepositBankAccount> GetSortedRecords(
            IQueryable<DepositAccount> depositAccounts,
            int? pageIndex, int? take, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<DepositBankAccount>();

            if (pageIndex != null && take != null)
            {
                IEnumerable<DepositAccount> sortedResult = null;
                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "DateCreated")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = depositAccounts
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = depositAccounts
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "BankName")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = depositAccounts
                               .Include(d => d.BankAccount)
                               .OrderBy(f => f.BankAccount.BankName)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = depositAccounts
                               .Include(d => d.BankAccount)
                               .OrderByDescending(f => f.BankAccount.BankName)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "DepositBankId")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = depositAccounts
                               .OrderBy(f => f.Id)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = depositAccounts
                               .OrderByDescending(f => f.Id)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = depositAccounts
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = depositAccounts
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                }
                else
                {
                    sortedResult = depositAccounts
                       .OrderByDescending(f => f.DateCreated)
                       .Skip((int)take * (int)pageIndex)
                       .Take((int)take);
                }

                foreach (var bank in sortedResult)
                {
                    result.Add(MapDepositBankAccountFromEntity(bank));
                }
            }
            else
            {
                foreach (var bank in depositAccounts)
                {
                    result.Add(MapDepositBankAccountFromEntity(bank));

                }
            }

            return result;
        }




        public DepositEntry Add(DepositEntry depositEntry)
        {
            return _applicationContext.DepositEntrys.Add(depositEntry).Entity;
        }

        public void Update(DepositEntry depositEntry)
        {
            _applicationContext.Entry(depositEntry).State = EntityState.Modified;
        }

        public void Delete(DepositEntry depositEntry)
        {
            if (depositEntry != null)
            {
                _applicationContext.DepositEntrys.Remove(depositEntry);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _applicationContext.SaveChangesAsync();
        }


    }
}
