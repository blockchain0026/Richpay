using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pairing.Domain.Model.CloudDevices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Infrastructure.EntityConfigurations
{
    class CloudDeviceEntityTypeConfiguration : IEntityTypeConfiguration<CloudDevice>
    {
        public void Configure(EntityTypeBuilder<CloudDevice> cloudDeviceConfiguration)
        {
            cloudDeviceConfiguration.ToTable("cloudDevices", PairingContext.DEFAULT_SCHEMA);

            cloudDeviceConfiguration.HasKey(q => q.Id);

            cloudDeviceConfiguration.Ignore(q => q.DomainEvents);

            cloudDeviceConfiguration.Property(q => q.Id)
                .UseHiLo("clouddeviceseq", PairingContext.DEFAULT_SCHEMA);


            cloudDeviceConfiguration.Property(q => q.UserId).IsRequired();

            cloudDeviceConfiguration
                .Property<int>("_cloudDeviceStatusId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("CloudDeviceStatusId")
                .IsRequired();

          
            cloudDeviceConfiguration.Property(q => q.Name).IsRequired();
            cloudDeviceConfiguration.Property(q => q.Number).IsRequired();
            cloudDeviceConfiguration.Property(q => q.LoginUsername).IsRequired();
            cloudDeviceConfiguration.Property(q => q.LoginPassword).IsRequired();
            cloudDeviceConfiguration.Property(q => q.ApiKey).IsRequired();


            cloudDeviceConfiguration.Property(q => q.DateCreated).IsRequired();

        }
    }
}
