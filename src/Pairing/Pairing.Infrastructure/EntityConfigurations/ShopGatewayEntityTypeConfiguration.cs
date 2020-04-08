using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pairing.Domain.Model.FourthParties;
using Pairing.Domain.Model.FourthPartyGateways;
using Pairing.Domain.Model.QrCodes;
using Pairing.Domain.Model.ShopGateways;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Infrastructure.EntityConfigurations
{
    class ShopGatewayEntityTypeConfiguration : IEntityTypeConfiguration<ShopGateway>
    {
        public void Configure(EntityTypeBuilder<ShopGateway> shopGatewayConfiguration)
        {
            shopGatewayConfiguration.ToTable("shopGateways", PairingContext.DEFAULT_SCHEMA);

            shopGatewayConfiguration.HasKey(s => s.Id);

            shopGatewayConfiguration.Ignore(s => s.DomainEvents);

            shopGatewayConfiguration.Property(s => s.Id)
                .UseHiLo("shopgatewayseq", PairingContext.DEFAULT_SCHEMA);


            shopGatewayConfiguration.Property(s => s.ShopId).IsRequired();

            shopGatewayConfiguration
                .Property<int>("_shopGatewayTypeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("ShopGatewayTypeId")
                .IsRequired();

            shopGatewayConfiguration
                .Property<int>("_paymentChannelId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PaymentChannelId")
                .IsRequired();

            shopGatewayConfiguration
                .Property<int>("_paymentSchemeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PaymentSchemeId")
                .IsRequired();


            shopGatewayConfiguration.Property(s => s.FourthPartyGatewayId).IsRequired(false);

            shopGatewayConfiguration
                .OwnsOne(s => s.AlipayPreference, a =>
                {
                    a.WithOwner();

                    a.Property(w => w.SecondsBeforePayment).IsRequired();
                    a.Property(w => w.IsAmountUnchangeable).IsRequired();
                    a.Property(w => w.IsAccountUnchangeable).IsRequired();
                    a.Property(w => w.IsH5RedirectByScanEnabled).IsRequired();
                    a.Property(w => w.IsH5RedirectByClickEnabled).IsRequired();
                    a.Property(w => w.IsH5RedirectByPickingPhotoEnabled).IsRequired();
                });

            shopGatewayConfiguration.Property(s => s.DateCreated).IsRequired();


            shopGatewayConfiguration.HasOne<ShopGatewayType>()
                .WithMany()
                .HasForeignKey("_shopGatewayTypeId");
            shopGatewayConfiguration.HasOne<PaymentChannel>()
                .WithMany()
                .HasForeignKey("_paymentChannelId");
            shopGatewayConfiguration.HasOne<PaymentScheme>()
                .WithMany()
                .HasForeignKey("_paymentSchemeId");


            shopGatewayConfiguration.HasOne<FourthPartyGateway>()
                .WithMany()
                .IsRequired(false)
                .HasForeignKey("FourthPartyGatewayId")
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
