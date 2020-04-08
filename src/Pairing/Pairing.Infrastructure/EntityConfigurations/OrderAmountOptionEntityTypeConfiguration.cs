using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pairing.Domain.Model.ShopSettingsDomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Infrastructure.EntityConfigurations
{
    class OrderAmountOptionEntityTypeConfiguration
        : IEntityTypeConfiguration<OrderAmountOption>
    {
        public void Configure(EntityTypeBuilder<OrderAmountOption> orderAmountOptionConfiguration)
        {
            orderAmountOptionConfiguration.ToTable("orderAmountOptions", PairingContext.DEFAULT_SCHEMA);

            orderAmountOptionConfiguration.HasKey(b => b.Id);

            orderAmountOptionConfiguration.Ignore(b => b.DomainEvents);

            orderAmountOptionConfiguration.Property(b => b.Id)
                .UseHiLo("orderamountoptionseq");

            orderAmountOptionConfiguration
                .Property<string>("ShopId")
                .IsRequired();


            orderAmountOptionConfiguration
                .Property<decimal>("_amount")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,3)")
                .IsRequired();


            orderAmountOptionConfiguration
                .Property<DateTime>("_dateCreated")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("DateCreated")
                .IsRequired();
        }
    }
}
