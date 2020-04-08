using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Infrastructure.EntityConfigurations
{
    class ShopSettingsEntityTypeConfiguration : IEntityTypeConfiguration<ShopSettings>
    {
        public void Configure(EntityTypeBuilder<ShopSettings> shopSettingsConfiguration)
        {
            shopSettingsConfiguration.ToTable("shopSettingss", PairingContext.DEFAULT_SCHEMA);

            shopSettingsConfiguration.HasKey(s => s.Id);

            shopSettingsConfiguration.Ignore(s => s.DomainEvents);

            shopSettingsConfiguration.Property(s => s.Id)
                .UseHiLo("shopsettingsseq", PairingContext.DEFAULT_SCHEMA);


            shopSettingsConfiguration.Property(s => s.ShopId).IsRequired();
            shopSettingsConfiguration.Property(s => s.IsOpen).IsRequired();

            var navigation = shopSettingsConfiguration.Metadata.FindNavigation(nameof(ShopSettings.OrderAmountOptions));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
