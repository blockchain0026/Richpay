using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class BankInfoEntityTypeConfiguration : IEntityTypeConfiguration<BankInfo>
    {
        public void Configure(EntityTypeBuilder<BankInfo> bankInfoConfiguration)
        {
            bankInfoConfiguration.ToTable("bankInfos");

            bankInfoConfiguration.HasKey(d => d.QrCodeId);
            bankInfoConfiguration.Property(d => d.QrCodeId)
                .ValueGeneratedNever()
                .IsRequired();

            bankInfoConfiguration.Property(d => d.BankName).IsRequired();
            bankInfoConfiguration.Property(d => d.BankMark).IsRequired();
            bankInfoConfiguration.Property(d => d.AccountName).IsRequired();
            bankInfoConfiguration.Property(d => d.CardNumber).IsRequired();
        }
    }
}
