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

namespace WebMVC.Applications.Queries.Withdrawals
{
    public class WithdrawalQueries : IWithdrawalQueries
    {
        private readonly DistributingContext _distributingContext;
        private readonly ApplicationDbContext _applicationContext;

        public WithdrawalQueries(DistributingContext distributingContext, ApplicationDbContext appicationContext)
        {
            _distributingContext = distributingContext ?? throw new ArgumentNullException(nameof(distributingContext));
            _applicationContext = appicationContext ?? throw new ArgumentNullException(nameof(appicationContext));
        }



        public async Task<WithdrawalEntry> GetWithdrawalEntryAsync(int withdrawalId)
        {
            return await _applicationContext.WithdrawalEntrys.Where(w => w.WithdrawalId == withdrawalId).FirstOrDefaultAsync();
        }

        public async Task<List<WithdrawalEntry>> GetWithdrawalEntrysAsync(int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false, string direction = SortDirections.Descending)
        {
            var result = new List<WithdrawalEntry>();

            IQueryable<WithdrawalEntry> withdrawalEntrys = null;

            if (isInProcess)
            {
                if (from != null && to != null)
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => EF.Functions.Like(f.UserType, "%" + userType + "%")
                        && f.WithdrawalStatus != "Success"
                        && f.WithdrawalStatus != "Canceled"
                        && EF.Functions.Like(f.WithdrawalStatus, "%" + status + "%")
                        && f.DateCreated >= from
                        && f.DateCreated <= to
                        );
                }
                else
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => EF.Functions.Like(f.UserType, "%" + userType + "%")
                        && f.WithdrawalStatus != "Success"
                        && f.WithdrawalStatus != "Canceled"
                        && EF.Functions.Like(f.WithdrawalStatus, "%" + status + "%")
                        );
                }
            }
            else
            {
                if (from != null && to != null)
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => EF.Functions.Like(f.UserType, "%" + userType + "%")
                        && (EF.Functions.Like(f.WithdrawalStatus, "%" + "Success" + "%") || EF.Functions.Like(f.WithdrawalStatus, "%" + "Canceled" + "%"))
                        && f.DateCreated >= from
                        && f.DateCreated <= to
                        );
                }
                else
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => EF.Functions.Like(f.UserType, "%" + userType + "%")
                        && (EF.Functions.Like(f.WithdrawalStatus, "%" + "Success" + "%") || EF.Functions.Like(f.WithdrawalStatus, "%" + "Canceled" + "%"))
                        );
                }
            }

            /*
            if (!string.IsNullOrWhiteSpace(status) && !string.IsNullOrWhiteSpace(userType))
            {
                withdrawalEntrys = _applicationContext.WithdrawalEntrys
                    .Where(f => f.UserType == userType && f.WithdrawalStatus == status);
            }
            else if (!string.IsNullOrWhiteSpace(status))
            {
                withdrawalEntrys = _applicationContext.WithdrawalEntrys
                    .Where(f => f.WithdrawalStatus == status);
            }
            else if (!string.IsNullOrWhiteSpace(userType))
            {
                if (isInProcess)
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => f.UserType == userType && f.WithdrawalStatus != "Success" && f.WithdrawalStatus != "Canceled");
                }
                else
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => f.UserType == userType && (f.WithdrawalStatus == "Success" || f.WithdrawalStatus == "Canceled"));
                }
            }
            else
            {
                if (isInProcess)
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => f.WithdrawalStatus != "Success" && f.WithdrawalStatus != "Canceled");
                }
                else
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => f.WithdrawalStatus == "Success" || f.WithdrawalStatus == "Canceled");
                }
            }*/

            IQueryable<WithdrawalEntry> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = withdrawalEntrys
                    .AsNoTracking()
                    .Include(w => w.WithdrawalBankOption)
                    .Where(w => w.WithdrawalId.ToString() == searchString
                    || w.UserId == searchString
                    || EF.Functions.Like(w.Username, "%" + searchString + "%")
                    || EF.Functions.Like(w.FullName, "%" + searchString + "%")
                    || EF.Functions.Like(w.WithdrawalBankOption.BankName, "%" + searchString + "%")
                    || EF.Functions.Like(w.AccountName, "%" + searchString + "%")
                    || EF.Functions.Like(w.AccountNumber, "%" + searchString + "%")
                    || EF.Functions.Like(w.Description, "%" + searchString + "%")
                    );
            }
            else
            {
                searchResult = withdrawalEntrys.AsNoTracking();
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

        public async Task<int> GetWithdrawalEntrysTotalCount(string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false)
        {
            IQueryable<WithdrawalEntry> withdrawalEntrys = null;

            if (isInProcess)
            {
                if (from != null && to != null)
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => EF.Functions.Like(f.UserType, "%" + userType + "%")
                        && f.WithdrawalStatus != "Success"
                        && f.WithdrawalStatus != "Canceled"
                        && EF.Functions.Like(f.WithdrawalStatus, "%" + status + "%")
                        && f.DateCreated >= from
                        && f.DateCreated <= to
                        );
                }
                else
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => EF.Functions.Like(f.UserType, "%" + userType + "%")
                        && f.WithdrawalStatus != "Success"
                        && f.WithdrawalStatus != "Canceled"
                        && EF.Functions.Like(f.WithdrawalStatus, "%" + status + "%")
                        );
                }
            }
            else
            {
                if (from != null && to != null)
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => EF.Functions.Like(f.UserType, "%" + userType + "%")
                        && (EF.Functions.Like(f.WithdrawalStatus, "%" + "Success" + "%") || EF.Functions.Like(f.WithdrawalStatus, "%" + "Canceled" + "%"))
                        && f.DateCreated >= from
                        && f.DateCreated <= to
                        );
                }
                else
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => EF.Functions.Like(f.UserType, "%" + userType + "%")
                        && (EF.Functions.Like(f.WithdrawalStatus, "%" + "Success" + "%") || EF.Functions.Like(f.WithdrawalStatus, "%" + "Canceled" + "%"))
                        );
                }
            }

            /*if (!string.IsNullOrWhiteSpace(status) && !string.IsNullOrWhiteSpace(userType))
            {
                withdrawalEntrys = _applicationContext.WithdrawalEntrys
                    .Where(f => f.UserType == userType && f.WithdrawalStatus == status);
            }
            else if (!string.IsNullOrWhiteSpace(status))
            {
                withdrawalEntrys = _applicationContext.WithdrawalEntrys
                    .Where(f => f.WithdrawalStatus == status);
            }
            else if (!string.IsNullOrWhiteSpace(userType))
            {
                if (isInProcess)
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => f.UserType == userType && f.WithdrawalStatus != "Success" && f.WithdrawalStatus != "Canceled");
                }
                else
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => f.UserType == userType && (f.WithdrawalStatus == "Success" || f.WithdrawalStatus == "Canceled"));
                }
            }
            else
            {
                if (isInProcess)
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => f.WithdrawalStatus != "Success" && f.WithdrawalStatus != "Canceled");
                }
                else
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => f.WithdrawalStatus == "Success" || f.WithdrawalStatus == "Canceled");
                }
            }*/

            if (!string.IsNullOrEmpty(searchString))
            {
                return await withdrawalEntrys
                    .Include(w => w.WithdrawalBankOption)
                    .Where(w => w.WithdrawalId.ToString() == searchString
                    || w.UserId == searchString
                    || EF.Functions.Like(w.Username, "%" + searchString + "%")
                    || EF.Functions.Like(w.FullName, "%" + searchString + "%")
                    || EF.Functions.Like(w.WithdrawalBankOption.BankName, "%" + searchString + "%")
                    || EF.Functions.Like(w.AccountName, "%" + searchString + "%")
                    || EF.Functions.Like(w.AccountNumber, "%" + searchString + "%")
                    || EF.Functions.Like(w.Description, "%" + searchString + "%")
                    ).CountAsync();
            }
            else
            {
                return await withdrawalEntrys.CountAsync();

            }
        }

        public async Task<List<WithdrawalEntry>> GetWithdrawalEntrysByUserIdAsync(string userId, int? pageIndex, int? take, string searchString = null, string sortField = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false, string direction = SortDirections.Descending)
        {
            var result = new List<WithdrawalEntry>();

            IQueryable<WithdrawalEntry> withdrawalEntrys = null;


            if (isInProcess)
            {
                if (from != null && to != null)
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => f.UserId == userId
                        && f.WithdrawalStatus != "Success"
                        && f.WithdrawalStatus != "Canceled"
                        && EF.Functions.Like(f.WithdrawalStatus, "%" + status + "%")
                        && f.DateCreated >= from
                        && f.DateCreated <= to
                        );
                }
                else
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => f.UserId == userId
                        && f.WithdrawalStatus != "Success"
                        && f.WithdrawalStatus != "Canceled"
                        && EF.Functions.Like(f.WithdrawalStatus, "%" + status + "%")
                        );
                }
            }
            else
            {
                if (from != null && to != null)
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => f.UserId == userId
                        && (EF.Functions.Like(f.WithdrawalStatus, "%" + "Success" + "%") || EF.Functions.Like(f.WithdrawalStatus, "%" + "Canceled" + "%"))
                        && f.DateCreated >= from
                        && f.DateCreated <= to
                        );
                }
                else
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => f.UserId == userId
                        && (EF.Functions.Like(f.WithdrawalStatus, "%" + "Success" + "%") || EF.Functions.Like(f.WithdrawalStatus, "%" + "Canceled" + "%"))
                        );
                }
            }

            IQueryable<WithdrawalEntry> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = withdrawalEntrys
                    .AsNoTracking()
                    .Include(w => w.WithdrawalBankOption)
                    .Where(w => w.WithdrawalId.ToString() == searchString
                    || EF.Functions.Like(w.WithdrawalBankOption.BankName, "%" + searchString + "%")
                    || EF.Functions.Like(w.AccountName, "%" + searchString + "%")
                    || EF.Functions.Like(w.AccountNumber, "%" + searchString + "%")
                    || EF.Functions.Like(w.Description, "%" + searchString + "%")
                    );
            }
            else
            {
                searchResult = withdrawalEntrys.AsNoTracking();
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

        public async Task<int> GetWithdrawalEntrysTotalCountByUserId(string userId, string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = null, string status = null, bool isInProcess = false)
        {
            IQueryable<WithdrawalEntry> withdrawalEntrys = null;

            if (isInProcess)
            {
                if (from != null && to != null)
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => f.UserId == userId
                        && f.WithdrawalStatus != "Success"
                        && f.WithdrawalStatus != "Canceled"
                        && EF.Functions.Like(f.WithdrawalStatus, "%" + status + "%")
                        && f.DateCreated >= from
                        && f.DateCreated <= to
                        );
                }
                else
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => f.UserId == userId
                        && f.WithdrawalStatus != "Success"
                        && f.WithdrawalStatus != "Canceled"
                        && EF.Functions.Like(f.WithdrawalStatus, "%" + status + "%")
                        );
                }
            }
            else
            {
                if (from != null && to != null)
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => f.UserId == userId
                        && (EF.Functions.Like(f.WithdrawalStatus, "%" + "Success" + "%") || EF.Functions.Like(f.WithdrawalStatus, "%" + "Canceled" + "%"))
                        && f.DateCreated >= from
                        && f.DateCreated <= to
                        );
                }
                else
                {
                    withdrawalEntrys = _applicationContext.WithdrawalEntrys
                        .Where(f => f.UserId == userId
                        && (EF.Functions.Like(f.WithdrawalStatus, "%" + "Success" + "%") || EF.Functions.Like(f.WithdrawalStatus, "%" + "Canceled" + "%"))
                        );
                }
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                return await withdrawalEntrys
                    .Include(w => w.WithdrawalBankOption)
                    .Where(w => w.WithdrawalId.ToString() == searchString
                    || EF.Functions.Like(w.WithdrawalBankOption.BankName, "%" + searchString + "%")
                    || EF.Functions.Like(w.AccountName, "%" + searchString + "%")
                    || EF.Functions.Like(w.AccountNumber, "%" + searchString + "%")
                    || EF.Functions.Like(w.Description, "%" + searchString + "%")
                    ).CountAsync();
            }
            else
            {
                return await withdrawalEntrys.CountAsync();

            }
        }



        public List<WithdrawalBankOption> GetWithdrawalBankOptions(int? pageIndex, int? take, string searchString = null, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<WithdrawalBankOption>();

            IQueryable<WithdrawalBank> withdrawalBankOptions = null;

            withdrawalBankOptions = _distributingContext.WithdrawalBanks;

            IQueryable<WithdrawalBank> searchResult;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchResult = withdrawalBankOptions
                    .Where(w => w.Id.ToString() == searchString
                    || w.BankName.Contains(searchString)
                    );
            }
            else
            {
                searchResult = withdrawalBankOptions;
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

        public async Task<int> GetWithdrawalBankOptionsTotalCount(string searchString = null)
        {
            var result = 0;
            if (!string.IsNullOrEmpty(searchString))
            {
                result = await _distributingContext.WithdrawalBanks
                    .Where(w => w.Id.ToString() == searchString
                    || w.BankName.Contains(searchString)
                    ).CountAsync();
            }
            else
            {
                result = await _distributingContext.WithdrawalBanks.CountAsync();
            }

            return result;
        }



        private WithdrawalBankOption MapWithdrawalBankOptionFromEntity(WithdrawalBank entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("The entity must be provided for mapping.");
            }

            var withdrawalBankOption = new WithdrawalBankOption
            {
                WithdrawalBankId = entity.Id,
                BankName = entity.BankName,
                DateCreated = entity.DateCreated.ToFullString()
            };

            return withdrawalBankOption;
        }

        private async Task<List<WithdrawalEntry>> GetSortedRecords(
            IQueryable<WithdrawalEntry> withdrawalEntrys,
            int? pageIndex, int? take, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<WithdrawalEntry>();

            if (pageIndex != null && take != null)
            {
                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "DateCreated")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await withdrawalEntrys
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await withdrawalEntrys
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "WithdrawalId")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await withdrawalEntrys
                               .OrderBy(f => f.WithdrawalId)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await withdrawalEntrys
                               .OrderByDescending(f => f.WithdrawalId)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "WithdrawalStatus")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await withdrawalEntrys
                               .OrderBy(f => f.WithdrawalStatus)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await withdrawalEntrys
                               .OrderByDescending(f => f.WithdrawalStatus)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "Username")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await withdrawalEntrys
                               .OrderBy(f => f.Username)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await withdrawalEntrys
                               .OrderByDescending(f => f.Username)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "UserType")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await withdrawalEntrys
                               .OrderBy(f => f.UserType)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await withdrawalEntrys
                               .OrderByDescending(f => f.UserType)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "TotalAmount")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await withdrawalEntrys
                               .OrderBy(f => f.TotalAmount)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await withdrawalEntrys
                               .OrderByDescending(f => f.TotalAmount)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "CommissionRateInThousandth")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await withdrawalEntrys
                                .OrderBy(f => f.CommissionRateInThousandth)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await withdrawalEntrys
                               .OrderByDescending(f => f.CommissionRateInThousandth)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "CommissionAmount")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await withdrawalEntrys
                                .OrderBy(f => f.CommissionAmount)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await withdrawalEntrys
                               .OrderByDescending(f => f.CommissionAmount)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "ActualAmount")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await withdrawalEntrys
                                .OrderBy(f => f.ActualAmount)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await withdrawalEntrys
                               .OrderByDescending(f => f.ActualAmount)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "ApprovedBy")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await withdrawalEntrys
                                .OrderBy(f => f.ApprovedByAdminName)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await withdrawalEntrys
                               .OrderByDescending(f => f.ApprovedByAdminName)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else if (sortField == "CancellationApprovedBy")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await withdrawalEntrys
                                .OrderBy(f => f.CancellationApprovedByAdminName)
                                .Skip((int)take * (int)pageIndex)
                                .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await withdrawalEntrys
                               .OrderByDescending(f => f.CancellationApprovedByAdminName)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                    else
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            result = await withdrawalEntrys
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                        else
                        {
                            result = await withdrawalEntrys
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take).ToListAsync();
                        }
                    }
                }
                else
                {
                    result = await withdrawalEntrys
                       .OrderByDescending(f => f.DateCreated)
                       .Skip((int)take * (int)pageIndex)
                       .Take((int)take)
                       .ToListAsync();
                }
            }
            else
            {
                result = await withdrawalEntrys.ToListAsync();
            }

            return result;
        }

        private List<WithdrawalBankOption> GetSortedRecords(
            IQueryable<WithdrawalBank> withdrawalBanks,
            int? pageIndex, int? take, string sortField = null, string direction = SortDirections.Descending)
        {
            var result = new List<WithdrawalBankOption>();

            if (pageIndex != null && take != null)
            {
                IEnumerable<WithdrawalBank> sortedResult = null;
                if (!string.IsNullOrEmpty(sortField))
                {
                    if (sortField == "DateCreated")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = withdrawalBanks
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = withdrawalBanks
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "BankName")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = withdrawalBanks
                               .OrderBy(f => f.BankName)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = withdrawalBanks
                               .OrderByDescending(f => f.BankName)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else if (sortField == "WithdrawalBankId")
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = withdrawalBanks
                               .OrderBy(f => f.Id)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = withdrawalBanks
                               .OrderByDescending(f => f.Id)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                    else
                    {
                        if (direction == SortDirections.Ascending)
                        {
                            sortedResult = withdrawalBanks
                               .OrderBy(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                        else
                        {
                            sortedResult = withdrawalBanks
                               .OrderByDescending(f => f.DateCreated)
                               .Skip((int)take * (int)pageIndex)
                               .Take((int)take);
                        }
                    }
                }
                else
                {
                    sortedResult = withdrawalBanks
                       .OrderByDescending(f => f.DateCreated)
                       .Skip((int)take * (int)pageIndex)
                       .Take((int)take);
                }

                foreach (var bank in sortedResult)
                {
                    result.Add(MapWithdrawalBankOptionFromEntity(bank));
                }
            }
            else
            {
                foreach (var bank in withdrawalBanks)
                {
                    result.Add(MapWithdrawalBankOptionFromEntity(bank));

                }
            }

            return result;
        }

        /*private async Task<IQueryable<WithdrawalEntry>> Search(IQueryable<WithdrawalEntry> withdrawalEntries)
        { 

        }*/




        public WithdrawalEntry Add(WithdrawalEntry withdrawalEntry)
        {
            return _applicationContext.WithdrawalEntrys.Add(withdrawalEntry).Entity;
        }

        public void Update(WithdrawalEntry withdrawalEntry)
        {
            _applicationContext.Entry(withdrawalEntry).State = EntityState.Modified;
        }

        public void Delete(WithdrawalEntry withdrawalEntry)
        {
            if (withdrawalEntry != null)
            {
                _applicationContext.WithdrawalEntrys.Remove(withdrawalEntry);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _applicationContext.SaveChangesAsync();
        }
    }
}
