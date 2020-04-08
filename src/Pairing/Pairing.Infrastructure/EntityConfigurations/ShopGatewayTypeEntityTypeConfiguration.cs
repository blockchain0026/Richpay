using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pairing.Domain.Model.ShopGateways;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Infrastructure.EntityConfigurations
{
    class ShopGatewayTypeEntityTypeConfiguration
        : IEntityTypeConfiguration<ShopGatewayType>
    {
        public void Configure(EntityTypeBuilder<ShopGatewayType> shopGatewayTypeConfiguration)
        {
            shopGatewayTypeConfiguration.ToTable("shopGatewayType", PairingContext.DEFAULT_SCHEMA);

            shopGatewayTypeConfiguration.HasKey(o => o.Id);

            shopGatewayTypeConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            shopGatewayTypeConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
