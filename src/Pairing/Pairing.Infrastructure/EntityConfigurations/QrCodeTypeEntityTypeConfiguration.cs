using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Infrastructure.EntityConfigurations
{
    class QrCodeTypeEntityTypeConfiguration
    : IEntityTypeConfiguration<QrCodeType>
    {
        public void Configure(EntityTypeBuilder<QrCodeType> qrCodeTypeConfiguration)
        {
            qrCodeTypeConfiguration.ToTable("qrCodeType", PairingContext.DEFAULT_SCHEMA);

            qrCodeTypeConfiguration.HasKey(o => o.Id);

            qrCodeTypeConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            qrCodeTypeConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
