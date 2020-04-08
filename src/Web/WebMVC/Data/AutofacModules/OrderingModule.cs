using Autofac;
using Distributing.Infrastructure.Idempotency;
using Newtonsoft.Json;
using Ordering.Domain.Model.Orders;
using Ordering.Domain.Model.ShopApis;
using Ordering.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.IpWhitelists;
using WebMVC.Applications.Queries.Orders;
using WebMVC.Applications.Queries.RunningAccounts;
using WebMVC.Infrastructure.Resolver;

namespace WebMVC.Data.AutofacModules
{
    public class OrderingModule
    : Autofac.Module
    {
        public string QueriesConnectionString { get; }

        public OrderingModule(string qconstr)
        {
            QueriesConnectionString = qconstr;

        }

        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<OrderQueries>()
                .As<IOrderQueries>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RunningAccountQueries>()
                .As<IRunningAccountQueries>()
                .InstancePerLifetimeScope();

            builder.RegisterType<IpWhitelistQueries>()
                .As<IIpWhitelistQueries>()
                .InstancePerLifetimeScope();


            builder.RegisterType<OrderRepository>()
                .As<IOrderRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ShopApiRepository>()
                .As<IShopApiRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RequestManager>()
               .As<IRequestManager>()
               .InstancePerLifetimeScope();
        }
    }
}
