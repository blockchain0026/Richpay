using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Commissions;
using Distributing.Domain.Model.Distributions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class DistributionEntityTypeConfiguration : IEntityTypeConfiguration<Distribution>
    {
        public void Configure(EntityTypeBuilder<Distribution> distributionConfiguration)
        {
            distributionConfiguration.ToTable("distributions", DistributingContext.DEFAULT_SCHEMA);

            distributionConfiguration.HasKey(d => d.Id);

            distributionConfiguration.Ignore(d => d.DomainEvents);

            distributionConfiguration.Property(d => d.Id)
                .UseHiLo("distributionseq", DistributingContext.DEFAULT_SCHEMA);

            //BankAccount value object persisted as owned entity type supported since EF Core 2.0


            /*distributionConfiguration.Property(d => d.DistributionId)
                .HasMaxLength(200)
                .IsRequired();

            distributionConfiguration.HasIndex("DistributionId")
              .IsUnique(true);*/

            distributionConfiguration
                .Property<int>("_distributionTypeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("DistributionTypeId")
                .IsRequired();

            distributionConfiguration.Property<int>("CommissionId").IsRequired();
            //distributionConfiguration.Property(d => d.UserId).IsRequired();

            //To speed up queries.
            distributionConfiguration
                .HasIndex(r => r.UserId)
                .IsUnique(false);

            distributionConfiguration
                .OwnsOne(d => d.Order, b =>
                {
                    b.WithOwner();
                    b.Property(o => o.TrackingNumber).IsRequired();
                    b.Property(o => o.ShopOrderId).IsRequired();
                    b.Property(o => o.Amount).HasColumnType("decimal(18,0)").IsRequired();
                    b.Property(o => o.CommissionAmount).HasColumnType("decimal(18,3)").IsRequired();
                    b.Property(o => o.ShopId).IsRequired();
                    b.Property(o => o.TraderId).IsRequired();
                    b.Property(o => o.DateCreated).IsRequired();
                });

            distributionConfiguration.Property<int>("BalanceId").IsRequired();


            distributionConfiguration.Property(d => d.DistributedAmount).HasColumnType("decimal(18,3)").IsRequired();

            distributionConfiguration.Property(d => d.DateDistributed).IsRequired(false);
            distributionConfiguration.Property(d => d.IsFinished).IsRequired();



            /*distributionConfiguration.HasOne<Commission>()
                .WithMany()
                .IsRequired()
                .HasForeignKey("CommissionId")
                .OnDelete(DeleteBehavior.Restrict);*/
            
            distributionConfiguration.HasOne<Commission>()
                .WithMany()
                .IsRequired()
                .HasForeignKey("CommissionId")
                .OnDelete(DeleteBehavior.Cascade);

            //Comment out for concurrency reason.
            /*distributionConfiguration.HasOne<Balance>()
                .WithMany()
                .IsRequired()
                .HasForeignKey("BalanceId")
                .OnDelete(DeleteBehavior.ClientSetNull);*/

            distributionConfiguration.HasOne(d => d.DistributionType)
                .WithMany()
                .HasForeignKey("_distributionTypeId");
        }
    }
}
