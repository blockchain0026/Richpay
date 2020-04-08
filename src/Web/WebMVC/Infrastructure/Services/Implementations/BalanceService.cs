using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Deposits;
using Distributing.Domain.Model.Roles;
using Distributing.Domain.Model.Shared;
using Distributing.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Bankbook;
using WebMVC.Data;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;

namespace WebMVC.Infrastructure.Services.Implementations
{
    public class BalanceService : IBalanceService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IBankbookQueries _bankbookQueries;

        private readonly IBalanceRepository _balanceRepository;
        private readonly IDepositRepository _depositRepository;

        private readonly Distributing.Domain.Model.Shared.IDateTimeService _distributingDateTimeService;
    
        private readonly IBalanceDomainService _balanceDomainService;

        private readonly DistributingContext _distributingContext;
        private readonly ApplicationDbContext _applicationDbContext;

        public BalanceService(UserManager<ApplicationUser> userManager, IBankbookQueries bankbookQueries, IBalanceRepository balanceRepository, IDepositRepository depositRepository, IDateTimeService distributingDateTimeService, IBalanceDomainService balanceDomainService, DistributingContext distributingContext, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _bankbookQueries = bankbookQueries ?? throw new ArgumentNullException(nameof(bankbookQueries));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _depositRepository = depositRepository ?? throw new ArgumentNullException(nameof(depositRepository));
            _distributingDateTimeService = distributingDateTimeService ?? throw new ArgumentNullException(nameof(distributingDateTimeService));
            _balanceDomainService = balanceDomainService ?? throw new ArgumentNullException(nameof(balanceDomainService));
            _distributingContext = distributingContext ?? throw new ArgumentNullException(nameof(distributingContext));
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        }

        public async Task ChangeBalance(string type, string userId, decimal amount, string description, string createByUserId)
        {
            //Checking the changing is made by manager.
            var changeByUser = await _userManager.FindByIdAsync(createByUserId);
            if (changeByUser.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException($"Only manager can change shop's balance.");
            }

            //Checking the balance is exist.
            /*var balance = await _balanceRepository.GetByUserIdAsync(userId);
            if (balance == null)
            {
                throw new KeyNotFoundException($"No balance found by given user Id: {userId}. ");
            }*/

            var admin = new Distributing.Domain.Model.Roles.Admin(changeByUser.Id, changeByUser.UserName);
            if (type == BalanceChangeType.Deposit)
            {
                //Checking the balance is exist.
                var balance = await _balanceRepository.GetByUserIdAsync(userId);
                if (balance == null)
                {
                    throw new KeyNotFoundException($"No balance found by given user Id: {userId}. ");
                }

                //Create a new deposit entity.
                var deposit = Deposit.FromAdmin(
                    balance,
                    admin,
                    amount,
                    description,
                    _distributingDateTimeService
                    );
                _depositRepository.Add(deposit);

            }
            else if (type == BalanceChangeType.Withdraw)
            {
                /*balance.WithdrawByAdmin(
                    admin,
                    amount,
                    description,
                    _distributingDateTimeService
                    );
                _balanceRepository.Update(balance);*/

                await this.WithdrawByAdmin(
                    userId,
                    amount,
                    admin,
                    description
                    );
            }
            else if (type == BalanceChangeType.Freeze)
            {
                /*balance.Freeze(
                    admin,
                    amount,
                    description,
                    _distributingDateTimeService
                    );
                _balanceRepository.Update(balance);*/

                await this.FreezeBalance(
                    userId,
                    amount,
                    admin,
                    description
                    );
            }
            else
            {
                throw new ArgumentOutOfRangeException("The balance change type is invalid.");
            }


            //Save changes and execute related domain events.
            await _depositRepository.UnitOfWork.SaveEntitiesAsync();
        }


        private async Task FreezeBalance(string userId, decimal amount, Admin admin, string description)
        {
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
                                //Freeze.
                                await _balanceDomainService.Freeze(
                                    userId,
                                    amount,
                                    admin,
                                    description,
                                    this._distributingDateTimeService
                                    );


                                //Save changes and execute related domain events.
                                await _balanceRepository.UnitOfWork.SaveEntitiesAsync();

                                // Saves all view model created at previous processes.
                                // The Queries all belongs to one db ccontext, so only need to save once.
                                await _bankbookQueries.SaveChangesAsync();

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

        private async Task WithdrawByAdmin(string userId, decimal amount, Admin admin, string description)
        {
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
                                //Freeze.
                                await _balanceDomainService.WithdrawByAdmin(
                                    userId,
                                    amount,
                                    admin,
                                    description,
                                    this._distributingDateTimeService
                                    );


                                //Save changes and execute related domain events.
                                await _balanceRepository.UnitOfWork.SaveEntitiesAsync();

                                // Saves all view model created at previous processes.
                                // The Queries all belongs to one db ccontext, so only need to save once.
                                await _bankbookQueries.SaveChangesAsync();

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

    }
}
