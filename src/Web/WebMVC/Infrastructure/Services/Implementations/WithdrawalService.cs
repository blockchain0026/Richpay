using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Banks;
using Distributing.Domain.Model.Deposits;
using Distributing.Domain.Model.Shared;
using Distributing.Domain.Model.Withdrawals;
using Distributing.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Deposits;
using WebMVC.Applications.Queries.Withdrawals;
using WebMVC.Data;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;

namespace WebMVC.Infrastructure.Services.Implementations
{
    public class WithdrawalService : IWithdrawalService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IWithdrawalQueries _withdrawalQueries;

        private readonly IBalanceRepository _balanceRepository;
        private readonly IWithdrawalRepository _withdrawalRepository;
        private readonly IWithdrawalBankRepository _withdrawalBankRepository;

        private readonly IDateTimeService _dateTimeService;
        private readonly IOpenTimeService _openTimeService;

        private readonly IBalanceDomainService _balanceDomainService;

        private readonly DistributingContext _distributingContext;
        private readonly ApplicationDbContext _applicationDbContext;

        public WithdrawalService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IWithdrawalQueries withdrawalQueries, IBalanceRepository balanceRepository, IWithdrawalRepository withdrawalRepository, IWithdrawalBankRepository withdrawalBankRepository, IDateTimeService dateTimeService, IOpenTimeService openTimeService, IBalanceDomainService balanceDomainService, DistributingContext distributingContext, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _withdrawalQueries = withdrawalQueries ?? throw new ArgumentNullException(nameof(withdrawalQueries));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _withdrawalRepository = withdrawalRepository ?? throw new ArgumentNullException(nameof(withdrawalRepository));
            _withdrawalBankRepository = withdrawalBankRepository ?? throw new ArgumentNullException(nameof(withdrawalBankRepository));
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            _openTimeService = openTimeService ?? throw new ArgumentNullException(nameof(openTimeService));
            _balanceDomainService = balanceDomainService ?? throw new ArgumentNullException(nameof(balanceDomainService));
            _distributingContext = distributingContext ?? throw new ArgumentNullException(nameof(distributingContext));
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        }

        public List<WithdrawalBankOption> GetWithdrawalBankOptions(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending)
        {
            var result = this._withdrawalQueries.GetWithdrawalBankOptions(
                pageIndex,
                take,
                searchString,
                sortField,
                direction
                );

            return result;
        }

        public async Task<int> GetWithdrawalBankOptionsTotalCount(string searchString = null)
        {
            var count = await this._withdrawalQueries.GetWithdrawalBankOptionsTotalCount(searchString);

            return count;
        }

        public async Task<List<WithdrawalEntry>> GetWithdrawalEntrys(int pageIndex, int take, string searchByUserId, string searchString = "", string sortField = "",
            DateTime? from = null, DateTime? to = null,
            string userType = "", string direction = SortDirections.Descending)
        {
            var user = await ValidateUserExist(searchByUserId);
            if (user.BaseRoleType != BaseRoleType.Manager)
            {
                return await _withdrawalQueries.GetWithdrawalEntrysByUserIdAsync(
                    searchByUserId,
                    pageIndex,
                    take,
                    searchString,
                    sortField,
                    from,
                    to,
                    userType,
                    null,
                    false,
                    direction
                    );
            }

            return await _withdrawalQueries.GetWithdrawalEntrysAsync(
                pageIndex,
                take,
                searchString,
                sortField,
                from,
                to,
                userType,
                null,
                false,
                direction
                );
        }

        public async Task<int> GetWithdrawalEntrysTotalCount(string searchByUserId, string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = "")
        {
            var user = await ValidateUserExist(searchByUserId);

            if (user.BaseRoleType != BaseRoleType.Manager)
            {
                return await this._withdrawalQueries.GetWithdrawalEntrysTotalCountByUserId(
                    searchByUserId,
                    searchString,
                    from,
                    to,
                    userType,
                    null,
                    false
                    );
            }

            return await this._withdrawalQueries.GetWithdrawalEntrysTotalCount(
                searchString,
                from,
                to,
                userType,
                null,
                false
                );
        }

        public async Task<List<WithdrawalEntry>> GetPendingWithdrawalEntrys(int pageIndex, int take, string searchByUserId, string searchString = "", string sortField = "",
            DateTime? from = null, DateTime? to = null,
            string userType = "", string status = "", string direction = SortDirections.Descending)
        {
            var user = await ValidateUserExist(searchByUserId);
            if (user.BaseRoleType != BaseRoleType.Manager)
            {
                return await _withdrawalQueries.GetWithdrawalEntrysByUserIdAsync(
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

            return await _withdrawalQueries.GetWithdrawalEntrysAsync(
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

        public async Task<int> GetPendingWithdrawalEntrysTotalCount(string searchByUserId, string searchString = null,
            DateTime? from = null, DateTime? to = null,
            string userType = "", string status = "")
        {
            var user = await ValidateUserExist(searchByUserId);

            if (user.BaseRoleType != BaseRoleType.Manager)
            {
                return await this._withdrawalQueries.GetWithdrawalEntrysTotalCountByUserId(
                    searchByUserId,
                    searchString,
                    from,
                    to,
                    userType,
                    status,
                    true
                    );
            }

            return await this._withdrawalQueries.GetWithdrawalEntrysTotalCount(
                searchString,
                from,
                to,
                userType,
                status,
                true
                );
        }

        public IEnumerable<SelectListItem> GetWithdrawalBankOptionSelectList()
        {
            var selectList = new List<SelectListItem>();

            var bankOptions = _withdrawalQueries.GetWithdrawalBankOptions(null, null);

            selectList.Add(new SelectListItem()
            {
                Value = string.Empty,
                Text = "选择提现银行...",
                Selected = true
            });

            foreach (var bankOption in bankOptions)
            {
                selectList.Add(new SelectListItem()
                {
                    Value = bankOption.WithdrawalBankId.ToString(),
                    Text = bankOption.BankName,
                    Selected = false
                });
            }

            return selectList;
        }




        public async Task CreateWithdrawalBankOption(string bankName)
        {
            var withdrawalBank = WithdrawalBank.From(
                bankName,
                _dateTimeService
                );

            this._withdrawalBankRepository.Add(withdrawalBank);
            await this._withdrawalBankRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task DeleteWithdrawalBankOption(int withdrawalBankAccountId)
        {
            var withdrawalBank = await this._withdrawalBankRepository.GetByWithdrawalBankIdAsync(withdrawalBankAccountId);
            if (withdrawalBank == null)
            {
                throw new KeyNotFoundException("No withdrawal bank found by given id.");
            }

            this._withdrawalBankRepository.Delete(withdrawalBank);

            await this._withdrawalBankRepository.UnitOfWork.SaveChangesAsync();
        }

        public async Task DeleteWithdrawalBankOptions(List<int> ids)
        {
            foreach (var withdrawalBankAccountId in ids)
            {
                var withdrawalBank = await this._withdrawalBankRepository.GetByWithdrawalBankIdAsync(withdrawalBankAccountId);
                if (withdrawalBank == null)
                {
                    throw new KeyNotFoundException("No withdrawal bank found by given id.");
                }

                this._withdrawalBankRepository.Delete(withdrawalBank);
            }
            await this._withdrawalBankRepository.UnitOfWork.SaveChangesAsync();
        }


        public async Task CreateWithdrawal(string userId, int withdrawalAmount, int withdrawalBankOptionId, string accountName, string accountNumber, string description, string createByUserId)
        {
            //If the withdrawal isn't create by manager, check the withdrawal account is his account.
            var withdrawalByUser = await _userManager.FindByIdAsync(createByUserId);
            if (withdrawalByUser == null)
            {
                throw new KeyNotFoundException("No user found by given user id.");
            }

            if (withdrawalByUser.BaseRoleType != BaseRoleType.Manager)
            {
                if (userId != withdrawalByUser.Id)
                {
                    throw new InvalidOperationException($"A {withdrawalByUser.BaseRoleType} Can only withdrawal to his account.");
                }
            }

            //Checking the balance is exist.
            /*var balance = await _balanceRepository.GetByUserIdAsync(userId);
            if (balance == null)
            {
                throw new KeyNotFoundException($"No balance found by given user Id: {userId}. ");
            }
            */

            var distribuingStrategy = _distributingContext.Database.CreateExecutionStrategy();
            var applicationStrategy = _applicationDbContext.Database.CreateExecutionStrategy();

            await distribuingStrategy.ExecuteAsync(async () =>
            {
                await applicationStrategy.ExecuteAsync(async () =>
                {
                    using (var distributingTransaction = await _distributingContext.BeginTransactionAsync())
                    {
                        using (var applicationTransaction = await _applicationDbContext.BeginTransactionAsync())
                        {
                            try
                            {
                                //Withdraw.
                                var withdrawalBank = await this._withdrawalBankRepository.GetByWithdrawalBankIdAsync(withdrawalBankOptionId);
                                if (withdrawalBank == null)
                                {
                                    throw new ArgumentNullException("The withdrawal bank must be provided when user submit a withdrawal request.");
                                }

                                await _balanceDomainService.Withdraw(
                                    userId,
                                    withdrawalAmount,
                                    withdrawalBank,
                                    accountName,
                                    accountNumber,
                                    description,
                                    this._dateTimeService,
                                    this._openTimeService
                                    );

                                /*balance.Withdraw(
                                    withdrawalAmount,
                                    withdrawalBank,
                                    accountName,
                                    accountNumber,
                                    description,
                                    _dateTimeService,
                                    _openTimeService
                                    );*/

                                //Save changes and execute related domain events.
                                await _balanceRepository.UnitOfWork.SaveEntitiesAsync();

                                // Saves all view model created at previous processes.
                                // The Queries all belongs to one db ccontext, so only need to save once.
                                await _withdrawalQueries.SaveChangesAsync();

                                //Commit Transaction.
                                //If there is any error, transaction will rollback, and throw the error after the rollback.
                                await _distributingContext.CommitTransactionOnlyAsync(distributingTransaction);
                                await _applicationDbContext.CommitTransactionOnlyAsync(applicationTransaction);

                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                //If the transaction failed because of concurrencies and conflicts,
                                //Rollback the change to database.
                                _distributingContext.RollbackTransaction();
                                _applicationDbContext.RollbackTransaction();

                                throw new DbUpdateConcurrencyException("系统繁忙中，请稍后几秒再尝试。"); //Throw the exception and return the message to the client.
                            }
                            catch
                            {
                                _distributingContext.RollbackTransaction();
                                _applicationDbContext.RollbackTransaction();

                                throw;
                            }
                            finally
                            {
                                _distributingContext.DisposeTransaction();
                                _applicationDbContext.DisposeTransaction();
                            }

                        }
                    }
                });
            });

        }


        public async Task ApproveWithdrawal(int withdrawalId, string aprrovedByAdminId)
        {
            //Checking the admin is exist.
            var admin = await _userManager.FindByIdAsync(aprrovedByAdminId);
            if (admin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (admin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can approve a withdrawal.");
            }

            //Checking the withdrawal is exist.
            var withdrawal = await this._withdrawalRepository.GetByWithdrawalIdAsync(withdrawalId);
            if (withdrawal == null)
            {
                throw new KeyNotFoundException("No withdrawal found by given withdrawal id.");
            }

            //Approve
            withdrawal.Approve(
                new Distributing.Domain.Model.Roles.Admin(admin.Id, admin.UserName)
                );

            _withdrawalRepository.Update(withdrawal);
            await _withdrawalRepository.UnitOfWork.SaveEntitiesAsync();
        }


        public async Task ApproveWithdrawals(List<int> withdrawalIds, string aprrovedByAdminId)
        {
            //Checking the admin is exist.
            var admin = await _userManager.FindByIdAsync(aprrovedByAdminId);
            if (admin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (admin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can approve a withdrawal.");
            }


            foreach (var withdrawalId in withdrawalIds)
            {
                //Checking the withdrawal is exist.
                var withdrawal = await this._withdrawalRepository.GetByWithdrawalIdAsync(withdrawalId);
                if (withdrawal == null)
                {
                    throw new KeyNotFoundException("No withdrawal found by given withdrawal id.");
                }

                //Checking the balance is exist.
                /*var balance = await _balanceRepository.GetByBalanceIdAsync(withdrawal.BalanceId);
                if (balance == null)
                {
                    throw new KeyNotFoundException($"No balance found.");
                }*/


                withdrawal.Approve(
                    new Distributing.Domain.Model.Roles.Admin(admin.Id, admin.UserName)
                    );

                _withdrawalRepository.Update(withdrawal);
            }

            await _withdrawalRepository.UnitOfWork.SaveEntitiesAsync();
        }



        public async Task ConfirmWithdrawalPaymentReceived(int withdrawalId, string confirmByUserId)
        {
            //Checking the user is exist.
            var confirmByUser = await _userManager.FindByIdAsync(confirmByUserId);
            if (confirmByUser == null)
            {
                throw new KeyNotFoundException("No user found by given admin id.");
            }

            //Checking the withdrawal is exist.
            var withdrawal = await this._withdrawalRepository.GetByWithdrawalIdAsync(withdrawalId);
            if (withdrawal == null)
            {
                throw new KeyNotFoundException("No withdrawal found by given withdrawal id.");
            }

            //Checking the withdrawal is belong to the user.
            if (withdrawal.UserId != confirmByUser.Id)
            {
                throw new InvalidOperationException("User can not confirm a withdrawal that is not create by him.");
            }

            withdrawal.ConfirmPaymentReceived(
                confirmByUser.Id,
                _dateTimeService
                );

            _withdrawalRepository.Update(withdrawal);
            await _withdrawalRepository.UnitOfWork.SaveEntitiesAsync();
        }



        public async Task ForceWithdrawalSuccess(int withdrawalId, string forcedByAdminId)
        {
            //Checking the admin is exist.
            var forcedByAdmin = await _userManager.FindByIdAsync(forcedByAdminId);
            if (forcedByAdmin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (forcedByAdmin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can force a withdrawal success.");
            }

            //Checking the withdrawal is exist.
            var withdrawal = await this._withdrawalRepository.GetByWithdrawalIdAsync(withdrawalId);
            if (withdrawal == null)
            {
                throw new KeyNotFoundException("No withdrawal found by given withdrawal id.");
            }

            withdrawal.ForceSuccess(
                new Distributing.Domain.Model.Roles.Admin(forcedByAdmin.Id, forcedByAdmin.UserName),
                _dateTimeService
                );

            _withdrawalRepository.Update(withdrawal);
            await _withdrawalRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task ForceWithdrawalsSuccess(List<int> withdrawalIds, string forcedByAdminId)
        {
            //Checking the admin is exist.
            var forcedByAdmin = await _userManager.FindByIdAsync(forcedByAdminId);
            if (forcedByAdmin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (forcedByAdmin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can force a withdrawal success.");
            }

            foreach (var withdrawalId in withdrawalIds)
            {
                //Checking the withdrawal is exist.
                var withdrawal = await this._withdrawalRepository.GetByWithdrawalIdAsync(withdrawalId);
                if (withdrawal == null)
                {
                    throw new KeyNotFoundException("No withdrawal found by given withdrawal id.");
                }

                withdrawal.ForceSuccess(
                    new Distributing.Domain.Model.Roles.Admin(forcedByAdmin.Id, forcedByAdmin.UserName),
                    _dateTimeService
                    );

                _withdrawalRepository.Update(withdrawal);
            }
            await _withdrawalRepository.UnitOfWork.SaveEntitiesAsync();
        }


        public async Task CancelWithdrawal(int withdrawalId, string cancelByUserId)
        {
            //Checking the user is exist.
            var cancelByUser = await _userManager.FindByIdAsync(cancelByUserId);
            if (cancelByUser == null)
            {
                throw new KeyNotFoundException("No user found by given user id.");
            }

            //Checking the withdrawal is exist.
            var withdrawal = await this._withdrawalRepository.GetByWithdrawalIdAsync(withdrawalId);
            if (withdrawal == null)
            {
                throw new KeyNotFoundException("No withdrawal found by given withdrawal id.");
            }

            //Checking the withdrawal is create by the user.
            if (withdrawal.UserId != cancelByUser.Id)
            {
                throw new InvalidOperationException("User can not cancel someone else's withdrawal.");
            }

            withdrawal.Cancel(cancelByUser.Id, _dateTimeService);

            _withdrawalRepository.Update(withdrawal);
            await _withdrawalRepository.UnitOfWork.SaveEntitiesAsync();
        }


        public async Task ApproveCancellation(int withdrawalId, string aprrovedByAdminId)
        {
            //Checking the admin is exist.
            var aprrovedByAdmin = await _userManager.FindByIdAsync(aprrovedByAdminId);
            if (aprrovedByAdmin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (aprrovedByAdmin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can force a withdrawal success.");
            }

            //Checking the withdrawal is exist.
            var withdrawal = await this._withdrawalRepository.GetByWithdrawalIdAsync(withdrawalId);
            if (withdrawal == null)
            {
                throw new KeyNotFoundException("No withdrawal found by given withdrawal id.");
            }

            withdrawal.ApproveCancellation(
                new Distributing.Domain.Model.Roles.Admin(aprrovedByAdmin.Id, aprrovedByAdmin.UserName),
                _dateTimeService
                );

            _withdrawalRepository.Update(withdrawal);
            await _withdrawalRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task ApproveCancellations(List<int> withdrawalIds, string aprrovedByAdminId)
        {
            //Checking the admin is exist.
            var aprrovedByAdmin = await _userManager.FindByIdAsync(aprrovedByAdminId);
            if (aprrovedByAdmin == null)
            {
                throw new KeyNotFoundException("No admin found by given admin id.");
            }
            if (aprrovedByAdmin.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Only admin can force a withdrawal success.");
            }

            foreach (var withdrawalId in withdrawalIds)
            {
                //Checking the withdrawal is exist.
                var withdrawal = await this._withdrawalRepository.GetByWithdrawalIdAsync(withdrawalId);
                if (withdrawal == null)
                {
                    throw new KeyNotFoundException("No withdrawal found by given withdrawal id.");
                }

                withdrawal.ApproveCancellation(
                    new Distributing.Domain.Model.Roles.Admin(aprrovedByAdmin.Id, aprrovedByAdmin.UserName),
                    _dateTimeService
                    );

                _withdrawalRepository.Update(withdrawal);

            }

            await _withdrawalRepository.UnitOfWork.SaveEntitiesAsync();
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
