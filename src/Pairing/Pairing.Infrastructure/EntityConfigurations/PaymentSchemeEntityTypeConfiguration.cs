using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Infrastructure.EntityConfigurations
{
    class PaymentSchemeEntityTypeConfiguration
        : IEntityTypeConfiguration<PaymentScheme>
    {
        public void Configure(EntityTypeBuilder<PaymentScheme> paymentSchemeConfiguration)
        {
            paymentSchemeConfiguration.ToTable("paymentScheme", PairingContext.DEFAULT_SCHEMA);

            paymentSchemeConfiguration.HasKey(o => o.Id);

            paymentSchemeConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            paymentSchemeConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
