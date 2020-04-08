using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class BarcodeInfoForManualEntityTypeConfiguration : IEntityTypeConfiguration<BarcodeInfoForManual>
    {
        public void Configure(EntityTypeBuilder<BarcodeInfoForManual> barcodeInfoForManualConfiguration)
        {
            barcodeInfoForManualConfiguration.ToTable("barcodeInfoForManuals");

            barcodeInfoForManualConfiguration.HasKey(d => d.QrCodeId);
            barcodeInfoForManualConfiguration.Property(d => d.QrCodeId)
                .ValueGeneratedNever()
                .IsRequired();

            barcodeInfoForManualConfiguration.Property(d => d.QrCodeUrl).IsRequired();
            barcodeInfoForManualConfiguration.Property(d => d.Amount).HasColumnType("decimal(18,0)").IsRequired(false);
        }
    }
}
