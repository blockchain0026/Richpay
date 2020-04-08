using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class RunningAccountRecordEntityTypeConfiguration : IEntityTypeConfiguration<RunningAccountRecord>
    {
        public void Configure(EntityTypeBuilder<RunningAccountRecord> runningAccountRecordConfiguration)
        {
            runningAccountRecordConfiguration.ToTable("runningAccountRecords");

            runningAccountRecordConfiguration.HasKey(r => r.Id);
            runningAccountRecordConfiguration.Property(r => r.Id)
                .UseHiLo("runningaccountrecordseq");

            runningAccountRecordConfiguration
                .HasIndex(r => new { r.UserId, r.DateCreated })
                .IsUnique(false);
            runningAccountRecordConfiguration.Property(r => r.UserId).IsUnicode(false);
            runningAccountRecordConfiguration.Property(r => r.UserName).IsRequired().IsUnicode(false);
            runningAccountRecordConfiguration.Property(r => r.UserFullName).IsRequired();
            runningAccountRecordConfiguration.Property(r => r.DownlineUserId).IsRequired(false).IsUnicode(false);
            runningAccountRecordConfiguration.Property(r => r.DownlineUserName).IsRequired(false).IsUnicode(false);
            runningAccountRecordConfiguration.Property(r => r.DownlineFullName).IsRequired(false);
            runningAccountRecordConfiguration.Property(r => r.OrderTrackingNumber).IsRequired().IsUnicode(false);
            runningAccountRecordConfiguration.Property(r => r.Status).IsRequired().IsUnicode(false);

            runningAccountRecordConfiguration.Property(r => r.Amount).HasColumnType("decimal(18,0)").IsRequired();
            runningAccountRecordConfiguration.Property(r => r.CommissionAmount).HasColumnType("decimal(18,3)").IsRequired();
            runningAccountRecordConfiguration.Property(r => r.DistributionId).IsRequired(false);

            runningAccountRecordConfiguration.Property(r => r.DistributedAmount).HasColumnType("decimal(18,3)").IsRequired(false);


            runningAccountRecordConfiguration.Property(r => r.ShopId).IsRequired().IsUnicode(false);
            runningAccountRecordConfiguration.Property(r => r.ShopUserName).IsRequired().IsUnicode(false);
            runningAccountRecordConfiguration.Property(r => r.ShopFullName).IsRequired();


            runningAccountRecordConfiguration.Property(r => r.TraderId).IsRequired(false).IsUnicode(false);
            runningAccountRecordConfiguration.Property(r => r.TraderUserName).IsRequired(false).IsUnicode(false);
            runningAccountRecordConfiguration.Property(r => r.TraderFullName).IsRequired(false);

            //runningAccountRecordConfiguration.Property(r => r.DateCreated).IsRequired();
        }
    }
}
