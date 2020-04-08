using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Model.ShopApis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Infrastructure.EntityConfigurations
{
    class IpWhitelistEntityTypeConfiguration
     : IEntityTypeConfiguration<IpWhitelist>
    {
        public void Configure(EntityTypeBuilder<IpWhitelist> ipWhitelistConfiguration)
        {
            ipWhitelistConfiguration.ToTable("ipWhitelists", OrderingContext.DEFAULT_SCHEMA);

            ipWhitelistConfiguration.HasKey(i => i.Id);

            ipWhitelistConfiguration.Ignore(i => i.DomainEvents);

            ipWhitelistConfiguration.Property(i => i.Id)
                .UseHiLo("balancewithdrawalseq");

            ipWhitelistConfiguration
                .Property<string>("_address")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Address")
                .IsRequired();
        }
    }
}
