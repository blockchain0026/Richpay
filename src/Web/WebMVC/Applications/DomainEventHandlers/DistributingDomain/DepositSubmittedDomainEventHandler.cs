using Distributing.Domain.Events;
using Distributing.Domain.Model.Deposits;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Deposits;
using WebMVC.Extensions;
using WebMVC.Models.Queries;

namespace WebMVC.Applications.DomainEventHandlers.DistributingDomain
{
    /// <summary>
    /// Update view model when balance created.
    /// </summary>
    public class DepositSubmittedDomainEventHandler
         : INotificationHandler<DepositSubmittedDomainEvent>
    {
        private readonly IDepositQueries _depositQueries;
        private readonly UserManager<ApplicationUser> _userManager;

        public DepositSubmittedDomainEventHandler(IDepositQueries depositQueries, UserManager<ApplicationUser> userManager)
        {
            _depositQueries = depositQueries ?? throw new ArgumentNullException(nameof(depositQueries));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task Handle(DepositSubmittedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var deposit = domainEvent.Deposit;

            if (deposit.GetDepositType.Id == DepositType.ByUser.Id)
            {
                var user = await _userManager.FindByIdAsync(domainEvent.UserId);
                var depositEntryVM = new DepositEntry
                {
                    DepositId = deposit.Id,
                    DepositStatus = deposit.GetDepositStatus.Name,
                    BalanceId = deposit.BalanceId,
                    UserId = user?.Id,
                    CreateByUplineId = deposit.CreateByUplineId,
                    Username = user?.UserName,
                    FullName = user?.FullName,
                    UserType = user?.BaseRoleType.ToString(),
                    DepositBankAccount = new DepositBankAccount
                    {
                        BankName = deposit.BankAccount.BankName,
                        AccountName = deposit.BankAccount.AccountName,
                        AccountNumber = deposit.BankAccount.AccountNumber,
                    },
                    TotalAmount = (int)deposit.TotalAmount,
                    CommissionRateInThousandth = (int)(deposit.CommissionRate * 1000),
                    CommissionAmount = deposit.CommissionAmount,
                    ActualAmount = deposit.ActualAmount,
                    VerifiedByAdminId = deposit.VerifiedBy?.AdminId,
                    VerifiedByAdminName = deposit.VerifiedBy?.Name,
                    DateCreated = deposit.DateCreated
                };


                _depositQueries.Add(depositEntryVM);
                await _depositQueries.SaveChangesAsync();
            }

        }
    }
}
