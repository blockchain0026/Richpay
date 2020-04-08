using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Deposits;
using Distributing.Domain.Model.Frozens;
using Distributing.Domain.Model.Shared;
using Distributing.Domain.Model.Transfers;
using Distributing.Domain.Model.Withdrawals;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Model.ShopApis;
using Pairing.Domain.Model.ShopGateways;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Balances;
using WebMVC.Applications.Queries.Bankbook;
using WebMVC.Applications.Queries.Commission;
using WebMVC.Applications.Queries.Frozen;
using WebMVC.Applications.Queries.ShopAgents;
using WebMVC.Applications.Queries.ShopGateways;
using WebMVC.Applications.Queries.Shops;
using WebMVC.Applications.Queries.ShopSettings;
using WebMVC.Data;
using WebMVC.Extensions;
using WebMVC.Models;
using WebMVC.Models.Roles;
using WebMVC.ViewModels;
using Distributing.Infrastructure;
using Distributing.Domain.Model.Roles;
using WebMVC.Applications.Queries.IpWhitelists;
using Distributing.Domain.Model.Chains;
using WebMVC.Models.Queries;

namespace WebMVC.Infrastructure.Services.Implementations
{
    public class ShopService : IShopService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IBalanceQueries _balanceQueries;
        private readonly ICommissionQueries _commissionQueries;
        private readonly IBankbookQueries _bankbookQueries;
        private readonly IFrozenQueries _frozenQueries;
        private readonly IShopQueries _shopQueries;
        private readonly IShopAgentQueries _shopAgentQueries;
        private readonly IShopGatewayQueries _shopGatewayQueries;
        private readonly IShopSettingsQueries _shopSettingsQueries;
        private readonly IIpWhitelistQueries _ipWhitelistQueries;

        private readonly IBalanceRepository _balanceRepository;
        private readonly IChainRepository _chainRepository;
        private readonly ICommissionRepository _commissionRepository;
        private readonly ITransferRepository _transferRepository;
        private readonly IDepositRepository _depositRepository;
        private readonly IWithdrawalRepository _withdrawalRepository;
        private readonly IFrozenRepository _frozenRepository;
        private readonly IShopGatewayRepository _shopGatewayRepository;
        private readonly IShopSettingsRepository _shopSettingsRepository;
        private readonly IShopApiRepository _shopApiRepository;


        private readonly Distributing.Domain.Model.Shared.IDateTimeService _distributingDateTimeService;
        private readonly Pairing.Domain.Model.Shared.IDateTimeService _pairingDateTimeService;
        private readonly ISystemConfigurationService _systemConfigurationService;
        private readonly IBalanceDomainService _balanceDomainService;


        private readonly DistributingContext _distributingContext;
        private readonly ApplicationDbContext _applicationDbContext;

