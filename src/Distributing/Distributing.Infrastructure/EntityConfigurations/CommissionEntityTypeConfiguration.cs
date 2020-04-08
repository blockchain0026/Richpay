using Distributing.Domain.Model.Commissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class CommissionEntityTypeConfiguration : IEntityTypeConfiguration<Commission>
    {
        public void Configure(EntityTypeBuilder<Commission> commissionConfiguration)
        {
            commissionConfiguration.ToTable("commissions", DistributingContext.DEFAULT_SCHEMA);

            commissionConfiguration.HasKey(c => c.Id);

            commissionConfiguration.Ignore(c => c.DomainEvents);

            commissionConfiguration.Property(c => c.Id)
                .UseHiLo("commissionseq", DistributingContext.DEFAULT_SCHEMA);


            /*commissionConfiguration.Property(c => c.CommissionId)
                .HasMaxLength(200)
                .IsRequired();

            commissionConfiguration.HasIndex("CommissionId")
              .IsUnique(true);*/


            //commissionConfiguration.Property(c => c.UserId).IsRequired();
            commissionConfiguration
                .HasIndex(r => r.UserId)
                .IsUnique(false);
            commissionConfiguration.Property(c => c.BalanceId).IsRequired();

            commissionConfiguration
                .Property<int>("_userTypeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("UserTypeId")
                .IsRequired();

            commissionConfiguration.Property(c => c.UplineCommissionId).IsRequired(false);

            //Only need precision of 4 and scale of 3. (x.xxx)
            commissionConfiguration.Property(c => c.RateAlipay).HasColumnType("decimal(4,3)").IsRequired();
            commissionConfiguration.Property(c => c.RateWechat).HasColumnType("decimal(4,3)").IsRequired();
            commissionConfiguration.Property(c => c.RateRebateAlipay).HasColumnType("decimal(4,3)").IsRequired();
            commissionConfiguration.Property(c => c.RateRebateWechat).HasColumnType("decimal(4,3)").IsRequired();

            commissionConfiguration.Property(c => c.IsEnabled).IsRequired();

            commissionConfiguration.HasOne<Commission>()
                .WithMany()
                .IsRequired(false)
                .HasForeignKey("UplineCommissionId")
                .OnDelete(DeleteBehavior.ClientSetNull);

            commissionConfiguration.HasOne(c => c.UserType)
                .WithMany()
                .HasForeignKey("_userTypeId");
        }
    }
}
