using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class DepositEntryEntityTypeConfiguration : IEntityTypeConfiguration<DepositEntry>
    {
        public void Configure(EntityTypeBuilder<DepositEntry> depositEntryConfiguration)
        {
            depositEntryConfiguration.ToTable("depositEntrys");

            depositEntryConfiguration.HasKey(d => d.DepositId);
            depositEntryConfiguration.Property(d => d.DepositId)
                .ValueGeneratedNever()
                .IsRequired();

            depositEntryConfiguration.Property(d => d.DepositStatus).IsRequired();
            depositEntryConfiguration.Property(d => d.BalanceId).IsRequired();
            depositEntryConfiguration.Property(d => d.CreateByUplineId).IsRequired(false);

            depositEntryConfiguration.Property(d => d.UserId).IsRequired();
            depositEntryConfiguration.Property(d => d.Username).IsRequired();
            depositEntryConfiguration.Property(d => d.FullName).IsRequired();
            depositEntryConfiguration.Property(d => d.UserType).IsRequired();

            depositEntryConfiguration
                       .OwnsOne(d => d.DepositBankAccount, a =>
                       {
                           a.WithOwner();
                           a.Ignore(w => w.Name);
                           a.Property(w => w.BankAccountId).IsRequired();
                           a.Property(w => w.BankName).IsRequired();
                           a.Property(w => w.AccountName).IsRequired();
                           a.Property(w => w.AccountNumber).IsRequired();
                           a.Property(w => w.DateCreated).IsRequired(false);
                       });

            depositEntryConfiguration.Property(d => d.TotalAmount).IsRequired();
            depositEntryConfiguration.Property(d => d.CommissionRateInThousandth).IsRequired();
            depositEntryConfiguration.Property(d => d.CommissionAmount).HasColumnType("decimal(18,3)").IsRequired();
            depositEntryConfiguration.Property(d => d.ActualAmount).HasColumnType("decimal(18,3)").IsRequired();

            depositEntryConfiguration.Property(d => d.Description).IsRequired(false);

            depositEntryConfiguration.Property(d => d.VerifiedByAdminId).IsRequired(false);
            depositEntryConfiguration.Property(d => d.VerifiedByAdminName).IsRequired(false);

            //depositEntryConfiguration.Property(d => d.DateCreated).IsRequired();
            depositEntryConfiguration.Property(d => d.DateFinished).IsRequired(false);
            depositEntryConfiguration
                .HasIndex(d => new { d.DateCreated, d.UserId })
                .IsUnique(false);


        }
    }
}
