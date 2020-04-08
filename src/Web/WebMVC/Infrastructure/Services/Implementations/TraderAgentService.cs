using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Chains;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Deposits;
using Distributing.Domain.Model.Frozens;
using Distributing.Domain.Model.Shared;
using Distributing.Domain.Model.Transfers;
using Distributing.Domain.Model.Withdrawals;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Balances;
using WebMVC.Applications.Queries.Bankbook;
using WebMVC.Applications.Queries.Commission;
using WebMVC.Applications.Queries.Frozen;
using WebMVC.Applications.Queries.TraderAgents;
using WebMVC.Extensions;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;
using WebMVC.ViewModels;

namespace WebMVC.Infrastructure.Services.Implementations
{
    public class TraderAgentService : ITraderAgentService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IBalanceQueries _balanceQueries;
        private readonly ICommissionQueries _commissionQueries;
        private readonly IBankbookQueries _bankbookQueries;
        private readonly IFrozenQueries _frozenQueries;
        private readonly ITraderAgentQueries _traderAgentQueries;

        private readonly IBalanceRepository _balanceRepository;
        private readonly IChainRepository _chainRepository;
        private readonly ICommissionRepository _commissionRepository;
        private readonly ITransferRepository _transferRepository;
        private readonly IDepositRepository _depositRepository;
        private readonly IWithdrawalRepository _withdrawalRepository;
        private readonly IFrozenRepository _frozenRepository;

        private readonly IDateTimeService _dateTimeService;
        private readonly ISystemConfigurationService _systemConfigurationService;

