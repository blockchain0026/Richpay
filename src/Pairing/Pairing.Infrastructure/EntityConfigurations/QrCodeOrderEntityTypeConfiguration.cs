using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Infrastructure.EntityConfigurations
{
    class QrCodeOrderEntityTypeConfiguration
        : IEntityTypeConfiguration<QrCodeOrder>
    {
        public void Configure(EntityTypeBuilder<QrCodeOrder> qrCodeOrderConfiguration)
        {
            qrCodeOrderConfiguration.ToTable("qrCodeOrders", PairingContext.DEFAULT_SCHEMA);

            qrCodeOrderConfiguration.HasKey(b => b.Id);

            qrCodeOrderConfiguration.Ignore(b => b.DomainEvents);

            qrCodeOrderConfiguration.Property(b => b.Id)
                .UseHiLo("qrcodeorderseq");

            qrCodeOrderConfiguration
                .Property<string>("OrderTrackingNumber")
                .IsRequired();


            qrCodeOrderConfiguration
                .Property<decimal>("_amount")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,3)")
                .IsRequired();

            qrCodeOrderConfiguration
                .Property<bool>("_isSuccess")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("IsSuccess")
                .IsRequired();


            qrCodeOrderConfiguration
                .Property<bool>("_isFailed")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("IsFailed")
                .IsRequired();


            qrCodeOrderConfiguration
                .Property<DateTime>("_dateCreated")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("DateCreated")
                .IsRequired();

            qrCodeOrderConfiguration
                .Property<DateTime?>("_dateFinished")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("DateFinished")
                .IsRequired(false);

        }
    }
}
