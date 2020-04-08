using Distributing.Domain.Model.Banks;
using Distributing.Domain.Model.Roles;
using Distributing.Domain.Model.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Domain.Model.Balances
{
    public interface IBalanceDomainService
    {
        /// <summary>
        /// The available balance is Balance's available balance + Sum of Distributions.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<decimal> GetAvailableBalanceAsync(string userId);

        /// <summary>
        /// 1. Get the user's distributions and Sum the total.
        /// 2. Call the Balance.Distribute to add the amount to user's balance.
        /// 2. Call Balance.Withdrawal to run the withdrawal logics.
        /// 3. Delete the distributions retrieved at the start.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        Task Withdraw(string userId, decimal amount,
            WithdrawalBank withdrawalBank, string accountName, string accountNumber, string description,
            IDateTimeService dateTimeService, IOpenTimeService openTimeService);

        Task WithdrawByAdmin(string userId, decimal amount, Admin admin, string description, IDateTimeService dateTimeService);

        Task Freeze(string userId, decimal amount, Admin byAdmin, string description, IDateTimeService dateTimeService);
    }
}
