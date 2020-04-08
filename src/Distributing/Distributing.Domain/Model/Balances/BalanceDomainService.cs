using Distributing.Domain.Exceptions;
using Distributing.Domain.Model.Banks;
using Distributing.Domain.Model.Distributions;
using Distributing.Domain.Model.Roles;
using Distributing.Domain.Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributing.Domain.Model.Balances
{
    public class BalanceDomainService : IBalanceDomainService
    {
        private readonly IDistributionRepository _distributionRepository;
        private readonly IBalanceRepository _balanceRepository;

        public BalanceDomainService(IDistributionRepository distributionRepository, IBalanceRepository balanceRepository)
        {
            _distributionRepository = distributionRepository ?? throw new ArgumentNullException(nameof(distributionRepository));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
        }


        public async Task<decimal> GetAvailableBalanceAsync(string userId)
        {
            //Get user's available balance.
            var amountAvailable = await _balanceRepository.GetAmountAvailableByUserIdAsync(userId);

            //Get sum of user's distributions.
            var distributionSum = await _distributionRepository.GetSumOfDistributedAmountByUserIdAsync(userId);

            return amountAvailable + distributionSum;
        }


        public async Task Withdraw(string userId, decimal amount,
            WithdrawalBank withdrawalBank, string accountName, string accountNumber, string description,
            IDateTimeService dateTimeService, IOpenTimeService openTimeService)
        {
            //Get the user's distributions.
            var distributions = await _distributionRepository.GetByUserIdAsync(userId, true);

            //Sum the total.
            var distributedAmount = distributions.Sum(d => d.DistributedAmount);

            //Distribute the amount to balance.
            var balance = await _balanceRepository.GetByUserIdAsync(userId);
            if (balance is null)
            {
                throw new DistributingDomainException("No balance found by given user Id.");
            }
            balance.Distribute(distributedAmount);

            //Withdraw.
            balance.Withdraw(
                amount,
                withdrawalBank,
                accountName,
                accountNumber,
                description,
                dateTimeService,
                openTimeService
                );
            _balanceRepository.Update(balance);


            //Delete the distributions.
            _distributionRepository.DeleteRange(distributions);
        }


        public async Task WithdrawByAdmin(string userId, decimal amount, Admin admin, string description, IDateTimeService dateTimeService)
        {
            //Get the user's distributions.
            var distributions = await _distributionRepository.GetByUserIdAsync(userId, true);

            //Sum the total.
            var distributedAmount = distributions.Sum(d => d.DistributedAmount);

            //Distribute the amount to balance.
            var balance = await _balanceRepository.GetByUserIdAsync(userId);
            if (balance is null)
            {
                throw new DistributingDomainException("No balance found by given user Id.");
            }
            balance.Distribute(distributedAmount);

            //Withdraw.
            balance.WithdrawByAdmin(
                admin,
                amount,
                description,
                dateTimeService
                );
            _balanceRepository.Update(balance);

            //Delete the distributions.
            _distributionRepository.DeleteRange(distributions);
        }


        public async Task Freeze(string userId, decimal amount, Admin byAdmin, string description, IDateTimeService dateTimeService)
        {
            //Get the user's distributions.
            var distributions = await _distributionRepository.GetByUserIdAsync(userId, true);

            //Sum the total.
            var distributedAmount = distributions.Sum(d => d.DistributedAmount);

            //Distribute the amount to balance.
            var balance = await _balanceRepository.GetByUserIdAsync(userId);
            if (balance is null)
            {
                throw new DistributingDomainException("No balance found by given user Id.");
            }
            balance.Distribute(distributedAmount);

            //Freeze.
            balance.Freeze(
                byAdmin,
                amount,
                description,
                dateTimeService
                );
            _balanceRepository.Update(balance);

            //Delete the distributions.
            _distributionRepository.DeleteRange(distributions);
        }
    }
}
