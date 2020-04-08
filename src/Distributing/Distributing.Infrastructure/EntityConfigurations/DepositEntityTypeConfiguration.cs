using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Deposits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class DepositEntityTypeConfiguration : IEntityTypeConfiguration<Deposit>
    {
        public void Configure(EntityTypeBuilder<Deposit> depositConfiguration)
        {
            depositConfiguration.ToTable("deposits", DistributingContext.DEFAULT_SCHEMA);

            depositConfiguration.HasKey(d => d.Id);

            depositConfiguration.Ignore(d => d.DomainEvents);

            depositConfiguration.Property(d => d.Id)
                .UseHiLo("depositseq", DistributingContext.DEFAULT_SCHEMA);

            //BankAccount value object persisted as owned entity type supported since EF Core 2.0


            /*depositConfiguration.Property(d => d.DepositId)
                .HasMaxLength(200)
                .IsRequired();

            depositConfiguration.HasIndex("DepositId")
              .IsUnique(true);*/

            depositConfiguration
                .Property<int>("_depositStatusId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("DepositStatus")
                .IsRequired();

            depositConfiguration
                .Property<int>("_depositTypeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("DepositType")
                .IsRequired();

            depositConfiguration
                .Property<int>("BalanceId")
                .HasColumnName("BalanceId")
                .IsRequired();

            depositConfiguration.Property(d => d.UserId).IsRequired().IsUnicode(false);
            depositConfiguration.Property(d => d.CreateByUplineId).IsRequired(false).IsUnicode(false);

            depositConfiguration
                .OwnsOne(d => d.BankAccount, b =>
                {
                    b.WithOwner();
                });

            depositConfiguration.Property(d => d.TotalAmount).HasColumnType("decimal(18,3)").IsRequired();

            //Only need precision of 4 and scale of 3. (x.xxx)
            depositConfiguration.Property(d => d.CommissionRate).HasColumnType("decimal(4,3)").IsRequired();

            depositConfiguration.Property(d => d.CommissionAmount).HasColumnType("decimal(18,3)").IsRequired();
            depositConfiguration.Property(d => d.ActualAmount).HasColumnType("decimal(18,3)").IsRequired();

            depositConfiguration
                .OwnsOne(d => d.BalanceRecord, b =>
                {
                    b.WithOwner();
                    b.Property(r => r.BalanceBefore).HasColumnType("decimal(18,3)").IsRequired();
                    b.Property(r => r.BalanceAfter).HasColumnType("decimal(18,3)").IsRequired(false);
                });

            depositConfiguration
                .OwnsOne(d => d.VerifiedBy, b =>
                {
                    b.WithOwner();
                });

            depositConfiguration.Property(d => d.Description).IsRequired(false);

            depositConfiguration.Property(d => d.DateCreated).IsRequired();
            depositConfiguration.Property(d => d.DateFinished).IsRequired(false);



            depositConfiguration.HasOne<Balance>()
                .WithMany()
                .IsRequired()
                .HasForeignKey("BalanceId")
                .OnDelete(DeleteBehavior.Cascade);

            depositConfiguration.HasOne(w => w.DepositStatus)
                .WithMany()
                .HasForeignKey("_depositStatusId");
            depositConfiguration.HasOne(w => w.DepositType)
                .WithMany()
                .HasForeignKey("_depositTypeId");
        }
    }
}