        public ShopService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IBalanceQueries balanceQueries, ICommissionQueries commissionQueries, IBankbookQueries bankbookQueries, IFrozenQueries frozenQueries, IShopQueries shopQueries, IShopAgentQueries shopAgentQueries, IShopGatewayQueries shopGatewayQueries, IShopSettingsQueries shopSettingsQueries, IIpWhitelistQueries ipWhitelistQueries, IBalanceRepository balanceRepository, IChainRepository chainRepository, ICommissionRepository commissionRepository, ITransferRepository transferRepository, IDepositRepository depositRepository, IWithdrawalRepository withdrawalRepository, IFrozenRepository frozenRepository, IShopGatewayRepository shopGatewayRepository, IShopSettingsRepository shopSettingsRepository, IShopApiRepository shopApiRepository, IDateTimeService distributingDateTimeService, Pairing.Domain.Model.Shared.IDateTimeService pairingDateTimeService, ISystemConfigurationService systemConfigurationService, IBalanceDomainService balanceDomainService, DistributingContext distributingContext, ApplicationDbContext applicationDbContext)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _balanceQueries = balanceQueries ?? throw new ArgumentNullException(nameof(balanceQueries));
            _commissionQueries = commissionQueries ?? throw new ArgumentNullException(nameof(commissionQueries));
            _bankbookQueries = bankbookQueries ?? throw new ArgumentNullException(nameof(bankbookQueries));
            _frozenQueries = frozenQueries ?? throw new ArgumentNullException(nameof(frozenQueries));
            _shopQueries = shopQueries ?? throw new ArgumentNullException(nameof(shopQueries));
            _shopAgentQueries = shopAgentQueries ?? throw new ArgumentNullException(nameof(shopAgentQueries));
            _shopGatewayQueries = shopGatewayQueries ?? throw new ArgumentNullException(nameof(shopGatewayQueries));
            _shopSettingsQueries = shopSettingsQueries ?? throw new ArgumentNullException(nameof(shopSettingsQueries));
            _ipWhitelistQueries = ipWhitelistQueries ?? throw new ArgumentNullException(nameof(ipWhitelistQueries));
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _chainRepository = chainRepository ?? throw new ArgumentNullException(nameof(chainRepository));
            _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
            _transferRepository = transferRepository ?? throw new ArgumentNullException(nameof(transferRepository));
            _depositRepository = depositRepository ?? throw new ArgumentNullException(nameof(depositRepository));
            _withdrawalRepository = withdrawalRepository ?? throw new ArgumentNullException(nameof(withdrawalRepository));
            _frozenRepository = frozenRepository ?? throw new ArgumentNullException(nameof(frozenRepository));
            _shopGatewayRepository = shopGatewayRepository ?? throw new ArgumentNullException(nameof(shopGatewayRepository));
            _shopSettingsRepository = shopSettingsRepository ?? throw new ArgumentNullException(nameof(shopSettingsRepository));
            _shopApiRepository = shopApiRepository ?? throw new ArgumentNullException(nameof(shopApiRepository));
            _distributingDateTimeService = distributingDateTimeService ?? throw new ArgumentNullException(nameof(distributingDateTimeService));
            _pairingDateTimeService = pairingDateTimeService ?? throw new ArgumentNullException(nameof(pairingDateTimeService));
            _systemConfigurationService = systemConfigurationService ?? throw new ArgumentNullException(nameof(systemConfigurationService));
            _balanceDomainService = balanceDomainService ?? throw new ArgumentNullException(nameof(balanceDomainService));
            _distributingContext = distributingContext ?? throw new ArgumentNullException(nameof(distributingContext));
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        }


        #region Queries
        public async Task<Shop> GetShop(string shopId, string searchByUplineId = null)
        {
            var shop = await _shopQueries.GetShop(shopId);

            if (shop == null)
            {
                throw new Exception("No shop found.");
            }

            //If this is searching by a shop agent, check he is the user's upline.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                if (string.IsNullOrEmpty(shop.UplineUserId) || shop.UplineUserId != searchByUplineId)
                {
                    throw new InvalidOperationException("Shop agent can not read user who isn't his downline.");
                }
            }

            return shop;
        }

        public List<string> GetIpWhitelistByShopId(string shopId)
        {
            var ipWhitelists = _ipWhitelistQueries.GetIpWhitelistsByShopId(shopId);

            return ipWhitelists;
        }


        public async Task<List<Shop>> GetShops(int pageIndex, int take, string searchString = "", string sortField = "", string direction = SortDirections.Descending)
        {
            List<Shop> itemsOnPage = null;

            itemsOnPage = await _shopQueries.GetShops(
                pageIndex,
                take,
                searchString,
                sortField,
                direction
                );

            return itemsOnPage;
        }

        public async Task<int> GetShopsTotalCount(string searchString = null)
        {
            var count = await _shopQueries.GetShopsTotalCount(searchString);

            return count;
        }

        public List<Shop> GetPendingReviewShops(int pageIndex, int take, string searchString = "", string sortField = "", string direction = "desc")
        {
            List<Shop> itemsOnPage = null;

            itemsOnPage = _shopQueries.GetPendingReviews(
                pageIndex,
                take,
                null,
                searchString,
                sortField,
                direction
                );

            return itemsOnPage;
        }

        public async Task<int> GetPendingReviewShopsTotalCount(string searchString = null)
        {
            var count = await _shopQueries.GetPendingReviewsTotalCount(null, searchString);
            return count;
        }

        public List<Shop> GetPendingReviewDownlines(int pageIndex, int take, string searchByUplineId, string searchString = "", string sortField = "", string direction = SortDirections.Descending)
        {
            List<Shop> itemsOnPage = null;

            itemsOnPage = _shopQueries.GetPendingReviews(
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
            var count = await _shopQueries.GetPendingReviewsTotalCount(searchByUplineId, searchString);
            return count;
        }

        public async Task<List<Shop>> GetDownlines(int pageIndex, int take, string searchByUplineId, string searchString = "", string sortField = "", string direction = SortDirections.Descending)
        {
            List<Shop> itemsOnPage = null;
            itemsOnPage = await _shopQueries.GetDownlines(
                pageIndex,
                take,
                searchByUplineId,
                searchString,
                sortField,
                direction
                );

            return itemsOnPage;
        }

        public async Task<int> GetDownlinesTotalCount(string searchByUplineId, string searchString = null)
        {

            var count = await _shopQueries.GetDownlinesTotalCount(searchByUplineId);

            return count;
        }



        public async Task<int> GetAwaitingUnfrozeByAdminTotalCount(string userId, string searchByUplineId = null, string searchString = null)
        {
            //If this is searching by an user's upline, checking the upline is the user's upline.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                //Checking the data is searched by user's upline.
                if (!await IsUplineOf(searchByUplineId, userId))
                {
                    throw new ArgumentNullException("Shop agent can not search shop's frozen record who isn't his downline.");
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
            //If this is searching by an user's upline, checking the upline is the user's upline.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                //Checking the data is searched by user's upline.
                if (!await IsUplineOf(searchByUplineId, userId))
                {
                    throw new ArgumentNullException("Shop agent can not search shop's frozen record who isn't his downline.");
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
            //Should validate the give id is a shop's id.
            //
            //

            return await _commissionQueries.GetCommissionFromShopUserAsync(shopAgentId);
        }

        public async Task<List<BankbookRecord>> GetBankbookRecords(int pageIndex, int take, string shopId, string searchByUplineId = null, string searchString = "", string sortField = "", string direction = SortDirections.Descending)
        {
            //If this is searching by an user's upline, checking the upline is the user's upline.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                //Checking the data is searched by user's upline.
                if (!await IsUplineOf(searchByUplineId, shopId))
                {
                    throw new ArgumentNullException("Shop agent can not search shop's bankbook record who isn't his downline.");
                }
            }

            var bankbookRecords = _bankbookQueries.GetBankbookRecordsByUserIdAsync(
                shopId,
                pageIndex,
                take,
                searchString,
                sortField,
                direction
                );


            return bankbookRecords;
        }

        public async Task<int> GetBankbookRecordsTotalCount(string shopId, string searchByUplineId = null, string searchString = null)
        {
            //If this is searching by an user's upline, checking the upline is the user's upline.
            if (!string.IsNullOrEmpty(searchByUplineId))
            {
                //Checking the data is searched by user's upline.
                if (!await IsUplineOf(searchByUplineId, shopId))
                {
                    throw new ArgumentNullException("Shop agent can not search shop's bankbook record who isn't his downline.");
                }
            }

            var count = await _bankbookQueries.GetBankbookRecordsTotalCountAsync(shopId, searchString);

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
                throw new KeyNotFoundException("No shop agent found by given Id.");
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



        public async Task<List<ShopGatewayEntry>> GetShopGateways(int pageIndex, int take, string searchUserId, string shopId, string searchString = "", string sortField = "",
            string shopGatewayType = null, string paymentChannel = null, string paymentScheme = null,
            string direction = SortDirections.Descending)
        {
            //Check the user exist.
            var searchByUser = await _userManager.FindByIdAsync(searchUserId);

            if (searchByUser is null)
            {
                throw new KeyNotFoundException("No user found by given user Id.");
            }

            //Check the user is admin or shop gateway's owner.
            if (searchByUser.BaseRoleType != BaseRoleType.Manager)
            {
                if (searchByUser.BaseRoleType != BaseRoleType.Shop || searchByUser.Id != shopId)
                {
                    throw new InvalidOperationException("用户没有此权限。");
                }
            }


            var shopGatewayEntries = await _shopGatewayQueries.GetShopGatewayEntrysByShopIdAsync(
                shopId,
                pageIndex,
                take,
                searchString,
                sortField,
                shopGatewayType,
                paymentChannel,
                paymentScheme,
                direction
                );

            return shopGatewayEntries;
        }

        public async Task<int> GetShopGatewaysTotalCount(string searchByUserId, string shopId, string searchString = null,
            string shopGatewayType = null, string paymentChannel = null, string paymentScheme = null)
        {
            //Check the user exist.
            var searchByUser = await _userManager.FindByIdAsync(searchByUserId);

            if (searchByUser is null)
            {
                throw new KeyNotFoundException("No user found by given user Id.");
            }

            //Check the user is admin or shop gateway's owner.
            if (searchByUser.BaseRoleType != BaseRoleType.Manager)
            {
                if (searchByUser.BaseRoleType != BaseRoleType.Shop || searchByUser.Id != shopId)
                {
                    throw new InvalidOperationException("用户没有此权限。");
                }
            }


            var count = await _shopGatewayQueries.GetShopGatewayEntrysTotalCountByShopIdAsync(
                shopId,
                searchString,
                shopGatewayType,
                paymentChannel,
                paymentScheme
                );

            return count;
        }

        public async Task<List<decimal>> GetShopOrderAmountOpitions(string searchByUserId, string shopId)
        {
            //Check the user exist.
            var searchByUser = await _userManager.FindByIdAsync(searchByUserId);

            if (searchByUser is null)
            {
                throw new KeyNotFoundException("No user found by given user Id.");
            }

            //Check the user's permission.
            if (searchByUser.BaseRoleType != BaseRoleType.Manager)
            {
                //Only shop settings owner and trader can get options. (trader need these info to create barcodes)
                if (searchByUser.BaseRoleType == BaseRoleType.Shop)
                {
                    if (searchByUser.Id != shopId)
                    {
                        throw new InvalidOperationException("商户只能查看自己的金额选项");
                    }
                }
                else if (searchByUser.BaseRoleType == BaseRoleType.Trader)
                {
                    //Fine
                }
                else
                {
                    throw new InvalidOperationException("用户没有此权限。");
                }
            }

            //Get shop's order amount options.
            return await this._shopSettingsQueries.GetOrderAmountOptionsByShopIdAsync(shopId);
        }

        #endregion


        #region Operations
        public async Task CreateShop(Shop shop, string password, string createByShopAgentId = null)
        {
            //If this is requested by a shop agent, validate the data he submitted.
            if (!string.IsNullOrEmpty(createByShopAgentId))
            {
                var createByShopAgent = await _shopAgentQueries.GetShopAgent(createByShopAgentId);
                if (createByShopAgent == null)
                {
                    throw new InvalidOperationException("The user creating downlines is not a shop agent or is not reviewed yet.");
                }

                this.ValidateDownlineDataChangeSubmittedByShop(shop, createByShopAgent);
            }

            ApplicationUser uplineUser = null;
            //Checking the upline is reviewed.
            if (!string.IsNullOrWhiteSpace(shop.UplineUserId))
            {
                //Checking the upline is not shop himself.
                /*if (shop.UplineUserId == shop.ShopId)
                {
                    throw new ArgumentException("User's upline can not be himself.");
                }*/

                //Checking the upline user exist.
                uplineUser = await _userManager.FindByIdAsync(shop.UplineUserId);
                if (uplineUser == null)
                {
                    throw new ArgumentException("No upline user found by given user id.");
                }

                //Checking the upline is reviewed.
                if (!uplineUser.IsReviewed)
                {
                    throw new ArgumentException("User can only create downline after reviewed.");
                }

                //Checking the upline is a shop.
                if (uplineUser.BaseRoleType != BaseRoleType.ShopAgent)
                {
                    throw new ArgumentException("A shop can not be a downline of user who is not a shop agent.");
                }
            }

            //Validate the date time format.
            DateTime dateCreated;

            if (!string.IsNullOrWhiteSpace(shop.DateCreated))
            {
                if (!DateTime.TryParseExact(
                    shop.DateCreated,
                    DateTimeExtensions.GetFormatFullString(),
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out dateCreated))
                {
                    throw new ArgumentException("Invaild param:" + shop.DateCreated +
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
                FullName = shop.FullName,
                Nickname = shop.FullName,
                PhoneNumber = shop.PhoneNumber,
                Email = shop.Email,
                UserName = shop.Username,

                //Must be reviewed first.
                IsEnabled = false,

                //For testing, must remove in formal environment.
                IsReviewed = shop.IsReviewed,

                Wechat = shop.Wechat ?? string.Empty,
                QQ = shop.QQ ?? string.Empty,

                //Must set to shop.
                BaseRoleType = BaseRoleType.Shop,

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
                var shopUser = await _userManager.FindByNameAsync(shop.Username);
                await shopUser.AddToRole(_userManager, _roleManager, Roles.Shop);


                //For testing, must remove in formal environment.
                if (shop.IsReviewed)
                {
                    await shopUser.AddToRole(_userManager, _roleManager, Roles.UserReviewed);
                }

                //Add to view model database.
                var shopVm = new Shop
                {
                    ShopId = shopUser.Id,
                    Username = shopUser.UserName,
                    Password = password,
                    FullName = shopUser.FullName,
                    SiteAddress = shop.SiteAddress,
                    PhoneNumber = shopUser.PhoneNumber,
                    Email = shopUser.Email,
                    Wechat = shopUser.Wechat,
                    QQ = shopUser.QQ,
                    UplineUserId = uplineUser?.Id,
                    UplineUserName = uplineUser?.UserName,
                    UplineFullName = uplineUser?.FullName,

                    IsEnabled = shopUser.IsEnabled,
                    IsReviewed = shopUser.IsReviewed,
                    LastLoginIP = shopUser.LastLoginIP,
                    DateLastLoggedIn = shopUser.DateLastLoggedIn,
                    DateCreated = shopUser.DateCreated.ToFullString(),

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
                        WithdrawalCommissionRateInThousandth = 0
                    },
                    RebateCommission = new RebateCommission
                    {
                        RateRebateAlipayInThousandth = 0,
                        RateRebateWechatInThousandth = 0
                    }
                };

                _shopQueries.Add(shopVm);
                await _shopQueries.SaveChangesAsync();


                //Add Balance
                var balance = shop.Balance;
                var newBalance = Distributing.Domain.Model.Balances.Balance.From(
                    shopUser.Id,
                    UserType.Shop,
                    balance.WithdrawalLimit.DailyAmountLimit,
                    balance.WithdrawalLimit.DailyFrequencyLimit,
                    balance.WithdrawalLimit.EachAmountUpperLimit,
                    balance.WithdrawalLimit.EachAmountLowerLimit,
                    (decimal)balance.WithdrawalCommissionRateInThousandth / 1000M,
                    0
                    );

                var balanceCreated = _balanceRepository.Add(newBalance);

                //Add Commission
                Commission uplineCommission = null;
                if (!string.IsNullOrEmpty(shop.UplineUserId))
                {
                    uplineCommission = await _commissionRepository.GetByUserIdAsync(uplineUser.Id);

                    if (uplineCommission == null)
                    {
                        throw new ArgumentException("No upline commission found by given username.");
                    }
                }

                var rebateCommission = shop.RebateCommission;

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
                    shopUser.Id,
                    chainNumber,
                    UserType.Shop,
                    rebateCommission.RateRebateAlipayInThousandth / 1000M,
                    rebateCommission.RateRebateWechatInThousandth / 1000M,
                    false,
                    uplineCommission
                    );

                _commissionRepository.Add(newCommission);

                await _commissionRepository.UnitOfWork.SaveEntitiesAsync();


                //Add Shop Settings
                var shopSettings = new ShopSettings(shopUser.Id);
                this._shopSettingsRepository.Add(shopSettings);

                await _shopSettingsRepository.UnitOfWork.SaveEntitiesAsync();


                //Add Shop Gateways
                await CreateShopGateways(shopUser.Id);

                //Add Shop api.
                var shopApi = ShopApi.From(shopUser.Id);
                _shopApiRepository.Add(shopApi);
                await _shopApiRepository.UnitOfWork.SaveEntitiesAsync();

                //return await GetShop(shopUser.Id);
            }
            else
            {
                throw new Exception(result.Errors.ToString());
            }
        }

        public async Task UpdateShop(Shop shop, string password = null, string updateByShopAgentId = null)
        {
            //Properties can not changed:
            //    ShopId, Username, IsReviewed, LastLoginIP, DateLastLoggedIn, DateCreated.

            var user = await _userManager.FindByIdAsync(shop.ShopId);

            //If this is updated by a shop agent, check the shop's upline is him.
            if (!string.IsNullOrEmpty(updateByShopAgentId))
            {
                if (user.UplineId != updateByShopAgentId)
                {
                    throw new InvalidOperationException("Shop agent can not update shop who isn't his downline.");
                }
            }


            #region Update User Base Info and Roles
            //Checking the user to update is a shop.
            if (string.IsNullOrWhiteSpace(shop.ShopId))
            {
                throw new ArgumentNullException("Invalid shop id.");
            }

            if (user.BaseRoleType != BaseRoleType.Shop)
            {
                throw new Exception("The user status to update is not a shop.");
            }

            //Update shop's base info.
            user.FullName = shop.FullName;
            user.Nickname = shop.FullName;
            user.PhoneNumber = shop.PhoneNumber;
            user.Email = shop.Email;
            user.Wechat = shop.Wechat ?? string.Empty;
            user.QQ = shop.QQ ?? string.Empty;

            //Update shop's rights.
            if (shop.IsEnabled != user.IsEnabled)
            {
                user.IsEnabled = shop.IsEnabled;
                //Later need to update commission's status.
            }
            #endregion

            #region Update Balance
            var balance = await _balanceRepository.GetByUserIdAsync(user.Id);
            if (balance == null)
            {
                throw new ArgumentNullException("No balance found by given user Id.");
            }
            balance.UpdateWithdrawalFeeRate(
                shop.Balance.WithdrawalCommissionRateInThousandth / 1000M);
            balance.UpdateWithdrawalLimit(
                shop.Balance.WithdrawalLimit.DailyAmountLimit,
                shop.Balance.WithdrawalLimit.DailyFrequencyLimit,
                shop.Balance.WithdrawalLimit.EachAmountUpperLimit,
                shop.Balance.WithdrawalLimit.EachAmountLowerLimit
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

            var inputUplineUserId = shop.UplineUserId;

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
                //Update user's upline id.
                if (!string.IsNullOrEmpty(inputUplineUserId))
                {
                    //Chekcing the upline user assigned is not shop himself.
                    if (!string.IsNullOrEmpty(inputUplineUserId))
                    {
                        if (inputUplineUserId == user.Id)
                        {
                            throw new ArgumentException("User's upline can not be himself.");
                        }
                    }

                    uplineUser = await _userManager.FindByIdAsync(shop.UplineUserId);

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
            if (shop.IsEnabled != user.IsEnabled)
            {
                if (shop.IsEnabled)
                {
                    commission.Enable();
                }
                else
                {
                    commission.Disable();
                }
            }

            commission.UpdateRebateRate(
                shop.RebateCommission.RateRebateAlipayInThousandth / 1000M,
                shop.RebateCommission.RateRebateWechatInThousandth / 1000M,
                uplineCommission
                );
            _commissionRepository.Update(commission);
            #endregion


            //Save entities and execute domain event.
            await _commissionRepository.UnitOfWork.SaveEntitiesAsync();

            var passwordChanged = false;
            //Forced change shop's password.
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
            var shopVM = await _shopQueries.GetShop(user.Id);
            if (shopVM == null)
            {
                throw new KeyNotFoundException("No shop view data found.");
            }

            shopVM.FullName = user.FullName;
            shopVM.SiteAddress = shop.SiteAddress;
            shopVM.PhoneNumber = user.PhoneNumber;
            shopVM.Email = user.Email;
            shopVM.Wechat = user.Wechat ?? string.Empty;
            shopVM.QQ = user.QQ ?? string.Empty;
            if (uplineChanged)
            {
                shopVM.UplineUserId = uplineUser?.Id;
                shopVM.UplineUserName = uplineUser?.UserName;
                shopVM.UplineFullName = uplineUser?.FullName;
            }
            if (passwordChanged)
            {
                shopVM.Password = password;
            }
            shopVM.IsEnabled = user.IsEnabled;
            shopVM.IsReviewed = user.IsReviewed;

            _shopQueries.Update(shopVM);

            await _shopQueries.SaveChangesAsync();
            #endregion
        }

        public async Task UpdateShopStatus(List<AccountStatus> accounts, string updateByShopAgentId = null)
        {
            //The code needs to update since it doesn't achieve atomic.
            foreach (var account in accounts)
            {
                var user = await _userManager.FindByIdAsync(account.UserId);

                //If this is updated by a shop agent, check the shop's upline is him.
                if (!string.IsNullOrEmpty(updateByShopAgentId))
                {
                    if (user.UplineId != updateByShopAgentId)
                    {
                        throw new InvalidOperationException("Shop agent can not update shop who isn't his downline.");
                    }
                }

                //Checking the user's role.
                if (user.BaseRoleType != BaseRoleType.Shop)
                {
                    throw new Exception("The user status to update is not a shop.");
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
                var shopVM = await _shopQueries.GetShop(user.Id);
                if (shopVM == null)
                {
                    throw new KeyNotFoundException("No shop view data found.");
                }

                shopVM.IsEnabled = user.IsEnabled;

                _shopQueries.Update(shopVM);

                await _shopQueries.SaveChangesAsync();
            }
        }

        public async Task ReviewShops(List<AccountReview> accounts)
        {
            //The code needs to update since it doesn't achieve atomic.

            var toDeletes = new List<ApplicationUser>();

            foreach (var account in accounts)
            {
                var user = await _userManager.FindByIdAsync(account.UserId);
                if (user.BaseRoleType != BaseRoleType.Shop)
                {
                    throw new Exception("The user to review is not a shop.");
                }

                if (account.IsReviewed)
                {
                    //Validate all entity is ready.
                    var balance = await _balanceRepository.GetByUserIdAsync(user.Id);
                    if (balance == null)
                    {
                        throw new SystemException("资料缺失，请拒绝审核此交易员，并重新添加。");
                    }

                    var commission = await _commissionRepository.GetByUserIdAsync(user.Id);
                    if (commission == null)
                    {
                        throw new SystemException("资料缺失，请拒绝审核此交易员，并重新添加。");
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
                            var shopVM = await _shopQueries.GetShop(user.Id);
                            if (shopVM == null)
                            {
                                throw new KeyNotFoundException("No shop view data found.");
                            }

                            shopVM.IsReviewed = user.IsReviewed;

                            _shopQueries.Update(shopVM);

                            await _shopQueries.SaveChangesAsync();
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

            //Delete shops.
            await DeleteShops(toDeletes);
        }

        public async Task DeleteShop(string shopId = null, string shopUsername = null, string deleteByShopAgentId = null)
        {
            ApplicationUser user;

            if (shopId != null)
            {
                user = await _userManager.FindByIdAsync(shopId);
            }
            else if (shopUsername != null)
            {
                user = await _userManager.FindByNameAsync(shopUsername);
            }
            else
            {
                throw new ArgumentNullException("Must provided id of shop or username of shop to delete it.");
            }


            if (user != null)
            {
                //If this is delete by a shop agent, check the shop's upline is him.
                if (!string.IsNullOrEmpty(deleteByShopAgentId))
                {
                    if (user.UplineId != deleteByShopAgentId)
                    {
                        throw new InvalidOperationException("Shop agent can not delete shop who isn't his downline.");
                    }
                }

                //Delele user.
                if (await _userManager.IsInRoleAsync(user, Roles.Shop))
                {
                    await DeleteWithoutSaveEntities(user);

                    await _shopQueries.SaveChangesAsync();
                    await _commissionRepository.UnitOfWork.SaveEntitiesAsync();
                    await _shopSettingsRepository.UnitOfWork.SaveEntitiesAsync();
                    await _shopApiRepository.UnitOfWork.SaveEntitiesAsync();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The user to delete is not a shop.");
                }
            }
        }

        public async Task DeleteShops(List<ApplicationUser> users, string deleteByShopAgentId = null)
        {
            var idList = new List<string>();

            foreach (var user in users)
            {
                //If this is delete by a shop agent, check the shop's upline is him.
                if (!string.IsNullOrEmpty(deleteByShopAgentId))
                {
                    if (user.UplineId != deleteByShopAgentId)
                    {
                        throw new InvalidOperationException("Shop agent can not delete shop who isn't his downline.");
                    }
                }

                if (await _userManager.IsInRoleAsync(user, Roles.Shop))
                {
                    await DeleteWithoutSaveEntities(user);

                    idList.Add(user.Id);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The user to delete is not a shop.");
                }
            }
            await _shopQueries.SaveChangesAsync();
            await _commissionRepository.UnitOfWork.SaveEntitiesAsync();
            await _shopSettingsRepository.UnitOfWork.SaveEntitiesAsync();
            await _shopApiRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task DeleteShops(List<string> shopIds, string deleteByShopAgentId = null)
        {
            var users = new List<ApplicationUser>();

            foreach (var shopId in shopIds)
            {
                var user = await _userManager.FindByIdAsync(shopId);

                if (user == null)
                {
                    throw new KeyNotFoundException("No user found by given Id.");
                }

                //If this is delete by a shop agent, check the shop's upline is him.
                if (!string.IsNullOrEmpty(deleteByShopAgentId))
                {
                    if (user.UplineId != deleteByShopAgentId)
                    {
                        throw new InvalidOperationException("Shop agent can not delete shop who isn't his downline.");
                    }
                }

                users.Add(user);
            }

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, Roles.Shop))
                {
                    await DeleteWithoutSaveEntities(user);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The user to delete is not a shop.");
                }
            }

            await _shopQueries.SaveChangesAsync();
            await _commissionRepository.UnitOfWork.SaveEntitiesAsync();
            await _shopSettingsRepository.UnitOfWork.SaveEntitiesAsync();
            await _shopApiRepository.UnitOfWork.SaveEntitiesAsync();
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
                _distributingDateTimeService
                );

            _frozenRepository.Update(frozen);

            await _frozenRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task CreatePlatformShopGateway(string shopId, string paymentChannel, string paymentScheme,
            string gatewayNumber, string name,
            int secondsBeforePayment, bool isAmountUnchangeable, bool isAccountUnchangeable, bool isH5RedirectByScanEnabled, bool isH5RedirectByClickEnabled, bool isH5RedirectByPickingPhotoEnabled)
        {
            var shop = await _shopQueries.GetShop(shopId);
            if (shop == null)
            {
                throw new KeyNotFoundException("No shop found by given user Id.");
            }


            var shopGateway = ShopGateway.FromPlatForm(
                shopId,
                Pairing.Domain.Model.QrCodes.PaymentChannel.FromName(paymentChannel),
                Pairing.Domain.Model.QrCodes.PaymentScheme.FromName(paymentScheme),
                _pairingDateTimeService,
                secondsBeforePayment,
                isAmountUnchangeable,
                isAccountUnchangeable,
                isH5RedirectByScanEnabled,
                isH5RedirectByClickEnabled,
                isH5RedirectByPickingPhotoEnabled
                );

            _shopGatewayRepository.Update(shopGateway);

            await _shopGatewayRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task UpdateShopGatewayAlipayPreference(int shopGatewayId,
            int secondsBeforePayment, bool isAmountUnchangeable, bool isAccountUnchangeable,
            bool isH5RedirectByScanEnabled, bool isH5RedirectByClickEnabled, bool isH5RedirectByPickingPhotoEnabled)
        {
            var shopGateway = await _shopGatewayRepository.GetByShopGatewayIdAsync(shopGatewayId);
            if (shopGateway == null)
            {
                throw new KeyNotFoundException("No shop gateway found by given Id.");
            }

            shopGateway.UpdateAlipayPreference(
                secondsBeforePayment,
                isAmountUnchangeable,
                isAccountUnchangeable,
                isH5RedirectByScanEnabled,
                isH5RedirectByClickEnabled,
                isH5RedirectByPickingPhotoEnabled
                );

            this._shopGatewayRepository.Update(shopGateway);

            await _shopGatewayRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public void DeleteShopGateway(int shopGatewayId)
        {
            throw new NotImplementedException();
        }

        public async Task AddAmountOption(string shopId, decimal amount)
        {
            var shopSettings = await _shopSettingsRepository.GetByShopIdAsync(shopId);
            if (shopSettings is null)
            {
                throw new KeyNotFoundException("找无商户设定档。");
            }

            shopSettings.AddAmountOption(amount, _pairingDateTimeService);

            _shopSettingsRepository.Update(shopSettings);

            await _shopSettingsRepository.UnitOfWork.SaveEntitiesAsync();
        }

        public async Task DeleteAmountOption(string shopId, decimal amount)
        {
            var shopSettings = await _shopSettingsRepository.GetByShopIdAsync(shopId);
            if (shopSettings is null)
            {
                throw new KeyNotFoundException("找无商户设定档。");
            }

            shopSettings.DeleteAmountOption(amount);

            _shopSettingsRepository.Update(shopSettings);

            await _shopSettingsRepository.UnitOfWork.SaveEntitiesAsync();
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
                    .SingleOrDefaultAsync();

            while (!isAboveAgentChain)
            {
                //Check the current user's id is equal to shop's id.
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

                var uplineUser = await _userManager.Users
                    .Where(u => u.Id == uplineId)
                    .SingleOrDefaultAsync();

                currentUser = uplineUser;
            }

            return isAboveAgentChain;
        }

        private async Task CreateShopGateways(string shopId)
        {
            //Wechat barcode.
            var wechatBarcodeGateway = ShopGateway.FromPlatForm(
                shopId,
                Pairing.Domain.Model.QrCodes.PaymentChannel.Wechat,
                Pairing.Domain.Model.QrCodes.PaymentScheme.Barcode,
                this._pairingDateTimeService,
                30,
                true,
                true,
                true,
                true,
                true
                );
            var createdWechatBarcodeGateway = _shopGatewayRepository.Add(wechatBarcodeGateway);
            //Wechat barcode vm.
            this._shopGatewayQueries.Add(
                _shopGatewayQueries.MapFromEntity(createdWechatBarcodeGateway));


            //Wechat merchant.
            var wechatMerchantGateway = ShopGateway.FromPlatForm(
                shopId,
                Pairing.Domain.Model.QrCodes.PaymentChannel.Wechat,
                Pairing.Domain.Model.QrCodes.PaymentScheme.Merchant,
                this._pairingDateTimeService,
                30,
                true,
                true,
                true,
                true,
                true
                );
            var createdWechatMerchantGateway = _shopGatewayRepository.Add(wechatMerchantGateway);
            //Wechat merchant vm.
            this._shopGatewayQueries.Add(
                _shopGatewayQueries.MapFromEntity(createdWechatMerchantGateway));


            //Alipay barcode.
            var alipayBarcodeGateway = ShopGateway.FromPlatForm(
                shopId,
                Pairing.Domain.Model.QrCodes.PaymentChannel.Alipay,
                Pairing.Domain.Model.QrCodes.PaymentScheme.Barcode,
                this._pairingDateTimeService,
                30,
                true,
                true,
                true,
                true,
                true
                );
            var createdAlipayBarcodeGateway = _shopGatewayRepository.Add(alipayBarcodeGateway);
            //Alipay barcode vm.
            this._shopGatewayQueries.Add(
                _shopGatewayQueries.MapFromEntity(createdAlipayBarcodeGateway));


            //Alipay merchant.
            var alipayMerchantGateway = ShopGateway.FromPlatForm(
                shopId,
                Pairing.Domain.Model.QrCodes.PaymentChannel.Alipay,
                Pairing.Domain.Model.QrCodes.PaymentScheme.Merchant,
                this._pairingDateTimeService,
                30,
                true,
                true,
                true,
                true,
                true
                );
            var createdAlipayMerchantGateway = _shopGatewayRepository.Add(alipayMerchantGateway);
            //Alipay merchant vm.
            this._shopGatewayQueries.Add(
                _shopGatewayQueries.MapFromEntity(createdAlipayMerchantGateway));


            //Alipay transaction.
            var alipayTransactionGateway = ShopGateway.FromPlatForm(
                shopId,
                Pairing.Domain.Model.QrCodes.PaymentChannel.Alipay,
                Pairing.Domain.Model.QrCodes.PaymentScheme.Transaction,
                this._pairingDateTimeService,
                30,
                true,
                true,
                true,
                true,
                true
                );
            var createdAlipayTransactionGateway = _shopGatewayRepository.Add(alipayTransactionGateway);
            //Alipay transaction vm.
            this._shopGatewayQueries.Add(
                _shopGatewayQueries.MapFromEntity(createdAlipayTransactionGateway));


            //Alipay bank.
            var alipayBankGateway = ShopGateway.FromPlatForm(
                shopId,
                Pairing.Domain.Model.QrCodes.PaymentChannel.Alipay,
                Pairing.Domain.Model.QrCodes.PaymentScheme.Bank,
                this._pairingDateTimeService,
                30,
                true,
                true,
                true,
                true,
                true
                );
            var createdAlipayBankGateway = _shopGatewayRepository.Add(alipayBankGateway);
            //Alipay bank vm.
            this._shopGatewayQueries.Add(
                _shopGatewayQueries.MapFromEntity(createdAlipayBankGateway));


            //Alipay envelop.
            var alipayEnvelopGateway = ShopGateway.FromPlatForm(
                shopId,
                Pairing.Domain.Model.QrCodes.PaymentChannel.Alipay,
                Pairing.Domain.Model.QrCodes.PaymentScheme.Envelop,
                this._pairingDateTimeService,
                30,
                true,
                true,
                true,
                true,
                true
                );
            var createdAlipayEnvelopGateway = _shopGatewayRepository.Add(alipayEnvelopGateway);
            //Alipay envelop vm.
            this._shopGatewayQueries.Add(
                _shopGatewayQueries.MapFromEntity(createdAlipayEnvelopGateway));


            //Alipay envelop password.
            var alipayPasswordGateway = ShopGateway.FromPlatForm(
                shopId,
                Pairing.Domain.Model.QrCodes.PaymentChannel.Alipay,
                Pairing.Domain.Model.QrCodes.PaymentScheme.EnvelopPassword,
                this._pairingDateTimeService,
                30,
                true,
                true,
                true,
                true,
                true
                );
            var createdAlipayPasswordGatewayGateway = _shopGatewayRepository.Add(alipayPasswordGateway);
            //Alipay envelop password vm.
            this._shopGatewayQueries.Add(
                _shopGatewayQueries.MapFromEntity(createdAlipayPasswordGatewayGateway));


            await _shopGatewayRepository.UnitOfWork.SaveEntitiesAsync();
            await _shopGatewayQueries.SaveChangesAsync();
        }

        private async Task DeleteWithoutSaveEntities(ApplicationUser user)
        {
            //Delete user, balance and commission. (Which belongs to distributing domain scope)
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

            //Delete shop settings. (Which belongs to pairing domain scope)
            var shopSettings = await _shopSettingsRepository.GetByShopIdAsync(user.Id);
            if (shopSettings != null)
            {
                _shopSettingsRepository.Delete(shopSettings);
            }

            //Delete shop gateways entities and vm. (Which belongs to pairing domain scope)
            var shopGateways = await _shopGatewayRepository.GetByShopIdAsync(user.Id);
            foreach (var shopGateway in shopGateways)
            {
                _shopGatewayRepository.Delete(shopGateway);
            }
            var shopGatewayEntries = await _shopGatewayQueries.GetShopGatewayEntrysByShopIdAsync(
                user.Id,
                null,
                null,
                null,
                null,
                null,
                null,
                null
                );
            foreach (var shopGatewayEntry in shopGatewayEntries)
            {
                _shopGatewayQueries.Delete(shopGatewayEntry);
            }


            //Delete shop api .(Which belongs to ordering domain scope)
            var shopApi = await _shopApiRepository.GetByShopIdAsync(user.Id);
            _shopApiRepository.Delete(shopApi);


            //Delete view model.
            var shopVM = await _shopQueries.GetShop(user.Id);
            if (shopVM != null)
            {
                _shopQueries.Delete(shopVM);
            }

            //Delete the reference from downlines.
            var downlines = _userManager.Users.Where(u => u.UplineId == user.Id);
            foreach (var downline in downlines)
            {
                downline.UplineId = null;
                await _userManager.UpdateAsync(downline);

                //Update view model.
                var downlineVM = await _shopQueries.GetShop(downline.Id);
                if (downlineVM != null)
                {
                    downlineVM.UplineUserId = null;
                    downlineVM.UplineUserName = null;
                    downlineVM.UplineFullName = null;
                    _shopQueries.Update(downlineVM);
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

        private void ValidateDownlineDataChangeSubmittedByShop(Shop downlineData, ShopAgent shopAgent)
        {
            //Only manager can change withdrawal and deposit rate.
            var withdrawalConf = _systemConfigurationService.GetWithdrawalAndDepositAsync();
            if (downlineData.Balance.WithdrawalCommissionRateInThousandth != withdrawalConf.WithdrawalTemplate.CommissionInThousandth
                || downlineData.Balance.WithdrawalLimit.DailyAmountLimit != withdrawalConf.WithdrawalTemplate.DailyAmountLimit
                || downlineData.Balance.WithdrawalLimit.DailyFrequencyLimit != withdrawalConf.WithdrawalTemplate.DailyFrequencyLimit
                || downlineData.Balance.WithdrawalLimit.EachAmountLowerLimit != withdrawalConf.WithdrawalTemplate.EachAmountLowerLimit
                || downlineData.Balance.WithdrawalLimit.EachAmountUpperLimit != withdrawalConf.WithdrawalTemplate.EachAmountUpperLimit)
            {
                throw new InvalidOperationException("Shop can not change withdrawal & deposit commission rates.");
            }

            //Checking the given id of upline is matched,
            if (string.IsNullOrWhiteSpace(downlineData.UplineUserId)
                || downlineData.UplineUserId != shopAgent.ShopAgentId)
            {
                throw new InvalidOperationException("Invalid upline Id.");
            }
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


        #endregion
    }
}
