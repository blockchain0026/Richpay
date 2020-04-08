using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class TraderEntityTypeConfiguration : IEntityTypeConfiguration<Trader>
    {
        public void Configure(EntityTypeBuilder<Trader> traderConfiguration)
        {
            traderConfiguration.ToTable("traders", "application");

            traderConfiguration.HasKey(t => t.TraderId);

            traderConfiguration.Property(t => t.Username).IsRequired();
            traderConfiguration.Property(t => t.Password).IsRequired();
            traderConfiguration.Property(t => t.FullName).IsRequired();
            traderConfiguration.Property(t => t.Nickname).IsRequired();
            traderConfiguration.Property(t => t.PhoneNumber).IsRequired();
            traderConfiguration.Property(t => t.Email).IsRequired();
            traderConfiguration.Property(t => t.Wechat).IsRequired(false);
            traderConfiguration.Property(t => t.QQ).IsRequired(false);

            traderConfiguration.Property(t => t.UplineUserId).IsRequired(false);
            traderConfiguration.Property(t => t.UplineUserName).IsRequired(false);
            traderConfiguration.Property(t => t.UplineFullName).IsRequired(false);

            traderConfiguration
                .OwnsOne(t => t.Balance, b =>
                {
                    b.WithOwner();
                    b.Property(b => b.AmountAvailable).HasColumnType("decimal(18,3)").IsRequired();
                    b.Property(b => b.AmountFrozen).HasColumnType("decimal(18,3)").IsRequired();
                    b.Property(b => b.WithdrawalCommissionRateInThousandth).IsRequired();
                    b.Property(b => b.DepositCommissionRateInThousandth).IsRequired();
                    b.OwnsOne(b => b.WithdrawalLimit, w =>
                    {
                        w.WithOwner();
                        w.Property(w => w.DailyAmountLimit).HasColumnType("decimal(18,0)").IsRequired();
                        w.Property(w => w.DailyFrequencyLimit).IsRequired();
                        w.Property(w => w.EachAmountUpperLimit).HasColumnType("decimal(18,0)").IsRequired();
                        w.Property(w => w.EachAmountLowerLimit).HasColumnType("decimal(18,0)").IsRequired();
                    });
                });

            traderConfiguration
                .OwnsOne(t => t.TradingCommission, a =>
                {
                    a.WithOwner();
                    a.Property(w => w.RateAlipayInThousandth).IsRequired();
                    a.Property(w => w.RateWechatInThousandth).IsRequired();
                });

            traderConfiguration.Property(t => t.IsEnabled).IsRequired();
            traderConfiguration.Property(t => t.IsReviewed).IsRequired();

            traderConfiguration.Property(t => t.LastLoginIP).IsRequired(false);
            traderConfiguration.Property(t => t.DateLastLoggedIn).IsRequired(false);
            traderConfiguration.Property(t => t.DateCreated).IsRequired();
        }
    }
}
