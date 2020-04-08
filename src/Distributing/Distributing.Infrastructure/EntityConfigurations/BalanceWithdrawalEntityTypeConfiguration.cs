using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Withdrawals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class BalanceWithdrawalEntityTypeConfiguration
     : IEntityTypeConfiguration<BalanceWithdrawal>
    {
        public void Configure(EntityTypeBuilder<BalanceWithdrawal> balanceWithdrawalConfiguration)
        {
            balanceWithdrawalConfiguration.ToTable("balanceWithdrawals", DistributingContext.DEFAULT_SCHEMA);

            balanceWithdrawalConfiguration.HasKey(b => b.Id);

            balanceWithdrawalConfiguration.Ignore(b => b.DomainEvents);

            balanceWithdrawalConfiguration.Property(b => b.Id)
                .UseHiLo("balancewithdrawalseq");

            balanceWithdrawalConfiguration
                .Property<int>("WithdrawalId")
                .IsRequired();

            balanceWithdrawalConfiguration
                .Property<bool>("_isSuccess")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("IsSuccess")
                .IsRequired();


            balanceWithdrawalConfiguration
                .Property<bool>("_isFailed")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("IsFailed")
                .IsRequired();

            balanceWithdrawalConfiguration
                .Property<decimal>("_amount")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,3)")
                .IsRequired();

            balanceWithdrawalConfiguration
                .Property<DateTime>("_dateWithdraw")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("DateWithdraw")
                .IsRequired();



            balanceWithdrawalConfiguration.HasOne<Withdrawal>()
                .WithMany()
                .IsRequired()
                .HasForeignKey("WithdrawalId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
