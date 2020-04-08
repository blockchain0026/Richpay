using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pairing.Domain.Model.FourthPartyGateways;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Infrastructure.EntityConfigurations
{
    class FourthPartyGatewayEntityTypeConfiguration : IEntityTypeConfiguration<FourthPartyGateway>
    {
        public void Configure(EntityTypeBuilder<FourthPartyGateway> fourthPartyGatewayConfiguration)
        {
            fourthPartyGatewayConfiguration.ToTable("fourthPartyGateways", PairingContext.DEFAULT_SCHEMA);

            fourthPartyGatewayConfiguration.HasKey(f => f.Id);

            fourthPartyGatewayConfiguration.Ignore(f => f.DomainEvents);

            fourthPartyGatewayConfiguration.Property(f => f.Id)
                .UseHiLo("fourthpartygatewayseq", PairingContext.DEFAULT_SCHEMA);


            fourthPartyGatewayConfiguration.Property(f => f.UserId).IsRequired();

            fourthPartyGatewayConfiguration
                .Property<int>("_paymentChannelId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PaymentChannelId")
                .IsRequired();

            fourthPartyGatewayConfiguration
                .Property<int>("_paymentSchemeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PaymentSchemeId")
                .IsRequired();


            fourthPartyGatewayConfiguration.Property(f => f.Name).IsRequired();

            fourthPartyGatewayConfiguration.Property(f => f.OpenFrom).IsRequired();
            fourthPartyGatewayConfiguration.Property(f => f.OpenTo).IsRequired();

            fourthPartyGatewayConfiguration.Property(f => f.IsEnabled).IsRequired();


            fourthPartyGatewayConfiguration.HasOne<PaymentChannel>()
                .WithMany()
                .HasForeignKey("_paymentChannelId");
            fourthPartyGatewayConfiguration.HasOne<PaymentScheme>()
                .WithMany()
                .HasForeignKey("_paymentSchemeId");

        }
    }
}
