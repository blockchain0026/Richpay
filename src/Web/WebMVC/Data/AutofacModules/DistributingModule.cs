using Autofac;
using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Banks;
using Distributing.Domain.Model.Chains;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Deposits;
using Distributing.Domain.Model.Distributions;
using Distributing.Domain.Model.Frozens;
using Distributing.Domain.Model.Transfers;
using Distributing.Domain.Model.Withdrawals;
using Distributing.Infrastructure.Idempotency;
using Distributing.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.Balances;
using WebMVC.Applications.Queries.Bankbook;
using WebMVC.Applications.Queries.Commission;
using WebMVC.Applications.Queries.Deposits;
using WebMVC.Applications.Queries.Frozen;
using WebMVC.Applications.Queries.ShopAgents;
using WebMVC.Applications.Queries.Shops;
using WebMVC.Applications.Queries.TraderAgents;
using WebMVC.Applications.Queries.Traders;
using WebMVC.Applications.Queries.Withdrawals;

namespace WebMVC.Data.AutofacModules
{
    public class DistributingModule
     : Autofac.Module
    {
        public string QueriesConnectionString { get; }

        public DistributingModule(string qconstr)
        {
            QueriesConnectionString = qconstr;

        }

        protected override void Load(ContainerBuilder builder)
        {

            /*builder.Register(c => new OrderQueries(QueriesConnectionString))
                .As<IOrderQueries>()
                .InstancePerLifetimeScope();*/

            builder.RegisterType<BalanceQueries>()
                .As<IBalanceQueries>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CommissionQueries>()
                .As<ICommissionQueries>()
                .InstancePerLifetimeScope();

            builder.RegisterType<BankbookQueries>()
                .As<IBankbookQueries>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<FrozenQueries>()
                .As<IFrozenQueries>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<TraderAgentQueries>()
                .As<ITraderAgentQueries>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<TraderQueries>()
                .As<ITraderQueries>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ShopAgentQueries>()
                .As<IShopAgentQueries>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<ShopQueries>()
                .As<IShopQueries>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<DepositQueries>()
                .As<IDepositQueries>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<WithdrawalQueries>()
                .As<IWithdrawalQueries>()
                .InstancePerLifetimeScope();




            builder.RegisterType<BalanceRepository>()
                .As<IBalanceRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ChainRepository>()
                .As<IChainRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CommissionRepository>()
                .As<ICommissionRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DepositAccountRepository>()
                .As<IDepositAccountRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DepositRepository>()
                .As<IDepositRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<DistributionRepository>()
                .As<IDistributionRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<WithdrawalBankRepository>()
                .As<IWithdrawalBankRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<WithdrawalRepository>()
                .As<IWithdrawalRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<FrozenRepository>()
                .As<IFrozenRepository>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<TransferRepository>()
                .As<ITransferRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RequestManager>()
               .As<IRequestManager>()
               .InstancePerLifetimeScope();
        }
    }
}
