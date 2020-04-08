using Distributing.Domain.Model.Frozens;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Distributing.Infrastructure.EntityConfigurations
{
    class FrozenTypeEntityTypeConfiguration
        : IEntityTypeConfiguration<FrozenType>
    {
        public void Configure(EntityTypeBuilder<FrozenType> frozenTypeConfiguration)
        {
            frozenTypeConfiguration.ToTable("frozentype", DistributingContext.DEFAULT_SCHEMA);

            frozenTypeConfiguration.HasKey(o => o.Id);

            frozenTypeConfiguration.Property(o => o.Id)
                .HasDefaultValue(1)
                .ValueGeneratedNever()
                .IsRequired();

            frozenTypeConfiguration.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
        }
    }
}
