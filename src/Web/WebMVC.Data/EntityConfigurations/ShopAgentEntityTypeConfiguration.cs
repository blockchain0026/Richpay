using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class ShopAgentEntityTypeConfiguration : IEntityTypeConfiguration<ShopAgent>
    {
        public void Configure(EntityTypeBuilder<ShopAgent> shopAgentConfiguration)
        {
            shopAgentConfiguration.ToTable("shopAgents", "application");

            shopAgentConfiguration.HasKey(t => t.ShopAgentId);

            shopAgentConfiguration.Property(t => t.Username).IsRequired();
            shopAgentConfiguration.Property(t => t.Password).IsRequired();
            shopAgentConfiguration.Property(t => t.FullName).IsRequired();
            shopAgentConfiguration.Property(t => t.Nickname).IsRequired();
            shopAgentConfiguration.Property(t => t.PhoneNumber).IsRequired();
            shopAgentConfiguration.Property(t => t.Email).IsRequired();
            shopAgentConfiguration.Property(t => t.Wechat).IsRequired(false);
            shopAgentConfiguration.Property(t => t.QQ).IsRequired(false);

            shopAgentConfiguration.Property(t => t.UplineUserId).IsRequired(false);
            shopAgentConfiguration.Property(t => t.UplineUserName).IsRequired(false);
            shopAgentConfiguration.Property(t => t.UplineFullName).IsRequired(false);

            shopAgentConfiguration
                .OwnsOne(t => t.Balance, b =>
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

            shopAgentConfiguration
                .OwnsOne(t => t.RebateCommission, a =>
                {
                    a.WithOwner();
                    a.Property(w => w.RateRebateAlipayInThousandth).IsRequired();
                    a.Property(w => w.RateRebateWechatInThousandth).IsRequired();
                });

            shopAgentConfiguration.Property(t => t.IsEnabled).IsRequired();
            shopAgentConfiguration.Property(t => t.IsReviewed).IsRequired();
            shopAgentConfiguration.Property(t => t.HasGrantRight).IsRequired();

            shopAgentConfiguration.Property(t => t.LastLoginIP).IsRequired(false);
            shopAgentConfiguration.Property(t => t.DateLastLoggedIn).IsRequired(false);
            shopAgentConfiguration.Property(t => t.DateCreated).IsRequired();
        }
    }
}
