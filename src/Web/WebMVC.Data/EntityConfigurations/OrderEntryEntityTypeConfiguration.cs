using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class OrderEntryEntityTypeConfiguration : IEntityTypeConfiguration<OrderEntry>
    {
        public void Configure(EntityTypeBuilder<OrderEntry> orderEntryConfiguration)
        {
            orderEntryConfiguration.ToTable("orderEntrys");

            orderEntryConfiguration.HasKey(w => w.OrderId);
            orderEntryConfiguration.Property(w => w.OrderId)
                .ValueGeneratedNever()
                .IsRequired();

            orderEntryConfiguration.Property(w => w.TrackingNumber).IsRequired().IsUnicode(false);
            orderEntryConfiguration.Property(w => w.OrderType).IsRequired().IsUnicode(false);
            orderEntryConfiguration.Property(w => w.OrderStatus).IsRequired().IsUnicode(false);

            orderEntryConfiguration.Property(w => w.OrderStatusDescription).IsRequired(false);
            orderEntryConfiguration.Property(w => w.IsTestOrder).IsRequired();
            orderEntryConfiguration.Property(w => w.IsExpired).IsRequired();


            orderEntryConfiguration.Property(w => w.ShopId).IsRequired().IsUnicode(false);
            orderEntryConfiguration.Property(w => w.ShopUserName).IsRequired().IsUnicode(false);
            orderEntryConfiguration.Property(w => w.ShopFullName).IsRequired().IsUnicode(false);
            orderEntryConfiguration.Property(w => w.ShopOrderId).IsRequired().IsUnicode(false);
            orderEntryConfiguration.Property(w => w.RateRebateShop).IsRequired();
            orderEntryConfiguration.Property(w => w.RateRebateFinal).IsRequired();
            orderEntryConfiguration.Property(w => w.ToppestTradingRate).HasColumnType("decimal(4,3)").IsRequired(false);


            orderEntryConfiguration.Property(w => w.OrderPaymentChannel).IsRequired().IsUnicode(false);
            orderEntryConfiguration.Property(w => w.OrderPaymentScheme).IsRequired().IsUnicode(false);

            orderEntryConfiguration.Property(w => w.IpAddress).IsRequired(false).IsUnicode(false);
            orderEntryConfiguration.Property(w => w.Device).IsRequired(false).IsUnicode(false);
            orderEntryConfiguration.Property(w => w.Location).IsRequired(false).IsUnicode(false);

            orderEntryConfiguration.Property(w => w.TraderId).IsRequired(false).IsUnicode(false);
            orderEntryConfiguration.Property(w => w.TraderUserName).IsRequired(false).IsUnicode(false);
            orderEntryConfiguration.Property(w => w.TraderFullName).IsRequired(false).IsUnicode(false);
            orderEntryConfiguration.Property(w => w.QrCodeId).IsRequired(false);
            orderEntryConfiguration.Property(w => w.FourthPartyId).IsRequired(false).IsUnicode(false);
            orderEntryConfiguration.Property(w => w.FourthPartyName).IsRequired(false).IsUnicode(false);

            orderEntryConfiguration.Property(w => w.Amount).HasColumnType("decimal(18,3)").IsRequired();
            orderEntryConfiguration.Property(w => w.AmountPaid).HasColumnType("decimal(18,3)").IsRequired(false);
            orderEntryConfiguration.Property(w => w.ShopUserCommissionRealized).HasColumnType("decimal(18,3)").IsRequired(false);
            orderEntryConfiguration.Property(w => w.TradingUserCommissionRealized).HasColumnType("decimal(18,3)").IsRequired(false);
            orderEntryConfiguration.Property(w => w.PlatformCommissionRealized).HasColumnType("decimal(18,3)").IsRequired(false);
            orderEntryConfiguration.Property(w => w.TraderCommissionRealized).HasColumnType("decimal(18,3)").IsRequired(false);
            orderEntryConfiguration.Property(w => w.ShopCommissionRealized).HasColumnType("decimal(18,3)").IsRequired(false);

            //orderEntryConfiguration.Property(w => w.DateCreated).IsRequired();

            //To speed up queries.
            orderEntryConfiguration
                //.HasIndex(r => r.DateCreated)
                .HasIndex(o => new { o.DateCreated, o.TraderId, o.ShopId })
                .IsUnique(false);

            orderEntryConfiguration.Property(w => w.DatePaired).IsRequired(false).IsUnicode(false);
            orderEntryConfiguration.Property(w => w.DatePaymentRecieved).IsRequired(false).IsUnicode(false);

        }
    }
}
