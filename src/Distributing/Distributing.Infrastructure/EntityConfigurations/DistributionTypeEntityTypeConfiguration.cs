using Distributing.Domain.Model.Distributions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class DistributionTypeEntityTypeConfiguration
    : IEntityTypeConfiguration<DistributionType>
    {
        public void Configure(EntityTypeBuilder<DistributionType> distributionTypeConfiguration)
        {
            distributionTypeConfiguration.ToTable("distributiontypes", DistributingContext.DEFAULT_SCHEMA);

            distributionTypeConfiguration.HasKey(o => o.Id);

            distributionTypeConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            distributionTypeConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
