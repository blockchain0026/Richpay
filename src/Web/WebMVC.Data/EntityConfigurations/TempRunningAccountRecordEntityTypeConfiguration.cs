using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class TempRunningAccountRecordEntityTypeConfiguration : IEntityTypeConfiguration<TempRunningAccountRecord>
    {
        public void Configure(EntityTypeBuilder<TempRunningAccountRecord> tempRunningAccountRecordConfiguration)
        {
            tempRunningAccountRecordConfiguration.ToTable("tempRunningAccountRecords");

            tempRunningAccountRecordConfiguration.HasKey(w => w.Id);
            tempRunningAccountRecordConfiguration.Property(w => w.Id)
                .ValueGeneratedNever()
                .IsRequired();

            tempRunningAccountRecordConfiguration.Property(w => w.UserId).IsRequired();
            tempRunningAccountRecordConfiguration.Property(w => w.OrderTrackingNumber).IsRequired();
        }
    }
}
