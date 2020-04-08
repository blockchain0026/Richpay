using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Commissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class BalanceEntityTypeConfiguration : IEntityTypeConfiguration<Balance>
    {
        public void Configure(EntityTypeBuilder<Balance> balanceConfiguration)
        {
            balanceConfiguration.ToTable("balances", DistributingContext.DEFAULT_SCHEMA);

            balanceConfiguration.HasKey(b => b.Id);

            balanceConfiguration.Ignore(b => b.DomainEvents);

            balanceConfiguration.Property(b => b.Id)
                .UseHiLo("balanceseq", DistributingContext.DEFAULT_SCHEMA);

            balanceConfiguration
                .OwnsOne(b => b.WithdrawalLimit, a =>
                {
                    a.WithOwner();
                    a.Property(w => w.DailyAmountLimit).HasColumnType("decimal(18,0)").IsRequired();
                    a.Property(w => w.DailyFrequencyLimit).IsRequired();
                    a.Property(w => w.EachAmountUpperLimit).HasColumnType("decimal(18,0)").IsRequired();
                    a.Property(w => w.EachAmountLowerLimit).HasColumnType("decimal(18,0)").IsRequired();

                });

            /*balanceConfiguration.Property(b => b.BalanceId)
                .HasMaxLength(200)
                .IsRequired();

            balanceConfiguration.HasIndex("BalanceId")
              .IsUnique(true);*/

            balanceConfiguration.Property(b => b.UserId).IsRequired();

            //Set as a concurrency token to prevent concurrencies and conficts.
            balanceConfiguration.Property(b => b.AmountAvailable)
                .HasColumnType("decimal(18,3)")
                .IsRequired();
                //.IsConcurrencyToken();


            //Only need precision of 4 and scale of 3. (x.xxx)
            balanceConfiguration.Property(b => b.WithdrawalCommissionRate)
                .HasColumnType("decimal(4,3)")
                .IsRequired();
            balanceConfiguration.Property(b => b.DepositCommissionRate).HasColumnType("decimal(4,3)").IsRequired();

            balanceConfiguration
                .Property<int>("_userTypeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("UserTypeId")
                .IsRequired();

            balanceConfiguration
                .Property(b => b.RowVersion)
                .IsRowVersion();

            var navigation = balanceConfiguration.Metadata.FindNavigation(nameof(Balance.BalanceWithdrawals));

            // DDD Patterns comment:
            //Set as field (New since EF 1.1) to access the OrderItem collection property through its field
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            balanceConfiguration.HasOne<UserType>()
                .WithMany()
                .HasForeignKey("_userTypeId");
        }
    }
}
