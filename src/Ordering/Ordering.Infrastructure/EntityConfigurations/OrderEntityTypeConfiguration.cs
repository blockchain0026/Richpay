using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Model.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Infrastructure.EntityConfigurations
{
    class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> orderConfiguration)
        {
            orderConfiguration.ToTable("orders", OrderingContext.DEFAULT_SCHEMA);

            orderConfiguration.HasKey(o => o.Id);

            orderConfiguration.Ignore(o => o.DomainEvents);

            orderConfiguration.Property(o => o.Id)
                .UseHiLo("orderseq", OrderingContext.DEFAULT_SCHEMA);


            orderConfiguration
                .Property<int>("_orderTypeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("OrderType")
                .IsRequired();

            orderConfiguration
                .Property<int>("_orderStatusId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("OrderStatus")
                .IsRequired();


            orderConfiguration.Property(o => o.OrderStatusDescription).IsRequired(false);
            orderConfiguration.Property(o => o.ExpirationTimeInSeconds).IsRequired(false);

            orderConfiguration.Property(o => o.IsTestOrder).IsRequired();
            orderConfiguration.Property(o => o.IsExpired).IsRequired();

            orderConfiguration.Property(o => o.ExpirationTimeInSeconds).IsRequired();


            orderConfiguration
                .OwnsOne(o => o.ShopInfo, a =>
                  {
                      a.WithOwner();
                      a.Property(w => w.ShopId).IsRequired();
                      a.Property(w => w.ShopOrderId).IsRequired();
                      a.Property(w => w.ShopReturnUrl).IsRequired();
                      a.Property(w => w.ShopOkReturnUrl).IsRequired();

                      a.Property(w => w.RateRebateShop).HasColumnType("decimal(18,3)").IsRequired();
                      a.Property(w => w.RateRebateFinal).HasColumnType("decimal(18,3)").IsRequired();
                  });

            orderConfiguration
                .Property<int>("_orderPaymentChannelId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("OrderPaymentChannel")
                .IsRequired();

            orderConfiguration
                .Property<int>("_orderPaymentSchemeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("OrderPaymentScheme")
                .IsRequired();


            orderConfiguration
                .OwnsOne(o => o.AlipayPagePreference, a =>
                {
                    a.WithOwner();
                    a.Property(w => w.SecondsBeforePayment).IsRequired();
                    a.Property(w => w.IsAmountUnchangeable).IsRequired();
                    a.Property(w => w.IsAccountUnchangeable).IsRequired();
                    a.Property(w => w.IsH5RedirectByScanEnabled).IsRequired();
                    a.Property(w => w.IsH5RedirectByClickEnabled).IsRequired();
                    a.Property(w => w.IsH5RedirectByPickingPhotoEnabled).IsRequired();
                });


            orderConfiguration
                .OwnsOne(o => o.PayerInfo, a =>
                {
                    a.WithOwner();
                    a.Property(w => w.IpAddress).IsRequired();
                    a.Property(w => w.Device).IsRequired(false);
                    a.Property(w => w.Location).IsRequired(false);
                });

            orderConfiguration
                .OwnsOne(o => o.PayeeInfo, a =>
                {
                    a.WithOwner();
                    a.Property(w => w.TraderId).IsRequired(false);
                    a.Property(w => w.QrCodeId).IsRequired(false);
                    a.Property(w => w.FourthPartyId).IsRequired(false);
                    a.Property(w => w.FourthPartyName).IsRequired(false);
                    a.Property(w => w.FourthPartyOrderPaymentUrl).IsRequired(false);
                    a.Property(w => w.FourthPartyOrderNumber).IsRequired(false);
                    a.Property(w => w.ToppestTradingRate).HasColumnType("decimal(4,3)").IsRequired();
                });

            orderConfiguration.Property(o => o.Amount).HasColumnType("decimal(18,0)").IsRequired();
            orderConfiguration.Property(o => o.AmountPaid).HasColumnType("decimal(18,3)").IsRequired(false);
            orderConfiguration.Property(o => o.CommissionRealized).HasColumnType("decimal(18,3)").IsRequired(false);

            orderConfiguration.Property(o => o.DateCreated).IsRequired();
            orderConfiguration.Property(o => o.DatePaired).IsRequired(false);
            orderConfiguration.Property(o => o.DatePaymentRecieved).IsRequired(false);


            orderConfiguration.HasOne(o => o.OrderType)
                .WithMany()
                .HasForeignKey("_orderTypeId");

            orderConfiguration.HasOne(o => o.OrderStatus)
                .WithMany()
                .HasForeignKey("_orderStatusId");

            orderConfiguration.HasOne(o => o.OrderPaymentChannel)
                .WithMany()
                .HasForeignKey("_orderPaymentChannelId");

            orderConfiguration.HasOne(o => o.OrderPaymentScheme)
                .WithMany()
                .HasForeignKey("_orderPaymentSchemeId");
        }
    }
}
