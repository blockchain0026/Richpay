using Distributing.Domain.Model.Withdrawals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class WithdrawalTypeEntityTypeConfiguration
        : IEntityTypeConfiguration<WithdrawalType>
    {
        public void Configure(EntityTypeBuilder<WithdrawalType> withdrawalTypeConfiguration)
        {
            withdrawalTypeConfiguration.ToTable("withdrawaltype", DistributingContext.DEFAULT_SCHEMA);

            withdrawalTypeConfiguration.HasKey(o => o.Id);

            withdrawalTypeConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            withdrawalTypeConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
