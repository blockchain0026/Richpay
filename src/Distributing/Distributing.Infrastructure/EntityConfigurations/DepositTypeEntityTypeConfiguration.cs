using Distributing.Domain.Model.Deposits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    
    class DepositTypeEntityTypeConfiguration
    : IEntityTypeConfiguration<DepositType>
    {
        public void Configure(EntityTypeBuilder<DepositType> depositTypeConfiguration)
        {
            depositTypeConfiguration.ToTable("deposittype", DistributingContext.DEFAULT_SCHEMA);

            depositTypeConfiguration.HasKey(o => o.Id);

            depositTypeConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            depositTypeConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
