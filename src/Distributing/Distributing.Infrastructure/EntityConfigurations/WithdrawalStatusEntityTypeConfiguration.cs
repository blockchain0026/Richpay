using Distributing.Domain.Model.Withdrawals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class WithdrawalStatusEntityTypeConfiguration
        : IEntityTypeConfiguration<WithdrawalStatus>
    {
        public void Configure(EntityTypeBuilder<WithdrawalStatus> withdrawalStatusConfiguration)
        {
            withdrawalStatusConfiguration.ToTable("withdrawalstatus", DistributingContext.DEFAULT_SCHEMA);

            withdrawalStatusConfiguration.HasKey(o => o.Id);

            withdrawalStatusConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            withdrawalStatusConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
