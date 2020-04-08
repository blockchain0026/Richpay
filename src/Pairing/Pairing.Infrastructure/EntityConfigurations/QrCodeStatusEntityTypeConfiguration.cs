using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Infrastructure.EntityConfigurations
{
    class QrCodeStatusEntityTypeConfiguration
        : IEntityTypeConfiguration<QrCodeStatus>
    {
        public void Configure(EntityTypeBuilder<QrCodeStatus> qrCodeStatusConfiguration)
        {
            qrCodeStatusConfiguration.ToTable("qrCodeStatus", PairingContext.DEFAULT_SCHEMA);

            qrCodeStatusConfiguration.HasKey(o => o.Id);

            qrCodeStatusConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            qrCodeStatusConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
