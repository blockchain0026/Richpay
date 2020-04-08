using Distributing.Domain.Model.Deposits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class DepositStatusEntityTypeConfiguration
        :IEntityTypeConfiguration<DepositStatus>
    {
        public void Configure(EntityTypeBuilder<DepositStatus> depositStatusConfiguration)
        {
            depositStatusConfiguration.ToTable("depositstatus", DistributingContext.DEFAULT_SCHEMA);

            depositStatusConfiguration.HasKey(o => o.Id);

            depositStatusConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            depositStatusConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
