using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class TraderAgentEntityTypeConfiguration : IEntityTypeConfiguration<TraderAgent>
    {
        public void Configure(EntityTypeBuilder<TraderAgent> traderAgentConfiguration)
        {
            traderAgentConfiguration.ToTable("traderAgents","application");

            traderAgentConfiguration.HasKey(t => t.TraderAgentId);

            /*traderAgentConfiguration.Property(t => t.Id)
                .UseHiLo("traderagentseq");

            traderAgentConfiguration.Property(t => t.TraderAgentId)
                .HasMaxLength(200)
                .IsRequired();


            traderAgentConfiguration.HasIndex("TraderAgentId")
              .IsUnique(true);*/

            traderAgentConfiguration.Property(t => t.Username).IsRequired();
            traderAgentConfiguration.Property(t => t.Password).IsRequired();
            traderAgentConfiguration.Property(t => t.FullName).IsRequired();
            traderAgentConfiguration.Property(t => t.Nickname).IsRequired();
            traderAgentConfiguration.Property(t => t.PhoneNumber).IsRequired();
            traderAgentConfiguration.Property(t => t.Email).IsRequired();
            traderAgentConfiguration.Property(t => t.Wechat).IsRequired(false);
            traderAgentConfiguration.Property(t => t.QQ).IsRequired(false);

            traderAgentConfiguration.Property(t => t.UplineUserId).IsRequired(false);
            traderAgentConfiguration.Property(t => t.UplineUserName).IsRequired(false);
            traderAgentConfiguration.Property(t => t.UplineFullName).IsRequired(false);

            traderAgentConfiguration
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

            traderAgentConfiguration
                .OwnsOne(t => t.TradingCommission, a =>
                {
                    a.WithOwner();
                    a.Property(w => w.RateAlipayInThousandth).IsRequired();
                    a.Property(w => w.RateWechatInThousandth).IsRequired();
                });

            traderAgentConfiguration.Property(t => t.IsEnabled).IsRequired();
            traderAgentConfiguration.Property(t => t.IsReviewed).IsRequired();
            traderAgentConfiguration.Property(t => t.HasGrantRight).IsRequired();

            traderAgentConfiguration.Property(t => t.LastLoginIP).IsRequired(false);
            traderAgentConfiguration.Property(t => t.DateLastLoggedIn).IsRequired(false);
            traderAgentConfiguration.Property(t => t.DateCreated).IsRequired();
        }
    }
}
