using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class ShopEntityTypeConfiguration : IEntityTypeConfiguration<Shop>
    {
        public void Configure(EntityTypeBuilder<Shop> shopConfiguration)
        {
            shopConfiguration.ToTable("shops", "application");

            shopConfiguration.HasKey(s => s.ShopId);

            shopConfiguration.Property(s => s.Username).IsRequired();
            shopConfiguration.Property(t => t.Password).IsRequired();
            shopConfiguration.Property(s => s.FullName).IsRequired();
            shopConfiguration.Property(s => s.SiteAddress).IsRequired();
            shopConfiguration.Property(s => s.PhoneNumber).IsRequired();
            shopConfiguration.Property(s => s.Email).IsRequired();
            shopConfiguration.Property(s => s.Wechat).IsRequired(false);
            shopConfiguration.Property(s => s.QQ).IsRequired(false);

            shopConfiguration.Property(s => s.UplineUserId).IsRequired(false);
            shopConfiguration.Property(s => s.UplineUserName).IsRequired(false);
            shopConfiguration.Property(s => s.UplineFullName).IsRequired(false);

            shopConfiguration
                .OwnsOne(s => s.Balance, b =>
                {
                    b.WithOwner();
                    b.Property(b => b.AmountAvailable).HasColumnType("decimal(18,3)").IsRequired();
                    b.Property(b => b.AmountFrozen).HasColumnType("decimal(18,3)").IsRequired();
                    b.Property(b => b.WithdrawalCommissionRateInThousandth).IsRequired();
                    b.OwnsOne(b => b.WithdrawalLimit, w =>
                    {
                        w.WithOwner();
                        w.Property(w => w.DailyAmountLimit).HasColumnType("decimal(18,0)").IsRequired();
                        w.Property(w => w.DailyFrequencyLimit).IsRequired();
                        w.Property(w => w.EachAmountUpperLimit).HasColumnType("decimal(18,0)").IsRequired();
                        w.Property(w => w.EachAmountLowerLimit).HasColumnType("decimal(18,0)").IsRequired();
                    });
                });

            shopConfiguration
                .OwnsOne(s => s.RebateCommission, a =>
                {
                    a.WithOwner();
                    a.Property(w => w.RateRebateAlipayInThousandth).IsRequired();
                    a.Property(w => w.RateRebateWechatInThousandth).IsRequired();
                });

            shopConfiguration.Property(s => s.IsOpen).IsRequired();
            shopConfiguration.Property(s => s.IsEnabled).IsRequired();
            shopConfiguration.Property(s => s.IsReviewed).IsRequired();

            shopConfiguration.Property(s => s.LastLoginIP).IsRequired(false);
            shopConfiguration.Property(s => s.DateLastLoggedIn).IsRequired(false);
            shopConfiguration.Property(s => s.DateCreated).IsRequired();

            shopConfiguration.Property(s => s.DateLastTrade).IsRequired(false);
        }
    }
}
