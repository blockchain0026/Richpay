using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pairing.Domain.Model.CloudDevices;
using Pairing.Domain.Model.QrCodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pairing.Infrastructure.EntityConfigurations
{
    class QrCodeEntityTypeConfiguration : IEntityTypeConfiguration<QrCode>
    {
        public void Configure(EntityTypeBuilder<QrCode> qrCodeConfiguration)
        {
            qrCodeConfiguration.ToTable("qrCodes", PairingContext.DEFAULT_SCHEMA);

            qrCodeConfiguration.HasKey(q => q.Id);

            qrCodeConfiguration.Ignore(q => q.DomainEvents);

            qrCodeConfiguration.Property(q => q.Id)
                .UseHiLo("qrcodeseq", PairingContext.DEFAULT_SCHEMA);


            qrCodeConfiguration.Property(q => q.UserId).IsRequired();
            qrCodeConfiguration.Property(q => q.CloudDeviceId).IsRequired(false);

            qrCodeConfiguration
                .Property<int>("_qrCodeTypeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("QrCodeTypeId")
                .IsRequired();

            qrCodeConfiguration
                .Property<int>("_paymentChannelId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PaymentChannelId")
                .IsRequired();

            qrCodeConfiguration
                .Property<int>("_paymentSchemeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PaymentSchemeId")
                .IsRequired();

            qrCodeConfiguration
                .Property<int>("_qrCodeStatusId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("QrCodeStatusId")
                .IsRequired();

            //Pairing status need a concurrency plan to resolve concurrencies and conlicts.
            qrCodeConfiguration
                .Property<int>("_pairingStatusId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("PairingStatusId")
                .IsRequired()
                .IsConcurrencyToken();

            qrCodeConfiguration.Property(q => q.CodeStatusDiscription).IsRequired(false);
            qrCodeConfiguration.Property(q => q.PairingStatusDescription).IsRequired(false);

            qrCodeConfiguration
                .OwnsOne(q => q.QrCodeSettings, a =>
                {
                    a.WithOwner();
                    a.Property(w => w.AutoPairingBySuccessRate).IsRequired();
                    a.Property(w => w.AutoPairingByQuotaLeft).IsRequired();
                    a.Property(w => w.AutoPairingByBusinessHours).IsRequired();
                    a.Property(w => w.AutoPairingByCurrentConsecutiveFailures).IsRequired();
                    a.Property(w => w.AutoPairngByAvailableBalance).IsRequired();

                    //Only need 3 digits and 2 scale.
                    a.Property(w => w.SuccessRateThreshold).HasColumnType("decimal(3,2)").IsRequired();

                    a.Property(w => w.SuccessRateMinOrders).IsRequired();
                    a.Property(w => w.QuotaLeftThreshold).HasColumnType("decimal(18,3)").IsRequired();
                    a.Property(w => w.CurrentConsecutiveFailuresThreshold).IsRequired();
                    a.Property(w => w.AvailableBalanceThreshold).HasColumnType("decimal(18,3)").IsRequired();
                });

            /*qrCodeConfiguration
                .OwnsOne(q => q.Quota, a =>
                {
                    a.WithOwner();

                    a.Property(w => w.DailyAmountLimit).HasColumnType("decimal(18,0)").IsRequired();
                    a.Property(w => w.OrderAmountUpperLimit).HasColumnType("decimal(18,0)").IsRequired();
                    a.Property(w => w.OrderAmountLowerLimit).HasColumnType("decimal(18,0)").IsRequired();
                });*/

            qrCodeConfiguration.Property(w => w.DailyAmountLimit).HasColumnType("decimal(18,0)").IsRequired();
            qrCodeConfiguration.Property(w => w.OrderAmountUpperLimit).HasColumnType("decimal(18,0)").IsRequired();
            qrCodeConfiguration.Property(w => w.OrderAmountLowerLimit).HasColumnType("decimal(18,0)").IsRequired();

            qrCodeConfiguration.Property(q => q.FullName).IsRequired();
            qrCodeConfiguration.Property(q => q.IsApproved).IsRequired();

            qrCodeConfiguration
                .OwnsOne(q => q.ApprovedBy, a =>
                {
                    a.WithOwner();

                    a.Property(w => w.AdminId).IsRequired();
                    a.Property(w => w.Name).IsRequired();
                });


            qrCodeConfiguration
                .OwnsOne(q => q.BarcodeDataForManual, a =>
                {
                    a.WithOwner();

                    a.Property(w => w.QrCodeUrl).IsRequired();
                    a.Property(w => w.Amount).HasColumnType("decimal(18,0)").IsRequired(false);
                });

            qrCodeConfiguration
                .OwnsOne(q => q.BarcodeDataForAuto, a =>
                {
                    a.WithOwner();

                    a.Property(w => w.CloudDeviceUsername).IsRequired();
                    a.Property(w => w.CloudDevicePassword).IsRequired();
                    a.Property(w => w.CloudDeviceNumber).IsRequired();
                });

            qrCodeConfiguration
                .OwnsOne(q => q.MerchantData, a =>
                {
                    a.WithOwner();

                    a.Property(w => w.AppId).IsRequired();
                    a.Property(w => w.AlipayPublicKey).IsRequired(false);
                    a.Property(w => w.WechatApiCertificate).IsRequired(false);
                    a.Property(w => w.PrivateKey).IsRequired();
                    a.Property(w => w.MerchantId).IsRequired();
                });

            qrCodeConfiguration
                .OwnsOne(q => q.TransactionData, a =>
                {
                    a.WithOwner();

                    a.Property(w => w.UserId).IsRequired();
                });

            qrCodeConfiguration
                .OwnsOne(q => q.BankData, a =>
                {
                    a.WithOwner();

                    a.Property(w => w.BankName).IsRequired();
                    a.Property(w => w.BankMark).IsRequired();
                    a.Property(w => w.AccountName).IsRequired();
                    a.Property(w => w.CardNumber).IsRequired();
                });

            qrCodeConfiguration
                .OwnsOne(q => q.PairingData, a =>
                {
                    a.WithOwner();
                    a.Property(w => w.TotalSuccess).IsRequired();
                    a.Property(w => w.TotalFailures).IsRequired();
                    a.Property(w => w.HighestConsecutiveSuccess).IsRequired();
                    a.Property(w => w.HighestConsecutiveFailures).IsRequired();
                    a.Property(w => w.CurrentConsecutiveSuccess).IsRequired();
                    a.Property(w => w.CurrentConsecutiveFailures).IsRequired();
                    a.Property(w => w.SuccessRate).HasColumnType("decimal(3,2)").IsRequired();
                });


            qrCodeConfiguration.Property(w => w.IsOnline).IsRequired();
            qrCodeConfiguration.Property(w => w.MinCommissionRate).HasColumnType("decimal(4,3)").IsRequired();
            qrCodeConfiguration.Property(w => w.AvailableBalance).HasColumnType("decimal(18,3)").IsRequired();
            qrCodeConfiguration.Property(w => w.SpecifiedShopId).IsRequired(false).IsUnicode(false);
            qrCodeConfiguration.Property(w => w.QuotaLeftToday).HasColumnType("decimal(18,3)").IsRequired();
            qrCodeConfiguration.Property(w => w.DateLastTraded).IsRequired(false);

            qrCodeConfiguration.Property(q => q.DateCreated).IsRequired();

            qrCodeConfiguration
                .HasIndex("_pairingStatusId","_paymentChannelId", "_paymentSchemeId", "DateLastTraded", "MinCommissionRate", "AvailableBalance", "SpecifiedShopId", "QuotaLeftToday")
                .IsUnique(false)
                .IsClustered(false);

            //var navigation = qrCodeConfiguration.Metadata.FindNavigation(nameof(QrCode.QrCodeOrders));

            // DDD Patterns comment:
            //Set as field (New since EF 1.1) to access the OrderItem collection property through its field
            //navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            qrCodeConfiguration.HasOne(q => q.QrCodeType)
                .WithMany()
                .HasForeignKey("_qrCodeTypeId");
            qrCodeConfiguration.HasOne(q => q.PaymentChannel)
                .WithMany()
                .HasForeignKey("_paymentChannelId");
            qrCodeConfiguration.HasOne(q => q.PaymentScheme)
                .WithMany()
                .HasForeignKey("_paymentSchemeId");
            qrCodeConfiguration.HasOne(q => q.QrCodeStatus)
                .WithMany()
                .HasForeignKey("_qrCodeStatusId");
            qrCodeConfiguration.HasOne(q => q.PairingStatus)
                .WithMany()
                .HasForeignKey("_pairingStatusId");


            qrCodeConfiguration.HasOne<CloudDevice>()
                .WithMany()
                .IsRequired(false)
                .HasForeignKey("CloudDeviceId")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
