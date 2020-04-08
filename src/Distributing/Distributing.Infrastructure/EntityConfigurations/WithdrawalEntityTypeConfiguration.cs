using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Withdrawals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class WithdrawalEntityTypeConfiguration : IEntityTypeConfiguration<Withdrawal>
    {
        public void Configure(EntityTypeBuilder<Withdrawal> withdrawalConfiguration)
        {
            withdrawalConfiguration.ToTable("withdrawals", DistributingContext.DEFAULT_SCHEMA);

            withdrawalConfiguration.HasKey(d => d.Id);

            withdrawalConfiguration.Ignore(d => d.DomainEvents);

            withdrawalConfiguration.Property(d => d.Id)
                .UseHiLo("withdrawalseq", DistributingContext.DEFAULT_SCHEMA);

            //BankAccount value object persisted as owned entity type supported since EF Core 2.0


            /*withdrawalConfiguration.Property(d => d.WithdrawalId)
                .HasMaxLength(200)
                .IsRequired();

            withdrawalConfiguration.HasIndex("WithdrawalId")
              .IsUnique(true);*/

            withdrawalConfiguration
                .Property<int>("_withdrawalStatusId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("WithdrawalStatus")
                .IsRequired();

            withdrawalConfiguration
                .Property<int>("_withdrawalTypeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("WithdrawalType")
                .IsRequired();

            withdrawalConfiguration
                .Property<int>("BalanceId")
                .HasColumnName("BalanceId")
                .IsRequired();

            withdrawalConfiguration.Property(d => d.UserId).IsRequired();

            withdrawalConfiguration
                .OwnsOne(d => d.BankAccount, b =>
                {
                    b.WithOwner();
                });

            withdrawalConfiguration.Property(d => d.TotalAmount).HasColumnType("decimal(18,3)").IsRequired();

            //Only need precision of 4 and scale of 3. (x.xxx)
            withdrawalConfiguration.Property(d => d.CommissionRate).HasColumnType("decimal(4,3)").IsRequired();

            withdrawalConfiguration.Property(d => d.CommissionAmount).HasColumnType("decimal(18,3)").IsRequired();
            withdrawalConfiguration.Property(d => d.ActualAmount).HasColumnType("decimal(18,3)").IsRequired();

            /*withdrawalConfiguration
                .OwnsOne(d => d.BalanceRecord, b =>
                {
                    b.WithOwner();
                    b.Property(r => r.BalanceBefore).HasColumnType("decimal(18,3)").IsRequired();
                    b.Property(r => r.BalanceAfter).HasColumnType("decimal(18,3)").IsRequired(false);
                });*/

            withdrawalConfiguration
                .OwnsOne(d => d.ApprovedBy, b =>
                {
                    b.WithOwner();
                });
            withdrawalConfiguration
                .OwnsOne(d => d.CancellationApprovedBy, b =>
                {
                    b.WithOwner();
                });

            withdrawalConfiguration.Property(d => d.Description).IsRequired(false);

            withdrawalConfiguration.Property(d => d.DateCreated).IsRequired();
            withdrawalConfiguration.Property(d => d.DateFinished).IsRequired(false);



            withdrawalConfiguration.HasOne<Balance>()
                .WithMany()
                .IsRequired()
                .HasForeignKey("BalanceId")
                .OnDelete(DeleteBehavior.Cascade);

            withdrawalConfiguration.HasOne(w => w.WithdrawalStatus)
                .WithMany()
                .HasForeignKey("_withdrawalStatusId");
            withdrawalConfiguration.HasOne(w => w.WithdrawalType)
                .WithMany()
                .HasForeignKey("_withdrawalTypeId");
        }
    }
}
