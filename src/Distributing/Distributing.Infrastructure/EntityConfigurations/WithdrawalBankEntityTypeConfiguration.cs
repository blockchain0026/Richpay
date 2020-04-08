using Distributing.Domain.Model.Banks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class WithdrawalBankEntityTypeConfiguration : IEntityTypeConfiguration<WithdrawalBank>
    {
        public void Configure(EntityTypeBuilder<WithdrawalBank> withdrawalBankConfiguration)
        {
            withdrawalBankConfiguration.ToTable("withdrawalBanks", DistributingContext.DEFAULT_SCHEMA);

            withdrawalBankConfiguration.HasKey(w => w.Id);

            withdrawalBankConfiguration.Ignore(w => w.DomainEvents);

            withdrawalBankConfiguration.Property(w => w.Id)
                .UseHiLo("withdrawalbankseq", DistributingContext.DEFAULT_SCHEMA);


            /*withdrawalBankConfiguration.Property(w => w.WithdrawalBankId)
                .HasMaxLength(200)
                .IsRequired();

            withdrawalBankConfiguration.HasIndex("WithdrawalBankId")
              .IsUnique(true);*/

            withdrawalBankConfiguration.Property(w => w.BankName).IsRequired();
            withdrawalBankConfiguration.Property(w => w.DateCreated).IsRequired();
        }
    }
}
