using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models.Queries;

namespace WebMVC.Data.EntityConfigurations
{
    class TransactionInfoEntityTypeConfiguration : IEntityTypeConfiguration<TransactionInfo>
    {
        public void Configure(EntityTypeBuilder<TransactionInfo> transactionInfoConfiguration)
        {
            transactionInfoConfiguration.ToTable("transactionInfos");

            transactionInfoConfiguration.HasKey(d => d.QrCodeId);
            transactionInfoConfiguration.Property(d => d.QrCodeId)
                .ValueGeneratedNever()
                .IsRequired();

            transactionInfoConfiguration.Property(d => d.AlipayUserId).IsRequired();
        }
    }
}
