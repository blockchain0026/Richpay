using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class BarcodeInfoForAutoEntityTypeConfiguration : IEntityTypeConfiguration<BarcodeInfoForAuto>
    {
        public void Configure(EntityTypeBuilder<BarcodeInfoForAuto> barcodeInfoForAutoConfiguration)
        {
            barcodeInfoForAutoConfiguration.ToTable("barcodeInfoForAutos");

            barcodeInfoForAutoConfiguration.HasKey(d => d.QrCodeId);
            barcodeInfoForAutoConfiguration.Property(d => d.QrCodeId)
                .ValueGeneratedNever()
                .IsRequired();

            barcodeInfoForAutoConfiguration.Property(d => d.CloudDeviceUsername).IsRequired();
            barcodeInfoForAutoConfiguration.Property(d => d.CloudDevicePassword).IsRequired();
            barcodeInfoForAutoConfiguration.Property(d => d.CloudDeviceNumber).IsRequired();
        }
    }
}
