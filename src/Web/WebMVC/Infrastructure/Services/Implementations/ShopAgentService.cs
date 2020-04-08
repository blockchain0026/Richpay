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
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Balances;
using WebMVC.Applications.Queries.Bankbook;
using WebMVC.Applications.Queries.Commission;
using WebMVC.Applications.Queries.Frozen;
using WebMVC.Applications.Queries.ShopAgents;
using WebMVC.Extensions;
using WebMVC.Models;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;
using WebMVC.ViewModels;

namespace WebMVC.Infrastructure.Services.Implementations
{
    public class ShopAgentService : IShopAgentService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IBalanceQueries _balanceQueries;
        private readonly ICommissionQueries _commissionQueries;
        private readonly IBankbookQueries _bankbookQueries;
        private readonly IFrozenQueries _frozenQueries;
        private readonly IShopAgentQueries _shopAgentQueries;

        private readonly IBalanceRepository _balanceRepository;
        private readonly IChainRepository _chainRepository;
        private readonly ICommissionRepository _commissionRepository;
        private readonly ITransferRepository _transferRepository;
        private readonly IDepositRepository _depositRepository;
        private readonly IFrozenRepository _frozenRepository;

        private readonly IDateTimeService _dateTimeService;
        private readonly ISystemConfigurationService _systemConfigurationService;

