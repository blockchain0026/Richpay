using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Model.ShopApis;
using Ordering.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Infrastructure.EntityConfigurations
{
    class ShopApiEntityTypeConfiguration : IEntityTypeConfiguration<ShopApi>
    {
        public void Configure(EntityTypeBuilder<ShopApi> shopApiConfiguration)
        {
            shopApiConfiguration.ToTable("shopApis", OrderingContext.DEFAULT_SCHEMA);

            shopApiConfiguration.HasKey(s => s.Id);

            shopApiConfiguration.Ignore(s => s.DomainEvents);

            shopApiConfiguration.Property(s => s.Id)
                .UseHiLo("shopapiseq", OrderingContext.DEFAULT_SCHEMA);


            shopApiConfiguration.Property(s => s.ShopId).IsRequired();
            shopApiConfiguration.Property(s => s.ApiKey).IsRequired();


            var navigation = shopApiConfiguration.Metadata.FindNavigation(nameof(ShopApi.IpWhitelists));

            // DDD Patterns comment:
            //Set as field (New since EF 1.1) to access the OrderItem collection property through its field
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
