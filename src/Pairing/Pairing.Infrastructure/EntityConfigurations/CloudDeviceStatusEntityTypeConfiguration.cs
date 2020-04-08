using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pairing.Domain.Model.CloudDevices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Infrastructure.EntityConfigurations
{
    class CloudDeviceStatusEntityTypeConfiguration
        : IEntityTypeConfiguration<CloudDeviceStatus>
    {
        public void Configure(EntityTypeBuilder<CloudDeviceStatus> cloudDeviceStatusConfiguration)
        {
            cloudDeviceStatusConfiguration.ToTable("cloudDeviceStatus", PairingContext.DEFAULT_SCHEMA);

            cloudDeviceStatusConfiguration.HasKey(o => o.Id);

            cloudDeviceStatusConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            cloudDeviceStatusConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