        public ShopAgentService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IBalanceQueries balanceQueries, ICommissionQueries commissionQueries, IBankbookQueries bankbookQueries, IFrozenQueries frozenQueries, IShopAgentQueries shopAgentQueries, IBalanceRepository balanceRepository, IChainRepository chainRepository, ICommissionRepository commissionRepository, ITransferRepository transferRepository, IDepositRepository depositRepository, IFrozenRepository frozenRepository, IDateTimeService dateTimeService, ISystemConfigurationService systemConfigurationService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _balanceQueries = balanceQueries ?? throw new ArgumentNullException(nameof(balanceQueries));
            _commissionQueries = commissionQueries ?? throw new ArgumentNullException(nameof(commissionQueries));
            _bankbookQueries = bankbookQueries ?? throw new ArgumentNullException(nameof(bankbookQueries));
            _frozenQueries = frozenQueries ?? throw new ArgumentNullException(nameof(frozenQueries));
            _shopAgentQueries = shopAgentQueries ?? throw new ArgumentNullException(nameof(shopAgentQueries));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _chainRepository = chainRepository ?? throw new ArgumentNullException(nameof(chainRepository));
            _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
            _transferRepository = transferRepository ?? throw new ArgumentNullException(nameof(transferRepository));
            _depositRepository = depositRepository ?? throw new ArgumentNullException(nameof(depositRepository));
            _frozenRepository = frozenRepository ?? throw new ArgumentNullException(nameof(frozenRepository));
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            _systemConfigurationService = systemConfigurationService ?? throw new ArgumentNullException(nameof(systemConfigurationService));
        }


        #region Queries
        public async Task<ShopAgent> GetShopAgent(string shopAgentId, string searchByUplineId = null)
        {
            var shopAgent = await _shopAgentQueries.GetShopAgent(shopAgentId);

            if (shopAgent == null)
            {
                throw new Exception("No shop agent found.");
            }

            //If this is searching by ashop agent, check he is the user's upline.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                //Use asp identity's service to run the logic.
                //We cannot insert domain logic into view model (trader agent queries).
                var shopAgentUser = await _userManager.FindByIdAsync(searchByUplineId);
                var bottomUser = await _userManager.FindByIdAsync(shopAgentId);
                if (!await IsAboveAgentChainAsync(shopAgentUser, bottomUser))
                {
                    throw new InvalidOperationException("Shop agent can not search user's details who isn't his downline.");
                }
            }

            return shopAgent;
        }

        public async Task<List<ShopAgent>> GetShopAgents(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending)
        {
            List<ShopAgent> itemsOnPage = null;

            itemsOnPage = await _shopAgentQueries.GetShopAgents(
                pageIndex,
                take,
                searchString,
                sortField,
                direction
                );

            return itemsOnPage;
        }

        public async Task<int> GetShopAgentsTotalCount(string searchString = null)
        {
            var count = await _shopAgentQueries.GetShopAgentsTotalCount(searchString);

            return count;
        }

        public List<ShopAgent> GetPendingReviewShopAgents(int pageIndex, int take, string searchString = "", string sortField = "", string direction = "desc")
        {
            List<ShopAgent> itemsOnPage = null;

            itemsOnPage = _shopAgentQueries.GetPendingReviews(
                pageIndex,
                take,
                null,
                searchString,
                sortField,
                direction
                );

            return itemsOnPage;
        }

        public async Task<int> GetPendingReviewShopAgentsTotalCount(string searchString = null)
        {
            var count = await _shopAgentQueries.GetPendingReviewsTotalCount(null, searchString);
            return count;
        }

        public List<ShopAgent> GetPendingReviewDownlines(int pageIndex, int take, string searchByUplineId, string searchString = "", string sortField = "", string direction = "desc")
        {
            List<ShopAgent> itemsOnPage = null;

            itemsOnPage = _shopAgentQueries.GetPendingReviews(
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
            var count = await _shopAgentQueries.GetPendingReviewsTotalCount(searchByUplineId, searchString);
            return count;
        }


        public async Task<List<ShopAgent>> GetDownlines(int pageIndex, int take, string topUserId = null, string searchByUplineId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending)
        {
            List<ShopAgent> itemsOnPage = null;

            //If this is searching by an user's upline, checking the upline is above the top user.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                //Use asp identity's service to run the logic.
                //We cannot insert domain logic into view model (trader agent queries).
                var shopAgent = await _userManager.FindByIdAsync(searchByUplineId);
                var bottomUser = await _userManager.FindByIdAsync(topUserId);
                if (!await IsAboveAgentChainAsync(shopAgent, bottomUser))
                {
                    throw new ArgumentNullException("Shop agent can not search downline who isn't his downline.");
                }
            }

            itemsOnPage = await _shopAgentQueries.GetDownlines(
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
                var shopAgent = await _userManager.FindByIdAsync(searchByUplineId);
                var bottomUser = await _userManager.FindByIdAsync(topUserId);
                if (!await IsAboveAgentChainAsync(shopAgent, bottomUser))
                {
                    throw new ArgumentNullException("Shop agent can not search downline who isn't his downline.");
                }
            }

            var count = await _shopAgentQueries.GetDownlinesTotalCount(topUserId, searchString);

            return count;
        }

        public async Task<int> GetAwaitingUnfrozeByAdminTotalCount(string userId, string searchByUplineId = null, string searchString = null)
        {
            //If this is searching by an user's upline, checking the upline is above the top user.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                //Use asp identity's service to run the logic.
                //We cannot insert domain logic into view model (trader agent queries).
                var shopAgent = await _userManager.FindByIdAsync(searchByUplineId);
                var bottomUser = await _userManager.FindByIdAsync(userId);
                if (!await IsAboveAgentChainAsync(shopAgent, bottomUser))
                {
                    throw new ArgumentNullException("Shop agent can not search downline who isn't his downline.");
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
                var shopAgent = await _userManager.FindByIdAsync(searchByUplineId);
                var bottomUser = await _userManager.FindByIdAsync(userId);
                if (!await IsAboveAgentChainAsync(shopAgent, bottomUser))
                {
                    throw new ArgumentNullException("Shop agent can not search downline who isn't his downline.");
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


        public async Task<RebateCommission> GetRebateCommissionFromShopAgentId(string shopAgentId)
        {
            //Should validate the give id is a shop agent's id.
            //
            //

            return await _commissionQueries.GetCommissionFromShopUserAsync(shopAgentId);
        }

        public async Task<List<BankbookRecord>> GetBankbookRecords(int pageIndex, int take, string shopAgentId, string searchByUplineId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending)
        {
            //Checking the bankbook is owned by a shop agent.
            /*var userToSearch = await GetShopAgent(shopAgentId);

            if (userToSearch == null)
            {
                throw new InvalidOperationException("The bankbook to search is not owned by ashop agent.");
            }*/

            //If this is searching by an user's upline, checking the upline is above the top user.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                //Use asp identity's service to run the logic.
                //We cannot insert domain logic into view model (trader agent queries).
                var shopAgent = await _userManager.FindByIdAsync(searchByUplineId);
                var bottomUser = await _userManager.FindByIdAsync(shopAgentId);
                if (!await IsAboveAgentChainAsync(shopAgent, bottomUser))
                {
                    throw new ArgumentNullException("Shop agent can not search downline's bankbook record who isn't his downline.");
                }
            }

            var bankbookRecords = _bankbookQueries.GetBankbookRecordsByUserIdAsync(
                shopAgentId,
                pageIndex,
                take,
                searchString,
                sortField,
                direction
                );


            return bankbookRecords;
        }

        public async Task<int> GetBankbookRecordsTotalCount(string shopAgentId, string searchByUplineId = null, string searchString = null)
        {
            //If this is searching by an user's upline, checking the upline is above the top user.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                //Use asp identity's service to run the logic.
                //We cannot insert domain logic into view model (trader agent queries).
                var shopAgent = await _userManager.FindByIdAsync(searchByUplineId);
                var bottomUser = await _userManager.FindByIdAsync(shopAgentId);
                if (!await IsAboveAgentChainAsync(shopAgent, bottomUser))
                {
                    throw new ArgumentNullException("Shop agent can not search downline's bankbook record who isn't his downline.");
                }
            }

            var count = await _bankbookQueries.GetBankbookRecordsTotalCountAsync(shopAgentId, searchString);

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


        public async Task<bool> IsUplineOf(string shopAgentId, string downlineId)
        {
            var shopAgent = await _shopAgentQueries.GetShopAgent(shopAgentId);
            if (shopAgent == null)
            {
                throw new KeyNotFoundException("Noshop agent found by given Id.");
            }

            var user = _userManager.Users
                .Where(u => u.Id == downlineId && u.UplineId == shopAgentId)
                .FirstOrDefault();

            if (user != null)
            {
                return true;
            }

            return false;
        }
        #endregion


        #region Operations
        public async Task CreateShopAgents(ShopAgent shopAgent, string password, string createByShopAgentId = null)
        {
            //If this is requested by a shop agent, validate the data he submitted.
            if (!string.IsNullOrEmpty(createByShopAgentId))
            {
                var createByShopAgent = await this.GetShopAgent(createByShopAgentId);
                if (createByShopAgent == null)
                {
                    throw new InvalidOperationException("The user creating downlines is not ashop agent.");
                }

                this.ValidateDownlineDataChangeSubmittedByShopAgent(shopAgent, createByShopAgent);
            }

            ApplicationUser uplineUser = null;

            //Checking the upline is valid.
            if (!string.IsNullOrWhiteSpace(shopAgent.UplineUserId))
            {
                //Checking the upline is not shop agent himself.
                /*if (shopAgent.UplineUserId == shopAgent.ShopAgentId)
                {
                    throw new ArgumentException("User's upline can not be himself.");
                }*/

                //Checking the upline user exist.
                uplineUser = await _userManager.FindByIdAsync(shopAgent.UplineUserId);
                if (uplineUser == null)
                {
                    throw new ArgumentException("No upline user found by given user id.");
                }

                //Checking the upline is reviewed.
                if (!uplineUser.IsReviewed)
                {
                    throw new ArgumentException("User can only create downline after reviewed.");
                }

                //Checking the upline is a shop agent.
                if (uplineUser.BaseRoleType != BaseRoleType.ShopAgent)
                {
                    throw new ArgumentException("Ashop agent can not be a downline of user who is not ashop agent.");
                }
            }

            //Validate the date time format.
            DateTime dateCreated;

            if (!string.IsNullOrWhiteSpace(shopAgent.DateCreated))
            {
                if (!DateTime.TryParseExact(
                    shopAgent.DateCreated,
                    DateTimeExtensions.GetFormatFullString(),
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out dateCreated))
                {
                    throw new ArgumentException("Invaild param:" + shopAgent.DateCreated +
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
                FullName = shopAgent.FullName,
                Nickname = shopAgent.Nickname,
                PhoneNumber = shopAgent.PhoneNumber,
                Email = shopAgent.Email,
                UserName = shopAgent.Username,

                Wechat = shopAgent.Wechat ?? string.Empty,
                QQ = shopAgent.QQ ?? string.Empty,

                //Must be reviewed first.
                IsEnabled = false,

                //For testing, must remove in formal environment.
                IsReviewed = shopAgent.IsReviewed,


                //Must set to shop agent.
                BaseRoleType = BaseRoleType.ShopAgent,

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
                var shopAgentUser = await _userManager.FindByNameAsync(shopAgent.Username);
                await shopAgentUser.AddToRole(_userManager, _roleManager, Roles.ShopAgent);

                if (shopAgent.HasGrantRight)
                {
                    await shopAgentUser.AddToRole(_userManager, _roleManager, Roles.ShopAgentWithGrantRight);
                }

                //For testing, must remove in formal environment.
                if (shopAgent.IsReviewed)
                {
                    await shopAgentUser.AddToRole(_userManager, _roleManager, Roles.UserReviewed);
                }

                //Add to view model database.
                var shopAgentVm = new ShopAgent
                {
                    ShopAgentId = shopAgentUser.Id,
                    Username = shopAgentUser.UserName,
                    Password = password,
                    FullName = shopAgentUser.FullName,
                    Nickname = shopAgentUser.Nickname,
                    PhoneNumber = shopAgentUser.PhoneNumber,
                    Email = shopAgentUser.Email,
                    Wechat = shopAgentUser.Wechat,
                    QQ = shopAgentUser.QQ,
                    UplineUserId = uplineUser?.Id,
                    UplineUserName = uplineUser?.UserName,
                    UplineFullName = uplineUser?.FullName,

                    IsEnabled = shopAgentUser.IsEnabled,
                    IsReviewed = shopAgentUser.IsReviewed,
                    HasGrantRight = shopAgent.HasGrantRight,
                    LastLoginIP = shopAgentUser.LastLoginIP,
                    DateLastLoggedIn = shopAgentUser.DateLastLoggedIn,
                    DateCreated = shopAgentUser.DateCreated.ToFullString(),

                    //Initailize
                    Balance = new ShopUserBalance
                    {
                        WithdrawalLimit = new Models.Queries.WithdrawalLimit
                        {
                            DailyAmountLimit = 0,
                            DailyFrequencyLimit = 0,
                            EachAmountUpperLimit = 0,
                            EachAmountLowerLimit = 0
                        },
                        WithdrawalCommissionRateInThousandth = 0,
                    },
                    RebateCommission = new RebateCommission
                    {
                        RateRebateAlipayInThousandth = 0,
                        RateRebateWechatInThousandth = 0
                    }
                };

                _shopAgentQueries.Add(shopAgentVm);
                await _shopAgentQueries.SaveChangesAsync();


                //Add Balance
                var balance = shopAgent.Balance;
                var newBalance = Distributing.Domain.Model.Balances.Balance.From(
                    shopAgentUser.Id,
                    UserType.ShopAgent,
                    balance.WithdrawalLimit.DailyAmountLimit,
                    balance.WithdrawalLimit.DailyFrequencyLimit,
                    balance.WithdrawalLimit.EachAmountUpperLimit,
                    balance.WithdrawalLimit.EachAmountLowerLimit,
                    (decimal)balance.WithdrawalCommissionRateInThousandth / 1000M,
                    0 //Shop agent has no deposit options.
                    );

                var balanceCreated = _balanceRepository.Add(newBalance);

                //Add Commission
                Commission uplineCommission = null;
                if (!string.IsNullOrEmpty(shopAgent.UplineUserId))
                {
                    uplineCommission = await _commissionRepository.GetByUserIdAsync(uplineUser.Id);

                    if (uplineCommission == null)
                    {
                        throw new ArgumentException("No upline commission found by given username.");
                    }
                }

                var rebateCommission = shopAgent.RebateCommission;

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

                var newCommission = Commission.FromShopUser(
                    balanceCreated.Id,
                    shopAgentUser.Id,
                    chainNumber,
                    UserType.ShopAgent,
                    rebateCommission.RateRebateAlipayInThousandth / 1000M,
                    rebateCommission.RateRebateWechatInThousandth / 1000M,
                    false,
                    uplineCommission
                    );

                _commissionRepository.Add(newCommission);

                await _commissionRepository.UnitOfWork.SaveEntitiesAsync();


                //return await GetShopAgent(shopAgentUser.Id);
            }
            else
            {
                throw new Exception(result.Errors.ToString());
            }
        }

        public async Task UpdateShopAgent(ShopAgent shopAgent, string password = null)
        {
            //Only manager can update shop agent.
            //Properties can not changed:
            //    ShopAgentId, Username, IsReviewed, LastLoginIP, DateLastLoggedIn, DateCreated.

            #region Update User Base Info and Roles
            //Checking the user to update is a shop agent.
            if (string.IsNullOrWhiteSpace(shopAgent.ShopAgentId))
            {
                throw new ArgumentNullException("Invalid shop agent id.");
            }

            var user = await _userManager.FindByIdAsync(shopAgent.ShopAgentId);
            if (user.BaseRoleType != BaseRoleType.ShopAgent)
            {
                throw new Exception("The user status to update is not a shop agent.");
            }

            //Updateshop agent's base info.
            user.FullName = shopAgent.FullName;
            user.Nickname = shopAgent.Nickname;
            user.PhoneNumber = shopAgent.PhoneNumber;
            user.Email = shopAgent.Email;
            user.Wechat = shopAgent.Wechat ?? string.Empty;
            user.QQ = shopAgent.QQ ?? string.Empty;

            //Updateshop agent's rights.
            if (shopAgent.IsEnabled != user.IsEnabled)
            {
                user.IsEnabled = shopAgent.IsEnabled;
                //Later need to update commission's status.
            }
            var hasGrantRight = await HasGrantRightAsync(user);
            if (shopAgent.HasGrantRight != hasGrantRight)
            {
                if (shopAgent.HasGrantRight)
                {
                    await user.AddToRole(_userManager, _roleManager, Roles.ShopAgentWithGrantRight);
                }
                else
                {
                    await user.RemoveFromRole(_userManager, _roleManager, Roles.ShopAgentWithGrantRight);
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
                shopAgent.Balance.WithdrawalCommissionRateInThousandth / 1000M);
            balance.UpdateWithdrawalLimit(
                shopAgent.Balance.WithdrawalLimit.DailyAmountLimit,
                shopAgent.Balance.WithdrawalLimit.DailyFrequencyLimit,
                shopAgent.Balance.WithdrawalLimit.EachAmountUpperLimit,
                shopAgent.Balance.WithdrawalLimit.EachAmountLowerLimit
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

            var inputUplineUserId = shopAgent.UplineUserId;

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
                    //Chekcing the upline user assigned is notshop agent himself.
                    if (!string.IsNullOrEmpty(inputUplineUserId))
                    {
                        if (inputUplineUserId == user.Id)
                        {
                            throw new ArgumentException("User's upline can not be himself.");
                        }
                    }

                    uplineUser = await _userManager.FindByIdAsync(shopAgent.UplineUserId);

                    //Checking the upline user exist.
                    if (uplineUser == null)
                    {
                        throw new ArgumentException("No upline user found by given user id.");
                    }

                    //Chekcing the upline user is reviewed.
                    if (!uplineUser.IsReviewed)
                    {
                        throw new ArgumentException("Shop agent can only create downline after reviewed.");
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
            if (shopAgent.IsEnabled != user.IsEnabled)
            {
                if (shopAgent.IsEnabled)
                {
                    commission.Enable();
                }
                else
                {
                    commission.Disable();
                }
            }

            commission.UpdateRebateRate(
                shopAgent.RebateCommission.RateRebateAlipayInThousandth / 1000M,
                shopAgent.RebateCommission.RateRebateWechatInThousandth / 1000M,
                uplineCommission
                );
            _commissionRepository.Update(commission);
            #endregion


            //Save entities and execute domain event.
            await _commissionRepository.UnitOfWork.SaveEntitiesAsync();

            var passwordChanged = false;
            //Forced change shop agent's password.
            if (!string.IsNullOrEmpty(password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var changeResult = await _userManager.ResetPasswordAsync(user, token, password);

                if (!changeResult.Succeeded)
                {
                    throw new Exception(changeResult.ToString());
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
            var shopAgentVM = await _shopAgentQueries.GetShopAgent(user.Id);
            if (shopAgentVM == null)
            {
                throw new KeyNotFoundException("Noshop agent view data found.");
            }

            shopAgentVM.FullName = user.FullName;
            shopAgentVM.Nickname = user.Nickname;
            shopAgentVM.PhoneNumber = user.PhoneNumber;
            shopAgentVM.Email = user.Email;
            shopAgentVM.Wechat = user.Wechat ?? string.Empty;
            shopAgentVM.QQ = user.QQ ?? string.Empty;
            if (uplineChanged)
            {
                shopAgentVM.UplineUserId = uplineUser?.Id;
                shopAgentVM.UplineUserName = uplineUser?.UserName;
                shopAgentVM.UplineFullName = uplineUser?.FullName;
            }
            if (passwordChanged)
            {
                shopAgentVM.Password = password;
            }
            shopAgentVM.IsEnabled = user.IsEnabled;
            shopAgentVM.IsReviewed = user.IsReviewed;
            shopAgentVM.HasGrantRight = await HasGrantRightAsync(user);

            _shopAgentQueries.Update(shopAgentVM);

            await _shopAgentQueries.SaveChangesAsync();
            #endregion
        }

        public async Task UpdateShopAgentStatus(List<AccountStatus> accounts)
        {
            //The code needs to update since it doesn't achieve atomic.
            foreach (var account in accounts)
            {
                var user = await _userManager.FindByIdAsync(account.UserId);
                if (user.BaseRoleType != BaseRoleType.ShopAgent)
                {
                    throw new Exception("The user status to update is not a shop agent.");
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
                var shopAgentVM = await _shopAgentQueries.GetShopAgent(user.Id);
                if (shopAgentVM == null)
                {
                    throw new KeyNotFoundException("Noshop agent view data found.");
                }

                shopAgentVM.IsEnabled = user.IsEnabled;

                _shopAgentQueries.Update(shopAgentVM);

                await _shopAgentQueries.SaveChangesAsync();
            }
        }

        public async Task ReviewShopAgents(List<AccountReview> accounts)
        {
            //The code needs to update since it doesn't achieve atomic.

            var toDeletes = new List<string>();

            foreach (var account in accounts)
            {
                var user = await _userManager.FindByIdAsync(account.UserId);

                if (user == null)
                {
                    if (account.IsReviewed)
                    {
                        throw new SystemException("资料缺失，请拒绝审核此代理，并重新添加。");
                    }
                    toDeletes.Add(account.UserId);
                    continue;
                }

                if (user.BaseRoleType != BaseRoleType.ShopAgent)
                {
                    if (account.IsReviewed)
                    {
                        throw new SystemException("资料缺失，请拒绝审核此代理，并重新添加。");
                    }
                    toDeletes.Add(account.UserId);
                    continue;
                }

                if (account.IsReviewed)
                {
                    //Validate all entity is ready.
                    var balance = await _balanceRepository.GetByUserIdAsync(user.Id);
                    if (balance == null)
                    {
                        throw new KeyNotFoundException("资料缺失，请拒绝审核此代理，并重新添加。");
                    }

                    var commission = await _commissionRepository.GetByUserIdAsync(user.Id);
                    if (commission == null)
                    {
                        throw new KeyNotFoundException("资料缺失，请拒绝审核此代理，并重新添加。");
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
                            var shopAgentVM = await _shopAgentQueries.GetShopAgent(user.Id);
                            if (shopAgentVM == null)
                            {
                                throw new KeyNotFoundException("Noshop agent view data found.");
                            }

                            shopAgentVM.IsReviewed = user.IsReviewed;

                            _shopAgentQueries.Update(shopAgentVM);

                            await _shopAgentQueries.SaveChangesAsync();
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
                    toDeletes.Add(user.Id);
                }
            }

            //Deleteshop agents.
            await DeleteShopAgents(toDeletes);
        }

        public async Task DeleteShopAgent(string shopAgentId = null, string shopAgentUsername = null)
        {
            ApplicationUser user;

            if (shopAgentId != null)
            {
                user = await _userManager.FindByIdAsync(shopAgentId);
            }
            else if (shopAgentUsername != null)
            {
                user = await _userManager.FindByNameAsync(shopAgentUsername);
            }
            else
            {
                throw new ArgumentNullException("Must provided id of shop agent or username ofshop agent to delete it.");
            }

            if (user != null)
            {
                //Delele user.
                if (await _userManager.IsInRoleAsync(user, Roles.ShopAgent))
                {
                    await DeleteWithoutSaveEntities(user.Id);

                    await _shopAgentQueries.SaveChangesAsync();
                    await _commissionRepository.UnitOfWork.SaveEntitiesAsync();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The user to delete is not a shop agent.");
                }
            }
            else
            {
                if (shopAgentId != null)
                {
                    await DeleteWithoutSaveEntities(shopAgentId);

                    await _shopAgentQueries.SaveChangesAsync();
                    await _commissionRepository.UnitOfWork.SaveEntitiesAsync();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        public async Task DeleteShopAgents(List<ApplicationUser> users)
        {
            var idList = new List<string>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, Roles.ShopAgent))
                {
                    await DeleteWithoutSaveEntities(user.Id);

                    idList.Add(user.Id);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The user to delete is not ashop agent.");
                }
            }
            await _shopAgentQueries.SaveChangesAsync();
            await _commissionRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task DeleteShopAgents(List<string> shopAgentIds)
        {
            var users = new List<ApplicationUser>();

            foreach (var shopAgentId in shopAgentIds)
            {
                var user = await _userManager.FindByIdAsync(shopAgentId);
                if (user == null)
                {
                    if (!string.IsNullOrWhiteSpace(shopAgentId))
                    {
                        await DeleteWithoutSaveEntities(shopAgentId);

                    }
                    else
                    {
                        throw new KeyNotFoundException("No user found by given Id.");
                    }
                }
                else
                {
                    users.Add(user);
                }
            }

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, Roles.ShopAgent))
                {
                    await DeleteWithoutSaveEntities(user.Id);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The user to delete is not ashop agent.");
                }
            }

            await _shopAgentQueries.SaveChangesAsync();
            await _commissionRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task ChangeBalance(string type, string userId, decimal amount, string description, string createByUserId)
        {
            //Checking the changing is made by manager.
            var changeByUser = await _userManager.FindByIdAsync(createByUserId);
            if (changeByUser.BaseRoleType != BaseRoleType.Manager)
            {
                throw new InvalidOperationException($"Only manager can change shop agent's balance.");
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
        private async Task<RebateCommission> GetRebateCommissionAsync(ApplicationUser user)
        {
            return await _commissionQueries.GetCommissionFromShopUserAsync(user.Id);
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
            return await _userManager.IsInRoleAsync(user, Roles.ShopAgentWithGrantRight);
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

        private async Task<bool> IsAboveAgentChainAsync(ApplicationUser shopAgent, ApplicationUser bottomUser)
        {
            if (bottomUser == null || shopAgent == null)
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
                if (shopAgent.Id == currentUser.Id)
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

        private async Task DeleteWithoutSaveEntities(string userId)
        {
            //Delete user, balance and commission.
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var deleteUserResult = await _userManager.DeleteAsync(user);
                if (!deleteUserResult.Succeeded)
                {
                    throw new Exception("Delete user failed. user id:" + user.Id);
                }
            }

            var balance = await _balanceRepository.GetByUserIdAsync(userId);
            if (balance != null)
            {
                _balanceRepository.Delete(balance);
            }

            var commission = await _commissionRepository.GetByUserIdAsync(userId);
            if (commission != null)
            {
                _commissionRepository.Delete(commission);
            }

            //Delete view model.
            var shopAgentVM = await _shopAgentQueries.GetShopAgent(userId);
            if (shopAgentVM != null)
            {
                _shopAgentQueries.Delete(shopAgentVM);
            }

            //Delete the reference from downlines.
            var downlines = _userManager.Users.Where(u => u.UplineId == userId);
            foreach (var downline in downlines)
            {
                downline.UplineId = null;
                await _userManager.UpdateAsync(downline);

                //Update view model.
                var downlineVM = await _shopAgentQueries.GetShopAgent(downline.Id);
                if (downlineVM != null)
                {
                    downlineVM.UplineUserId = null;
                    downlineVM.UplineUserName = null;
                    downlineVM.UplineFullName = null;
                    _shopAgentQueries.Update(downlineVM);
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

        private void ValidateDownlineDataChangeSubmittedByShopAgent(ShopAgent downlineData, ShopAgent shopAgent)
        {
            //Only manager can change withdrawal and deposit rate.
            var withdrawalConf = _systemConfigurationService.GetWithdrawalAndDepositAsync();
            if (downlineData.Balance.WithdrawalCommissionRateInThousandth != withdrawalConf.WithdrawalTemplate.CommissionInThousandth
                || downlineData.Balance.WithdrawalLimit.DailyAmountLimit != withdrawalConf.WithdrawalTemplate.DailyAmountLimit
                || downlineData.Balance.WithdrawalLimit.DailyFrequencyLimit != withdrawalConf.WithdrawalTemplate.DailyFrequencyLimit
                || downlineData.Balance.WithdrawalLimit.EachAmountLowerLimit != withdrawalConf.WithdrawalTemplate.EachAmountLowerLimit
                || downlineData.Balance.WithdrawalLimit.EachAmountUpperLimit != withdrawalConf.WithdrawalTemplate.EachAmountUpperLimit)
            {
                throw new InvalidOperationException("Shop agent can not set withdrawal & deposit commission rates.");
            }

            //Checking the given id of upline is matched,
            if (string.IsNullOrWhiteSpace(downlineData.UplineUserId)
                || downlineData.UplineUserId != shopAgent.ShopAgentId)
            {
                throw new InvalidOperationException("Invalid upline Id.");
            }
        }
        #endregion
    }
}
