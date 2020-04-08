using Autofac;
using Autofac.Extensions.DependencyInjection;
using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Distributions;
using Distributing.Domain.Model.Shared;
using Distributing.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ordering.Infrastructure;
using Pairing.Domain.Model.QrCodes;
using Pairing.Infrastructure;
using System;
using System.Net;
using System.Reflection;
using Util.Tools.QrCode.QrCoder;
using WebMVC.Applications.CacheServices;
using WebMVC.Applications.DomainServices.DistributingDomain;
using WebMVC.Applications.DomainServices.PairingDomain;
using WebMVC.Applications.Queries;
using WebMVC.Applications.SideEffectServices;
using WebMVC.Data;
using WebMVC.Data.AutofacModules;
using WebMVC.Extensions;
using WebMVC.Hubs;
using WebMVC.Infrastructure.ApiClients;
using WebMVC.Infrastructure.Handlers;
using WebMVC.Infrastructure.HostServices;
using WebMVC.Infrastructure.Resolver;
using WebMVC.Infrastructure.Services;
using WebMVC.Infrastructure.Services.Implementations;
using WebMVC.Models.Permissions;
using WebMVC.Models.Queries;
using WebMVC.Settings;

namespace WebMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                })
                .Services
                .AddCustomMvc(Configuration)
                .AddCustomDbContext(Configuration)
                .AddCustomIdentity(Configuration)
                .AddCustomAuthorization(Configuration)
                .AddCustomDomainService(Configuration)
                .AddCustomAppService(Configuration)
                .AddCustomHostedService(Configuration)
                .AddCustomAuthentication(Configuration);


            services.AddRazorPages();
            services.AddSignalR();


            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            //configure autofac
            var container = new ContainerBuilder();
            container.Populate(services);

            container.RegisterModule(new MediatorModule());
            container.RegisterModule(new DistributingModule(Configuration.GetConnectionString("DistributingConnection")));
            container.RegisterModule(new PairingModule(Configuration.GetConnectionString("PairingConnection")));
            container.RegisterModule(new OrderingModule(Configuration.GetConnectionString("OrderingConnection")));



            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                /*app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();*/
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseForwardedHeaders();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();

                endpoints.MapHub<OrderStatisticHub>("/orderStatisticHub");
                endpoints.MapHub<WithdrawalVoiceHub>("/withdrawalVoiceHub");
                endpoints.MapHub<DepositVoiceHub>("/depositVoiceHub");
                endpoints.MapHub<PendingReviewMemberVoiceHub>("/memberVoiceHub");
                endpoints.MapHub<PendingReviewQrCodeVoiceHub>("/qrcodeVoiceHub");
                endpoints.MapHub<RunningAccountRecordStatisticHub>("/runningAccountRecordStatisticHub");
            });

            //Must seeding first. because other context will use this database.
            DistributingDbContextSeed.SeedAsync(app, env).Wait();
            PairingDbContextSeed.SeedAsync(app, env).Wait();
            OrderingDbContextSeed.SeedAsync(app, env).Wait();

            ApplicationDbContextSeed.SimpleSeedAsync(app, env).Wait();
        }

    }

    static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEntityFrameworkSqlServer()
                          .AddDbContext<ApplicationDbContext>(options =>
                          {
                              options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                                  sqlServerOptionsAction: sqlOptions =>
                                  {
                                      sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                      sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                      sqlOptions.CommandTimeout(1000 * 60);
                                  });
                          },
                          ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                          );

            services.AddEntityFrameworkSqlServer()
                   .AddDbContext<DistributingContext>(options =>
                   {
                       options.UseSqlServer(configuration.GetConnectionString("DistributingConnection"),
                           sqlServerOptionsAction: sqlOptions =>
                           {
                               sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                               sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                               //sqlOptions.CommandTimeout(1000 * 60);
                           });
                   },
                       ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                   );

            services.AddEntityFrameworkSqlServer()
                   .AddDbContext<PairingContext>(options =>
                   {
                       options.UseSqlServer(configuration.GetConnectionString("PairingConnection"),
                           sqlServerOptionsAction: sqlOptions =>
                           {
                               sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                               sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                               //sqlOptions.CommandTimeout(1000 * 60);
                           });
                       //options.EnableSensitiveDataLogging();
                   },
                       ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                   );

            services.AddEntityFrameworkSqlServer()
                   .AddDbContext<OrderingContext>(options =>
                   {
                       options.UseSqlServer(configuration.GetConnectionString("OrderingConnection"),
                           sqlServerOptionsAction: sqlOptions =>
                           {
                               sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                               sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                               //sqlOptions.CommandTimeout(1000 * 60);
                           });
                   },
                       ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                   );

            return services;
        }

        public static IServiceCollection AddCustomIdentity(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = true;
                options.Stores.MaxLengthForKeys = 128;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                 .AddRazorPagesOptions(options =>
                 {
                     //options.AllowAreas = true;
                     options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                     options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                 });

            //Must be called after calling AddIdentity.
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.Secure = CookieSecurePolicy.Always;
            });
            void CheckSameSite(HttpContext httpContext, CookieOptions options)
            {
                options.SameSite = SameSiteMode.Strict;
                if (options.SameSite == SameSiteMode.None)
                {
                    var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                    if (DisallowsSameSiteNone(userAgent))
                    {
                        options.SameSite = SameSiteMode.Unspecified;
                    }
                }
            }

            static bool DisallowsSameSiteNone(string userAgent)
            {
                // Cover all iOS based browsers here. This includes:
                // - Safari on iOS 12 for iPhone, iPod Touch, iPad
                // - WkWebview on iOS 12 for iPhone, iPod Touch, iPad
                // - Chrome on iOS 12 for iPhone, iPod Touch, iPad
                // All of which are broken by SameSite=None, because they use the iOS networking
                // stack.
                if (userAgent.Contains("CPU iPhone OS 12") ||
                    userAgent.Contains("iPad; CPU OS 12"))
                {
                    return true;
                }

                // Cover Mac OS X based browsers that use the Mac OS networking stack. 
                // This includes:
                // - Safari on Mac OS X.
                // This does not include:
                // - Chrome on Mac OS X
                // Because they do not use the Mac OS networking stack.
                if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
                    userAgent.Contains("Version/") && userAgent.Contains("Safari"))
                {
                    return true;
                }

                // Cover Chrome 50-69, because some versions are broken by SameSite=None, 
                // and none in this range require it.
                // Note: this covers some pre-Chromium Edge versions, 
                // but pre-Chromium Edge does not require SameSite=None.
                if (userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6"))
                {
                    return true;
                }

                return false;
            }

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddHttpsRedirection(options => options.HttpsPort = 443);
            // Add Authentication services          
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(setup => setup.ExpireTimeSpan = TimeSpan.FromDays(7));

            return services;
        }

        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            //Add predefined policy.
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Permissions.Dashboards.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Dashboards.View));
                });

                #region Hubs
                options.AddPolicy(Permissions.Hubs.DepositVoice, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Hubs.DepositVoice));
                });
                options.AddPolicy(Permissions.Hubs.OrderStatistic, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Hubs.OrderStatistic));
                });
                options.AddPolicy(Permissions.Hubs.WithdrawalVoice, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Hubs.WithdrawalVoice));
                });
                #endregion

                #region Additional
                options.AddPolicy(Permissions.Additional.Reviewed, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Additional.Reviewed));
                });
                #endregion

                #region Personal
                options.AddPolicy(Permissions.Personal.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Personal.View));
                });
                options.AddPolicy(Permissions.Personal.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Personal.Edit));
                });

                #region Two Factor Auth
                options.AddPolicy(Permissions.Personal.TwoFactorAuth.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Personal.TwoFactorAuth.View));
                });
                options.AddPolicy(Permissions.Personal.TwoFactorAuth.Enable, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Personal.TwoFactorAuth.Enable));
                });
                #endregion

                options.AddPolicy(Permissions.Personal.Manager.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Personal.Manager.View));
                });
                options.AddPolicy(Permissions.Personal.Manager.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Personal.Manager.Edit));
                });

                options.AddPolicy(Permissions.Personal.TraderAgent.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Personal.TraderAgent.View));
                });
                options.AddPolicy(Permissions.Personal.TraderAgent.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Personal.TraderAgent.Edit));
                });

                options.AddPolicy(Permissions.Personal.Trader.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Personal.Trader.View));
                });
                options.AddPolicy(Permissions.Personal.Trader.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Personal.Trader.Edit));
                });

                options.AddPolicy(Permissions.Personal.Shop.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Personal.Shop.View));
                });
                options.AddPolicy(Permissions.Personal.Shop.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Personal.Shop.Edit));
                });

                options.AddPolicy(Permissions.Personal.ShopAgent.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Personal.ShopAgent.View));
                });
                options.AddPolicy(Permissions.Personal.ShopAgent.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Personal.ShopAgent.Edit));
                });

                #endregion

                #region Managers
                options.AddPolicy(Permissions.Administration.Managers.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Administration.Managers.View));
                });

                options.AddPolicy(Permissions.Administration.Managers.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Administration.Managers.Create));
                });
                options.AddPolicy(Permissions.Administration.Managers.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Administration.Managers.Edit));
                });
                options.AddPolicy(Permissions.Administration.Managers.Delete, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Administration.Managers.Delete));
                });
                #endregion

                #region Roles
                options.AddPolicy(Permissions.Administration.Roles.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Administration.Roles.View));
                });
                options.AddPolicy(Permissions.Administration.Roles.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Administration.Roles.Create));
                });
                options.AddPolicy(Permissions.Administration.Roles.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Administration.Roles.Edit));
                });
                options.AddPolicy(Permissions.Administration.Roles.Delete, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Administration.Roles.Delete));
                });
                #endregion

                #region System Configuration
                options.AddPolicy(Permissions.SystemConfiguration.SiteBaseInfo.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.SystemConfiguration.SiteBaseInfo.View));
                });
                options.AddPolicy(Permissions.SystemConfiguration.SiteBaseInfo.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.SystemConfiguration.SiteBaseInfo.Edit));
                });

                options.AddPolicy(Permissions.SystemConfiguration.Payment.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.SystemConfiguration.Payment.View));
                });
                options.AddPolicy(Permissions.SystemConfiguration.Payment.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.SystemConfiguration.Payment.Edit));
                });

                options.AddPolicy(Permissions.SystemConfiguration.SystemNotificationSound.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.SystemConfiguration.SystemNotificationSound.View));
                });
                options.AddPolicy(Permissions.SystemConfiguration.SystemNotificationSound.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.SystemConfiguration.SystemNotificationSound.Edit));
                });

                options.AddPolicy(Permissions.SystemConfiguration.UserNotification.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.SystemConfiguration.UserNotification.View));
                });
                options.AddPolicy(Permissions.SystemConfiguration.UserNotification.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.SystemConfiguration.UserNotification.Edit));
                });

                options.AddPolicy(Permissions.SystemConfiguration.WithdrawalAndDeposit.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.SystemConfiguration.WithdrawalAndDeposit.View));
                });
                options.AddPolicy(Permissions.SystemConfiguration.WithdrawalAndDeposit.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.SystemConfiguration.WithdrawalAndDeposit.Edit));
                });

                options.AddPolicy(Permissions.SystemConfiguration.PaymentChannel.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.SystemConfiguration.PaymentChannel.View));
                });
                options.AddPolicy(Permissions.SystemConfiguration.PaymentChannel.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.SystemConfiguration.PaymentChannel.Edit));
                });

                options.AddPolicy(Permissions.SystemConfiguration.QrCodeConf.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.SystemConfiguration.QrCodeConf.View));
                });
                options.AddPolicy(Permissions.SystemConfiguration.QrCodeConf.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.SystemConfiguration.QrCodeConf.Edit));
                });
                #endregion

                #region Trader Agents
                options.AddPolicy(Permissions.Organization.TraderAgents.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.TraderAgents.View));
                });
                options.AddPolicy(Permissions.Organization.TraderAgents.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.TraderAgents.Create));
                });
                options.AddPolicy(Permissions.Organization.TraderAgents.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.TraderAgents.Edit));
                });
                options.AddPolicy(Permissions.Organization.TraderAgents.Delete, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.TraderAgents.Delete));
                });

                #region Downlines
                options.AddPolicy(Permissions.Organization.TraderAgents.Downlines.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.TraderAgents.Downlines.View));
                });
                options.AddPolicy(Permissions.Organization.TraderAgents.Downlines.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.TraderAgents.Downlines.Create));
                });
                options.AddPolicy(Permissions.Organization.TraderAgents.Downlines.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.TraderAgents.Downlines.Edit));
                });
                options.AddPolicy(Permissions.Organization.TraderAgents.Downlines.Delete, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.TraderAgents.Downlines.Delete));
                });
                #endregion

                #region Pending Review
                options.AddPolicy(Permissions.Organization.TraderAgents.PendingReview.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.TraderAgents.PendingReview.View));
                });
                options.AddPolicy(Permissions.Organization.TraderAgents.PendingReview.Review, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.TraderAgents.PendingReview.Review));
                });
                #endregion

                #region Bankbook Records
                options.AddPolicy(Permissions.Organization.TraderAgents.BankBook.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.TraderAgents.BankBook.View));
                });
                #endregion

                #region Frozen Records
                options.AddPolicy(Permissions.Organization.TraderAgents.FrozenRecord.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.TraderAgents.FrozenRecord.View));
                });
                #endregion


                #region Transfer
                options.AddPolicy(Permissions.Organization.TraderAgents.Transfer.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.TraderAgents.Transfer.Create));
                });
                #endregion



                options.AddPolicy(Permissions.Organization.TraderAgents.ChangeBalance.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.TraderAgents.ChangeBalance.Create));
                });
                #endregion

                #region Traders
                options.AddPolicy(Permissions.Organization.Traders.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.Traders.View));
                });

                options.AddPolicy(Permissions.Organization.Traders.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.Traders.Create));
                });
                options.AddPolicy(Permissions.Organization.Traders.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.Traders.Edit));
                });
                options.AddPolicy(Permissions.Organization.Traders.Delete, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.Traders.Delete));
                });

                #region Pending Review
                options.AddPolicy(Permissions.Organization.Traders.PendingReview.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.TraderAgents.PendingReview.View));
                });
                options.AddPolicy(Permissions.Organization.Traders.PendingReview.Review, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.TraderAgents.PendingReview.Review));
                });
                #endregion

                #region Bankbook Records
                options.AddPolicy(Permissions.Organization.Traders.BankBook.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.Traders.BankBook.View));
                });
                #endregion

                #region Frozen Records
                options.AddPolicy(Permissions.Organization.Traders.FrozenRecord.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.Traders.FrozenRecord.View));
                });
                #endregion


                #region Transfer
                options.AddPolicy(Permissions.Organization.Traders.Transfer.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.Traders.Transfer.Create));
                });
                #endregion


                options.AddPolicy(Permissions.Organization.Traders.ChangeBalance.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.Organization.Traders.ChangeBalance.Create));
                });
                #endregion

                #region Shop Agents
                options.AddPolicy(Permissions.ShopManagement.ShopAgents.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.ShopAgents.View));
                });
                options.AddPolicy(Permissions.ShopManagement.ShopAgents.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.ShopAgents.Create));
                });
                options.AddPolicy(Permissions.ShopManagement.ShopAgents.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.ShopAgents.Edit));
                });
                options.AddPolicy(Permissions.ShopManagement.ShopAgents.Delete, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.ShopAgents.Delete));
                });

                #region Downlines
                options.AddPolicy(Permissions.ShopManagement.ShopAgents.Downlines.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.ShopAgents.Downlines.View));
                });
                options.AddPolicy(Permissions.ShopManagement.ShopAgents.Downlines.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.ShopAgents.Downlines.Create));
                });
                options.AddPolicy(Permissions.ShopManagement.ShopAgents.Downlines.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.ShopAgents.Downlines.Edit));
                });
                options.AddPolicy(Permissions.ShopManagement.ShopAgents.Downlines.Delete, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.ShopAgents.Downlines.Delete));
                });
                #endregion

                #region Pending Review
                options.AddPolicy(Permissions.ShopManagement.ShopAgents.PendingReview.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.ShopAgents.PendingReview.View));
                });
                options.AddPolicy(Permissions.ShopManagement.ShopAgents.PendingReview.Review, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.ShopAgents.PendingReview.Review));
                });
                #endregion

                #region Bankbook Records
                options.AddPolicy(Permissions.ShopManagement.ShopAgents.BankBook.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.ShopAgents.BankBook.View));
                });
                #endregion

                #region Frozen Records
                options.AddPolicy(Permissions.ShopManagement.ShopAgents.FrozenRecord.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.ShopAgents.FrozenRecord.View));
                });
                #endregion

                options.AddPolicy(Permissions.ShopManagement.ShopAgents.ChangeBalance.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.ShopAgents.ChangeBalance.Create));
                });
                #endregion

                #region Shops
                options.AddPolicy(Permissions.ShopManagement.Shops.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.View));
                });

                options.AddPolicy(Permissions.ShopManagement.Shops.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.Create));
                });
                options.AddPolicy(Permissions.ShopManagement.Shops.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.Edit));
                });
                options.AddPolicy(Permissions.ShopManagement.Shops.Delete, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.Delete));
                });

                #region Pending Review
                options.AddPolicy(Permissions.ShopManagement.Shops.PendingReview.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.PendingReview.View));
                });
                options.AddPolicy(Permissions.ShopManagement.Shops.PendingReview.Review, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.PendingReview.Review));
                });
                #endregion

                #region Bankbook Records
                options.AddPolicy(Permissions.ShopManagement.Shops.BankBook.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.BankBook.View));
                });
                #endregion

                #region Frozen Records
                options.AddPolicy(Permissions.ShopManagement.Shops.FrozenRecord.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.FrozenRecord.View));
                });
                #endregion

                #region Change Balance
                options.AddPolicy(Permissions.ShopManagement.Shops.ChangeBalance.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.ChangeBalance.Create));
                });
                #endregion

                #region Shop Gateway
                options.AddPolicy(Permissions.ShopManagement.Shops.ShopGateway.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.ShopGateway.View));
                });
                options.AddPolicy(Permissions.ShopManagement.Shops.ShopGateway.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.ShopGateway.Create));
                });
                options.AddPolicy(Permissions.ShopManagement.Shops.ShopGateway.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.ShopGateway.Edit));
                });
                options.AddPolicy(Permissions.ShopManagement.Shops.ShopGateway.Delete, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.ShopGateway.Delete));
                });
                #endregion

                #region Amount Option
                options.AddPolicy(Permissions.ShopManagement.Shops.ShopGateway.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.ShopGateway.View));
                });
                options.AddPolicy(Permissions.ShopManagement.Shops.ShopGateway.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.ShopGateway.Create));
                });
                options.AddPolicy(Permissions.ShopManagement.Shops.ShopGateway.Delete, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.ShopGateway.Delete));
                });
                #endregion

                #region Api Key
                options.AddPolicy(Permissions.ShopManagement.Shops.ApiKey.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.ShopManagement.Shops.ApiKey.Create));
                });
                #endregion

                #endregion

                #region Withdrawal
                options.AddPolicy(Permissions.WithdrawalManagement.Withdrawals.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.WithdrawalManagement.Withdrawals.View));
                });
                options.AddPolicy(Permissions.WithdrawalManagement.Withdrawals.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.WithdrawalManagement.Withdrawals.Create));
                });
                options.AddPolicy(Permissions.WithdrawalManagement.Withdrawals.SearchUser, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.WithdrawalManagement.Withdrawals.SearchUser));
                });


                options.AddPolicy(Permissions.WithdrawalManagement.PendingReview.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.WithdrawalManagement.PendingReview.View));
                });
                options.AddPolicy(Permissions.WithdrawalManagement.PendingReview.Approve, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.WithdrawalManagement.PendingReview.Approve));
                });
                options.AddPolicy(Permissions.WithdrawalManagement.PendingReview.ForceSuccess, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.WithdrawalManagement.PendingReview.ForceSuccess));
                });
                options.AddPolicy(Permissions.WithdrawalManagement.PendingReview.ConfirmPayment, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.WithdrawalManagement.PendingReview.ConfirmPayment));
                });
                options.AddPolicy(Permissions.WithdrawalManagement.PendingReview.Cancel, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.WithdrawalManagement.PendingReview.Cancel));
                });
                options.AddPolicy(Permissions.WithdrawalManagement.PendingReview.ApproveCancellation, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.WithdrawalManagement.PendingReview.ApproveCancellation));
                });


                options.AddPolicy(Permissions.WithdrawalManagement.BankOptions.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.WithdrawalManagement.BankOptions.View));
                });
                options.AddPolicy(Permissions.WithdrawalManagement.BankOptions.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.WithdrawalManagement.BankOptions.Create));
                });
                options.AddPolicy(Permissions.WithdrawalManagement.BankOptions.Delete, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.WithdrawalManagement.BankOptions.Delete));
                });
                #endregion

                #region Deposit
                options.AddPolicy(Permissions.DepositManagement.Deposits.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.DepositManagement.Deposits.View));
                });
                options.AddPolicy(Permissions.DepositManagement.Deposits.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.DepositManagement.Deposits.Create));
                });
                options.AddPolicy(Permissions.DepositManagement.Deposits.SearchUser, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.DepositManagement.Deposits.SearchUser));
                });
                options.AddPolicy(Permissions.DepositManagement.Deposits.SearchTraderUser, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.DepositManagement.Deposits.SearchTraderUser));
                });


                options.AddPolicy(Permissions.DepositManagement.PendingReview.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.DepositManagement.PendingReview.View));
                });
                options.AddPolicy(Permissions.DepositManagement.PendingReview.Verify, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.DepositManagement.PendingReview.Verify));
                });
                options.AddPolicy(Permissions.DepositManagement.PendingReview.Cancel, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.DepositManagement.PendingReview.Cancel));
                });


                options.AddPolicy(Permissions.DepositManagement.DepositBankAccounts.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.DepositManagement.DepositBankAccounts.View));
                });
                options.AddPolicy(Permissions.DepositManagement.DepositBankAccounts.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.DepositManagement.DepositBankAccounts.Create));
                });
                options.AddPolicy(Permissions.DepositManagement.DepositBankAccounts.Delete, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.DepositManagement.DepositBankAccounts.Delete));
                });
                #endregion

                #region QrCode Management

                #region Manual
                options.AddPolicy(Permissions.QrCodeManagement.Manual.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.QrCodeManagement.Manual.View));
                });
                options.AddPolicy(Permissions.QrCodeManagement.Manual.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.QrCodeManagement.Manual.Create));
                });
                options.AddPolicy(Permissions.QrCodeManagement.Manual.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.QrCodeManagement.Manual.Edit));
                });
                options.AddPolicy(Permissions.QrCodeManagement.Manual.SearchTrader, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.QrCodeManagement.Manual.SearchTrader));
                });
                options.AddPolicy(Permissions.QrCodeManagement.Manual.SearchShop, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.QrCodeManagement.Manual.SearchShop));
                });
                options.AddPolicy(Permissions.QrCodeManagement.Manual.Enable, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.QrCodeManagement.Manual.Enable));
                });
                options.AddPolicy(Permissions.QrCodeManagement.Manual.Disable, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.QrCodeManagement.Manual.Disable));
                });
                options.AddPolicy(Permissions.QrCodeManagement.Manual.StartPairing, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.QrCodeManagement.Manual.StartPairing));
                });
                options.AddPolicy(Permissions.QrCodeManagement.Manual.StopPairing, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.QrCodeManagement.Manual.StopPairing));
                });
                options.AddPolicy(Permissions.QrCodeManagement.Manual.ResetRiskControlData, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.QrCodeManagement.Manual.ResetRiskControlData));
                });


                //Pending  Review
                options.AddPolicy(Permissions.QrCodeManagement.Manual.PendingReview.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.QrCodeManagement.Manual.PendingReview.View));
                });
                options.AddPolicy(Permissions.QrCodeManagement.Manual.PendingReview.Approve, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.QrCodeManagement.Manual.PendingReview.Approve));
                });
                options.AddPolicy(Permissions.QrCodeManagement.Manual.PendingReview.Reject, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.QrCodeManagement.Manual.PendingReview.Reject));
                });

                //Pending  Review
                options.AddPolicy(Permissions.QrCodeManagement.Manual.CodeData.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.QrCodeManagement.Manual.CodeData.View));
                });
                options.AddPolicy(Permissions.QrCodeManagement.Manual.CodeData.Edit, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.QrCodeManagement.Manual.CodeData.Edit));
                });


                #endregion

                #endregion

                #region Order Management

                #region Platform Orders
                options.AddPolicy(Permissions.OrderManagement.PlatformOrders.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.OrderManagement.PlatformOrders.View));
                });
                options.AddPolicy(Permissions.OrderManagement.PlatformOrders.Create, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.OrderManagement.PlatformOrders.Create));
                });
                options.AddPolicy(Permissions.OrderManagement.PlatformOrders.ConfirmPayment, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.OrderManagement.PlatformOrders.ConfirmPayment));
                });

                #endregion

                #region Running Account Records
                options.AddPolicy(Permissions.OrderManagement.RunningAccountRecords.View, builder =>
                {
                    builder.AddRequirements(new PermissionRequirement(Permissions.OrderManagement.RunningAccountRecords.View));
                });
                #endregion

                #endregion


                // The rest omitted for brevity.
            });

            //Register permission handler.
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            return services;
        }

        public static IServiceCollection AddCustomAppService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IManagerService, ManagerService>();
            services.AddScoped<IRoleService, RoleService>();

            services.AddScoped<ITraderAgentService, TraderAgentService>();
            services.AddScoped<ITraderService, TraderService>();
            services.AddScoped<IShopAgentService, ShopAgentService>();
            services.AddScoped<IShopService, ShopService>();

            services.AddScoped<ISystemConfigurationService, SystemConfigurationService>();
            services.AddScoped<IPersonalService, PersonalService>();

            services.AddScoped<IBalanceService, BalanceService>();

            services.AddScoped<IWithdrawalService, WithdrawalService>();
            services.AddScoped<IDepositService, DepositService>();

            services.AddScoped<Util.Tools.QrCode.IQrCodeService, QrCoderService>();
            services.AddScoped<IQrCodeService, QrCodeService>();

            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IApiService, ApiService>();


            services.AddScoped<IQrCodeSideEffectService, QrCodeSideEffectService>();

            services.AddScoped<INewPayApiClient, NewPayApiClient>();

            services.AddScoped<IApplicationDateTimeService, ApplicationDateTimeService>();


            //Cache Services
            services.AddSingleton<IQrCodeQueueService, QrCodeQueueService>();
            services.AddSingleton<ICommissionCacheService, CommissionCacheService>();
            services.AddSingleton<IQrCodeCacheService, QrCodeCacheService>();
            services.AddSingleton<IUserCacheService, UserCacheService>();
            services.AddSingleton<IOrderDailyCacheService, OrderDailyCacheService>();

            return services;
        }

        public static IServiceCollection AddCustomDomainService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDateTimeService,
                Applications.DomainServices.DistributingDomain.DateTimeService>();
            services.AddScoped<
                Pairing.Domain.Model.Shared.IDateTimeService,
                Applications.DomainServices.PairingDomain.DateTimeService>();
            services.AddScoped<
                Ordering.Domain.Model.Shared.IDateTimeService,
                Applications.DomainServices.OrderingDomain.DateTimeService>();
            services.AddScoped<IOpenTimeService, OpenTimeService>();


            //services.AddScoped<IDistributionService, DistributionService>();
            services.AddScoped<IDistributionService, Applications.DomainServices.DistributingDomain.DistributionService>();
            services.AddScoped<IBalanceDomainService, BalanceDomainService>();

            services.AddScoped<IPairingDomainService, PairingDomainService>();


            return services;
        }

        public static IServiceCollection AddCustomHostedService(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Hosted Services
            //services.AddHostedService<ClearCompletedOrderBackgroundService>();

            // SignalR boradcasters
            services.AddHostedService<OrderStatisticSenderBroadcasterBackgroundService>();
            services.AddHostedService<WithdrawalVoiceBroadcasterBackgroundService>();
            services.AddHostedService<DepositVoiceBroadcasterBackgroundService>();
            services.AddHostedService<PendingReviewMemberVoiceBroadcasterBackgroundService>();
            services.AddHostedService<PendingReviewQrCodeVoiceBroadcasterBackgroundService>();
            //services.AddHostedService<RunningAccountRecordStatisticBroadcasterBackgroundService>();


            // Ordering Hosted Services
            //services.AddHostedService<ConfirmAwaitingPaymentOrderBackgroundService>(); //For testing.
            //services.AddHostedService<ResolveExpiredOrderBackgroundService>();

            //services.AddHostedService<ResolveCompletedDistributionBackgroundService>();

            //Cache Updating Hosted Services
            services.AddHostedService<UpdateCommissionCacheBackgroundService>();
            services.AddHostedService<UpdateQrCodeIdsForPairingBackgroundService>();
            services.AddHostedService<UpdateUserCacheBackgroundService>();
            //services.AddHostedService<UpdateDailyOrderCacheBackgroundService>(); //Deprecated, too slow.

            return services;
        }

        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<SystemConfigurations>(configuration);
            services.ConfigureWritable<SiteBaseInfo>(configuration.GetSection("SiteBaseInfo"));
            services.ConfigureWritable<Payment>(configuration.GetSection("Payment"));
            services.ConfigureWritable<SystemNotificationSound>(configuration.GetSection("SystemNotificationSound"));
            services.ConfigureWritable<UserNotification>(configuration.GetSection("UserNotification"));
            services.ConfigureWritable<WithdrawalAndDeposit>(configuration.GetSection("WithdrawalAndDeposit"));
            services.ConfigureWritable<PaymentChannel>(configuration.GetSection("PaymentChannel"));
            services.ConfigureWritable<QrCodeConf>(configuration.GetSection("QrCodeConf"));

            //Background Task Settings For Long Running Task.
            services.Configure<ClearCompletedOrderSettings>(configuration.GetSection("ClearCompletedOrderSettings"));
            services.Configure<ResolveCompletedDistributionSettings>(configuration.GetSection("ResolveCompletedDistributionSettings"));


            services.AddSession();
            services.AddDistributedMemoryCache();

            /*if (configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
            {
                services.AddDataProtection(opts =>
                {
                    opts.ApplicationDiscriminator = "eshop.webmvc";
                })
                .PersistKeysToRedis(ConnectionMultiplexer.Connect(configuration["DPConnectionString"]), "DataProtection-Keys");
            }*/

            return services;
        }

    }
}
