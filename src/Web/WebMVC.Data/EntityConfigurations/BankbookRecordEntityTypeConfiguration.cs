using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class BankbookRecordEntityTypeConfiguration : IEntityTypeConfiguration<BankbookRecord>
    {
        public void Configure(EntityTypeBuilder<BankbookRecord> bankbookRecordConfiguration)
        {
            bankbookRecordConfiguration.ToTable("bankbookRecords");

            bankbookRecordConfiguration.HasKey(b => b.Id);

            bankbookRecordConfiguration.Property(b => b.Id)
                .UseHiLo("bankbookrecordseq"/*, ApplicationDbContext.DEFAULT_SCHEMA*/);


            bankbookRecordConfiguration.Property(b => b.UserId).IsRequired();
            bankbookRecordConfiguration.Property(b => b.BalanceId).IsRequired();

            bankbookRecordConfiguration.Property(b => b.DateOccurred).IsRequired();
            bankbookRecordConfiguration.Property(b => b.Type).IsRequired();

            bankbookRecordConfiguration.Property(b => b.BalanceBefore).HasColumnType("decimal(18,3)").IsRequired();
            bankbookRecordConfiguration.Property(b => b.AmountChanged).HasColumnType("decimal(18,3)").IsRequired();
            bankbookRecordConfiguration.Property(b => b.BalanceAfter).HasColumnType("decimal(18,3)").IsRequired();

            bankbookRecordConfiguration.Property(b => b.TrackingId).IsRequired();
            bankbookRecordConfiguration.Property(b => b.Description).IsRequired(false);
        }
    }
}
