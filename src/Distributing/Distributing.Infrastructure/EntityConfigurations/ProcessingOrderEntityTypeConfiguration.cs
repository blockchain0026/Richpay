using Distributing.Domain.Model.Commissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class ProcessingOrderEntityTypeConfiguration
        : IEntityTypeConfiguration<ProcessingOrder>
    {
        public void Configure(EntityTypeBuilder<ProcessingOrder> processingOrderConfiguration)
        {
            processingOrderConfiguration.ToTable("processingOrders", DistributingContext.DEFAULT_SCHEMA);

            processingOrderConfiguration.HasKey(b => b.Id);

            processingOrderConfiguration.Ignore(b => b.DomainEvents);

            processingOrderConfiguration.Property(b => b.Id)
                .UseHiLo("processingorderseq");

            processingOrderConfiguration
                .Property<string>("TrackingNumber")
                .IsRequired();

            processingOrderConfiguration
                .Property<decimal>("_amount")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,0)")
                .IsRequired();
            
            processingOrderConfiguration
                .Property<decimal>("_commissionAmount")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("CommissionAmount")
                .HasColumnType("decimal(18,3)")
                .IsRequired();
        }
    }
}
