using Distributing.Domain.Model.Balances;
using Distributing.Domain.Model.Transfers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class TransferEntityTypeConfiguration : IEntityTypeConfiguration<Transfer>
    {
        public void Configure(EntityTypeBuilder<Transfer> transferConfiguration)
        {
            transferConfiguration.ToTable("transfers", DistributingContext.DEFAULT_SCHEMA);

            transferConfiguration.HasKey(t => t.Id);

            transferConfiguration.Ignore(t => t.DomainEvents);

            transferConfiguration.Property(t => t.Id)
                .UseHiLo("transferseq", DistributingContext.DEFAULT_SCHEMA);



            transferConfiguration
                .Property<int>("_initiatedById")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("InitiatedBy")
                .IsRequired();


            transferConfiguration.Property(t => t.FromBalanceId).IsRequired();
            transferConfiguration.Property(t => t.ToBalanceId).IsRequired();

            transferConfiguration.Property(t => t.Amount).HasColumnType("decimal(18,3)").IsRequired();


            transferConfiguration.Property(t => t.DateTransferred).IsRequired();

            transferConfiguration.HasOne<Balance>()
                .WithMany()
                .IsRequired()
                .HasForeignKey(t => t.FromBalanceId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            transferConfiguration.HasOne<Balance>()
                .WithMany()
                .IsRequired()
                .HasForeignKey(t => t.ToBalanceId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            transferConfiguration.HasOne(t => t.InitiatedBy)
                .WithMany()
                .HasForeignKey("_initiatedById");
        }
    }

}
