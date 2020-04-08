using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class WithdrawalEntryEntityTypeConfiguration : IEntityTypeConfiguration<WithdrawalEntry>
    {
        public void Configure(EntityTypeBuilder<WithdrawalEntry> withdrawalEntryConfiguration)
        {
            withdrawalEntryConfiguration.ToTable("withdrawalEntrys");

            withdrawalEntryConfiguration.HasKey(w => w.WithdrawalId);
            withdrawalEntryConfiguration.Property(w => w.WithdrawalId)
                .ValueGeneratedNever()
                .IsRequired();


            withdrawalEntryConfiguration.Property(w => w.WithdrawalStatus).IsRequired();

            withdrawalEntryConfiguration.Property(w => w.BalanceId).IsRequired();
            withdrawalEntryConfiguration.Property(w => w.UserId).IsRequired().IsUnicode(false);
            withdrawalEntryConfiguration.Property(w => w.Username).IsRequired();
            withdrawalEntryConfiguration.Property(w => w.FullName).IsRequired();
            withdrawalEntryConfiguration.Property(w => w.UserType).IsRequired();

            withdrawalEntryConfiguration
                       .OwnsOne(w => w.WithdrawalBankOption, a =>
                       {
                           a.WithOwner();
                           a.Property(w => w.WithdrawalBankId).IsRequired();
                           a.Property(w => w.BankName).IsRequired();
                           a.Property(w => w.DateCreated).IsRequired(false);
                       });


            withdrawalEntryConfiguration.Property(w => w.AccountName).IsRequired();
            withdrawalEntryConfiguration.Property(w => w.AccountNumber).IsRequired();


            withdrawalEntryConfiguration.Property(w => w.TotalAmount).IsRequired();
            withdrawalEntryConfiguration.Property(w => w.CommissionRateInThousandth).IsRequired();
            withdrawalEntryConfiguration.Property(w => w.CommissionAmount).HasColumnType("decimal(18,3)").IsRequired();
            withdrawalEntryConfiguration.Property(w => w.ActualAmount).HasColumnType("decimal(18,3)").IsRequired();


            withdrawalEntryConfiguration.Property(w => w.ApprovedByAdminId).IsRequired(false);
            withdrawalEntryConfiguration.Property(w => w.ApprovedByAdminName).IsRequired(false);
            withdrawalEntryConfiguration.Property(w => w.CancellationApprovedByAdminId).IsRequired(false);
            withdrawalEntryConfiguration.Property(w => w.CancellationApprovedByAdminName).IsRequired(false);


            withdrawalEntryConfiguration.Property(w => w.Description).IsRequired(false);

            //withdrawalEntryConfiguration.Property(w => w.DateCreated).IsRequired();
            withdrawalEntryConfiguration.Property(w => w.DateFinished).IsRequired(false);
            withdrawalEntryConfiguration
                .HasIndex(w => new { w.DateCreated, w.UserId })
                .IsUnique(false);
        }
    }
}
