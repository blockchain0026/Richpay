using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using WebMVC.Applications.CacheServices;
using WebMVC.Applications.DomainServices.DistributingDomain;
using WebMVC.Extensions;
using WebMVC.Infrastructure.Services;
using WebMVC.Models;
using WebMVC.Models.Permissions;
using WebMVC.Models.Queries;
using WebMVC.Models.Roles;
using WebMVC.ViewModels;

namespace WebMVC.Data
{
    public class ApplicationDbContextSeed
    {
        public static async Task SimpleSeedAsync(IApplicationBuilder applicationBuilder, IWebHostEnvironment env)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    /*var contentRootPath = env.ContentRootPath;
                    var webroot = env.WebRootPath;*/

                    context.Database.Migrate();

                    await CreateRoles(applicationBuilder.ApplicationServices);
                    /*if (!context.Currency.Any())
                    {
                        context.Currency.AddRange(GetPredefinedCurrency());
                        await context.SaveChangesAsync();
                    }*/

                    await context.SaveChangesAsync();
                }

            }

        }

        #region Roles & Permissions
        public static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();


                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var traderAgentService = scope.ServiceProvider.GetRequiredService<ITraderAgentService>();
                var traderService = scope.ServiceProvider.GetRequiredService<ITraderService>();
                var shopAgentService = scope.ServiceProvider.GetRequiredService<IShopAgentService>();
                var shopService = scope.ServiceProvider.GetRequiredService<IShopService>();
                var systemConfigurationService = scope.ServiceProvider.GetRequiredService<ISystemConfigurationService>();
                var depositService = scope.ServiceProvider.GetRequiredService<IDepositService>();
                var balanceService = scope.ServiceProvider.GetRequiredService<IBalanceService>();
                var qrCodeService = scope.ServiceProvider.GetRequiredService<IQrCodeService>();
                //var balanceRepository = scope.ServiceProvider.GetRequiredService<IBalanceRepository>();

                //adding customs roles
                string[] roleNames = {
                    Roles.Manager, Roles.Trader, Roles.TraderAgent, Roles.Shop, Roles.ShopAgent,
                    Roles.TraderAgentWithGrantRight,Roles.ShopAgentWithGrantRight,
                    Roles.UserReviewed,
                    Roles.Admin};

                IdentityResult roleResult;

                foreach (var roleName in roleNames)
                {
                    //creating the roles and seeding them to the database
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }


                //Creating role claims
                await AddDefaultAdminPermissions(roleManager);
                await AddDefaultManagerPermissions(roleManager);
                await AddDefaultTraderAgentPermissions(roleManager);
                await AddDefaultTraderPermissions(roleManager);
                await AddDefaultShopAgentPermissions(roleManager);
                await AddDefaultShopPermissions(roleManager);

                await AddDefaultTraderAgentWithGrantRightPermissions(roleManager);
                await AddDefaultShopAgentWithGrantRightPermissions(roleManager);
                await AddDefaultReviewedUserPermissions(roleManager);


                //Creating default users.
                //await CreateDefaultAdmins(configuration, userManager);
                await CreateDefaultAdmins(configuration, serviceProvider);
                //await CreateDefaultTraderAgents(configuration, userManager, systemConfigurationService, traderAgentService, balanceService);
                await CreateDefaultTraderAgents(configuration, serviceProvider);
                //await CreateDefaultTraders(configuration, userManager, systemConfigurationService, traderService, balanceService);
                await CreateDefaultTraders(configuration, serviceProvider);
                //await CreateDefaultShopAgents(configuration, userManager, systemConfigurationService, shopAgentService, balanceService);
                await CreateDefaultShopAgents(configuration, serviceProvider);
                //await CreateDefaultShops(configuration, userManager, systemConfigurationService, shopService, balanceService);
                await CreateDefaultShops(configuration, serviceProvider);

                //Create default Qr Codes
                //await CreateDefaultQrCodes(configuration, userManager, qrCodeService);
                //await CreateDefaultQrCodes(configuration, serviceProvider);
                await CreateDefaultQrCodesForOneTraderMode(configuration, serviceProvider);

            }
        }



        public static async Task CreateDefaultAdmins(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();


                Console.WriteLine("Creating Default Admins... Date: " + DateTime.UtcNow);
                //creating a super user who could maintain the web app
                var poweruser = new ApplicationUser
                {
                    UserName = configuration.GetSection("DefaultAccount")["DefaultUsername"] + "@gmail.com",
                    Email = configuration.GetSection("DefaultAccount")["DefaultEmail"],
                    FullName = Roles.Admin,
                    Nickname = Roles.Admin,
                    IsEnabled = true,
                    IsReviewed = true,
                    BaseRoleType = BaseRoleType.Manager,
                    DateCreated = DateTime.UtcNow
                };

                string userPassword = configuration.GetSection("DefaultAccount")["DefaultPassword"];
                var user = await userManager.FindByNameAsync(configuration.GetSection("DefaultAccount")["DefaultUsername"] + "@gmail.com");

                if (user == null)
                {
                    var createPowerUser = await userManager.CreateAsync(poweruser, userPassword);
                    if (createPowerUser.Succeeded)
                    {
                        //here we tie the new user to the "Admin" role 
                        await userManager.AddToRoleAsync(poweruser, "Admin");
                        //Add to base role.
                        await userManager.AddToRoleAsync(poweruser, Roles.Manager);
                    }

                    for (int i = 2; i <= 30; i++)
                    {
                        var username = configuration.GetSection("DefaultAccount")["DefaultUsername"] + i.ToString();

                        var dateCreated = DateTime.UtcNow.AddDays(i);
                        var testAdmin = new ApplicationUser
                        {
                            UserName = username + "@gmail.com",
                            Email = username + "@gmail.com",
                            PhoneNumber = "18987654321",
                            FullName = username,
                            Nickname = username,
                            IsEnabled = false,
                            IsReviewed = true,
                            BaseRoleType = BaseRoleType.Manager,
                            DateCreated = dateCreated
                        };
                        var result = await userManager.CreateAsync(testAdmin, userPassword);
                        if (result.Succeeded)
                        {
                            //here we tie the new user to the "Admin" role 
                            await userManager.AddToRoleAsync(testAdmin, Roles.Admin);

                            //Add to base role.
                            await userManager.AddToRoleAsync(testAdmin, Roles.Manager);
                        }
                    }
                }
            }
        }

        public static async Task CreateDefaultTraderAgents(
            IConfiguration configuration, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var traderAgentService = scope.ServiceProvider.GetRequiredService<ITraderAgentService>();
                var systemConfigurationService = scope.ServiceProvider.GetRequiredService<ISystemConfigurationService>();
                var balanceService = scope.ServiceProvider.GetRequiredService<IBalanceService>();
                //var balanceRepository = scope.ServiceProvider.GetRequiredService<IBalanceRepository>();

                Console.WriteLine("Creating Default Trader Agents... Date: " + DateTime.UtcNow);

                //Create default trader agents.
                var existingUser = await userManager.FindByNameAsync(configuration.GetSection("DefaultTraderAgent")["DefaultUsername"] + "1@gmail.com");

                if (existingUser == null)
                {
                    for (int i = 1; i <= 305; i++)
                    {
                        //Base info and conf.
                        var username = configuration.GetSection("DefaultTraderAgent")["DefaultUsername"] + (/*i == 1 ? "" :*/ i.ToString());
                        string password = configuration.GetSection("DefaultTraderAgent")["DefaultPassword"];

                        var dateCreated = DateTime.UtcNow.AddDays(i);

                        var balanceConf = systemConfigurationService.GetWithdrawalAndDepositAsync();


                        //Set Upline
                        string uplineId = null;
                        //->Three tiers.
                        /*if (i > 100)
                        {
                            var uplineUserName = configuration.GetSection("DefaultTraderAgent")["DefaultUsername"] + (i - 100).ToString() + "@gmail.com";
                            var uplineUser = await userManager.FindByNameAsync(uplineUserName);
                            uplineId = uplineUser.Id;
                        }*/

                        //->Custom first tier:5 second tier:100 third tier:100...
                        if (i > 5 && i <= 105)
                        {
                            var uplineUserName = configuration.GetSection("DefaultTraderAgent")["DefaultUsername"] + ((i % 5) + 1).ToString() + "@gmail.com";
                            var uplineUser = await userManager.FindByNameAsync(uplineUserName);
                            uplineId = uplineUser.Id;
                        }
                        if (i > 105)
                        {
                            var uplineUserName = configuration.GetSection("DefaultTraderAgent")["DefaultUsername"] + (i - 100).ToString() + "@gmail.com";
                            var uplineUser = await userManager.FindByNameAsync(uplineUserName);
                            uplineId = uplineUser.Id;

                        }

                        //Building balance.
                        var withdrawalLimit = new WithdrawalLimit
                        {
                            DailyAmountLimit = balanceConf.WithdrawalTemplate.DailyAmountLimit,
                            DailyFrequencyLimit = balanceConf.WithdrawalTemplate.DailyFrequencyLimit,
                            EachAmountUpperLimit = balanceConf.WithdrawalTemplate.EachAmountUpperLimit,
                            EachAmountLowerLimit = balanceConf.WithdrawalTemplate.EachAmountLowerLimit
                        };

                        var balance = new Balance
                        {
                            AmountAvailable = 0,
                            AmountFrozen = 0,
                            WithdrawalLimit = withdrawalLimit,
                            WithdrawalCommissionRateInThousandth = balanceConf.WithdrawalTemplate.CommissionInThousandth,
                            DepositCommissionRateInThousandth = 0
                        };






                        //Building commission.
                        var uplineCommission = await traderAgentService.GetTradingCommissionFromTraderAgentId(uplineId);
                        TradingCommission tradingCommission = null;
                        if (!string.IsNullOrEmpty(uplineId))
                        {
                            tradingCommission = new TradingCommission
                            {
                                RateAlipayInThousandth = uplineCommission.RateAlipayInThousandth - 1 > 0 ?
                               uplineCommission.RateAlipayInThousandth - 1 : 0,
                                RateWechatInThousandth = uplineCommission.RateWechatInThousandth - 1 > 0 ?
                               uplineCommission.RateAlipayInThousandth - 1 : 0,
                            };
                        }
                        else
                        {
                            tradingCommission = new TradingCommission
                            {
                                RateAlipayInThousandth = 18,
                                RateWechatInThousandth = 18
                            };
                        }

                        //Building trader agent.
                        var traderAgent = new TraderAgent
                        {
                            Username = username + "@gmail.com",
                            Email = username + "@gmail.com",
                            PhoneNumber = "18987654321",
                            FullName = username,
                            Nickname = username,
                            Balance = balance,
                            TradingCommission = tradingCommission,
                            IsEnabled = false,
                            IsReviewed = true,
                            HasGrantRight = false,
                            UplineUserId = uplineId,
                            DateCreated = dateCreated.ToFullString()
                        };

                        //Creating trader agent.
                        await traderAgentService.CreateTraderAgents(traderAgent, password);


                        //Create Deposit
                        var admin = await userManager.FindByNameAsync(configuration.GetSection("DefaultAccount")["DefaultUsername"] + "@gmail.com");
                        var traderAgentUser = await userManager.FindByNameAsync(traderAgent.Username);

                        Random randomBalanceGenerator = new Random();
                        var randomAmount = randomBalanceGenerator.Next(1, 10000);

                        await balanceService.ChangeBalance(
                            BalanceChangeType.Deposit,
                            traderAgentUser.Id,
                            randomAmount,
                            "Random generated admin deposit.",
                            admin.Id
                            );
                        /*await depositService.CreateDeposit(
                            traderAgentUser.Id,
                            randomAmount,
                            "Random generated admin deposit.",
                            admin.Id
                            );*/
                    }
                }
            }
        }

        public static async Task CreateDefaultTraders(
            IConfiguration configuration, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var traderService = scope.ServiceProvider.GetRequiredService<ITraderService>();
                var systemConfigurationService = scope.ServiceProvider.GetRequiredService<ISystemConfigurationService>();
                var balanceService = scope.ServiceProvider.GetRequiredService<IBalanceService>();

                Console.WriteLine("Creating Default Traders... Date: " + DateTime.UtcNow);
                //Create default traders.
                var existingUser = await userManager.FindByNameAsync(configuration.GetSection("DefaultTrader")["DefaultUsername"] + "1@gmail.com");

                if (existingUser == null)
                {
                    for (int i = 1; i <= 305; i++)
                    {
                        //Base info and conf.
                        var username = configuration.GetSection("DefaultTrader")["DefaultUsername"] + (/*i == 1 ? "" :*/ i.ToString());
                        string password = configuration.GetSection("DefaultTrader")["DefaultPassword"];

                        var dateCreated = DateTime.UtcNow.AddDays(i);

                        var balanceConf = systemConfigurationService.GetWithdrawalAndDepositAsync();


                        //Set Upline
                        string uplineId = null;


                        var uplineUserName = configuration.GetSection("DefaultTraderAgent")["DefaultUsername"] + i.ToString() + "@gmail.com";
                        var uplineUser = await userManager.FindByNameAsync(uplineUserName);
                        uplineId = uplineUser.Id;


                        //Building balance.
                        var withdrawalLimit = new WithdrawalLimit
                        {
                            DailyAmountLimit = balanceConf.WithdrawalTemplate.DailyAmountLimit,
                            DailyFrequencyLimit = balanceConf.WithdrawalTemplate.DailyFrequencyLimit,
                            EachAmountUpperLimit = balanceConf.WithdrawalTemplate.EachAmountUpperLimit,
                            EachAmountLowerLimit = balanceConf.WithdrawalTemplate.EachAmountLowerLimit
                        };

                        var balance = new Balance
                        {
                            AmountAvailable = 0,
                            AmountFrozen = 0,
                            WithdrawalLimit = withdrawalLimit,
                            WithdrawalCommissionRateInThousandth = balanceConf.WithdrawalTemplate.CommissionInThousandth,
                            DepositCommissionRateInThousandth = 0
                        };






                        //Building commission.
                        var uplineCommission = await traderService.GetTradingCommissionFromTraderAgentId(uplineId);
                        TradingCommission tradingCommission = null;
                        if (!string.IsNullOrEmpty(uplineId))
                        {
                            tradingCommission = new TradingCommission
                            {
                                RateAlipayInThousandth = uplineCommission.RateAlipayInThousandth - 1 > 0 ?
                               uplineCommission.RateAlipayInThousandth - 1 : 0,
                                RateWechatInThousandth = uplineCommission.RateWechatInThousandth - 1 > 0 ?
                               uplineCommission.RateAlipayInThousandth - 1 : 0,
                            };
                        }
                        else
                        {
                            tradingCommission = new TradingCommission
                            {
                                RateAlipayInThousandth = 18,
                                RateWechatInThousandth = 18
                            };
                        }

                        //Building trader.
                        var trader = new Trader
                        {
                            Username = username + "@gmail.com",
                            Email = username + "@gmail.com",
                            PhoneNumber = "18987654321",
                            FullName = username,
                            Nickname = username,
                            Balance = balance,
                            TradingCommission = tradingCommission,
                            IsEnabled = false,
                            IsReviewed = true,
                            UplineUserId = uplineId,
                            DateCreated = dateCreated.ToFullString()
                        };

                        //Creating trader.
                        await traderService.CreateTrader(trader, password);


                        //Create Deposit
                        var admin = await userManager.FindByNameAsync(configuration.GetSection("DefaultAccount")["DefaultUsername"] + "@gmail.com");
                        var traderUser = await userManager.FindByNameAsync(trader.Username);

                        Random randomBalanceGenerator = new Random();
                        var randomAmount = randomBalanceGenerator.Next(1, 10000);
                        await balanceService.ChangeBalance(
                            BalanceChangeType.Deposit,
                            traderUser.Id,
                            //randomAmount,
                            100000000, //For test.
                            "Random generated admin deposit.",
                            admin.Id
                            );
                        /*await depositService.CreateDeposit(
                            traderUser.Id,
                            //randomAmount,
                            1000000, //For test.
                            "Random generated admin deposit.",
                            admin.Id
                            );*/
                    }
                }
            }
        }

        public static async Task CreateDefaultShopAgents(
            IConfiguration configuration, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var shopAgentService = scope.ServiceProvider.GetRequiredService<IShopAgentService>();
                var systemConfigurationService = scope.ServiceProvider.GetRequiredService<ISystemConfigurationService>();
                var balanceService = scope.ServiceProvider.GetRequiredService<IBalanceService>();

                Console.WriteLine("Creating Default Shop Agents... Date: " + DateTime.UtcNow);
                //Create default trader agents.
                var existingUser = await userManager.FindByNameAsync(configuration.GetSection("DefaultShopAgent")["DefaultUsername"] + "1@gmail.com");

                if (existingUser == null)
                {
                    for (int i = 1; i <= 305; i++)
                    {
                        //Base info and conf.
                        var username = configuration.GetSection("DefaultShopAgent")["DefaultUsername"] + (/*i == 1 ? "" :*/ i.ToString());
                        string password = configuration.GetSection("DefaultShopAgent")["DefaultPassword"];

                        var dateCreated = DateTime.UtcNow.AddDays(i);

                        var balanceConf = systemConfigurationService.GetWithdrawalAndDepositAsync();


                        //Set Upline
                        string uplineId = null;
                        //->Three tiers.
                        /*if (i > 100)
                        {
                            var uplineUserName = configuration.GetSection("DefaultTraderAgent")["DefaultUsername"] + (i - 100).ToString() + "@gmail.com";
                            var uplineUser = await userManager.FindByNameAsync(uplineUserName);
                            uplineId = uplineUser.Id;
                        }*/

                        //->Custom first tier:5 second tier:100 third tier:100...
                        if (i > 5 && i <= 105)
                        {
                            var uplineUserName = configuration.GetSection("DefaultShopAgent")["DefaultUsername"] + ((i % 5) + 1).ToString() + "@gmail.com";
                            var uplineUser = await userManager.FindByNameAsync(uplineUserName);
                            uplineId = uplineUser.Id;
                        }
                        if (i > 105)
                        {
                            var uplineUserName = configuration.GetSection("DefaultShopAgent")["DefaultUsername"] + (i - 100).ToString() + "@gmail.com";
                            var uplineUser = await userManager.FindByNameAsync(uplineUserName);
                            uplineId = uplineUser.Id;

                        }

                        //Building balance.
                        var withdrawalLimit = new WithdrawalLimit
                        {
                            DailyAmountLimit = balanceConf.WithdrawalTemplate.DailyAmountLimit,
                            DailyFrequencyLimit = balanceConf.WithdrawalTemplate.DailyFrequencyLimit,
                            EachAmountUpperLimit = balanceConf.WithdrawalTemplate.EachAmountUpperLimit,
                            EachAmountLowerLimit = balanceConf.WithdrawalTemplate.EachAmountLowerLimit
                        };

                        var balance = new ShopUserBalance
                        {
                            AmountAvailable = 0,
                            AmountFrozen = 0,
                            WithdrawalLimit = withdrawalLimit,
                            WithdrawalCommissionRateInThousandth = balanceConf.WithdrawalTemplate.CommissionInThousandth,
                        };






                        //Building commission.
                        var uplineCommission = await shopAgentService.GetRebateCommissionFromShopAgentId(uplineId);
                        RebateCommission rebateCommission = null;
                        if (!string.IsNullOrEmpty(uplineId))
                        {
                            rebateCommission = new RebateCommission
                            {
                                RateRebateAlipayInThousandth = uplineCommission.RateRebateAlipayInThousandth + 2,
                                RateRebateWechatInThousandth = uplineCommission.RateRebateWechatInThousandth + 2
                            };
                        }
                        else
                        {
                            rebateCommission = new RebateCommission
                            {
                                RateRebateAlipayInThousandth = 22,
                                RateRebateWechatInThousandth = 22
                            };
                        }

                        //Building shop agent.

                        var shopAgent = new ShopAgent
                        {
                            Username = username + "@gmail.com",
                            Email = username + "@gmail.com",
                            PhoneNumber = "18987654321",
                            FullName = username,
                            Nickname = username,
                            Balance = balance,
                            RebateCommission = rebateCommission,
                            IsEnabled = false,
                            IsReviewed = true,
                            HasGrantRight = false,
                            UplineUserId = uplineId,
                            DateCreated = dateCreated.ToFullString()
                        };

                        //Creating shop agent.
                        await shopAgentService.CreateShopAgents(shopAgent, password);


                        //Create Deposit
                        var admin = await userManager.FindByNameAsync(configuration.GetSection("DefaultAccount")["DefaultUsername"] + "@gmail.com");
                        var shopAgentUser = await userManager.FindByNameAsync(shopAgent.Username);

                        Random randomBalanceGenerator = new Random();
                        var randomAmount = randomBalanceGenerator.Next(1, 10000);

                        await balanceService.ChangeBalance(
                            BalanceChangeType.Deposit,
                            shopAgentUser.Id,
                            randomAmount,
                            "Random generated admin deposit.",
                            admin.Id
                            );
                        /*await depositService.CreateDeposit(
                            shopAgentUser.Id,
                            randomAmount,
                            "Random generated admin deposit.",
                            admin.Id
                            );*/
                    }
                }
            }
        }

        public static async Task CreateDefaultShops(
            IConfiguration configuration, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var shopService = scope.ServiceProvider.GetRequiredService<IShopService>();
                var systemConfigurationService = scope.ServiceProvider.GetRequiredService<ISystemConfigurationService>();
                var balanceService = scope.ServiceProvider.GetRequiredService<IBalanceService>();

                Console.WriteLine("Creating Default Shops... Date: " + DateTime.UtcNow);
                //Create default shops.
                var existingUser = await userManager.FindByNameAsync(configuration.GetSection("DefaultShop")["DefaultUsername"] + "1@gmail.com");

                if (existingUser == null)
                {
                    for (int i = 1; i <= 305; i++)
                    {
                        //Base info and conf.
                        var username = configuration.GetSection("DefaultShop")["DefaultUsername"] + (/*i == 1 ? "" :*/ i.ToString());
                        string password = configuration.GetSection("DefaultShop")["DefaultPassword"];

                        var dateCreated = DateTime.UtcNow.AddDays(i);

                        var balanceConf = systemConfigurationService.GetWithdrawalAndDepositAsync();


                        //Set Upline
                        string uplineId = null;


                        var uplineUserName = configuration.GetSection("DefaultShopAgent")["DefaultUsername"] + i.ToString() + "@gmail.com";
                        var uplineUser = await userManager.FindByNameAsync(uplineUserName);
                        uplineId = uplineUser.Id;


                        //Building balance.
                        var withdrawalLimit = new WithdrawalLimit
                        {
                            DailyAmountLimit = balanceConf.WithdrawalTemplate.DailyAmountLimit,
                            DailyFrequencyLimit = balanceConf.WithdrawalTemplate.DailyFrequencyLimit,
                            EachAmountUpperLimit = balanceConf.WithdrawalTemplate.EachAmountUpperLimit,
                            EachAmountLowerLimit = balanceConf.WithdrawalTemplate.EachAmountLowerLimit
                        };

                        var balance = new ShopUserBalance
                        {
                            AmountAvailable = 0,
                            AmountFrozen = 0,
                            WithdrawalLimit = withdrawalLimit,
                            WithdrawalCommissionRateInThousandth = balanceConf.WithdrawalTemplate.CommissionInThousandth
                        };


                        //Building commission.
                        var uplineCommission = await shopService.GetRebateCommissionFromShopAgentId(uplineId);
                        RebateCommission rebateCommission = null;
                        if (!string.IsNullOrEmpty(uplineId))
                        {
                            rebateCommission = new RebateCommission
                            {
                                RateRebateAlipayInThousandth = uplineCommission.RateRebateAlipayInThousandth + 3,
                                RateRebateWechatInThousandth = uplineCommission.RateRebateWechatInThousandth + 3
                            };
                        }
                        else
                        {
                            rebateCommission = new RebateCommission
                            {
                                RateRebateAlipayInThousandth = 22,
                                RateRebateWechatInThousandth = 22
                            };
                        }

                        //Building shop.
                        var shop = new Shop
                        {
                            Username = username + "@gmail.com",
                            Email = username + "@gmail.com",
                            PhoneNumber = "18987654321",
                            FullName = username,
                            SiteAddress = "www.testshop.com",
                            Balance = balance,
                            RebateCommission = rebateCommission,
                            IsEnabled = false,
                            IsReviewed = true,
                            UplineUserId = uplineId,
                            DateCreated = dateCreated.ToFullString()
                        };

                        //Creating shop.
                        await shopService.CreateShop(shop, password);


                        //Create Deposit
                        var admin = await userManager.FindByNameAsync(configuration.GetSection("DefaultAccount")["DefaultUsername"] + "@gmail.com");
                        var shopUser = await userManager.FindByNameAsync(shop.Username);

                        Random randomBalanceGenerator = new Random();
                        var randomAmount = randomBalanceGenerator.Next(1, 10000);
                        await balanceService.ChangeBalance(
                            BalanceChangeType.Deposit,
                            shopUser.Id,
                            randomAmount,
                            "Random generated admin deposit.",
                            admin.Id
                            );
                        /*await depositService.CreateDeposit(
                            shopUser.Id,
                            randomAmount,
                            "Random generated admin deposit.",
                            admin.Id
                            );*/
                    }
                }
            }
        }

        public static async Task CreateDefaultQrCodes(
            IConfiguration configuration, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var qrCodeService = scope.ServiceProvider.GetRequiredService<IQrCodeService>();

                Console.WriteLine("Creating Default Qr Codes... Date: " + DateTime.UtcNow);
                //Get default admin.
                var admin = await userManager.FindByNameAsync(configuration.GetSection("DefaultAccount")["DefaultUsername"] + "@gmail.com");

                //Create default qr codes.
                var existingPendingQrCodeCount = await qrCodeService.GetPendingQrCodeEntrysTotalCount(admin.Id);
                var existingQrCodeCount = await qrCodeService.GetQrCodeEntrysTotalCount(admin.Id);

                /*for (int i = 1; i <= 305; i++)
                {
                    //Get trader.
                    var traderUsername = configuration.GetSection("DefaultTrader")["DefaultUsername"] + (i.ToString());
                    var trader = await userManager.FindByNameAsync(traderUsername + "@gmail.com");

                    await qrCodeService.CreateTransactionCode(
                        admin.Id,
                        trader.Id,
                        null,
                        QrCodeTypeOption.Manual.Value,
                        "Transaction",
                        new QrCodeEntrySetting
                        {
                            AutoPairingBySuccessRate = true,
                            AutoPairingByQuotaLeft = true,
                            AutoPairingByBusinessHours = false,
                            AutoPairingByCurrentConsecutiveFailures = true,
                            AutoPairngByAvailableBalance = true,
                            SuccessRateThresholdInHundredth = 50,
                            SuccessRateMinOrders = 5,
                            QuotaLeftThreshold = 200,
                            CurrentConsecutiveFailuresThreshold = 5,
                            AvailableBalanceThreshold = 0
                        },
                        10000000,
                        10000000,
                        100,
                        "二维码" + i.ToString(),
                        null,
                        "2088732318934891"
                        );
                }
                */
                if (existingQrCodeCount <= 0 && existingPendingQrCodeCount <= 0)
                {
                    for (int i = 1; i <= 1525; i++)
                    {
                        //Get trader.
                        var traderUsername = configuration.GetSection("DefaultTrader")["DefaultUsername"] + (/*i == 1 ? "" :*/ ((i % 305) + 1).ToString());
                        var trader = await userManager.FindByNameAsync(traderUsername + "@gmail.com");

                        //Get shop
                        var shopUsername = configuration.GetSection("DefaultShop")["DefaultUsername"] + (/*i == 1 ? "" :*/ ((i % 305) + 1).ToString());
                        var shop = await userManager.FindByNameAsync(shopUsername + "@gmail.com");


                        await qrCodeService.CreateTransactionCode(
                            admin.Id,
                            trader.Id,
                            null,
                            QrCodeTypeOption.Manual.Value,
                            "Transaction",
                            new QrCodeEntrySetting
                            {
                                AutoPairingBySuccessRate = true,
                                AutoPairingByQuotaLeft = true,
                                AutoPairingByBusinessHours = false,
                                AutoPairingByCurrentConsecutiveFailures = true,
                                AutoPairngByAvailableBalance = true,
                                SuccessRateThresholdInHundredth = 50,
                                SuccessRateMinOrders = 5,
                                QuotaLeftThreshold = 200,
                                CurrentConsecutiveFailuresThreshold = 5,
                                AvailableBalanceThreshold = 0
                            },
                            10000000,
                            10000000,
                            100,
                            "二维码" + i.ToString(),
                            null,
                            "2088732318934891"
                            );
                    }


                    for (int i = 1; i <= 0; i++)
                    {
                        //Get trader.
                        var traderUsername = configuration.GetSection("DefaultTrader")["DefaultUsername"] + (/*i == 1 ? "" :*/ i.ToString());
                        var trader = await userManager.FindByNameAsync(traderUsername + "@gmail.com");

                        //Get shop
                        var shopUsername = configuration.GetSection("DefaultShop")["DefaultUsername"] + (/*i == 1 ? "" :*/ i.ToString());
                        var shop = await userManager.FindByNameAsync(shopUsername + "@gmail.com");


                        await qrCodeService.CreateTransactionCode(
                            admin.Id,
                            trader.Id,
                            null,
                            QrCodeTypeOption.Manual.Value,
                            "Envelop",
                            new QrCodeEntrySetting
                            {
                                AutoPairingBySuccessRate = true,
                                AutoPairingByQuotaLeft = true,
                                AutoPairingByBusinessHours = false,
                                AutoPairingByCurrentConsecutiveFailures = true,
                                AutoPairngByAvailableBalance = true,
                                SuccessRateThresholdInHundredth = 50,
                                SuccessRateMinOrders = 5,
                                QuotaLeftThreshold = 200,
                                CurrentConsecutiveFailuresThreshold = 5,
                                AvailableBalanceThreshold = 0
                            },
                            5000,
                            5000,
                            300,
                            "二维码" + i.ToString(),
                            shop.Id,
                            "2088732318934891"
                            );
                    }
                }
            }

        }
        
        public static async Task CreateDefaultQrCodesForOneTraderMode(
            IConfiguration configuration, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var qrCodeService = scope.ServiceProvider.GetRequiredService<IQrCodeService>();

                Console.WriteLine("Creating Default Qr Codes... Date: " + DateTime.UtcNow);
                //Get default admin.
                var admin = await userManager.FindByNameAsync(configuration.GetSection("DefaultAccount")["DefaultUsername"] + "@gmail.com");

                //Create default qr codes.
                var existingPendingQrCodeCount = await qrCodeService.GetPendingQrCodeEntrysTotalCount(admin.Id);
                var existingQrCodeCount = await qrCodeService.GetQrCodeEntrysTotalCount(admin.Id);

                /*for (int i = 1; i <= 305; i++)
                {
                    //Get trader.
                    var traderUsername = configuration.GetSection("DefaultTrader")["DefaultUsername"] + (i.ToString());
                    var trader = await userManager.FindByNameAsync(traderUsername + "@gmail.com");

                    await qrCodeService.CreateTransactionCode(
                        admin.Id,
                        trader.Id,
                        null,
                        QrCodeTypeOption.Manual.Value,
                        "Transaction",
                        new QrCodeEntrySetting
                        {
                            AutoPairingBySuccessRate = true,
                            AutoPairingByQuotaLeft = true,
                            AutoPairingByBusinessHours = false,
                            AutoPairingByCurrentConsecutiveFailures = true,
                            AutoPairngByAvailableBalance = true,
                            SuccessRateThresholdInHundredth = 50,
                            SuccessRateMinOrders = 5,
                            QuotaLeftThreshold = 200,
                            CurrentConsecutiveFailuresThreshold = 5,
                            AvailableBalanceThreshold = 0
                        },
                        10000000,
                        10000000,
                        100,
                        "二维码" + i.ToString(),
                        null,
                        "2088732318934891"
                        );
                }
                */
                if (existingQrCodeCount <= 0 && existingPendingQrCodeCount <= 0)
                {
                    for (int i = 1; i <= 1000; i++)
                    {
                        //Get trader.
                        var traderUsername = configuration.GetSection("DefaultTrader")["DefaultUsername"] + ((i % 5) + 1).ToString();
                        var trader = await userManager.FindByNameAsync(traderUsername + "@gmail.com");

                        await qrCodeService.CreateTransactionCode(
                            admin.Id,
                            trader.Id,
                            null,
                            QrCodeTypeOption.Manual.Value,
                            "Transaction",
                            new QrCodeEntrySetting
                            {
                                AutoPairingBySuccessRate = true,
                                AutoPairingByQuotaLeft = true,
                                AutoPairingByBusinessHours = false,
                                AutoPairingByCurrentConsecutiveFailures = true,
                                AutoPairngByAvailableBalance = true,
                                SuccessRateThresholdInHundredth = 50,
                                SuccessRateMinOrders = 5,
                                QuotaLeftThreshold = 200,
                                CurrentConsecutiveFailuresThreshold = 5,
                                AvailableBalanceThreshold = 0
                            },
                            10000000,
                            10000000,
                            100,
                            "二维码" + i.ToString(),
                            null,
                            "2088732318934891"
                            );
                    }
                }
            }

        }


        #region Permission

        public static async Task AddDefaultAdminPermissions(RoleManager<IdentityRole> roleManager)
        {
            var role = await roleManager.FindByNameAsync(Roles.Admin);
            var roleClaims = await roleManager.GetClaimsAsync(role);
            var permissions = GetDefaultAdminPermissions();

            foreach (var permissionValue in permissions)
            {
                if (!roleClaims.Any(c => c.Value == permissionValue))
                    await roleManager.AddClaimAsync(
                        role,
                        new Claim(CustomClaimTypes.Permission, permissionValue));
            }
        }

        public static async Task AddDefaultManagerPermissions(RoleManager<IdentityRole> roleManager)
        {
            var role = await roleManager.FindByNameAsync(Roles.Manager);
            var roleClaims = await roleManager.GetClaimsAsync(role);
            var permissions = GetDefaultManagerPermissions();

            foreach (var permissionValue in permissions)
            {
                if (!roleClaims.Any(c => c.Value == permissionValue))
                    await roleManager.AddClaimAsync(
                        role,
                        new Claim(CustomClaimTypes.Permission, permissionValue));
            }
        }

        public static async Task AddDefaultTraderPermissions(RoleManager<IdentityRole> roleManager)
        {
            var role = await roleManager.FindByNameAsync(Roles.Trader);
            var roleClaims = await roleManager.GetClaimsAsync(role);
            var permissions = GetDefaultTraderPermissions();

            foreach (var permissionValue in permissions)
            {
                if (!roleClaims.Any(c => c.Value == permissionValue))
                    await roleManager.AddClaimAsync(
                        role,
                        new Claim(CustomClaimTypes.Permission, permissionValue));
            }
        }

        public static async Task AddDefaultTraderAgentPermissions(RoleManager<IdentityRole> roleManager)
        {
            var role = await roleManager.FindByNameAsync(Roles.TraderAgent);
            var roleClaims = await roleManager.GetClaimsAsync(role);
            var permissions = GetDefaultTraderAgentPermissions();

            foreach (var permissionValue in permissions)
            {
                if (!roleClaims.Any(c => c.Value == permissionValue))
                    await roleManager.AddClaimAsync(
                        role,
                        new Claim(CustomClaimTypes.Permission, permissionValue));
            }
        }

        public static async Task AddDefaultShopAgentPermissions(RoleManager<IdentityRole> roleManager)
        {
            var role = await roleManager.FindByNameAsync(Roles.ShopAgent);
            var roleClaims = await roleManager.GetClaimsAsync(role);
            var permissions = GetDefaultShopAgentPermissions();

            foreach (var permissionValue in permissions)
            {
                if (!roleClaims.Any(c => c.Value == permissionValue))
                    await roleManager.AddClaimAsync(
                        role,
                        new Claim(CustomClaimTypes.Permission, permissionValue));
            }
        }

        public static async Task AddDefaultShopPermissions(RoleManager<IdentityRole> roleManager)
        {
            var role = await roleManager.FindByNameAsync(Roles.Shop);
            var roleClaims = await roleManager.GetClaimsAsync(role);
            var permissions = GetDefaultShopPermissions();

            foreach (var permissionValue in permissions)
            {
                if (!roleClaims.Any(c => c.Value == permissionValue))
                    await roleManager.AddClaimAsync(
                        role,
                        new Claim(CustomClaimTypes.Permission, permissionValue));
            }
        }



        public static async Task AddDefaultTraderAgentWithGrantRightPermissions(RoleManager<IdentityRole> roleManager)

        {
            var role = await roleManager.FindByNameAsync(Roles.TraderAgentWithGrantRight);
            var roleClaims = await roleManager.GetClaimsAsync(role);
            var permissions = GetDefaultTraderAgentWithGrantRightPermissions();

            foreach (var permissionValue in permissions)
            {
                if (!roleClaims.Any(c => c.Value == permissionValue))
                    await roleManager.AddClaimAsync(
                        role,
                        new Claim(CustomClaimTypes.Permission, permissionValue));
            }
        }

        public static async Task AddDefaultShopAgentWithGrantRightPermissions(RoleManager<IdentityRole> roleManager)

        {
            var role = await roleManager.FindByNameAsync(Roles.ShopAgentWithGrantRight);
            var roleClaims = await roleManager.GetClaimsAsync(role);
            var permissions = GetDefaultShopAgentWithGrantRightPermissions();

            foreach (var permissionValue in permissions)
            {
                if (!roleClaims.Any(c => c.Value == permissionValue))
                    await roleManager.AddClaimAsync(
                        role,
                        new Claim(CustomClaimTypes.Permission, permissionValue));
            }
        }

        public static async Task AddDefaultReviewedUserPermissions(RoleManager<IdentityRole> roleManager)
        {
            var role = await roleManager.FindByNameAsync(Roles.UserReviewed);
            var roleClaims = await roleManager.GetClaimsAsync(role);
            var permissions = GetDefaultReviewedUserPermissions();

            foreach (var permissionValue in permissions)
            {
                if (!roleClaims.Any(c => c.Value == permissionValue))
                    await roleManager.AddClaimAsync(
                        role,
                        new Claim(CustomClaimTypes.Permission, permissionValue));
            }
        }





        public static List<string> GetDefaultAdminPermissions()
        {
            var permissions = new List<string>();

            //Hubs
            permissions.Add(Permissions.Hubs.DepositVoice);
            permissions.Add(Permissions.Hubs.OrderStatistic);
            permissions.Add(Permissions.Hubs.WithdrawalVoice);

            //Additional
            permissions.Add(Permissions.Additional.Reviewed);

            //Personal
            permissions.Add(Permissions.Personal.View);
            permissions.Add(Permissions.Personal.Edit);
            permissions.Add(Permissions.Personal.TwoFactorAuth.View);
            permissions.Add(Permissions.Personal.TwoFactorAuth.Enable);
            permissions.Add(Permissions.Personal.Manager.View);
            permissions.Add(Permissions.Personal.Manager.Edit);

            //Dashboard
            permissions.Add(Permissions.Dashboards.View);

            //Administrations
            permissions.Add(Permissions.Administration.Managers.View);
            permissions.Add(Permissions.Administration.Managers.Create);
            permissions.Add(Permissions.Administration.Managers.Edit);
            permissions.Add(Permissions.Administration.Managers.Delete);

            permissions.Add(Permissions.Administration.Roles.View);
            permissions.Add(Permissions.Administration.Roles.Create);
            permissions.Add(Permissions.Administration.Roles.Edit);
            permissions.Add(Permissions.Administration.Roles.Delete);

            /*permissions.Add(Permissions.Administration.Permissions.View);
            permissions.Add(Permissions.Administration.Permissions.Create);
            permissions.Add(Permissions.Administration.Permissions.Edit);
            permissions.Add(Permissions.Administration.Permissions.Delete);*/

            //System Configuration
            permissions.Add(Permissions.SystemConfiguration.SiteBaseInfo.View);
            permissions.Add(Permissions.SystemConfiguration.SiteBaseInfo.Edit);

            permissions.Add(Permissions.SystemConfiguration.Payment.View);
            permissions.Add(Permissions.SystemConfiguration.Payment.Edit);

            permissions.Add(Permissions.SystemConfiguration.SystemNotificationSound.View);
            permissions.Add(Permissions.SystemConfiguration.SystemNotificationSound.Edit);

            permissions.Add(Permissions.SystemConfiguration.UserNotification.View);
            permissions.Add(Permissions.SystemConfiguration.UserNotification.Edit);

            permissions.Add(Permissions.SystemConfiguration.WithdrawalAndDeposit.View);
            permissions.Add(Permissions.SystemConfiguration.WithdrawalAndDeposit.Edit);

            permissions.Add(Permissions.SystemConfiguration.PaymentChannel.View);
            permissions.Add(Permissions.SystemConfiguration.PaymentChannel.Edit);

            permissions.Add(Permissions.SystemConfiguration.QrCodeConf.View);
            permissions.Add(Permissions.SystemConfiguration.QrCodeConf.Edit);

            //Organization
            permissions.Add(Permissions.Organization.TraderAgents.View);
            permissions.Add(Permissions.Organization.TraderAgents.Create);
            permissions.Add(Permissions.Organization.TraderAgents.Edit);
            permissions.Add(Permissions.Organization.TraderAgents.Delete);

            permissions.Add(Permissions.Organization.TraderAgents.Downlines.View);
            permissions.Add(Permissions.Organization.TraderAgents.Downlines.Create);
            permissions.Add(Permissions.Organization.TraderAgents.Downlines.Edit);
            permissions.Add(Permissions.Organization.TraderAgents.Downlines.Delete);

            permissions.Add(Permissions.Organization.TraderAgents.PendingReview.View);
            permissions.Add(Permissions.Organization.TraderAgents.PendingReview.Review);
            permissions.Add(Permissions.Organization.TraderAgents.BankBook.View);
            permissions.Add(Permissions.Organization.TraderAgents.FrozenRecord.View);
            permissions.Add(Permissions.Organization.TraderAgents.Transfer.Create);
            permissions.Add(Permissions.Organization.TraderAgents.ChangeBalance.Create);

            permissions.Add(Permissions.Organization.Traders.View);
            permissions.Add(Permissions.Organization.Traders.Create);
            permissions.Add(Permissions.Organization.Traders.Edit);
            permissions.Add(Permissions.Organization.Traders.Delete);
            permissions.Add(Permissions.Organization.Traders.PendingReview.View);
            permissions.Add(Permissions.Organization.Traders.PendingReview.Review);
            permissions.Add(Permissions.Organization.Traders.BankBook.View);
            permissions.Add(Permissions.Organization.Traders.FrozenRecord.View);
            permissions.Add(Permissions.Organization.Traders.Transfer.Create);
            permissions.Add(Permissions.Organization.Traders.ChangeBalance.Create);

            //Shop Management
            permissions.Add(Permissions.ShopManagement.ShopAgents.View);
            permissions.Add(Permissions.ShopManagement.ShopAgents.Create);
            permissions.Add(Permissions.ShopManagement.ShopAgents.Edit);
            permissions.Add(Permissions.ShopManagement.ShopAgents.Delete);

            permissions.Add(Permissions.ShopManagement.ShopAgents.Downlines.View);
            permissions.Add(Permissions.ShopManagement.ShopAgents.Downlines.Create);
            permissions.Add(Permissions.ShopManagement.ShopAgents.Downlines.Edit);
            permissions.Add(Permissions.ShopManagement.ShopAgents.Downlines.Delete);

            permissions.Add(Permissions.ShopManagement.ShopAgents.PendingReview.View);
            permissions.Add(Permissions.ShopManagement.ShopAgents.PendingReview.Review);
            permissions.Add(Permissions.ShopManagement.ShopAgents.BankBook.View);
            permissions.Add(Permissions.ShopManagement.ShopAgents.FrozenRecord.View);
            permissions.Add(Permissions.ShopManagement.ShopAgents.ChangeBalance.Create);

            permissions.Add(Permissions.ShopManagement.Shops.View);
            permissions.Add(Permissions.ShopManagement.Shops.Create);
            permissions.Add(Permissions.ShopManagement.Shops.Edit);
            permissions.Add(Permissions.ShopManagement.Shops.Delete);
            permissions.Add(Permissions.ShopManagement.Shops.PendingReview.View);
            permissions.Add(Permissions.ShopManagement.Shops.PendingReview.Review);
            permissions.Add(Permissions.ShopManagement.Shops.BankBook.View);
            permissions.Add(Permissions.ShopManagement.Shops.FrozenRecord.View);
            permissions.Add(Permissions.ShopManagement.Shops.ChangeBalance.Create);

            permissions.Add(Permissions.ShopManagement.Shops.ShopGateway.View);
            permissions.Add(Permissions.ShopManagement.Shops.ShopGateway.Create);
            permissions.Add(Permissions.ShopManagement.Shops.ShopGateway.Edit);
            permissions.Add(Permissions.ShopManagement.Shops.ShopGateway.Delete);

            permissions.Add(Permissions.ShopManagement.Shops.AmountOption.View);
            permissions.Add(Permissions.ShopManagement.Shops.AmountOption.Create);
            permissions.Add(Permissions.ShopManagement.Shops.AmountOption.Delete);

            permissions.Add(Permissions.ShopManagement.Shops.ApiKey.Create);

            /*permissions.Add(Permissions.ShopManagement.Shops.View);
            permissions.Add(Permissions.ShopManagement.Shops.Create);
            permissions.Add(Permissions.ShopManagement.Shops.Edit);
            permissions.Add(Permissions.ShopManagement.Shops.Delete);
            permissions.Add(Permissions.ShopManagement.Shops.PendingReview.View);
            permissions.Add(Permissions.ShopManagement.Shops.PendingReview.Review);
            permissions.Add(Permissions.ShopManagement.Shops.BankBook.View);
            permissions.Add(Permissions.ShopManagement.Shops.FrozenRecord.View);
            permissions.Add(Permissions.ShopManagement.Shops.ChangeBalance.Create);*/


            //Withdrawal Management
            permissions.Add(Permissions.WithdrawalManagement.Withdrawals.View);
            permissions.Add(Permissions.WithdrawalManagement.Withdrawals.Create);
            permissions.Add(Permissions.WithdrawalManagement.Withdrawals.SearchUser);

            permissions.Add(Permissions.WithdrawalManagement.PendingReview.View);
            permissions.Add(Permissions.WithdrawalManagement.PendingReview.Approve);
            permissions.Add(Permissions.WithdrawalManagement.PendingReview.ForceSuccess);
            permissions.Add(Permissions.WithdrawalManagement.PendingReview.ApproveCancellation);

            permissions.Add(Permissions.WithdrawalManagement.BankOptions.View);
            permissions.Add(Permissions.WithdrawalManagement.BankOptions.Create);
            permissions.Add(Permissions.WithdrawalManagement.BankOptions.Delete);


            //Deposit Management
            permissions.Add(Permissions.DepositManagement.Deposits.View);
            permissions.Add(Permissions.DepositManagement.Deposits.Create);
            permissions.Add(Permissions.DepositManagement.Deposits.SearchUser);

            permissions.Add(Permissions.DepositManagement.PendingReview.View);
            permissions.Add(Permissions.DepositManagement.PendingReview.Verify);

            permissions.Add(Permissions.DepositManagement.DepositBankAccounts.View);
            permissions.Add(Permissions.DepositManagement.DepositBankAccounts.Create);
            permissions.Add(Permissions.DepositManagement.DepositBankAccounts.Delete);

            //QrCode Management
            permissions.Add(Permissions.QrCodeManagement.Manual.View);
            permissions.Add(Permissions.QrCodeManagement.Manual.Create);
            permissions.Add(Permissions.QrCodeManagement.Manual.Edit);
            permissions.Add(Permissions.QrCodeManagement.Manual.SearchTrader);
            permissions.Add(Permissions.QrCodeManagement.Manual.SearchShop);
            permissions.Add(Permissions.QrCodeManagement.Manual.Enable);
            permissions.Add(Permissions.QrCodeManagement.Manual.Disable);
            permissions.Add(Permissions.QrCodeManagement.Manual.StartPairing);
            permissions.Add(Permissions.QrCodeManagement.Manual.StopPairing);
            permissions.Add(Permissions.QrCodeManagement.Manual.ResetRiskControlData);

            permissions.Add(Permissions.QrCodeManagement.Manual.PendingReview.View);
            permissions.Add(Permissions.QrCodeManagement.Manual.PendingReview.Approve);
            permissions.Add(Permissions.QrCodeManagement.Manual.PendingReview.Reject);

            permissions.Add(Permissions.QrCodeManagement.Manual.CodeData.View);
            permissions.Add(Permissions.QrCodeManagement.Manual.CodeData.Edit);


            //Order Management
            permissions.Add(Permissions.OrderManagement.PlatformOrders.View);
            permissions.Add(Permissions.OrderManagement.PlatformOrders.Create);
            permissions.Add(Permissions.OrderManagement.PlatformOrders.ConfirmPayment);

            permissions.Add(Permissions.OrderManagement.RunningAccountRecords.View);


            return permissions;
        }

        public static List<string> GetDefaultManagerPermissions()
        {
            var permissions = new List<string>();

            //Personal
            permissions.Add(Permissions.Personal.View);
            permissions.Add(Permissions.Personal.Edit);
            permissions.Add(Permissions.Personal.TwoFactorAuth.View);
            permissions.Add(Permissions.Personal.TwoFactorAuth.Enable);
            permissions.Add(Permissions.Personal.Manager.View);
            permissions.Add(Permissions.Personal.Manager.Edit);

            return permissions;
        }

        public static List<string> GetDefaultTraderAgentPermissions()
        {
            var permissions = new List<string>();

            //Personal
            permissions.Add(Permissions.Personal.View);
            permissions.Add(Permissions.Personal.Edit);
            permissions.Add(Permissions.Personal.TwoFactorAuth.View);
            permissions.Add(Permissions.Personal.TwoFactorAuth.Enable);
            permissions.Add(Permissions.Personal.TraderAgent.View);
            permissions.Add(Permissions.Personal.TraderAgent.Edit);

            //Dashboard
            permissions.Add(Permissions.Dashboards.View);

            //Organization
            permissions.Add(Permissions.Organization.TraderAgents.Downlines.View);

            permissions.Add(Permissions.Organization.TraderAgents.PendingReview.View);
            permissions.Add(Permissions.Organization.TraderAgents.BankBook.View);
            permissions.Add(Permissions.Organization.TraderAgents.FrozenRecord.View);

            permissions.Add(Permissions.Organization.Traders.Create);
            permissions.Add(Permissions.Organization.Traders.PendingReview.View);
            permissions.Add(Permissions.Organization.Traders.BankBook.View);
            permissions.Add(Permissions.Organization.Traders.FrozenRecord.View);

            permissions.Add(Permissions.Organization.Traders.Edit);
            permissions.Add(Permissions.Organization.Traders.Delete);


            //Withdrawal Management
            permissions.Add(Permissions.WithdrawalManagement.Withdrawals.View);
            permissions.Add(Permissions.WithdrawalManagement.Withdrawals.Create);

            permissions.Add(Permissions.WithdrawalManagement.PendingReview.View);
            permissions.Add(Permissions.WithdrawalManagement.PendingReview.ConfirmPayment);
            permissions.Add(Permissions.WithdrawalManagement.PendingReview.Cancel);

            permissions.Add(Permissions.WithdrawalManagement.BankOptions.View);

            //Deposit Management
            permissions.Add(Permissions.DepositManagement.Deposits.View);
            permissions.Add(Permissions.DepositManagement.Deposits.Create);
            permissions.Add(Permissions.DepositManagement.Deposits.SearchTraderUser);

            permissions.Add(Permissions.DepositManagement.PendingReview.View);
            permissions.Add(Permissions.DepositManagement.PendingReview.Cancel);

            permissions.Add(Permissions.DepositManagement.DepositBankAccounts.View);

            //QrCode Management
            permissions.Add(Permissions.QrCodeManagement.Manual.View);
            permissions.Add(Permissions.QrCodeManagement.Manual.Create);
            permissions.Add(Permissions.QrCodeManagement.Manual.Edit);
            permissions.Add(Permissions.QrCodeManagement.Manual.SearchTrader);

            permissions.Add(Permissions.QrCodeManagement.Manual.PendingReview.View);

            permissions.Add(Permissions.QrCodeManagement.Manual.CodeData.View);
            permissions.Add(Permissions.QrCodeManagement.Manual.CodeData.Edit);


            //Order Management
            permissions.Add(Permissions.OrderManagement.RunningAccountRecords.View);

            return permissions;
        }

        public static List<string> GetDefaultTraderPermissions()
        {
            var permissions = new List<string>();


            //Personal
            permissions.Add(Permissions.Personal.View);
            permissions.Add(Permissions.Personal.Edit);
            permissions.Add(Permissions.Personal.TwoFactorAuth.View);
            permissions.Add(Permissions.Personal.TwoFactorAuth.Enable);
            permissions.Add(Permissions.Personal.Trader.View);
            permissions.Add(Permissions.Personal.Trader.Edit);

            //Dashboard
            permissions.Add(Permissions.Dashboards.View);


            //Withdrawal Management
            permissions.Add(Permissions.WithdrawalManagement.Withdrawals.View);
            permissions.Add(Permissions.WithdrawalManagement.Withdrawals.Create);

            permissions.Add(Permissions.WithdrawalManagement.PendingReview.View);
            permissions.Add(Permissions.WithdrawalManagement.PendingReview.ConfirmPayment);
            permissions.Add(Permissions.WithdrawalManagement.PendingReview.Cancel);

            permissions.Add(Permissions.WithdrawalManagement.BankOptions.View);

            //Deposit Management
            permissions.Add(Permissions.DepositManagement.Deposits.View);
            permissions.Add(Permissions.DepositManagement.Deposits.Create);

            permissions.Add(Permissions.DepositManagement.PendingReview.View);
            permissions.Add(Permissions.DepositManagement.PendingReview.Cancel);

            permissions.Add(Permissions.DepositManagement.DepositBankAccounts.View);

            //QrCode Management
            permissions.Add(Permissions.QrCodeManagement.Manual.View);
            permissions.Add(Permissions.QrCodeManagement.Manual.Create);
            permissions.Add(Permissions.QrCodeManagement.Manual.Edit);
            permissions.Add(Permissions.QrCodeManagement.Manual.StartPairing);
            permissions.Add(Permissions.QrCodeManagement.Manual.StopPairing);

            permissions.Add(Permissions.QrCodeManagement.Manual.PendingReview.View);

            permissions.Add(Permissions.QrCodeManagement.Manual.CodeData.View);
            permissions.Add(Permissions.QrCodeManagement.Manual.CodeData.Edit);


            //Order Management
            permissions.Add(Permissions.OrderManagement.PlatformOrders.View);
            permissions.Add(Permissions.OrderManagement.PlatformOrders.ConfirmPayment);
            permissions.Add(Permissions.OrderManagement.RunningAccountRecords.View);


            return permissions;
        }

        public static List<string> GetDefaultShopAgentPermissions()
        {
            var permissions = new List<string>();
            //Personal
            permissions.Add(Permissions.Personal.View);
            permissions.Add(Permissions.Personal.Edit);
            permissions.Add(Permissions.Personal.TwoFactorAuth.View);
            permissions.Add(Permissions.Personal.TwoFactorAuth.Enable);
            permissions.Add(Permissions.Personal.ShopAgent.View);
            permissions.Add(Permissions.Personal.ShopAgent.Edit);

            //Dashboard
            permissions.Add(Permissions.Dashboards.View);

            //Shop Management
            permissions.Add(Permissions.ShopManagement.ShopAgents.Downlines.View);

            permissions.Add(Permissions.ShopManagement.ShopAgents.PendingReview.View);
            permissions.Add(Permissions.ShopManagement.ShopAgents.BankBook.View);
            permissions.Add(Permissions.ShopManagement.ShopAgents.FrozenRecord.View);

            permissions.Add(Permissions.ShopManagement.Shops.Create);
            permissions.Add(Permissions.ShopManagement.Shops.PendingReview.View);
            permissions.Add(Permissions.ShopManagement.Shops.BankBook.View);
            permissions.Add(Permissions.ShopManagement.Shops.FrozenRecord.View);

            permissions.Add(Permissions.ShopManagement.Shops.Edit);
            permissions.Add(Permissions.ShopManagement.Shops.Delete);


            //Withdrawal Management
            permissions.Add(Permissions.WithdrawalManagement.Withdrawals.View);
            permissions.Add(Permissions.WithdrawalManagement.Withdrawals.Create);

            permissions.Add(Permissions.WithdrawalManagement.PendingReview.View);
            permissions.Add(Permissions.WithdrawalManagement.PendingReview.ConfirmPayment);
            permissions.Add(Permissions.WithdrawalManagement.PendingReview.Cancel);

            permissions.Add(Permissions.WithdrawalManagement.BankOptions.View);


            //Order Management
            permissions.Add(Permissions.OrderManagement.RunningAccountRecords.View);

            return permissions;
        }

        public static List<string> GetDefaultShopPermissions()
        {
            var permissions = new List<string>();

            //Personal
            permissions.Add(Permissions.Personal.View);
            permissions.Add(Permissions.Personal.Edit);
            permissions.Add(Permissions.Personal.TwoFactorAuth.View);
            permissions.Add(Permissions.Personal.TwoFactorAuth.Enable);
            permissions.Add(Permissions.Personal.Shop.View);
            permissions.Add(Permissions.Personal.Shop.Edit);


            //Dashboard
            permissions.Add(Permissions.Dashboards.View);


            //Shop Management
            permissions.Add(Permissions.ShopManagement.Shops.ShopGateway.View);
            permissions.Add(Permissions.ShopManagement.Shops.AmountOption.View);
            permissions.Add(Permissions.ShopManagement.Shops.ApiKey.Create);


            //Withdrawal Management
            permissions.Add(Permissions.WithdrawalManagement.Withdrawals.View);
            permissions.Add(Permissions.WithdrawalManagement.Withdrawals.Create);

            permissions.Add(Permissions.WithdrawalManagement.PendingReview.View);
            permissions.Add(Permissions.WithdrawalManagement.PendingReview.ConfirmPayment);
            permissions.Add(Permissions.WithdrawalManagement.PendingReview.Cancel);

            permissions.Add(Permissions.WithdrawalManagement.BankOptions.View);


            //Order Management
            permissions.Add(Permissions.OrderManagement.PlatformOrders.View);
            permissions.Add(Permissions.OrderManagement.PlatformOrders.Create);
            permissions.Add(Permissions.OrderManagement.RunningAccountRecords.View);

            return permissions;
        }


        public static List<string> GetDefaultTraderAgentWithGrantRightPermissions()
        {
            var permissions = new List<string>();

            permissions.Add(Permissions.Organization.TraderAgents.Downlines.Create);

            return permissions;
        }

        public static List<string> GetDefaultShopAgentWithGrantRightPermissions()
        {
            var permissions = new List<string>();

            permissions.Add(Permissions.ShopManagement.ShopAgents.Downlines.Create);

            return permissions;
        }

        public static List<string> GetDefaultReviewedUserPermissions()
        {
            var permissions = new List<string>();

            //Additional
            permissions.Add(Permissions.Additional.Reviewed);

            return permissions;
        }

        #endregion

        #endregion


        #region Predefined


        /*private static IEnumerable<Currency> GetPredefinedCurrency()
        {
            return new List<Currency>()
            {
                Currency.GBP,
                Currency.EUR,
                Currency.USD,
                Currency.TWD,
                Currency.RMB,
                Currency.YEN
            };
        }*/

        #endregion

        #region From Files



        #endregion

        #region Customs
        private static string[] GetHeaders(string[] requiredHeaders, string csvfile)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

            if (csvheaders.Count() != requiredHeaders.Count())
            {
                throw new Exception($"requiredHeader count '{ requiredHeaders.Count()}' is different then read header '{csvheaders.Count()}'");
            }

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }

        public static string GetCSV(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();

            return results;
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<ApplicationDbContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogTrace($"[{prefix}] Exception {exception.GetType().Name} with message ${exception.Message} detected on attempt {retry} of {retries}");
                    }
                );
        }

        #endregion
    }
}
