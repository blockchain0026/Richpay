using Distributing.Domain.Events;
using Distributing.Domain.Model.Withdrawals;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Withdrawals;
using WebMVC.Extensions;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.DomainEventHandlers.DistributingDomain
{
    /// <summary>
    /// Update view model: update withdrawal entry.
    /// </summary>
    public class WithdrawalSubmittedDomainEventHandler
                : INotificationHandler<WithdrawalSubmittedDomainEvent>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWithdrawalQueries _withdrawalQueries;

        public WithdrawalSubmittedDomainEventHandler(UserManager<ApplicationUser> userManager, IWithdrawalQueries withdrawalQueries)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _withdrawalQueries = withdrawalQueries ?? throw new ArgumentNullException(nameof(withdrawalQueries));
        }

        public async Task Handle(WithdrawalSubmittedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            //Update withdrawal entry.
            var withdrawal = domainEvent.Withdrawal;
            if (withdrawal.GetWithdrawalType.Id == WithdrawalType.ByUser.Id)
            {
                var user = await _userManager.FindByIdAsync(domainEvent.UserId);
                var withdrawalEntryVM = new WithdrawalEntry
                {
                    WithdrawalId = withdrawal.Id,
                    WithdrawalStatus = withdrawal.GetWithdrawalStatus.Name,
                    BalanceId = withdrawal.BalanceId,
                    UserId = user?.Id,
                    Username = user?.UserName,
                    FullName = user?.FullName,
                    UserType = user?.BaseRoleType.ToString(),
                    WithdrawalBankOption = new WithdrawalBankOption
                    {
                        BankName = withdrawal.BankAccount.BankName
                    },
                    AccountName = withdrawal.BankAccount.AccountName,
                    AccountNumber = withdrawal.BankAccount.AccountNumber,
                    TotalAmount = (int)withdrawal.TotalAmount,
                    CommissionRateInThousandth = (int)(withdrawal.CommissionRate * 1000),
                    CommissionAmount = withdrawal.CommissionAmount,
                    ActualAmount = withdrawal.ActualAmount,
                    ApprovedByAdminId = withdrawal.ApprovedBy?.AdminId,
                    ApprovedByAdminName = withdrawal.ApprovedBy?.Name,
                    CancellationApprovedByAdminId = withdrawal.CancellationApprovedBy?.AdminId,
                    CancellationApprovedByAdminName = withdrawal.CancellationApprovedBy?.Name,
                    Description = withdrawal.Description,
                    DateCreated = withdrawal.DateCreated
                };


                _withdrawalQueries.Add(withdrawalEntryVM);
                await _withdrawalQueries.SaveChangesAsync();
            }

        }
    }
}
