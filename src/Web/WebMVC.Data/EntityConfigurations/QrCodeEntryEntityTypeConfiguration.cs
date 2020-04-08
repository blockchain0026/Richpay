using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class QrCodeEntryEntityTypeConfiguration : IEntityTypeConfiguration<QrCodeEntry>
    {
        public void Configure(EntityTypeBuilder<QrCodeEntry> qrCodeEntryConfiguration)
        {
            qrCodeEntryConfiguration.ToTable("qrCodeEntrys");

            qrCodeEntryConfiguration.HasKey(w => w.QrCodeId);
            qrCodeEntryConfiguration.Property(w => w.QrCodeId)
                .ValueGeneratedNever()
                .IsRequired();


            qrCodeEntryConfiguration.Property(w => w.UserId).IsRequired();
            qrCodeEntryConfiguration.Property(w => w.Username).IsRequired();
            qrCodeEntryConfiguration.Property(w => w.UserFullName).IsRequired();

            qrCodeEntryConfiguration.Property(w => w.UplineUserId).IsRequired();
            qrCodeEntryConfiguration.Property(w => w.UplineUserName).IsRequired();
            qrCodeEntryConfiguration.Property(w => w.UplineFullName).IsRequired();


            qrCodeEntryConfiguration.Property(w => w.CloudDeviceId).IsRequired(false);

            qrCodeEntryConfiguration.Property(w => w.QrCodeType).IsRequired();
            qrCodeEntryConfiguration.Property(w => w.PaymentChannel).IsRequired();
            qrCodeEntryConfiguration.Property(w => w.PaymentScheme).IsRequired();
            qrCodeEntryConfiguration.Property(w => w.QrCodeStatus).IsRequired();
            qrCodeEntryConfiguration.Property(w => w.PairingStatus).IsRequired();

            qrCodeEntryConfiguration.Property(w => w.CodeStatusDescription).IsRequired(false);
            qrCodeEntryConfiguration.Property(w => w.PairingStatusDescription).IsRequired(false);



            qrCodeEntryConfiguration
                       .OwnsOne(w => w.QrCodeEntrySetting, a =>
                       {
                           a.WithOwner();
                           a.Property(w => w.AutoPairingBySuccessRate).IsRequired();
                           a.Property(w => w.AutoPairingByQuotaLeft).IsRequired();
                           a.Property(w => w.AutoPairingByBusinessHours).IsRequired();
                           a.Property(w => w.AutoPairingByCurrentConsecutiveFailures).IsRequired();
                           a.Property(w => w.AutoPairngByAvailableBalance).IsRequired();

                           //Only need 3 digits and 2 scale.
                           a.Property(w => w.SuccessRateThresholdInHundredth).IsRequired();

                           a.Property(w => w.SuccessRateMinOrders).IsRequired();
                           a.Property(w => w.QuotaLeftThreshold).HasColumnType("decimal(18,3)").IsRequired();
                           a.Property(w => w.CurrentConsecutiveFailuresThreshold).IsRequired();
                           a.Property(w => w.AvailableBalanceThreshold).HasColumnType("decimal(18,3)").IsRequired();
                       });


            qrCodeEntryConfiguration.Property(w => w.DailyAmountLimit).IsRequired();
            qrCodeEntryConfiguration.Property(w => w.OrderAmountUpperLimit).IsRequired();
            qrCodeEntryConfiguration.Property(w => w.OrderAmountLowerLimit).IsRequired();


            qrCodeEntryConfiguration.Property(w => w.FullName).IsRequired();
            qrCodeEntryConfiguration.Property(w => w.IsApproved).IsRequired();
            qrCodeEntryConfiguration.Property(w => w.ApprovedByAdminId).IsRequired(false);
            qrCodeEntryConfiguration.Property(w => w.ApprovedByAdminName).IsRequired(false);

            qrCodeEntryConfiguration.Property(w => w.IsOnline).IsRequired();
            qrCodeEntryConfiguration.Property(w => w.MinCommissionRateInThousandth).IsRequired();
            qrCodeEntryConfiguration.Property(w => w.AvailableBalance).HasColumnType("decimal(18,3)").IsRequired();
            qrCodeEntryConfiguration.Property(w => w.SpecifiedShopId).IsRequired(false);
            qrCodeEntryConfiguration.Property(w => w.QuotaLeftToday).HasColumnType("decimal(18,3)").IsRequired();
            qrCodeEntryConfiguration.Property(w => w.DateLastTraded).IsRequired(false);

            qrCodeEntryConfiguration
                       .OwnsOne(w => w.PairingInfo, a =>
                       {
                           a.WithOwner();

                           a.Property(w => w.TotalSuccess).IsRequired();
                           a.Property(w => w.TotalFailures).IsRequired();
                           a.Property(w => w.HighestConsecutiveSuccess).IsRequired();
                           a.Property(w => w.HighestConsecutiveFailures).IsRequired();
                           a.Property(w => w.CurrentConsecutiveSuccess).IsRequired();
                           a.Property(w => w.CurrentConsecutiveFailures).IsRequired();
                           a.Property(w => w.SuccessRateInPercent).IsRequired();
                       });


            qrCodeEntryConfiguration.Property(w => w.DateCreated).IsRequired();
        }
    }
}