        public TraderAgentService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IBalanceQueries balanceQueries, ICommissionQueries commissionQueries, IBankbookQueries bankbookQueries, IFrozenQueries frozenQueries, ITraderAgentQueries traderAgentQueries, IBalanceRepository balanceRepository, IChainRepository chainRepository, ICommissionRepository commissionRepository, ITransferRepository transferRepository, IDepositRepository depositRepository, IWithdrawalRepository withdrawalRepository, IFrozenRepository frozenRepository, IDateTimeService dateTimeService, ISystemConfigurationService systemConfigurationService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _balanceQueries = balanceQueries ?? throw new ArgumentNullException(nameof(balanceQueries));
            _commissionQueries = commissionQueries ?? throw new ArgumentNullException(nameof(commissionQueries));
            _bankbookQueries = bankbookQueries ?? throw new ArgumentNullException(nameof(bankbookQueries));
            _frozenQueries = frozenQueries ?? throw new ArgumentNullException(nameof(frozenQueries));
            _traderAgentQueries = traderAgentQueries ?? throw new ArgumentNullException(nameof(traderAgentQueries));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _chainRepository = chainRepository ?? throw new ArgumentNullException(nameof(chainRepository));
            _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
            _transferRepository = transferRepository ?? throw new ArgumentNullException(nameof(transferRepository));
            _depositRepository = depositRepository ?? throw new ArgumentNullException(nameof(depositRepository));
            _withdrawalRepository = withdrawalRepository ?? throw new ArgumentNullException(nameof(withdrawalRepository));
            _frozenRepository = frozenRepository ?? throw new ArgumentNullException(nameof(frozenRepository));
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            _systemConfigurationService = systemConfigurationService ?? throw new ArgumentNullException(nameof(systemConfigurationService));
        }



        #region Queries
        public async Task<TraderAgent> GetTraderAgent(string traderAgentId, string searchByUplineId = null)
        {
            var traderAgent = await _traderAgentQueries.GetTraderAgent(traderAgentId);

            if (traderAgent == null)
            {
                throw new Exception("No trader agent found.");
            }

            //If this is searching by a trader agent, check he is the user's upline.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                //Use asp identity's service to run the logic.
                //We cannot insert domain logic into view model (trader agent queries).
                var traderAgentUser = await _userManager.FindByIdAsync(searchByUplineId);
                var bottomUser = await _userManager.FindByIdAsync(traderAgentId);
                if (!await IsAboveAgentChainAsync(traderAgentUser, bottomUser))
                {
                    throw new InvalidOperationException("Trader agent can not search user's details who isn't his downline.");
                }
            }

            /*var tradingCommission = await GetTradingCommissionAsync(user);
            var balance = await GetBalanceAsync(user);

            var hasGrantRight = await HasGrantRightAsync(user);

            return user.MapToTraderAgent(
                tradingCommission,
                balance,
                hasGrantRight,
                await GetUplineUserAsync(user)
                );*/

            return traderAgent;
        }

        public async Task<List<TraderAgent>> GetTraderAgents(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending)
        {
            List<TraderAgent> itemsOnPage = null;

            itemsOnPage = await _traderAgentQueries.GetTraderAgents(
                pageIndex,
                take,
                searchString,
                sortField,
                direction
                );

            return itemsOnPage;
        }

        public async Task<int> GetTraderAgentsTotalCount(string searchString = null)
        {
            var count = await _traderAgentQueries.GetTraderAgentsTotalCount(searchString);

            return count;
        }

        public List<TraderAgent> GetPendingReviewTraderAgents(int pageIndex, int take, string searchString = "", string sortField = "", string direction = "desc")
        {
            List<TraderAgent> itemsOnPage = null;

            itemsOnPage = _traderAgentQueries.GetPendingReviews(
                pageIndex,
                take,
                null,
                searchString,
                sortField,
                direction
                );

            return itemsOnPage;
        }

        public async Task<int> GetPendingReviewTraderAgentsTotalCount(string searchString = null)
        {
            var count = await _traderAgentQueries.GetPendingReviewsTotalCount(null, searchString);
            return count;
        }


        public List<TraderAgent> GetPendingReviewDownlines(int pageIndex, int take, string searchByUplineId, string searchString = "", string sortField = "", string direction = "desc")
        {
            List<TraderAgent> itemsOnPage = null;

            itemsOnPage = _traderAgentQueries.GetPendingReviews(
                pageIndex,
                take,
                searchByUplineId,
                searchString,
                sortField,
                direction
                );

            return itemsOnPage;
        }

        public async Task<int> GetPendingReviewDownlinesTotalCount(string searchByUplineId, string searchString = null)
        {
            var count = await _traderAgentQueries.GetPendingReviewsTotalCount(searchByUplineId, searchString);
            return count;
        }


        public async Task<List<TraderAgent>> GetDownlines(int pageIndex, int take, string topUserId = null, string searchByUplineId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending)
        {
            List<TraderAgent> itemsOnPage = null;

            //If this is searching by an user's upline, checking the upline is above the top user.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                //Use asp identity's service to run the logic.
                //We cannot insert domain logic into view model (trader agent queries).
                var traderAgent = await _userManager.FindByIdAsync(searchByUplineId);
                var bottomUser = await _userManager.FindByIdAsync(topUserId);
                if (!await IsAboveAgentChainAsync(traderAgent, bottomUser))
                {
                    throw new ArgumentNullException("Trader agent can not search downline who isn't his downline.");
                }
            }

            itemsOnPage = await _traderAgentQueries.GetDownlines(
                pageIndex,
                take,
                topUserId,
                searchString,
                sortField,
                direction
                );

            return itemsOnPage;
        }

        public async Task<int> GetDownlinesTotalCount(string topUserId = null, string searchString = null, string searchByUplineId = null)
        {

            //If this is searching by an user's upline, Checking the upline is above the top user.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                var traderAgent = await _userManager.FindByIdAsync(searchByUplineId);
                var bottomUser = await _userManager.FindByIdAsync(topUserId);
                if (!await IsAboveAgentChainAsync(traderAgent, bottomUser))
                {
                    throw new ArgumentNullException("Trader agent can not search downline who isn't his downline.");
                }
            }

            var count = await _traderAgentQueries.GetDownlinesTotalCount(topUserId, searchString);

            return count;
        }

        public async Task<int> GetAwaitingUnfrozeByAdminTotalCount(string userId, string searchByUplineId = null, string searchString = null)
        {
            //If this is searching by an user's upline, checking the upline is above the top user.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                //Use asp identity's service to run the logic.
                //We cannot insert domain logic into view model (trader agent queries).
                var traderAgent = await _userManager.FindByIdAsync(searchByUplineId);
                var bottomUser = await _userManager.FindByIdAsync(userId);
                if (!await IsAboveAgentChainAsync(traderAgent, bottomUser))
                {
                    throw new ArgumentNullException("Trader agent can not search downline who isn't his downline.");
                }
            }

            return await _frozenQueries.GetFrozenRecordsTotalCountByUserId(
                userId,
                searchString,
                FrozenType.ByAdmin.Id,
                FrozenStatus.Frozen.Id);
        }

        public async Task<List<FrozenRecord>> GetAwaitingUnfrozeByAdmin(string userId, int pageIndex, int take, string searchByUplineId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending)
        {
            //If this is searching by an user's upline, checking the upline is above the top user.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                //Use asp identity's service to run the logic.
                //We cannot insert domain logic into view model (trader agent queries).
                var traderAgent = await _userManager.FindByIdAsync(searchByUplineId);
                var bottomUser = await _userManager.FindByIdAsync(userId);
                if (!await IsAboveAgentChainAsync(traderAgent, bottomUser))
                {
                    throw new ArgumentNullException("Trader agent can not search downline who isn't his downline.");
                }
            }

            return await _frozenQueries.GetFrozenRecordsByUserIdAsync(
                userId,
                pageIndex,
                take,
                searchString,
                sortField,
                FrozenType.ByAdmin.Id,
                FrozenStatus.Frozen.Id,
                direction
                );
        }


        public async Task<TradingCommission> GetTradingCommissionFromTraderAgentId(string traderAgentId)
        {
            //Should validate the give id is a trader agent's id.
            //
            //

            return await _commissionQueries.GetCommissionFromTradeUserAsync(traderAgentId);
        }

        public async Task<List<BankbookRecord>> GetBankbookRecords(int pageIndex, int take, string traderAgentId, string searchByUplineId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending)
        {
            //Checking the bankbook is owned by a trader agent.
            /*var userToSearch = await GetTraderAgent(traderAgentId);

            if (userToSearch == null)
            {
                throw new InvalidOperationException("The bankbook to search is not owned by a trader agent.");
            }*/

            //If this is searching by an user's upline, checking the upline is above the top user.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                //Use asp identity's service to run the logic.
                //We cannot insert domain logic into view model (trader agent queries).
                var traderAgent = await _userManager.FindByIdAsync(searchByUplineId);
                var bottomUser = await _userManager.FindByIdAsync(traderAgentId);
                if (!await IsAboveAgentChainAsync(traderAgent, bottomUser))
                {
                    throw new ArgumentNullException("Trader agent can not search downline's bankbook record who isn't his downline.");
                }
            }

            var bankbookRecords = _bankbookQueries.GetBankbookRecordsByUserIdAsync(
                traderAgentId,
                pageIndex,
                take,
                searchString,
                sortField,
                direction
                );


            return bankbookRecords;
        }

        public async Task<int> GetBankbookRecordsTotalCount(string traderAgentId, string searchByUplineId = null, string searchString = null)
        {
            //If this is searching by an user's upline, checking the upline is above the top user.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                //Use asp identity's service to run the logic.
                //We cannot insert domain logic into view model (trader agent queries).
                var traderAgent = await _userManager.FindByIdAsync(searchByUplineId);
                var bottomUser = await _userManager.FindByIdAsync(traderAgentId);
                if (!await IsAboveAgentChainAsync(traderAgent, bottomUser))
                {
                    throw new ArgumentNullException("Trader agent can not search downline's bankbook record who isn't his downline.");
                }
            }

            var count = await _bankbookQueries.GetBankbookRecordsTotalCountAsync(traderAgentId, searchString);

            return count;
        }

        public IEnumerable<SelectListItem> GetBalanceChagneTypes()
        {
            var items = new List<SelectListItem>();

            items.Add(new SelectListItem() { Value = null, Text = "Select...", Selected = true });

            items.Add(new SelectListItem()
            {
                Value = BalanceChangeType.Freeze,
                Text = BalanceChangeType.Freeze
            });
            items.Add(new SelectListItem()
            {
                Value = BalanceChangeType.Deposit,
                Text = BalanceChangeType.Deposit
            });
            items.Add(new SelectListItem()
            {
                Value = BalanceChangeType.Withdraw,
                Text = BalanceChangeType.Withdraw
            });

            return items;
        }


        public async Task<bool> IsUplineOf(string traderAgentId, string downlineId)
        {
            var traderAgent = await _traderAgentQueries.GetTraderAgent(traderAgentId);
            if (traderAgent == null)
            {
                throw new KeyNotFoundException("No trader agent found by given Id.");
            }

            var user = _userManager.Users
                .Where(u => u.Id == downlineId && u.UplineId == traderAgentId)
                .FirstOrDefault();

            if (user != null)
            {
                return true;
            }

            return false;
        }
        #endregion


        #region Operations
        public async Task CreateTraderAgents(TraderAgent traderAgent, string password, string createByTraderAgentId = null)
        {
            //If this is requested by a trader agent, validate the data he submitted.
            if (!string.IsNullOrEmpty(createByTraderAgentId))
            {
                var createByTraderAgent = await this.GetTraderAgent(createByTraderAgentId);
                if (createByTraderAgent == null)
                {
                    throw new InvalidOperationException("The user creating downlines is not a trader agent.");
                }

                this.ValidateDownlineDataChangeSubmittedByTraderAgent(traderAgent, createByTraderAgent);
            }

            ApplicationUser uplineUser = null;
            //Checking the upline is reviewed.
            if (!string.IsNullOrWhiteSpace(traderAgent.UplineUserId))
            {
                //Checking the upline is not trader agent himself.
                /*if (traderAgent.UplineUserId == traderAgent.TraderAgentId)
                {
                    throw new ArgumentException("User's upline can not be himself.");
                }*/

                //Checking the upline user exist.
                uplineUser = await _userManager.FindByIdAsync(traderAgent.UplineUserId);
                if (uplineUser == null)
                {
                    throw new ArgumentException("No upline user found by given user id.");
                }

                //Checking the upline is reviewed.
                if (!uplineUser.IsReviewed)
                {
                    throw new ArgumentException("User can only create downline after reviewed.");
                }

                //Checking the upline is a trader agent.
                if (uplineUser.BaseRoleType != BaseRoleType.TraderAgent)
                {
                    throw new ArgumentException("A trader agent can not be a downline of user who is not a trader agent.");
                }
            }

            //Validate the date time format.
            DateTime dateCreated;

            if (!string.IsNullOrWhiteSpace(traderAgent.DateCreated))
            {
                if (!DateTime.TryParseExact(
                    traderAgent.DateCreated,
                    DateTimeExtensions.GetFormatFullString(),
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out dateCreated))
                {
                    throw new ArgumentException("Invaild param:" + traderAgent.DateCreated +
                        " . Try use the format below:" + DateTimeExtensions.GetFormatFullString() + " .");
                }
            }
            else
            {
                dateCreated = DateTime.UtcNow;
            }

            //Create user.
            var user = new ApplicationUser
            {
                FullName = traderAgent.FullName,
                Nickname = traderAgent.Nickname,
                PhoneNumber = traderAgent.PhoneNumber,
                Email = traderAgent.Email,
                UserName = traderAgent.Username,

                //Must be reviewed first.
                IsEnabled = false,

                //For testing, must remove in formal environment.
                IsReviewed = traderAgent.IsReviewed,

                Wechat = traderAgent.Wechat ?? string.Empty,
                QQ = traderAgent.QQ ?? string.Empty,

                //Must set to trader agent.
                BaseRoleType = BaseRoleType.TraderAgent,

                //Must set to upline user.
                UplineId = uplineUser?.Id,


                DateCreated = dateCreated,

                //Disable lockout feature.
                LockoutEnabled = false
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                //Add to role.
                var traderAgentUser = await _userManager.FindByNameAsync(traderAgent.Username);
                await traderAgentUser.AddToRole(_userManager, _roleManager, Roles.TraderAgent);

                if (traderAgent.HasGrantRight)
                {
                    await traderAgentUser.AddToRole(_userManager, _roleManager, Roles.TraderAgentWithGrantRight);
                }

                //For testing, must remove in formal environment.
                if (traderAgent.IsReviewed)
                {
                    await traderAgentUser.AddToRole(_userManager, _roleManager, Roles.UserReviewed);
                }

                //Add to view model database.
                var traderAgentVm = new TraderAgent
                {
                    TraderAgentId = traderAgentUser.Id,
                    Username = traderAgentUser.UserName,
                    Password = password,
                    FullName = traderAgentUser.FullName,
                    Nickname = traderAgentUser.Nickname,
                    PhoneNumber = traderAgentUser.PhoneNumber,
                    Email = traderAgentUser.Email,
                    Wechat = traderAgentUser.Wechat,
                    QQ = traderAgentUser.QQ,
                    UplineUserId = uplineUser?.Id,
                    UplineUserName = uplineUser?.UserName,
                    UplineFullName = uplineUser?.FullName,

                    IsEnabled = traderAgentUser.IsEnabled,
                    IsReviewed = traderAgentUser.IsReviewed,
                    HasGrantRight = traderAgent.HasGrantRight,
                    LastLoginIP = traderAgentUser.LastLoginIP,
                    DateLastLoggedIn = traderAgentUser.DateLastLoggedIn,
                    DateCreated = traderAgentUser.DateCreated.ToFullString(),

                    //Initailize
                    Balance = new Models.Queries.Balance
                    {
                        WithdrawalLimit = new Models.Queries.WithdrawalLimit
                        {
                            DailyAmountLimit = 0,
                            DailyFrequencyLimit = 0,
                            EachAmountUpperLimit = 0,
                            EachAmountLowerLimit = 0
                        },
                        WithdrawalCommissionRateInThousandth = 0,
                        DepositCommissionRateInThousandth = 0,
                    },
                    TradingCommission = new TradingCommission
                    {
                        RateAlipayInThousandth = 0,
                        RateWechatInThousandth = 0
                    }
                };

                _traderAgentQueries.Add(traderAgentVm);
                await _traderAgentQueries.SaveChangesAsync();


                //Add Balance
                var balance = traderAgent.Balance;
                var newBalance = Distributing.Domain.Model.Balances.Balance.From(
                    traderAgentUser.Id,
                    UserType.TraderAgent,
                    balance.WithdrawalLimit.DailyAmountLimit,
                    balance.WithdrawalLimit.DailyFrequencyLimit,
                    balance.WithdrawalLimit.EachAmountUpperLimit,
                    balance.WithdrawalLimit.EachAmountLowerLimit,
                    (decimal)balance.WithdrawalCommissionRateInThousandth / 1000M,
                    (decimal)balance.DepositCommissionRateInThousandth / 1000M
                    );

                var balanceCreated = _balanceRepository.Add(newBalance);

                //Add Commission
                Commission uplineCommission = null;
                if (!string.IsNullOrEmpty(traderAgent.UplineUserId))
                {
                    uplineCommission = await _commissionRepository.GetByUserIdAsync(uplineUser.Id);

                    if (uplineCommission == null)
                    {
                        throw new ArgumentException("No upline commission found by given username.");
                    }
                }

                var tradingCommission = traderAgent.TradingCommission;

                int chainNumber = default(int);
                if (uplineCommission is null)
                {
                    //If the user has no upline, create a new Chain Number for him.
                    var chain = new Chain();
                    var createdChain = _chainRepository.Add(chain);
                    chainNumber = createdChain.Id;
                }
                else
                {
                    //If the user has upline, set the chain number to upline's chain number.
                    chainNumber = uplineCommission.ChainNumber;
                }

                var newCommission = Commission.FromTradeUser(
                    balanceCreated.Id,
                    traderAgentUser.Id,
                    chainNumber,
                    UserType.TraderAgent,
                    tradingCommission.RateAlipayInThousandth / 1000M,
                    tradingCommission.RateWechatInThousandth / 1000M,
                    false,
                    uplineCommission
                    );

                _commissionRepository.Add(newCommission);

                await _commissionRepository.UnitOfWork.SaveEntitiesAsync();


                //return await GetTraderAgent(traderAgentUser.Id);
            }
            else
            {
                throw new Exception(result.Errors.ToString());
            }
        }


        public async Task UpdateTraderAgent(TraderAgent traderAgent, string password = null)
        {
            //Only manager can update trader agent.
            //Properties can not changed:
            //    TraderAgentId, Username, IsReviewed, LastLoginIP, DateLastLoggedIn, DateCreated.

            #region Update User Base Info and Roles
            //Checking the user to update is a trader agent.
            if (string.IsNullOrWhiteSpace(traderAgent.TraderAgentId))
            {
                throw new ArgumentNullException("Invalid trader agent id.");
            }

            var user = await _userManager.FindByIdAsync(traderAgent.TraderAgentId);
            if (user.BaseRoleType != BaseRoleType.TraderAgent)
            {
                throw new Exception("The user status to update is not a trader agent.");
            }

            //Update trader agent's base info.
            user.FullName = traderAgent.FullName;
            user.Nickname = traderAgent.Nickname;
            user.PhoneNumber = traderAgent.PhoneNumber;
            user.Email = traderAgent.Email;
            user.Wechat = traderAgent.Wechat ?? string.Empty;
            user.QQ = traderAgent.QQ ?? string.Empty;

            //Update trader agent's rights.
            if (traderAgent.IsEnabled != user.IsEnabled)
            {
                user.IsEnabled = traderAgent.IsEnabled;
                //Later need to update commission's status.
            }
            var hasGrantRight = await HasGrantRightAsync(user);
            if (traderAgent.HasGrantRight != hasGrantRight)
            {
                if (traderAgent.HasGrantRight)
                {
                    await user.AddToRole(_userManager, _roleManager, Roles.TraderAgentWithGrantRight);
                }
                else
                {
                    await user.RemoveFromRole(_userManager, _roleManager, Roles.TraderAgentWithGrantRight);
                }
            }
            #endregion

            #region Update Balance
            var balance = await _balanceRepository.GetByUserIdAsync(user.Id);
            if (balance == null)
            {
                throw new ArgumentNullException("No balance found by given user Id.");
            }
            balance.UpdateWithdrawalFeeRate(
                traderAgent.Balance.WithdrawalCommissionRateInThousandth / 1000M);
            balance.UpdateDepositFeeRate(
                traderAgent.Balance.DepositCommissionRateInThousandth / 1000M);
            balance.UpdateWithdrawalLimit(
                traderAgent.Balance.WithdrawalLimit.DailyAmountLimit,
                traderAgent.Balance.WithdrawalLimit.DailyFrequencyLimit,
                traderAgent.Balance.WithdrawalLimit.EachAmountUpperLimit,
                traderAgent.Balance.WithdrawalLimit.EachAmountLowerLimit
                );
            _balanceRepository.Update(balance);
            #endregion

            #region Update Upline
            var commission = await _commissionRepository.GetByUserIdAsync(user.Id);
            if (commission == null)
            {
                throw new ArgumentNullException("No commission found by given user Id.");
            }

            //Check the user's current upline commission exist.
            Commission uplineCommission = null;
            if (commission.UplineCommissionId != null)
            {
                uplineCommission = await _commissionRepository.GetByCommissionIdAsync((int)commission.UplineCommissionId);
                if (uplineCommission == null)
                {
                    throw new ArgumentNullException("No commission found by upline commission id.");
                }
            }

            //Check the upline is changed from input.
            bool uplineChanged = false;

            var inputUplineUserId = traderAgent.UplineUserId;

            if (string.IsNullOrEmpty(inputUplineUserId))
            {
                if (!string.IsNullOrEmpty(user.UplineId))
                {
                    //The user has upline Id, but the input doesn't.
                    //So the request is to delete the upline.
                    uplineChanged = true;

                    uplineCommission = null;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(user.UplineId))
                {
                    if (user.UplineId != inputUplineUserId)
                    {
                        //The user has upline Id, but is different with input's one
                        //So the request is to change the upline.
                        uplineChanged = true;

                        uplineCommission = await _commissionRepository.GetByUserIdAsync(inputUplineUserId);
                        if (uplineCommission == null)
                        {
                            throw new ArgumentNullException("No commission found by given user Id.");
                        }
                    }
                }
                else
                {
                    //The user has no upline Id, but the input has
                    //So the request is to assign a new upline.
                    uplineChanged = true;

                    uplineCommission = await _commissionRepository.GetByUserIdAsync(inputUplineUserId);
                    if (uplineCommission == null)
                    {
                        throw new ArgumentNullException("No commission found by given user Id.");
                    }
                }
            }

            //If the upline is changed, then update the upline.
            ApplicationUser uplineUser = null;
            if (uplineChanged)
            {
                //Check the upline isn't downline of the user's downline.
                if (!string.IsNullOrEmpty(inputUplineUserId))
                {
                    var currentUser = await _userManager.FindByIdAsync(inputUplineUserId);
                    while (true)
                    {
                        if (string.IsNullOrEmpty(currentUser.UplineId))
                        {
                            break;
                        }
                        if (currentUser.UplineId == user.Id)
                        {
                            throw new InvalidOperationException("不可指派用户的下级为其上级。");
                        }

                        currentUser = await _userManager.FindByIdAsync(currentUser.UplineId);
                    }
                }


                //Update user's upline id.
                if (!string.IsNullOrEmpty(inputUplineUserId))
                {
                    //Chekcing the upline user assigned is not trader agent himself.
                    if (!string.IsNullOrEmpty(inputUplineUserId))
                    {
                        if (inputUplineUserId == user.Id)
                        {
                            throw new ArgumentException("User's upline can not be himself.");
                        }
                    }

                    uplineUser = await _userManager.FindByIdAsync(traderAgent.UplineUserId);

                    //Checking the upline user exist.
                    if (uplineUser == null)
                    {
                        throw new ArgumentException("No upline user found by given user id.");
                    }

                    //Chekcing the upline user is reviewed.
                    if (!uplineUser.IsReviewed)
                    {
                        throw new ArgumentException("Trader agent can only create downline after reviewed.");
                    }
                }
                user.UplineId = inputUplineUserId;


                //Update upline commission.
                commission.UplineDeleted();
                if (uplineCommission != null)
                {
                    commission.UplineAssigned(uplineCommission);
                }
            }

            #endregion

            #region Update Commission
            //Update commission's status
            if (traderAgent.IsEnabled != user.IsEnabled)
            {
                if (traderAgent.IsEnabled)
                {
                    commission.Enable();
                }
                else
                {
                    commission.Disable();
                }
            }

            commission.UpdateRate(
                traderAgent.TradingCommission.RateAlipayInThousandth / 1000M,
                traderAgent.TradingCommission.RateWechatInThousandth / 1000M,
                uplineCommission
                );
            _commissionRepository.Update(commission);
            #endregion


            //Save entities and execute domain event.
            await _commissionRepository.UnitOfWork.SaveEntitiesAsync();

            var passwordChanged = false;
            //Forced change trader agent's password.
            if (!string.IsNullOrEmpty(password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var changeResult = await _userManager.ResetPasswordAsync(user, token, password);

                if (!changeResult.Succeeded)
                {
                    throw new Exception(changeResult.Errors.ToString());
                }
                passwordChanged = true;
            }

            //Save user's info.
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new Exception("Failed to update user.");
            }


            #region Update View Model
            var traderAgentVM = await _traderAgentQueries.GetTraderAgent(user.Id);
            if (traderAgentVM == null)
            {
                throw new KeyNotFoundException("No trader agent view data found.");
            }

            traderAgentVM.FullName = user.FullName;
            traderAgentVM.Nickname = user.Nickname;
            traderAgentVM.PhoneNumber = user.PhoneNumber;
            traderAgentVM.Email = user.Email;
            traderAgentVM.Wechat = user.Wechat ?? string.Empty;
            traderAgentVM.QQ = user.QQ ?? string.Empty;
            if (uplineChanged)
            {
                traderAgentVM.UplineUserId = uplineUser?.Id;
                traderAgentVM.UplineUserName = uplineUser?.UserName;
                traderAgentVM.UplineFullName = uplineUser?.FullName;
            }
            if (passwordChanged)
            {
                traderAgentVM.Password = password;
            }
            traderAgentVM.IsEnabled = user.IsEnabled;
            traderAgentVM.IsReviewed = user.IsReviewed;
            traderAgentVM.HasGrantRight = await HasGrantRightAsync(user);

            _traderAgentQueries.Update(traderAgentVM);

            await _traderAgentQueries.SaveChangesAsync();
            #endregion
        }

        public async Task UpdateTraderAgentStatus(List<AccountStatus> accounts)
        {
            //The code needs to update since it doesn't achieve atomic.
            foreach (var account in accounts)
            {
                var user = await _userManager.FindByIdAsync(account.UserId);
                if (user.BaseRoleType != BaseRoleType.TraderAgent)
                {
                    throw new Exception("The user status to update is not a trader agent.");
                }
                user.IsEnabled = account.IsEnabled ??
                    throw new ArgumentException("Invalid Argument: IsEnabled (bool?)");

                //Update commission status.
                var commission = await this._commissionRepository.GetByUserIdAsync(user.Id);
                if (user.IsEnabled)
                {
                    commission.Enable();
                }
                else
                {
                    commission.Disable();
                }
                _commissionRepository.Update(commission);
                await _commissionRepository.UnitOfWork.SaveEntitiesAsync();

                //Update user account status.
                await _userManager.UpdateAsync(user);


                //Update view model.
                var traderAgentVM = await _traderAgentQueries.GetTraderAgent(user.Id);
                if (traderAgentVM == null)
                {
                    throw new KeyNotFoundException("No trader agent view data found.");
                }

                traderAgentVM.IsEnabled = user.IsEnabled;

                _traderAgentQueries.Update(traderAgentVM);

                await _traderAgentQueries.SaveChangesAsync();
            }
        }

        public async Task ReviewTraderAgents(List<AccountReview> accounts)
        {
            //The code needs to update since it doesn't achieve atomic.

            var toDeletes = new List<ApplicationUser>();

            foreach (var account in accounts)
            {
                var user = await _userManager.FindByIdAsync(account.UserId);
                if (user.BaseRoleType != BaseRoleType.TraderAgent)
                {
                    throw new Exception("The user to review is not a trader agent.");
                }

                if (account.IsReviewed)
                {
                    //Validate all entity is ready.
                    var balance = await _balanceRepository.GetByUserIdAsync(user.Id);
                    if (balance == null)
                    {
                        throw new SystemException("资料缺失，请拒绝审核此代理，并重新添加。");
                    }

                    var commission = await _commissionRepository.GetByUserIdAsync(user.Id);
                    if (commission == null)
                    {
                        throw new SystemException("资料缺失，请拒绝审核此代理，并重新添加。");
                    }

                    //Add to role and set 'IsReviewed' to true.
                    if (!await _userManager.IsInRoleAsync(user, Roles.UserReviewed))
                    {
                        var addToRoleResult = await _userManager.AddToRoleAsync(user, Roles.UserReviewed);
                        if (addToRoleResult.Succeeded)
                        {
                            user.IsReviewed = true;
                            var updateResult = await _userManager.UpdateAsync(user);
                            if (!updateResult.Succeeded)
                            {
                                throw new Exception("Failed to update user.");
                            }

                            //Update view model.
                            var traderAgentVM = await _traderAgentQueries.GetTraderAgent(user.Id);
                            if (traderAgentVM == null)
                            {
                                throw new KeyNotFoundException("No trader agent view data found.");
                            }

                            traderAgentVM.IsReviewed = user.IsReviewed;

                            _traderAgentQueries.Update(traderAgentVM);

                            await _traderAgentQueries.SaveChangesAsync();
                        }
                        else
                        {
                            throw new Exception("Failed to add user to role.");
                        }
                    }
                    else
                    {
                        if (!user.IsReviewed)
                        {
                            var updateResult = await _userManager.UpdateAsync(user);
                            if (!updateResult.Succeeded)
                            {
                                throw new Exception("Failed to update user.");
                            }
                        }
                    }
                }
                else
                {
                    //Checking the user has not been reviewed.
                    if (user.IsReviewed || await _userManager.IsInRoleAsync(user, Roles.UserReviewed))
                    {
                        throw new Exception("Can not cancel review on reviewed user.");
                    }

                    //Add to deletion list.
                    toDeletes.Add(user);
                }
            }

            //Delete trader agents.
            await DeleteTraderAgents(toDeletes);
        }

        public async Task DeleteTraderAgent(string traderAgentId = null, string traderAgentUsername = null)
        {
            ApplicationUser user;

            if (traderAgentId != null)
            {
                user = await _userManager.FindByIdAsync(traderAgentId);
            }
            else if (traderAgentUsername != null)
            {
                user = await _userManager.FindByNameAsync(traderAgentUsername);
            }
            else
            {
                throw new ArgumentNullException("Must provided id of trader agent or username of trader agent to delete it.");
            }

            if (user != null)
            {
                //Delele user.
                if (await _userManager.IsInRoleAsync(user, Roles.TraderAgent))
                {
                    await DeleteWithoutSaveEntities(user);

                    await _traderAgentQueries.SaveChangesAsync();
                    await _commissionRepository.UnitOfWork.SaveEntitiesAsync();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The user to delete is not a trader agent.");
                }
            }
        }

        public async Task DeleteTraderAgents(List<ApplicationUser> users)
        {
            var idList = new List<string>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, Roles.TraderAgent))
                {
                    await DeleteWithoutSaveEntities(user);

                    idList.Add(user.Id);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The user to delete is not a trader agent.");
                }
            }
            await _traderAgentQueries.SaveChangesAsync();
            await _commissionRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task DeleteTraderAgents(List<string> traderAgentIds)
        {
            var users = new List<ApplicationUser>();

            foreach (var traderAgentId in traderAgentIds)
            {
                var user = await _userManager.FindByIdAsync(traderAgentId);
                if (user == null)
                {
                    throw new KeyNotFoundException("No user found by given Id.");
                }
                users.Add(user);
            }

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, Roles.TraderAgent))
                {
                    await DeleteWithoutSaveEntities(user);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The user to delete is not a trader agent.");
                }
            }

            await _traderAgentQueries.SaveChangesAsync();
            await _commissionRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task SubmitTransfer(string fromTraderAgentId, string toTraderId, string transferByUserId, decimal amount)
        {
            //Check the payee is a trader.
            var payee = await _userManager.FindByIdAsync(toTraderId);
            if (payee == null || payee.BaseRoleType != BaseRoleType.Trader)
            {
                throw new InvalidOperationException("Payee must be a trader.");
            }

            //Check the payer is a trader agent.
            var payer = await GetTraderAgent(fromTraderAgentId);
            if (payer == null)
            {
                throw new InvalidOperationException("Payer must be a trader agent.");
            }

            //Make sure the payee is payer's downline.
            if (string.IsNullOrEmpty(payee.UplineId) || payee.UplineId != payer.TraderAgentId)
            {
                throw new InvalidOperationException("Payee must be trader agent's downline.");
            }

            //Checking if this is transfer by admin.
            var transferByUser = _userManager.Users.Where(u => u.Id == transferByUserId).FirstOrDefault();
            if (transferByUser == null)
            {
                throw new KeyNotFoundException("No user found by given user Id.");
            }
            var isByAdmin = transferByUser.BaseRoleType == BaseRoleType.Manager;

            var fromBalance = await _balanceRepository.GetByUserIdAsync(payer.TraderAgentId);
            var toBalance = await _balanceRepository.GetByUserIdAsync(payee.Id);

            Transfer transfer = null;
            if (isByAdmin)
            {
                transfer = Transfer.FromAdmin(
                    transferByUserId,
                    fromBalance,
                    toBalance,
                    amount,
                    _dateTimeService
                    );
            }
            else
            {
                transfer = Transfer.FromTraderAgent(
                    transferByUserId,
                    fromBalance,
                    toBalance,
                    amount,
                    _dateTimeService
                    );
            }
            var transferCreated = _transferRepository.Add(transfer);

            await _transferRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task ChangeBalance(string type, string userId, decimal amount, string description, string createByUserId)
        {
            //Checking the changing is made by manager.
            var changeByUser = await _userManager.FindByIdAsync(createByUserId);
            if (changeByUser.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException($"Only manager can change trader agent's balance.");
            }

            //Checking the balance is exist.
            var balance = await _balanceRepository.GetByUserIdAsync(userId);
            if (balance == null)
            {
                throw new KeyNotFoundException($"No balance found by given user Id: {userId}. ");
            }

            var admin = new Distributing.Domain.Model.Roles.Admin(changeByUser.Id, changeByUser.UserName);
            if (type == BalanceChangeType.Deposit)
            {
                //Create a new deposit entity.
                var deposit = Deposit.FromAdmin(
                    balance,
                    admin,
                    amount,
                    description,
                    _dateTimeService
                    );
                _depositRepository.Add(deposit);

            }
            else if (type == BalanceChangeType.Withdraw)
            {
                balance.WithdrawByAdmin(
                    admin,
                    amount,
                    description,
                    _dateTimeService
                    );
                _balanceRepository.Update(balance);
            }
            else if (type == BalanceChangeType.Freeze)
            {
                balance.Freeze(
                    admin,
                    amount,
                    description,
                    _dateTimeService
                    );
                _balanceRepository.Update(balance);
            }
            else
            {
                throw new ArgumentOutOfRangeException("The balance change type is invalid.");
            }


            //Save changes and execute related domain events.
            await _depositRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task Unfreeze(int frozenId, string unfrozeByUserId)
        {
            //Checking the balance is unfroze by an admin.
            var unfrozeByUser = await _userManager.FindByIdAsync(unfrozeByUserId);

            if (unfrozeByUser == null || unfrozeByUser.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException("Trander agent's balance can only unfreeze by an admin.");
            }

            var frozen = await this._frozenRepository.GetByFrozenIdAsync(frozenId);

            if (frozen == null)
            {
                throw new ArgumentNullException("No frozen found by given id.");
            }
            var balance = await _balanceRepository.GetByBalanceIdAsync(frozen.BalanceId);

            frozen.Unfreeze(
                balance,
                new Distributing.Domain.Model.Roles.Admin(unfrozeByUser.Id, unfrozeByUser.UserName),
                null,
                _dateTimeService
                );

            _frozenRepository.Update(frozen);

            await _frozenRepository.UnitOfWork.SaveEntitiesAsync();
        }
        #endregion


        #region Custom
        private async Task<TradingCommission> GetTradingCommissionAsync(ApplicationUser user)
        {
            return await _commissionQueries.GetCommissionFromTradeUserAsync(user.Id);
        }

        private async Task<Models.Queries.Balance> GetBalanceAsync(ApplicationUser user)
        {
            var balance = await _balanceQueries.GetBalanceFromUserAsync(user.Id);

            return balance;
        }

        private async Task<bool> IsReviewedAsync(ApplicationUser user)
        {
            return await _userManager.IsInRoleAsync(user, Roles.UserReviewed);
        }

        private async Task<bool> HasGrantRightAsync(ApplicationUser user)
        {
            return await _userManager.IsInRoleAsync(user, Roles.TraderAgentWithGrantRight);
        }

        private async Task<ApplicationUser> GetUplineUserAsync(ApplicationUser user)
        {
            /*var uplineUserId = await _commissionQueries.GetUplineUserIdFromUserAsync(user.Id);
            if (!string.IsNullOrEmpty(uplineUserId))
            {
                return await _userManager.FindByIdAsync(uplineUserId);
            }*/

            if (string.IsNullOrEmpty(user.UplineId))
            {
                return null;
            }

            return await _userManager.FindByIdAsync(user.UplineId);
        }

        private async Task<bool> IsAboveAgentChainAsync(ApplicationUser traderAgent, ApplicationUser bottomUser)
        {
            if (bottomUser == null || traderAgent == null)
            {
                throw new ArgumentNullException("Invalid user.");
            }
            /*if (string.IsNullOrEmpty(bottomUser.UplineId))
            {
                return false;
            }*/

            var isAboveAgentChain = false;
            var currentUser = await _userManager.Users
                    .Where(u => u.Id == bottomUser.Id)
                    .Select(u => new
                    {
                        u.Id,
                        u.UplineId
                    })
                    .FirstOrDefaultAsync();

            while (!isAboveAgentChain)
            {
                //Check the current user's id is equal to trader agent's id.
                if (traderAgent.Id == currentUser.Id)
                {
                    isAboveAgentChain = true;
                    break;
                }


                var uplineId = currentUser.UplineId;
                if (string.IsNullOrEmpty(uplineId))
                {
                    break;
                }

                //Get the upline user.
                var uplineUser = await _userManager.Users
                    .Where(u => u.Id == uplineId)
                    .Select(u => new
                    {
                        u.Id,
                        u.UplineId
                    })
                    .FirstOrDefaultAsync();

                //Set current user and check at the next loop.
                currentUser = uplineUser;
            }

            return isAboveAgentChain;
        }

        private async Task DeleteWithoutSaveEntities(ApplicationUser user)
        {
            //Delete user, balance and commission.
            var deleteUserResult = await _userManager.DeleteAsync(user);
            if (!deleteUserResult.Succeeded)
            {
                throw new Exception("Delete user failed. user id:" + user.Id);
            }

            var balance = await _balanceRepository.GetByUserIdAsync(user.Id);
            if (balance != null)
            {
                _balanceRepository.Delete(balance);
            }

            var commission = await _commissionRepository.GetByUserIdAsync(user.Id);
            if (commission != null)
            {
                _commissionRepository.Delete(commission);
            }

            //Delete view model.
            var traderAgentVM = await _traderAgentQueries.GetTraderAgent(user.Id);
            if (traderAgentVM != null)
            {
                _traderAgentQueries.Delete(traderAgentVM);
            }

            //Delete the reference from downlines.
            var downlines = _userManager.Users.Where(u => u.UplineId == user.Id);
            foreach (var downline in downlines)
            {
                downline.UplineId = null;
                await _userManager.UpdateAsync(downline);

                //Update view model.
                var downlineVM = await _traderAgentQueries.GetTraderAgent(downline.Id);
                if (downlineVM != null)
                {
                    downlineVM.UplineUserId = null;
                    downlineVM.UplineUserName = null;
                    downlineVM.UplineFullName = null;
                    _traderAgentQueries.Update(downlineVM);
                }
            }


            //If the commission has been created, delete all refercence from downline commissions.
            if (commission != null)
            {
                var downlineCommissions = await _commissionRepository.GetDownlinesAsnyc(commission.Id);
                foreach (var downlineCommission in downlineCommissions)
                {
                    downlineCommission.UplineDeleted();
                    _commissionRepository.Update(downlineCommission);
                }
            }

        }


        private void ValidateDownlineDataChangeSubmittedByTraderAgent(TraderAgent downlineData, TraderAgent traderAgent)
        {
            //Only manager can change withdrawal and deposit rate.
            var withdrawalConf = _systemConfigurationService.GetWithdrawalAndDepositAsync();
            if (downlineData.Balance.WithdrawalCommissionRateInThousandth != withdrawalConf.WithdrawalTemplate.CommissionInThousandth
                || downlineData.Balance.DepositCommissionRateInThousandth != 0
                || downlineData.Balance.WithdrawalLimit.DailyAmountLimit != withdrawalConf.WithdrawalTemplate.DailyAmountLimit
                || downlineData.Balance.WithdrawalLimit.DailyFrequencyLimit != withdrawalConf.WithdrawalTemplate.DailyFrequencyLimit
                || downlineData.Balance.WithdrawalLimit.EachAmountLowerLimit != withdrawalConf.WithdrawalTemplate.EachAmountLowerLimit
                || downlineData.Balance.WithdrawalLimit.EachAmountUpperLimit != withdrawalConf.WithdrawalTemplate.EachAmountUpperLimit)
            {
                throw new InvalidOperationException("Trader agent can not change withdrawal & deposit commission rates.");
            }

            //Checking the given id of upline is matched,
            if (string.IsNullOrWhiteSpace(downlineData.UplineUserId)
                || downlineData.UplineUserId != traderAgent.TraderAgentId)
            {
                throw new InvalidOperationException("Invalid upline Id.");
            }
        }
        #endregion
    }
}
