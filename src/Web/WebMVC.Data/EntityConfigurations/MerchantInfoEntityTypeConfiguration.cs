using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class MerchantInfoEntityTypeConfiguration : IEntityTypeConfiguration<MerchantInfo>
    {
        public void Configure(EntityTypeBuilder<MerchantInfo> merchantInfoConfiguration)
        {
            merchantInfoConfiguration.ToTable("merchantInfos");

            merchantInfoConfiguration.HasKey(d => d.QrCodeId);
            merchantInfoConfiguration.Property(d => d.QrCodeId)
                .ValueGeneratedNever()
                .IsRequired();

            merchantInfoConfiguration.Property(d => d.AppId).IsRequired();
            merchantInfoConfiguration.Property(d => d.AlipayPublicKey).IsRequired(false);
            merchantInfoConfiguration.Property(d => d.WechatApiCertificate).IsRequired(false);
            merchantInfoConfiguration.Property(d => d.PrivateKey).IsRequired();
            merchantInfoConfiguration.Property(d => d.MerchantId).IsRequired();
        }
    }
}
