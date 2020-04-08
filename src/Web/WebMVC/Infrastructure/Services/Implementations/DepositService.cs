using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Banks;
using Distributing.Domain.Model.Deposits;
using Distributing.Domain.Model.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Deposits;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;

namespace WebMVC.Infrastructure.Services.Implementations
{
    public class DepositService : IDepositService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /*private readonly IBalanceQueries _balanceQueries;
        private readonly ICommissionQueries _commissionQueries;
        private readonly IBankbookQueries _bankbookQueries;*/
        private readonly IDepositQueries _depositQueries;

        private readonly IBalanceRepository _balanceRepository;
        private readonly IDepositRepository _depositRepository;
        private readonly IDepositAccountRepository _depositAccountRepository;

        private readonly IDateTimeService _dateTimeService;
        private readonly IOpenTimeService _openTimeService;

        public DepositService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IDepositQueries depositQueries, IBalanceRepository balanceRepository, IDepositRepository depositRepository, IDepositAccountRepository depositAccountRepository, IDateTimeService dateTimeService, IOpenTimeService openTimeService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _depositQueries = depositQueries ?? throw new ArgumentNullException(nameof(depositQueries));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _depositRepository = depositRepository ?? throw new ArgumentNullException(nameof(depositRepository));
            _depositAccountRepository = depositAccountRepository ?? throw new ArgumentNullException(nameof(depositAccountRepository));
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            _openTimeService = openTimeService ?? throw new ArgumentNullException(nameof(openTimeService));
        }


        public async Task<DepositBankAccount> GetDepositBankAccount(int depositBankAccountId)
        {
            var result = await _depositQueries.GetDepositBankAccount(depositBankAccountId);
            if (result == null)
            {
                throw new ArgumentNullException("No deposit bank account found by given Id.");
            }
            return result;
        }

        public List<DepositBankAccount> GetDepositBankAccounts(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending)
        {
            var result = this._depositQueries.GetDepositBankAccounts(
                pageIndex,
                take,
                searchString,
                sortField,
                direction
                );

            return result;
        }

        public async Task<int> GetDepositBankAccountsTotalCount(string searchString = null)
        {
            var count = await this._depositQueries.GetDepositBankAccountsTotalCount(searchString);

            return count;
        }

        public async Task<List<DepositEntry>> GetDepositEntrys(int pageIndex, int take, string searchByUserId, string searchString = "", string sortField = "",
            DateTime? from = null, DateTime? to = null,
            string userType = "", string status = "", string direction = SortDirections.Descending)
        {
            var user = await ValidateUserExist(searchByUserId);

            if (user.BaseRoleType != BaseRoleType.Manager)
            {
                if (userType == Roles.Trader)
                {
                    return await _depositQueries.GetDepositEntrysByUplineIdAsync(
                        searchByUserId,
                        pageIndex,
                        take,
                        searchString,
                        sortField,
                        from,
                        to,
                        userType,
                        status,
                        false,
                        direction
                        );
                }
                else
                {
                    var onlySelf = userType == Roles.TraderAgent ? true : false;
                    return await _depositQueries.GetDepositEntrysByUserIdAsync(
                        searchByUserId,
                        pageIndex,
                        take,
                        searchString,
                        sortField,
                        from,
                        to,
                        userType,
                        status,
                        false,
                        onlySelf,
                        direction
                        );
                }
            }

            return await _depositQueries.GetDepositEntrysAsync(
                pageIndex,
                take,
                searchString,
                sortField,
                from,
                to,
                userType,
                status,
                false,
                direction
                );
        }

        public async Task<int> GetDepositEntrysTotalCount(string searchByUserId, string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = "", string status = "")
        {
            var user = await ValidateUserExist(searchByUserId);
            if (user.BaseRoleType != BaseRoleType.Manager)
            {
                return await this._depositQueries.GetDepositEntrysTotalCountByUserId(
                    searchByUserId,
                    searchString,
                    from,
                    to,
                    userType,
                    status,
                    false
                    );
            }

            return await this._depositQueries.GetDepositEntrysTotalCount(
                searchString,
                from,
                to,
                userType,
                status,
                false
                );
        }

        public async Task<List<DepositEntry>> GetPendingDepositEntrys(int pageIndex, int take, string searchByUserId, string searchString = "", string sortField = "",
            DateTime? from = null, DateTime? to = null,
            string userType = "", string status = "", string direction = SortDirections.Descending)
        {
            var user = await ValidateUserExist(searchByUserId);

            if (user.BaseRoleType != BaseRoleType.Manager)
            {
                if (userType == Roles.Trader)
                {
                    return await _depositQueries.GetDepositEntrysByUplineIdAsync(
                        searchByUserId,
                        pageIndex,
                        take,
                        searchString,
                        sortField,
                        from,
                        to,
                        userType,
                        status,
                        true,
                        direction
                        );
                }
                else
                {
                    var onlySelf = userType == Roles.TraderAgent ? true : false;
                    return await _depositQueries.GetDepositEntrysByUserIdAsync(
                        searchByUserId,
                        pageIndex,
                        take,
                        searchString,
                        sortField,
                        from,
                        to,
                        userType,
                        status,
                        true,
                        onlySelf,
                        direction
                        );
                }
            }

            return await _depositQueries.GetDepositEntrysAsync(
                pageIndex,
                take,
                searchString,
                sortField,
                from,
                to,
                userType,
                status,
                true,
                direction
                );
        }

        public async Task<int> GetPendingDepositEntrysTotalCount(string searchByUserId, string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = "", string status = "")
        {
            var user = await ValidateUserExist(searchByUserId);
            if (user.BaseRoleType != BaseRoleType.Manager)
            {
                if (userType == Roles.Trader)
                {
                    return await this._depositQueries.GetDepositEntrysTotalCountByUplineId(
                        searchByUserId,
                        searchString,
                        from,
                        to,
                        userType,
                        status,
                        true
                        );
                }
                else
                {
                    return await this._depositQueries.GetDepositEntrysTotalCountByUserId(
                        searchByUserId,
                        searchString,
                        from,
                        to,
                        userType,
                        status,
                        true
                        );
                }
            }

            return await this._depositQueries.GetDepositEntrysTotalCount(
                searchString,
                from,
                to,
                userType,
                status,
                true
                );
        }

        public IEnumerable<SelectListItem> GetDepositBankAccountSelectList()
        {
            var selectList = new List<SelectListItem>();

            var bankAccounts = _depositQueries.GetDepositBankAccounts(null, null);

            selectList.Add(new SelectListItem()
            {
                Value = string.Empty,
                Text = "选择存款帳戶...",
                Selected = true
            });

            foreach (var bankAccount in bankAccounts)
            {
                selectList.Add(new SelectListItem()
                {
                    Value = bankAccount.BankAccountId.ToString(),
                    Text = bankAccount.Name,
                    Selected = false
                });
            }

            return selectList;
        }




        public async Task CreateDepositBankAccount(string name, string bankName, string accountName, string accountNumber)
        {
            var depositAccount = DepositAccount.From(
                name,
                bankName,
                accountName,
                accountNumber,
                _dateTimeService
                );

            this._depositAccountRepository.Add(depositAccount);
            await this._depositAccountRepository.UnitOfWork.SaveEntitiesAsync();
        }
        public async Task DeleteDepositBankAccount(int depositBankAccountId)
        {
            var depositAccount = await this._depositAccountRepository.GetByDepositAccountIdAsync(depositBankAccountId);
            if (depositAccount == null)
            {
                throw new KeyNotFoundException("No deposit account found by given id.");
            }

            this._depositAccountRepository.Delete(depositAccount);

            await this._depositAccountRepository.UnitOfWork.SaveChangesAsync();
        }
        public async Task DeleteDepositBankAccounts(List<int> ids)
        {
            foreach (var depositBankAccountId in ids)
            {
                var depositAccount = await this._depositAccountRepository.GetByDepositAccountIdAsync(depositBankAccountId);
                if (depositAccount == null)
                {
                    throw new KeyNotFoundException("No deposit account found by given id.");
                }

                this._depositAccountRepository.Delete(depositAccount);
            }
            await this._depositAccountRepository.UnitOfWork.SaveChangesAsync();
        }


        public async Task CreateDeposit(string userId, int depositAmount, string description, string createByUserId, int? depositAccountId = null)
        {
            //If the deposit isn't create by manager, check the deposit account is his account.
            var depositByUser = await _userManager.FindByIdAsync(createByUserId);
            if (depositByUser == null)
            {
                throw new KeyNotFoundException("No user found by given user id.");
            }

            var createByUpline = false;
            if (depositByUser.BaseRoleType != BaseRoleType.Manager)
            {
                //Trader Agent can create deposit for his trader.
                if (depositByUser.BaseRoleType == BaseRoleType.TraderAgent)
                {
                    //If the deposit is not for himself, check this is for his traders.
                    if (userId != depositByUser.Id)
                    {
                        var userToDeposit = await _userManager.Users
                            .Where(u => u.Id == userId)
                            .Select(u => new
                            {
                                u.Id,
                                u.UplineId,
                                u.BaseRoleType
                            })
                            .FirstOrDefaultAsync();

                        if (string.IsNullOrEmpty(userToDeposit.UplineId) || userToDeposit.UplineId != depositByUser.Id || userToDeposit.BaseRoleType != BaseRoleType.Trader)
                        {
                            throw new InvalidOperationException("代理只能为自身或直属交易员申请存款。");
                        }
                        else
                        {
                            createByUpline = true;
                        }
                    }
                }
                else if (userId != depositByUser.Id)
                {
                    throw new InvalidOperationException($"A {depositByUser.BaseRoleType} Can only deposit to his account.");
                }
            }

            //Checking the balance is exist.
            var balance = await _balanceRepository.GetByUserIdAsync(userId);
            if (balance == null)
            {
                throw new KeyNotFoundException($"No balance found by given user Id: {userId}. ");
            }

            //Create a new deposit entity.
            if (depositAccountId == null)
            {
                throw new ArgumentNullException("The deposit account must be provided when user submit a deposit request.");
            }

            var depositAccount = await _depositAccountRepository.GetByDepositAccountIdAsync((int)depositAccountId);
            if (depositAccount == null)
            {
                throw new KeyNotFoundException("No deposit account found by given id.");
            }

            Deposit deposit = null;
            if (createByUpline)
            {
                deposit = Deposit.FromUser(
                    balance,
                    depositAccount,
                    depositAmount,
                    description,
                    _dateTimeService,
                    _openTimeService,
                    depositByUser.Id
                    );
            }
            else
            {
                deposit = Deposit.FromUser(
                    balance,
                    depositAccount,
                    depositAmount,
                    description,
                    _dateTimeService,
                    _openTimeService,
                    null
                    );
            }

            _depositRepository.Add(deposit);
            //Save changes and execute related domain events.
            await _depositRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task VerifyDeposit(int depositId, string verifyByAdminId)
        {
            //Checking the admin is exist.
            var admin = await _userManager.FindByIdAsync(verifyByAdminId);
            if (admin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (admin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can verify a deposit.");
            }

            //Checking the deposit is exist.
            var deposit = await this._depositRepository.GetByDepositIdAsync(depositId);
            if (deposit == null)
            {
                throw new KeyNotFoundException("No deposit found by given deposit id.");
            }

            //Checking the balance is exist.
            var balance = await _balanceRepository.GetByBalanceIdAsync(deposit.BalanceId);
            if (balance == null)
            {
                throw new KeyNotFoundException($"No balance found.");
            }


            //Verify
            deposit.Verify(
                new Distributing.Domain.Model.Roles.Admin(admin.Id, admin.UserName),
                balance,
                _dateTimeService
                );


            //Update and save entity.
            _depositRepository.Update(deposit);
            await _depositRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task VerifyDeposits(List<int> depositIds, string verifyByAdminId)
        {
            //Checking the admin is exist.
            var admin = await _userManager.FindByIdAsync(verifyByAdminId);
            if (admin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (admin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can verify a deposit.");
            }

            foreach (var depositId in depositIds)
            {
                //Checking the deposit is exist.
                var deposit = await this._depositRepository.GetByDepositIdAsync(depositId);
                if (deposit == null)
                {
                    throw new KeyNotFoundException("No deposit found by given deposit id.");
                }

                //Checking the balance is exist.
                var balance = await _balanceRepository.GetByBalanceIdAsync(deposit.BalanceId);
                if (balance == null)
                {
                    throw new KeyNotFoundException($"No balance found.");
                }


                //Verify
                deposit.Verify(
                    new Distributing.Domain.Model.Roles.Admin(admin.Id, admin.UserName),
                    balance,
                    _dateTimeService
                    );

                //Update entity
                _depositRepository.Update(deposit);
            }

            //Save entities.
            await _depositRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task CancelDeposit(int depositId, string cancelByUserId)
        {
            //Checking the user is exist.
            var cancelByUser = await _userManager.FindByIdAsync(cancelByUserId);
            if (cancelByUser == null)
            {
                throw new KeyNotFoundException("No user found by given user id.");
            }

            //Checking the deposit is exist.
            var deposit = await this._depositRepository.GetByDepositIdAsync(depositId);
            if (deposit == null)
            {
                throw new KeyNotFoundException("No deposit found by given deposit id.");
            }


            if (deposit.UserId != cancelByUser.Id)
            {
                //Checking the deposit is cancel by his upline trader agent and the deposit is for trader.
                if (cancelByUser.BaseRoleType == BaseRoleType.TraderAgent)
                {
                    var userToCancel = await _userManager.Users
                                .Where(u => u.Id == deposit.UserId)
                                .Select(u => new
                                {
                                    u.Id,
                                    u.UplineId,
                                    u.BaseRoleType
                                })
                                .FirstOrDefaultAsync();

                    if (userToCancel.BaseRoleType != BaseRoleType.Trader || string.IsNullOrEmpty(userToCancel.UplineId) || userToCancel.UplineId != cancelByUserId)
                    {
                        throw new InvalidOperationException("代理只能取消自己或直属交易员的存款。");
                    }
                }
                else
                {
                    throw new InvalidOperationException("用户只能取消自己的存款。");
                }
            }

            deposit.Cancel(_dateTimeService);

            _depositRepository.Update(deposit);
            await _depositRepository.UnitOfWork.SaveEntitiesAsync();
        }


        private async Task<ApplicationUser> ValidateUserExist(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("No user found by given user id.");
            }
            return user;
        }
    }
}
