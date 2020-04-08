using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Infrastructure.EntityConfigurations
{
    class PairingStatusEntityTypeConfiguration
    : IEntityTypeConfiguration<PairingStatus>
    {
        public void Configure(EntityTypeBuilder<PairingStatus> pairingStatusConfiguration)
        {
            pairingStatusConfiguration.ToTable("pairingStatus", PairingContext.DEFAULT_SCHEMA);

            pairingStatusConfiguration.HasKey(o => o.Id);

            pairingStatusConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            pairingStatusConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
