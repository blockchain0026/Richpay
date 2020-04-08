using Autofac;
using Pairing.Domain.Model.CloudDevices;
using Pairing.Domain.Model.QrCodes;
using Pairing.Domain.Model.ShopGateways;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using Pairing.Infrastructure.Idempotency;
using Pairing.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Applications.Queries.QrCodes;
using WebMVC.Applications.Queries.ShopGateways;
using WebMVC.Applications.Queries.ShopSettings;

namespace WebMVC.Data.AutofacModules
{
    public class PairingModule
     : Autofac.Module
    {
        public string QueriesConnectionString { get; }

        public PairingModule(string qconstr)
        {
            QueriesConnectionString = qconstr;

        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<QrCodeQueries>()
                .As<IQrCodeQueries>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ShopGatewayQueries>()
                .As<IShopGatewayQueries>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ShopSettingsQueries>()
                .As<IShopSettingsQueries>()
                .InstancePerLifetimeScope();

            builder.RegisterType<QrCodeRepository>()
                .As<IQrCodeRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CloudDeviceRepository>()
                .As<ICloudDeviceRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ShopGatewayRepository>()
                .As<IShopGatewayRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ShopSettingsRepository>()
                .As<IShopSettingsRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RequestManager>()
               .As<IRequestManager>()
               .InstancePerLifetimeScope();
        }
    }
}
