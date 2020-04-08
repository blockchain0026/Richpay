using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Infrastructure.EntityConfigurations
{
    class OrderPaymentChannelEntityTypeConfiguration : IEntityTypeConfiguration<OrderPaymentChannel>
    {
        public void Configure(EntityTypeBuilder<OrderPaymentChannel> orderPaymentChannelConfiguration)
        {
            orderPaymentChannelConfiguration.ToTable("orderPaymentChannel", OrderingContext.DEFAULT_SCHEMA);

            orderPaymentChannelConfiguration.HasKey(o => o.Id);

            orderPaymentChannelConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            orderPaymentChannelConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
