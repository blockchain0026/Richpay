using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Frozens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class FrozenEntityTypeConfiguration : IEntityTypeConfiguration<Frozen>
    {
        public void Configure(EntityTypeBuilder<Frozen> frozenConfiguration)
        {
            frozenConfiguration.ToTable("frozens", DistributingContext.DEFAULT_SCHEMA);

            frozenConfiguration.HasKey(f => f.Id);

            frozenConfiguration.Ignore(f => f.DomainEvents);

            frozenConfiguration.Property(f => f.Id)
                .UseHiLo("frozenseq", DistributingContext.DEFAULT_SCHEMA);



            frozenConfiguration
                .Property<int>("_frozenStatusId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("FrozenStatusId")
                .IsRequired();

            frozenConfiguration
                .Property<int>("_frozenTypeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("FrozenTypeId")
                .IsRequired();

            frozenConfiguration.Property(f => f.BalanceId).IsRequired();
            frozenConfiguration.Property(f => f.UserId).IsRequired();
            frozenConfiguration
                .OwnsOne(f => f.BalanceFrozenRecord, b =>
                {
                    b.WithOwner();
                    b.Property(r => r.BalanceBefore).HasColumnType("decimal(18,3)").IsRequired();
                    b.Property(r => r.BalanceAfter).HasColumnType("decimal(18,3)").IsRequired(false);
                });
            
            frozenConfiguration
                .OwnsOne(f => f.BalanceUnfrozenRecord, b =>
                {
                    b.WithOwner();
                    b.Property(r => r.BalanceBefore).HasColumnType("decimal(18,3)").IsRequired();
                    b.Property(r => r.BalanceAfter).HasColumnType("decimal(18,3)").IsRequired(false);
                });

            frozenConfiguration.Property(f => f.Amount).HasColumnType("decimal(18,3)").IsRequired();
            
            frozenConfiguration.Property(f => f.OrderTrackingNumber).IsRequired(false);
            frozenConfiguration.Property(f => f.WithdrawalId).IsRequired(false);
            frozenConfiguration
                .OwnsOne(f => f.ByAdmin, b =>
                {
                    b.WithOwner();
                });


            frozenConfiguration.Property(f => f.Description).IsRequired(false);

            frozenConfiguration.Property(f => f.DateFroze).IsRequired();
            frozenConfiguration.Property(f => f.DateUnfroze).IsRequired(false);

            frozenConfiguration.HasOne<Balance>()
                .WithMany()
                .IsRequired(false)
                .HasForeignKey(f => f.BalanceId)
                .OnDelete(DeleteBehavior.Cascade);

            frozenConfiguration.HasOne(f => f.FrozenStatus)
                .WithMany()
                .HasForeignKey("_frozenStatusId");
            frozenConfiguration.HasOne(f => f.FrozenType)
                .WithMany()
                .HasForeignKey("_frozenTypeId");
        }
    }
}
