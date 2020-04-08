using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Infrastructure.EntityConfigurations
{
    class OrderPaymentSchemeEntityTypeConfiguration : IEntityTypeConfiguration<OrderPaymentScheme>
    {
        public void Configure(EntityTypeBuilder<OrderPaymentScheme> orderPaymentSchemeConfiguration)
        {
            orderPaymentSchemeConfiguration.ToTable("orderPaymentScheme", OrderingContext.DEFAULT_SCHEMA);

            orderPaymentSchemeConfiguration.HasKey(o => o.Id);

            orderPaymentSchemeConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            orderPaymentSchemeConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
