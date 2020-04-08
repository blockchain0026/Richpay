using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class ShopGatewayEntryEntityTypeConfiguration : IEntityTypeConfiguration<ShopGatewayEntry>
    {
        public void Configure(EntityTypeBuilder<ShopGatewayEntry> shopGatewayEntryConfiguration)
        {
            shopGatewayEntryConfiguration.ToTable("shopGatewayEntrys");

            shopGatewayEntryConfiguration.HasKey(w => w.ShopGatewayId);
            shopGatewayEntryConfiguration.Property(w => w.ShopGatewayId)
                .ValueGeneratedNever()
                .IsRequired();


            shopGatewayEntryConfiguration.Property(w => w.ShopId).IsRequired();

            shopGatewayEntryConfiguration.Property(w => w.ShopGatewayType).IsRequired();
            shopGatewayEntryConfiguration.Property(w => w.PaymentChannel).IsRequired();
            shopGatewayEntryConfiguration.Property(w => w.PaymentScheme).IsRequired();

            shopGatewayEntryConfiguration.Property(w => w.FourthPartyGatewayId).IsRequired(false);



            shopGatewayEntryConfiguration
                       .OwnsOne(w => w.AlipayPreferenceInfo, a =>
                       {
                           a.WithOwner();
                           a.Property(w => w.SecondsBeforePayment).IsRequired();
                           a.Property(w => w.IsAmountUnchangeable).IsRequired();
                           a.Property(w => w.IsAccountUnchangeable).IsRequired();
                           a.Property(w => w.IsH5RedirectByScanEnabled).IsRequired();
                           a.Property(w => w.IsH5RedirectByClickEnabled).IsRequired();
                           a.Property(w => w.IsH5RedirectByPickingPhotoEnabled).IsRequired();
                       });


            shopGatewayEntryConfiguration.Property(w => w.DateCreated).IsRequired();
        }
    }
}
