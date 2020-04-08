using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Infrastructure.EntityConfigurations
{
    class PaymentChannelEntityTypeConfiguration
        : IEntityTypeConfiguration<PaymentChannel>
    {
        public void Configure(EntityTypeBuilder<PaymentChannel> paymentChannelConfiguration)
        {
            paymentChannelConfiguration.ToTable("paymentChannel", PairingContext.DEFAULT_SCHEMA);

            paymentChannelConfiguration.HasKey(o => o.Id);

            paymentChannelConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            paymentChannelConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
